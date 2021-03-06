using UnityEngine;

namespace Blockstacker.GameSettings.Changers
{
    public class GameSettingIntChanger : GameSettingChangerWithField<int>
    {
        [Space] [SerializeField] private bool _clampValue;

        [SerializeField] private int _maxValue;
        [SerializeField] private int _minValue;

        protected override void OnValidate()
        {
            base.OnValidate();
            if (_minValue > _maxValue) _minValue = _maxValue;
        }

        public void SetValue(string value)
        {
            if (!int.TryParse(value, out var intValue)) return;
            SetValue(_clampValue ? Mathf.Clamp(intValue, _minValue, _maxValue) : intValue);
            _valueField.SetTextWithoutNotify(intValue.ToString());
        }
    }
}