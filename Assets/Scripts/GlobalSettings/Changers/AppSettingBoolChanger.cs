using UnityEngine;
using UnityEngine.UI;

namespace Blockstacker.GlobalSettings.Changers
{
    public class AppSettingBoolChanger : AppSettingChangerBase<bool>
    {
        [Space] [SerializeField] private Toggle _toggle;

        private void Start()
        {
            RefreshValue();
            AppSettings.SettingsReloaded += RefreshValue;
        }
        
        private void RefreshValue() => _toggle.isOn = AppSettings.GetValue<bool>(_controlPath);
    }
}