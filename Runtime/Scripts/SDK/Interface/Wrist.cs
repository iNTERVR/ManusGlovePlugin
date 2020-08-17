using ManusVR.SDK.Apollo;
using UnityEngine;

namespace ManusVR.Hands
{
    public class Wrist : MonoBehaviour
    {
        public device_type_t DeviceType { get; set; }
        public Hand Hand { get; set; }

        /// <summary>
        /// Rotate the wrist towards the given rotation
        /// </summary>
        /// <param name="rotation"></param>
        public void RotateWrist(Quaternion rotation)
        {
            // note Quaternion(float x, float y, float z, float w);
            // note this not a local rotation but a rotation relative to the world coordinate system
            transform.rotation = rotation;
        }
    }
}
