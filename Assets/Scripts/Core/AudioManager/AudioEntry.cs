using System;
using UnityEngine;

namespace Core.AudioManager
{
    [Serializable]
    public struct AudioEntry
    {
        public string Key;
        public AudioClip Clip;
    }
}