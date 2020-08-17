using ManusVR.SDK.Apollo;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ManusVR.Hands
{
    public enum PhalangeType
    {
        Proximal = 1,
        Intermedial = 2,
        Distal = 3
    }

    [Serializable]
    public class HandRig
    {
        public Transform WristTransform;
        public List<FingerRig> Fingers => new List<FingerRig>() { Thumb, Index, Middle, Ring, Pinky };
        public FingerRig Thumb = new FingerRig(), Index = new FingerRig(), Middle = new FingerRig(), Ring = new FingerRig(), Pinky = new FingerRig();

        public FingerRig GetFingerRig(ApolloHandData.FingerName finger)
        {
            switch (finger)
            {
                case ApolloHandData.FingerName.Thumb:
                    return Thumb;
                case ApolloHandData.FingerName.Index:
                    return Index;
                case ApolloHandData.FingerName.Middle:
                    return Middle;
                case ApolloHandData.FingerName.Ring:
                    return Ring;
                case ApolloHandData.FingerName.Pinky:
                    return Pinky;
                default:
                    throw new ArgumentOutOfRangeException("finger", finger, null);
            }
        }
    }

    [Serializable]
    public class FingerRig
    {
        public List<Transform> Transforms => new List<Transform>() { Proximal, Intermedial, Distal };
        public Transform Proximal, Intermedial, Distal;
    }

    public class ManusRigger : MonoBehaviour
    {
        [SerializeField]
        public HandRig LeftHand, RightHand;

        public Transform GetWristTransform(device_type_t deviceType)
        {
            HandRig hand = GetHand(deviceType);

            return hand?.WristTransform;
        }

        public Transform GetFingerTransform(device_type_t deviceType, ApolloHandData.FingerName finger, PhalangeType phalange)
        {
            HandRig hand = GetHand(deviceType);

            if (hand == null)
            {
                throw new ArgumentOutOfRangeException("phalange", phalange, null);
            }

            switch (phalange)
            {
                case PhalangeType.Proximal:
                    return hand.GetFingerRig(finger).Proximal;
                case PhalangeType.Intermedial:
                    return hand.GetFingerRig(finger).Intermedial;
                case PhalangeType.Distal:
                    return hand.GetFingerRig(finger).Distal;

                default:
                    throw new ArgumentOutOfRangeException("phalange", phalange, null);
            }
        }

        private HandRig GetHand(device_type_t deviceType)
        {
            return deviceType == device_type_t.GLOVE_LEFT ? LeftHand : RightHand;
        }
    }
}
