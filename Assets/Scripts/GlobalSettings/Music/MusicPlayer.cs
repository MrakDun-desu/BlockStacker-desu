using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blockstacker.Common;
using Blockstacker.Common.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blockstacker.GlobalSettings.Music
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicPlayer : MonoSingleton<MusicPlayer>
    {
        [SerializeField] private MusicConfiguration _musicConfiguration;
        [Range(0, 10)] [SerializeField] private float _switchInterval;
        [Range(0, 10)] [SerializeField] private float _quietenTime = 1f;
        [Range(0, 0.1f)] [SerializeField] private float _quietenInterval = .01f;
        [SerializeField] private AudioClipCollection _defaultMusic = new();

        public static MusicConfiguration Configuration { get; private set; }

        private AudioSource _audioSource;
        private const string MENU_STRING = "Scene_Menu";
        private const string GAME_STRING = "Scene_Game";
        private string _currentSceneType = MENU_STRING;
        private float _nextSongStartTime;
        private float _timeUntilQuiet;

        public static List<MusicOption> ListAvailableOptions()
        {
            var outList =
                Configuration.GameMusicGroups.Keys.Select(groupName => new MusicOption(OptionType.Group, groupName)).ToList();
            outList.AddRange(Configuration.GameMusic.Select(trackName => new MusicOption(OptionType.Track, trackName)));

            return outList;
        }

        public void PlayImmediateWithoutLoop(string trackName)
        {
            PlayImmediate(trackName);
            _audioSource.loop = false;
        }

        public void PlayImmediate(string trackName)
        {
            _nextSongStartTime = float.PositiveInfinity;
            _timeUntilQuiet = 0;
            _audioSource.Stop();
            if (SoundPackLoader.Music.ContainsKey(trackName))
                _audioSource.clip = SoundPackLoader.Music[trackName];
            else if (_defaultMusic.ContainsKey(trackName))
                _audioSource.clip = _defaultMusic[trackName];
            else return;
            _audioSource.volume = 1;
            ResumeNormalPlaying();
        }

        public void PlayCustomGameTrackImmediate()
        {
            var customGameMusic = AppSettings.Sound.CustomGameMusic;
            if (customGameMusic is null || string.IsNullOrEmpty(customGameMusic.Name))
            {
                if (Configuration.GameMusic.TryGetRandomElement(out var trackName))
                    PlayImmediate(trackName);

                return;
            }

            switch (customGameMusic.OptionType)
            {
                case OptionType.Track:
                    PlayImmediate(customGameMusic.Name);
                    break;
                case OptionType.Group:
                    PlayFromGroupImmediate(customGameMusic.Name);
                    break;
                case OptionType.Random:
                    if (Configuration.GameMusic.TryGetRandomElement(out var trackName))
                        PlayImmediate(trackName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void PlayVictoryTrack()
        {
            if (Configuration.VictoryMusic.TryGetRandomElement(out var trackName))
                PlayNextTrack(trackName);
        }

        public void PlayLossTrack()
        {
            if (Configuration.LossMusic.TryGetRandomElement(out var trackName))
                PlayNextTrack(trackName);
        }

        public void PlayFromGroupImmediate(string groupName)
        {
            if (Configuration.GameMusicGroups[groupName].TryGetRandomElement(out var trackName))
                PlayImmediate(trackName);
        }

        public void StopPlaying()
        {
            _audioSource.Stop();
            _nextSongStartTime = float.PositiveInfinity;
        }

        private void ResumeNormalPlaying()
        {
            _audioSource.Play();
            _audioSource.loop = true;
        }

        protected override void Awake()
        {
            base.Awake();
            _audioSource = GetComponent<AudioSource>();
            Configuration = _musicConfiguration;

            SceneManager.sceneLoaded += OnSceneChanged;
        }

        public void Start()
        {
            if (Configuration.MenuMusic.TryGetRandomElement(out var nextTrack))
                PlayNextTrack(nextTrack);
        }

        private void PlayNextTrack(string trackName)
        {
            _nextSongStartTime = Time.time + _switchInterval;
            AudioClip nextSong = null;

            if (SoundPackLoader.Music.ContainsKey(trackName))
                nextSong = SoundPackLoader.Music[trackName];
            else if (_defaultMusic.ContainsKey(trackName))
                nextSong = _defaultMusic[trackName];

            StartCoroutine(PlayNextTrackCor(nextSong));
        }

        private IEnumerator PlayNextTrackCor(AudioClip nextTrack)
        {
            yield return new WaitForSeconds(_switchInterval + .01f);
            if (Time.time <= _nextSongStartTime) yield break;

            _timeUntilQuiet = 0;
            _audioSource.clip = nextTrack;
            _audioSource.volume = 1;
            ResumeNormalPlaying();
        }

        public IEnumerator MuteSourceOverTime()
        {
            _timeUntilQuiet = _quietenTime;
            while (_timeUntilQuiet > 0)
            {
                _timeUntilQuiet -= _quietenInterval;
                _audioSource.volume = _timeUntilQuiet / _quietenTime;
                yield return new WaitForSeconds(_quietenInterval);
            }
        }

        private void OnSceneChanged(Scene newScene, LoadSceneMode sceneMode)
        {
            if (sceneMode == LoadSceneMode.Additive) return;
            if (newScene.name.StartsWith(_currentSceneType)) return;

            StartCoroutine(MuteSourceOverTime());

            if (newScene.name.StartsWith(MENU_STRING))
            {
                if (Configuration.MenuMusic.TryGetRandomElement(out var nextTrack))
                    PlayNextTrack(nextTrack);
                _currentSceneType = MENU_STRING;
            }
            else if (newScene.name.StartsWith(GAME_STRING))
            {
                // game scenes handle music playing by themselves
                _currentSceneType = GAME_STRING;
            }
        }

    }
}