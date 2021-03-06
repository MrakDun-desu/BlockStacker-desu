using System;
using TMPro;
using UnityEngine;

namespace Blockstacker.GameSettings.Changers
{
    public class GameSettingEnumChanger<TEnum> : GameSettingChangerBase<TEnum> where TEnum : Enum
    {
        [Space] [SerializeField] private TMP_Dropdown _dropdown;

        [SerializeField] private EnumWithName[] _values;

        private void Start()
        {
            _dropdown.ClearOptions();
            for (var i = 0; i < _values.Length; i++)
            {
                var value = _values[i].Value;
                _dropdown.options.Add(new TMP_Dropdown.OptionData(_values[i].Name));
                if (Convert.ToInt64(value) == Convert.ToInt64(_gameSettingsSO.GetValue<TEnum>(_controlPath)))
                    _dropdown.SetValueWithoutNotify(i);
            }

            _dropdown.RefreshShownValue();
        }

        public void OnValuePicked(int index)
        {
            SetValue(_values[index].Value);
        }

        [Serializable]
        private struct EnumWithName
        {
            public string Name;
            public TEnum Value;

            public EnumWithName(TEnum value, string name)
            {
                Value = value;
                Name = name;
            }
        }
    }
}