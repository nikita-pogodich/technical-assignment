using System.Collections.Generic;
using UnityEngine;

namespace Core.AudioManager
{
    [CreateAssetMenu(
        fileName = "AudioDatabase",
        menuName = "Config/AudioDatabase",
        order = 0
    )]
    public class AudioDatabase : ScriptableObject
    {
        [field: SerializeField]
        public AudioEntry[] AudioEntries { get; private set; }

        private readonly Dictionary<string, AudioClip> _audioEntriesByKey = new();

        public void Init()
        {
            foreach (AudioEntry audioEntry in AudioEntries)
            {
                _audioEntriesByKey.TryAdd(audioEntry.Key, audioEntry.Clip);
            }
        }

        public bool TryGetAudioClip(string key, out AudioClip audioClip)
        {
            return _audioEntriesByKey.TryGetValue(key, out audioClip);
        }
    }
}