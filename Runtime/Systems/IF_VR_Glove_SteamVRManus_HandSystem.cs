using EcsRx.Groups;
using EcsRx.Events;
using UniRx;
using System.Collections.Generic;
using System;
using EcsRx.Extensions;
using EcsRx.Collections.Database;
using EcsRx.Unity.Extensions;
using InterVR.IF.VR.Components;
using EcsRx.Plugins.ReactiveSystems.Systems;
using EcsRx.Entities;
using UniRx.Triggers;
using EcsRx.Plugins.Views.Components;
using InterVR.IF.VR.Modules;
using InterVR.IF.VR.Glove.Modules;
using UnityEngine;
using InterVR.IF.Modules;
using InterVR.IF.VR.Defines;
using InterVR.IF.VR.Glove.Components;
using ManusVR.Hands;
using ManusVR.SDK.Apollo;
using ManusVR.SDK.Manus;
using InterVR.IF.VR.Plugin.Steam.InteractionSystem;
using Valve.VR;
using InterVR.IF.Blueprints;
using InterVR.IF.Defines;
using InterVR.IF.Components;

namespace InterVR.IF.VR.Glove.Plugin.SteamVRManus.Systems
{
    public class IF_VR_Glove_SteamVRManus_HandSystem : ISetupSystem, ITeardownSystem
    {
        public IGroup Group => new Group(typeof(IF_VR_Glove_Hand), typeof(ViewComponent));

        private Dictionary<IEntity, List<IDisposable>> subscriptionsPerEntity = new Dictionary<IEntity, List<IDisposable>>();
        private readonly IF_IGameObjectTool gameObjectTool;
        private readonly IF_VR_IInterface vrInterface;
        private readonly IF_VR_Glove_IInterface vrGloveInterface;
        private readonly IEntityDatabase entityDatabase;
        private readonly IEventSystem eventSystem;

        public IF_VR_Glove_SteamVRManus_HandSystem(IF_IGameObjectTool gameObjectTool,
            IF_VR_IInterface vrInterface,
            IF_VR_Glove_IInterface vrGloveInterface,
            IEntityDatabase entityDatabase,
            IEventSystem eventSystem)
        {
            this.gameObjectTool = gameObjectTool;
            this.vrInterface = vrInterface;
            this.vrGloveInterface = vrGloveInterface;
            this.entityDatabase = entityDatabase;
            this.eventSystem = eventSystem;
        }

        bool manusIsConnected(IEntity entity)
        {
            var vrGloveHand = entity.GetComponent<IF_VR_Glove_Hand>();
            HandDataManager.EnsureLoaded();
            var deviceType = vrGloveHand.Type == IF_VR_HandType.Left ? device_type_t.GLOVE_LEFT : device_type_t.GLOVE_RIGHT;
            vrGloveHand.Connected = Manus.ManusIsConnected(HandDataManager.ManusSession, deviceType);
            return vrGloveHand.Connected;
        }

        bool vrGloveIsActivated(IEntity entity)
        {
            var vrGloveHand = entity.GetComponent<IF_VR_Glove_Hand>();
            return vrGloveHand.Active.Value;
        }

        public void Setup(IEntity entity)
        {
            var subscriptions = new List<IDisposable>();
            subscriptionsPerEntity.Add(entity, subscriptions);

            Observable.EveryUpdate()
                .Where(x => manusIsConnected(entity) && HandDataManager.IsInitialised)
                .First()
                .Subscribe(x =>
                {
                    var vrGloveHand = entity.GetComponent<IF_VR_Glove_Hand>();
                    var gloveHandType = vrGloveHand.Type;
                    var vrGloveView = entity.GetGameObject();

                    // create wrist and setup
                    var pool = entityDatabase.GetCollection();
                    var wristEntity = pool.CreateEntity();
                    vrGloveHand.Wrist.gameObject.LinkEntity(wristEntity, pool);
                    var wristView = wristEntity.GetGameObject();
                    var wrist = wristEntity.AddComponent<IF_VR_Glove_Wrist>();
                    wrist.Type = gloveHandType;
                    vrGloveHand.WristEntity = wristEntity;

                    // instaniate render model prefab
                    var prefab = gloveHandType == IF_VR_HandType.Left ? IF_VR_Steam_Player.instance.leftHand.renderModelPrefab : IF_VR_Steam_Player.instance.rightHand.renderModelPrefab;
                    var parent = wristView.transform;
                    var instance = gameObjectTool.InstantiateWithInit(prefab, parent);
                    var skeleton = instance.GetComponentInChildren<SteamVR_Behaviour_Skeleton>();
                    var manusRigger = vrGloveView.AddComponent<ManusRigger>();
                    var handRig = gloveHandType == IF_VR_HandType.Left ? manusRigger.LeftHand = new HandRig() : manusRigger.RightHand = new HandRig();

                    handRig.WristTransform = vrGloveHand.Wrist;
                    handRig.Thumb.Proximal = skeleton.thumbProximal;
                    handRig.Thumb.Intermedial = skeleton.thumbMiddle;
                    handRig.Thumb.Distal = skeleton.thumbDistal;
                    handRig.Index.Proximal = skeleton.indexProximal;
                    handRig.Index.Intermedial = skeleton.indexMiddle;
                    handRig.Index.Distal = skeleton.indexDistal;
                    handRig.Middle.Proximal = skeleton.middleProximal;
                    handRig.Middle.Intermedial = skeleton.middleMiddle;
                    handRig.Middle.Distal = skeleton.middleDistal;
                    handRig.Ring.Proximal = skeleton.ringProximal;
                    handRig.Ring.Intermedial = skeleton.ringMiddle;
                    handRig.Ring.Distal = skeleton.ringDistal;
                    handRig.Pinky.Proximal = skeleton.pinkyProximal;
                    handRig.Pinky.Intermedial = skeleton.pinkyMiddle;
                    handRig.Pinky.Distal = skeleton.pinkyDistal;

                    var skeletonRoot = skeleton.root;
                    if (skeleton.mirroring == SteamVR_Behaviour_Skeleton.MirrorType.RightToLeft)
                    {
                        skeletonRoot.localRotation = Quaternion.Euler(0, 90, -90);
                    }
                    else
                    {
                        skeletonRoot.localRotation = Quaternion.Euler(0, 90, 90);
                    }

                    var hand = wristView.AddComponent<ManusVR.Hands.Hand>();
                    hand.DeviceType = gloveHandType == IF_VR_HandType.Left ? device_type_t.GLOVE_LEFT : device_type_t.GLOVE_RIGHT;
                    hand.Initialize(manusRigger);
                    if (gloveHandType == IF_VR_HandType.Left)
                    {
                        hand.HandYawOffset = vrGloveInterface.HandYawOffsetLeft.Value;
                        vrGloveInterface.HandYawOffsetLeft.DistinctUntilChanged().Subscribe(f =>
                        {
                            hand.HandYawOffset = f;
                        }).AddTo(subscriptions);
                    }
                    else
                    {
                        hand.HandYawOffset = vrGloveInterface.HandYawOffsetRight.Value;
                        vrGloveInterface.HandYawOffsetRight.DistinctUntilChanged().Subscribe(f =>
                        {
                            hand.HandYawOffset = f;
                        }).AddTo(subscriptions);
                    }

                    GameObject.Destroy(skeleton);
                    GameObject.Destroy(instance.GetComponentInChildren<Animator>());

                    // follow tracker
                    var vrHandTrackerEntity = vrInterface.GetHandTrackerEntity(gloveHandType);
                    pool.CreateEntity(new IF_FollowEntityBlueprint(IF_UpdateMomentType.Update,
                        vrHandTrackerEntity,
                        entity,
                        true,
                        true,
                        new Vector3(0.0f, 0.07f, -0.04f),
                        Vector3.zero));

                    var vrHandEntity = vrInterface.GetHandEntity(gloveHandType);
                    var vrHand = vrHandEntity.GetComponent<IF_VR_Hand>();
                    var followEntityEntities = entityDatabase.GetEntitiesFor(new Group(typeof(IF_FollowEntity)), 0);
                    foreach (var followEntityEntity in followEntityEntities)
                    {
                        var followEntity = followEntityEntity.GetComponent<IF_FollowEntity>();
                        if (followEntity.FollowSourceEntity.HasComponent<IF_VR_Hand>())
                        {
                            var vrHandSource = followEntity.FollowSourceEntity.GetComponent<IF_VR_Hand>();
                            if (vrHandSource.Type == vrGloveHand.Type)
                            {
                                followEntity.FollowTargetEntity = wristEntity;
                                if (vrHand.Type == IF_VR_HandType.Left)
                                {
                                    followEntity.OffsetPosition = new Vector3(-0.14f, 0.03f, -0.08f);
                                    followEntity.OffsetRotation = new Vector3(0.0f, -135.0f, 90.0f);
                                }
                                else
                                {
                                    followEntity.OffsetPosition = new Vector3(0.14f, -0.03f, 0.08f);
                                    followEntity.OffsetRotation = new Vector3(0.0f, 45.0f, 90.0f);
                                }
                                break;
                            }
                        }
                    }

                    if (gloveHandType == IF_VR_HandType.Left)
                    {
                        IF_VR_Steam_Player.instance.leftHand.Hide();
                    }
                    else if (gloveHandType == IF_VR_HandType.Right)
                    {
                        IF_VR_Steam_Player.instance.rightHand.Hide();
                    }

                    vrGloveHand.RenderModel = instance;
                    vrGloveHand.Active.Value = true;

                }).AddTo(subscriptions);

            Observable.EveryUpdate()
                .Where(x => vrGloveIsActivated(entity) && HandDataManager.IsPlayerNumberValid(vrGloveInterface.PlayerNumber))
                .Subscribe(x =>
                {
                    var vrGloveHand = entity.GetComponent<IF_VR_Glove_Hand>();
                    var vrGloveHandView = entity.GetGameObject();
                    var wristEntity = vrGloveHand.WristEntity;
                    var wristView = wristEntity.GetGameObject();
                    var hand = wristView.GetComponent<Hand>();
                    if (HandDataManager.CanGetHandData(vrGloveInterface.PlayerNumber, hand.DeviceType))
                    {
                        ApolloHandData handData = HandDataManager.GetHandData(vrGloveInterface.PlayerNumber, hand.DeviceType);
                        hand.AnimateHand(handData);
                        hand.UpdateHand(handData);
                    }

                    if (vrGloveHandView.activeSelf)
                    {
                        bool state = manusIsConnected(entity);
                        if (!state && vrGloveHand.GoingToDisconnect == false)
                        {
                            vrGloveHand.GoingToDisconnect = true;
                            vrGloveHand.DisconnectStateTimer = 0.0f;
                        }
                    }
                    else
                    {
                        bool state = manusIsConnected(entity);
                        if (state)
                        {
                            vrGloveHandView.SetActive(true);
                        }
                    }

                    if (vrGloveHand.GoingToDisconnect)
                    {
                        vrGloveHand.DisconnectStateTimer += Time.deltaTime;

                        bool state = manusIsConnected(entity);
                        if (state)
                        {
                            vrGloveHand.GoingToDisconnect = false;
                        }
                        else if (vrGloveHand.DisconnectStateTimer >= IF_VR_Glove_Hand.MaxDisconnectStateTimeSeconds)
                        {
                            vrGloveHandView.SetActive(false);
                            vrGloveHand.GoingToDisconnect = false;
                        }
                    }
                }).AddTo(subscriptions);
        }

        public void Teardown(IEntity entity)
        {
            if (subscriptionsPerEntity.TryGetValue(entity, out List<IDisposable> subscriptions))
            {
                subscriptions.DisposeAll();
                subscriptions.Clear();
                subscriptionsPerEntity.Remove(entity);
            }
        }
    }
}