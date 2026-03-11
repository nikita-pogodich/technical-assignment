using UnityEngine;

namespace Core.AudioManager
{
    public class UnityAudioManager : MonoBehaviour, IAudioManager
    {
        [SerializeField]
        private AudioDatabase _audioDatabase;

        [SerializeField]
        private AudioSource _sfxSource;

        [SerializeField]
        private AudioSource _musicSource;

        private float _masterVolume = 1f;
        private float _musicVolume = 1f;
        private float _sfxVolume = 1f;

        public void Init()
        {
            _audioDatabase.Init();

            if (_musicSource != null)
            {
                _musicSource.loop = true;
                _musicSource.playOnAwake = false;
            }

            if (_sfxSource != null)
            {
                _sfxSource.loop = false;
            }
        }

        public void PlaySound(string key)
        {
            if (_audioDatabase.TryGetAudioClip(key, out AudioClip clip))
            {
                _sfxSource.PlayOneShot(clip);
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
                if (_musicSource.clip == clip && _musicSource.isPlaying) return;

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
            _sfxSource.volume = _sfxVolume * _masterVolume;
        }
    }
}