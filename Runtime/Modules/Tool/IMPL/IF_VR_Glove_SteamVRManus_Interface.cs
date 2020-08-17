using InterVR.IF.VR.Defines;
using InterVR.IF.VR.Glove.Modules;
using ManusVR.Hands;
using UniRx;
using UnityEngine;

namespace InterVR.IF.VR.Glove.Plugin.SteamVRManus.Modules
{
    public class IF_VR_Glove_SteamVRManus_Interface : IF_VR_Glove_IInterface
    {
        int playerNumber = HandDataManager.invalidPlayerNumber;
        public int PlayerNumber
        {
            get
            {
                if (playerNumber == HandDataManager.invalidPlayerNumber)
                {
                    HandDataManager.GetPlayerNumber(out playerNumber);
                }

                return playerNumber;
            }
        }

        public FloatReactiveProperty HandYawOffsetLeft { get; private set; }
        public FloatReactiveProperty HandYawOffsetRight { get; private set; }

        public IF_VR_Glove_SteamVRManus_Interface()
        {
            HandYawOffsetLeft = new FloatReactiveProperty();
            HandYawOffsetRight = new FloatReactiveProperty();
        }

        Transform rootTransform;

        public Transform GetRootTransform()
        {
            return rootTransform;
        }

        public void SetRootTransform(Transform root)
        {
            rootTransform = root;
        }

        public bool GetGrabState(IF_VR_HandType handType)
        {
            var handData = HandDataManager.GetHandData(handType == IF_VR_HandType.Left ? ManusVR.SDK.Apollo.device_type_t.GLOVE_LEFT : ManusVR.SDK.Apollo.device_type_t.GLOVE_RIGHT);
            if (handData == null)
                return false;

            return handData.GetGrabState();
        }

        public bool GetGrabStateDown(IF_VR_HandType handType)
        {
            var handData = HandDataManager.GetHandData(handType == IF_VR_HandType.Left ? ManusVR.SDK.Apollo.device_type_t.GLOVE_LEFT : ManusVR.SDK.Apollo.device_type_t.GLOVE_RIGHT);
            if (handData == null)
                return false;

            return handData.GetGrabStateDown();
        }

        public bool GetGrabStateUp(IF_VR_HandType handType)
        {
            var handData = HandDataManager.GetHandData(handType == IF_VR_HandType.Left ? ManusVR.SDK.Apollo.device_type_t.GLOVE_LEFT : ManusVR.SDK.Apollo.device_type_t.GLOVE_RIGHT);
            if (handData == null)
                return false;

            return handData.GetGrabStateUp();
        }

        public void Dispose()
        {
            HandYawOffsetLeft.Dispose();
            HandYawOffsetRight.Dispose();
        }
    }
}