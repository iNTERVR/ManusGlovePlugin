using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace InterVR.IF.Glove.Plugin.Manus.Installer
{
    public class IF_Glove_Manus_MonoInstaller : MonoInstaller<IF_Glove_Manus_MonoInstaller>
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