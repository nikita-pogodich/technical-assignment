using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.AudioManager
{
    public class UnityAudioManager : MonoBehaviour, IAudioManager
    {
        [SerializeField]
        private AudioDatabase _audioDatabase;

        [SerializeField]
        private AudioSource _musicSource;

        [SerializeField]
        private int initialSfxPoolSize = 5;

        private readonly List<AudioSource> _sfxPool = new();

        private float _masterVolume = 1f;
        private float _musicVolume = 1f;
        private float _sfxVolume = 1f;

        public async UniTask Init()
        {
            _audioDatabase.Init();

            if (_musicSource != null)
            {
                _musicSource.loop = true;
                _musicSource.playOnAwake = false;
            }

            for (int i = 0; i < initialSfxPoolSize; i++)
            {
                CreateNewSfxSource();
            }

            await UniTask.CompletedTask;
        }

        public void PlaySound(string key, float pitch = 1f)
        {
            if (_audioDatabase.TryGetAudioClip(key, out AudioClip clip))
            {
                AudioSource source = GetAvailableSfxSource();

                source.pitch = pitch;
                source.clip = clip;
                source.Play();
            }
            else
            {
                Debug.LogWarning($"[UnityAudioManager] SFX not found for key: {key}");
            }
        }

        public void PlayMusic(string key)
        {
            if (_audioDatabase.TryGetAudioClip(key, out AudioClip clip))
            {
                if (_musicSource.clip == clip && _musicSource.isPlaying)
                {
                    return;
                }

                _musicSource.clip = clip;
                _musicSource.Play();
            }
            else
            {
                Debug.LogWarning($"[UnityAudioManager] Music not found for key: {key}");
            }
        }

        public void StopMusic()
        {
            if (_musicSource != null && _musicSource.isPlaying)
            {
                _musicSource.Stop();
            }
        }

        public void SetMasterVolume(float volume)
        {
            _masterVolume = Mathf.Clamp01(volume);
            UpdateSourceVolumes();
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);
            UpdateSourceVolumes();
        }

        public void SetSfxVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
            UpdateSourceVolumes();
        }

        private void UpdateSourceVolumes()
        {
            _musicSource.volume = _musicVolume * _masterVolume;

            foreach (var source in _sfxPool)
            {
                source.volume = _sfxVolume * _masterVolume;
            }
        }

        private AudioSource CreateNewSfxSource()
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.spatialBlend = 0f;
            newSource.loop = false;
            newSource.playOnAwake = false;
            newSource.bypassEffects = true;
            newSource.bypassListenerEffects = true;
            newSource.bypassReverbZones = true;
            newSource.volume = _sfxVolume * _masterVolume;

            _sfxPool.Add(newSource);
            return newSource;
        }

        private AudioSource GetAvailableSfxSource()
        {
            foreach (var source in _sfxPool)
            {
                if (source.isPlaying == false)
                {
                    return source;
                }
            }

            return CreateNewSfxSource();
        }
    }
}