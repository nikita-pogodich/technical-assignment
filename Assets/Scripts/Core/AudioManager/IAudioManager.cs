namespace Core.AudioManager
{
    public interface IAudioManager
    {
        void PlaySound(string key, float pitch = 1f);
        void PlayMusic(string key);
        void StopMusic();

        void SetMasterVolume(float volume);
        void SetMusicVolume(float volume);
        void SetSfxVolume(float volume);
    }
}