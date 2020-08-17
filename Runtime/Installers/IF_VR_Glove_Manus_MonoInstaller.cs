using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace InterVR.IF.VR.Glove.Plugin.Manus.Installer
{
    public class IF_VR_Glove_Manus_MonoInstaller : MonoInstaller<IF_VR_Glove_Manus_MonoInstaller>
    {
        public List<ScriptableObjectInstaller> settings;

        public override void InstallBindings()
        {
            var settingsInstaller = settings.Cast<IInstaller>();
            foreach (var installer in settingsInstaller)
            {
                Container.Inject(installer);
                installer.InstallBindings();
            }
        }
    }
}