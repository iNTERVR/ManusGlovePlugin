using System;
using UnityEngine;
using Zenject;

namespace InterVR.IF.VR.Glove.Plugin.Manus.SteamVRExample
{
    [CreateAssetMenu(fileName = "IF_VR_Glove_Manus_SteamVRExample_Settings", menuName = "InterVR/IF/Plugin/VR/Glove/Manus/SteamVRExample/Settings")]
    public class IF_VR_Glove_Manus_SteamVRExample_Installer : ScriptableObjectInstaller<IF_VR_Glove_Manus_SteamVRExample_Installer>
    {
#pragma warning disable 0649
        [SerializeField]
        Settings settings;
#pragma warning restore 0649

        public override void InstallBindings()
        {
            Container.BindInstance(settings).IfNotBound();
        }

        [Serializable]
        public class Settings
        {
            public string Name = "IF Manus VR Glove Plugin SteamVRExample Installer";
        }
    }
}