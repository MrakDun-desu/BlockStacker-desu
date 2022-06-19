using System.IO;
using System.Threading.Tasks;
using Blockstacker.Common;
using Blockstacker.GlobalSettings.Loaders;
using UnityEngine.Events;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class BackgroundPackApplier : SettingApplierBase
    {
        public UnityEvent LoadingStarted;
        public UnityEvent LoadingFinished;

        public override void OnSettingChanged()
        {
            LoadingStarted.Invoke();
            _ = ReloadAndInvoke();
        }
        
        private async Task ReloadAndInvoke()
        {
            var backgroundFolder = Path.Combine(CustomizationPaths.BackgroundPacks,
                AppSettings.Customization.BackgroundFolder);
            await BackgroundPackLoader.Reload(backgroundFolder);
            LoadingFinished.Invoke();
        }
    }
}