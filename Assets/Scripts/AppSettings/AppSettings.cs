using System;
using System.IO;
using System.Reflection;
using Blockstacker.Settings.SettingGroups;
using UnityEngine;

namespace Blockstacker.Settings
{
    public static class AppSettings
    {
        [Serializable]
        internal class SettingsContainer
        {
            public HandlingSettings Handling = new();
            public SoundSettings Sound = new();
            public GameplaySettings Gameplay = new();
            public VideoSettings Video = new();
            public CustomizationSettings Customization = new();
            public OtherSettings Others = new();
        }

        private static readonly SettingsContainer Settings = new();
        public static HandlingSettings Handling => Settings.Handling;
        public static SoundSettings Sound => Settings.Sound;
        public static GameplaySettings Gameplay => Settings.Gameplay;
        public static VideoSettings Video => Settings.Video;
        public static CustomizationSettings Customization => Settings.Customization;
        public static OtherSettings Other => Settings.Others;

        private static string SettingsPath => Path.Combine(Application.persistentDataPath, "appSettings.json");

        public static void Save()
        {
            File.WriteAllText(SettingsPath, JsonUtility.ToJson(Settings));
        }

        public static void Load()
        {
            if (!File.Exists(SettingsPath))
                return;

            JsonUtility.FromJsonOverwrite(File.ReadAllText(SettingsPath), Settings);
        }

        public static bool TrySetValue<T>(T value, string[] path)
        {
            if (path.Length == 0) return false;
            FieldInfo fieldInfo = null;
            object oldObject = null;
            object obj = Settings;
            Type type = obj.GetType();
            foreach (var fieldName in path) {
                fieldInfo = type.GetField(fieldName);
                if (fieldInfo == null) return false;

                oldObject = obj;
                obj = fieldInfo.GetValue(obj);
                if (obj == null) return false;

                type = obj.GetType();
            }

            if (type != typeof(T)) return false;

            fieldInfo.SetValue(oldObject, value);
            return true;
        }

        public static T GetValue<T>(string[] path)
        {
            if (path.Length == 0) return default;
            object obj = Settings;
            Type type = obj.GetType();
            foreach (var fieldName in path) {
                FieldInfo fieldInfo = type.GetField(fieldName);
                if (fieldInfo == null) return default;

                obj = fieldInfo.GetValue(obj);
                if (obj == null) return default;

                type = obj.GetType();
            }
            if (type == typeof(T)) {
                return (T)obj;
            }
            return default;
        }

        public static bool SettingExists<T>(string[] path)
        {
            if (path.Length == 0) return false;
            object obj = Settings;
            Type type = obj.GetType();
            foreach (var fieldName in path) {
                FieldInfo fieldInfo = type.GetField(fieldName);
                if (fieldInfo == null) return false;

                obj = fieldInfo.GetValue(obj);
                if (obj == null) return false;

                type = obj.GetType();
            }

            if (type != typeof(T)) return false;
            return true;
        }
    }
}