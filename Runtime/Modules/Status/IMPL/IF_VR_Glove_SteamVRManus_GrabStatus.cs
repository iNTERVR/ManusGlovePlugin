using UnityEngine;
using InterVR.IF.VR.Plugin.Steam.InteractionSystem;
using InterVR.IF.VR.Components;
using InterVR.IF.VR.Modules;
using InterVR.IF.VR.Defines;
using InterVR.IF.VR.Plugin.Steam.Extensions;
using InterVR.IF.VR.Glove.Modules;

namespace InterVR.IF.VR.Glove.Plugin.SteamVRManus.Modules
{
    public class IF_VR_Glove_SteamVRManus_GrabStatus : IF_VR_IGrabStatus
    {
        private readonly IF_VR_Glove_IInterface vrGloveInterface;

        public IF_VR_Glove_SteamVRManus_GrabStatus(IF_VR_Glove_IInterface vrGloveInterface)
        {
            this.vrGloveInterface = vrGloveInterface;
        }

        IF_VR_Steam_Hand convertSteamVRHand(IF_VR_Hand hand)
        {
            IF_VR_Steam_Hand steamVRHand;
            if (hand.Type == IF_VR_HandType.Left)
                steamVRHand = IF_VR_Steam_Player.instance.leftHand;
            else
                steamVRHand = IF_VR_Steam_Player.instance.rightHand;
            return steamVRHand;
        }

        public IF_VR_GrabType GetBestGrabbingType(IF_VR_Hand hand, IF_VR_GrabType preferred, bool forcePreference = false)
        {
            var steamVRHand = convertSteamVRHand(hand);
            if (steamVRHand.noSteamVRFallbackCamera)
            {
                if (Input.GetMouseButton(0))
                    return preferred;
                else
                    return IF_VR_Steam_GrabTypes.None.ConvertTo();
            }

            //if (preferred == IF_VR_Steam_GrabTypes.Pinch.ConvertTo())
            //{
            //    if (vrGloveInterface.GetGrabState(hand.Type))
            //        return IF_VR_Steam_GrabTypes.Pinch.ConvertTo();
            //    else if (forcePreference)
            //        return IF_VR_Steam_GrabTypes.None.ConvertTo();
            //}
            if (preferred == IF_VR_Steam_GrabTypes.Grip.ConvertTo())
            {
                if (vrGloveInterface.GetGrabState(hand.Type))
                    return IF_VR_Steam_GrabTypes.Grip.ConvertTo();
                else if (forcePreference)
                    return IF_VR_Steam_GrabTypes.None.ConvertTo();
            }

            //if (vrGloveInterface.GetGrabState(hand.Type))
            //    return IF_VR_Steam_GrabTypes.Pinch.ConvertTo();
            if (vrGloveInterface.GetGrabState(hand.Type))
                return IF_VR_Steam_GrabTypes.Grip.ConvertTo();
            return IF_VR_Steam_GrabTypes.None.ConvertTo();
        }

        public IF_VR_GrabType GetGrabEnding(IF_VR_Hand hand, IF_VR_GrabType explicitType = IF_VR_GrabType.None)
        {
            var steamVRHand = convertSteamVRHand(hand);
            if (explicitType != IF_VR_Steam_GrabTypes.None.ConvertTo())
            {
                if (steamVRHand.noSteamVRFallbackCamera)
                {
                    if (Input.GetMouseButtonUp(0))
                        return explicitType;
                    else
                        return IF_VR_Steam_GrabTypes.None.ConvertTo();
                }

                //if (explicitType == IF_VR_Steam_GrabTypes.Pinch.ConvertTo() && vrGloveInterface.GetGrabStateUp(hand.Type))
                //    return IF_VR_Steam_GrabTypes.Pinch.ConvertTo();
                if (explicitType == IF_VR_Steam_GrabTypes.Grip.ConvertTo() && vrGloveInterface.GetGrabStateUp(hand.Type))
                    return IF_VR_Steam_GrabTypes.Grip.ConvertTo();
            }
            else
            {
                if (steamVRHand.noSteamVRFallbackCamera)
                {
                    if (Input.GetMouseButtonUp(0))
                        return IF_VR_Steam_GrabTypes.Grip.ConvertTo();
                    else
                        return IF_VR_Steam_GrabTypes.None.ConvertTo();
                }

                //if (vrGloveInterface.GetGrabStateUp(hand.Type))
                //    return IF_VR_Steam_GrabTypes.Pinch.ConvertTo();
                if (vrGloveInterface.GetGrabStateUp(hand.Type))
                    return IF_VR_Steam_GrabTypes.Grip.ConvertTo();
            }
            return IF_VR_Steam_GrabTypes.None.ConvertTo();
        }

        public IF_VR_GrabType GetGrabStarting(IF_VR_Hand hand, IF_VR_GrabType explicitType = IF_VR_GrabType.None)
        {
            var steamVRHand = convertSteamVRHand(hand);
            if (explicitType != IF_VR_Steam_GrabTypes.None.ConvertTo())
            {
                if (steamVRHand.noSteamVRFallbackCamera)
                {
                    if (Input.GetMouseButtonDown(0))
                        return explicitType;
                    else
                        return IF_VR_Steam_GrabTypes.None.ConvertTo();
                }

                // pinch not supoorted
                //if (explicitType == IF_VR_Steam_GrabTypes.Pinch.ConvertTo() && vrGloveInterface.GetGrabStateDown(hand.Type))
                //    return IF_VR_Steam_GrabTypes.Pinch.ConvertTo();
                if (explicitType == IF_VR_Steam_GrabTypes.Grip.ConvertTo() && vrGloveInterface.GetGrabStateDown(hand.Type))
                    return IF_VR_Steam_GrabTypes.Grip.ConvertTo();
            }
            else
            {
                if (steamVRHand.noSteamVRFallbackCamera)
                {
                    if (Input.GetMouseButtonDown(0))
                        return IF_VR_Steam_GrabTypes.Grip.ConvertTo();
                    else
                        return IF_VR_Steam_GrabTypes.None.ConvertTo();
                }


                //if (vrGloveInterface.GetGrabStateDown(hand.Type))
                //    return IF_VR_Steam_GrabTypes.Pinch.ConvertTo();
                if (vrGloveInterface.GetGrabStateDown(hand.Type))
                    return IF_VR_Steam_GrabTypes.Grip.ConvertTo();
            }
            return IF_VR_Steam_GrabTypes.None.ConvertTo();
        }

        public bool IsGrabbingWithOppositeType(IF_VR_Hand hand, IF_VR_GrabType type)
        {
            var steamVRHand = convertSteamVRHand(hand);
            if (steamVRHand.noSteamVRFallbackCamera)
            {
                if (Input.GetMouseButton(0))
                    return true;
                else
                    return false;
            }

            //if (type == IF_VR_Steam_GrabTypes.Pinch.ConvertTo())
            //{
            //    return vrGloveInterface.GetGrabState(hand.Type);
            //}
            //else
            if (type == IF_VR_Steam_GrabTypes.Grip.ConvertTo())
            {
                return vrGloveInterface.GetGrabState(hand.Type);
            }
            return false;
        }

        public bool IsGrabbingWithType(IF_VR_Hand hand, IF_VR_GrabType type)
        {
            var steamVRHand = convertSteamVRHand(hand);
            if (steamVRHand.noSteamVRFallbackCamera)
            {
                if (Input.GetMouseButton(0))
                    return true;
                else
                    return false;
            }

            if (type == IF_VR_Steam_GrabTypes.Grip.ConvertTo())
            {
                return vrGloveInterface.GetGrabState(hand.Type);
            }
            //else
            //if (type == IF_VR_Steam_GrabTypes.Pinch.ConvertTo())
            //{
            //    return vrGloveInterface.GetGrabState(hand.Type);
            //}
            return false;
        }
    }
}