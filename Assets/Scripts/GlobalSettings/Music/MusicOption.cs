using System;

namespace Blockstacker.GlobalSettings.Music
{
    [Serializable]
    public record MusicOption
    {
        public OptionType OptionType;
        public string Name;

        public MusicOption(OptionType optionType, string name)
        {
            OptionType = optionType;
            Name = name;
        }
    }

    public enum OptionType : byte
    {
        Track,
        Group,
        Random
    }
}