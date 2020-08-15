using System;
using UnityEngine;
using Zenject;

namespace InterVR.IF.Glove.Plugin.Manus.Installer
{
    [CreateAssetMenu(fileName = "IF_Glove_Manus_Settings", menuName = "InterVR/IF/Plugin/Glove/Manus/Settings")]
    public class IF_Glove_Manus_Installer : ScriptableObjectInstaller<IF_Glove_Manus_Installer>
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
            public string Name = "IF Manus Glove Plugin Installer";
        }
    }
}