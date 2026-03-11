using System;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public class OrientationStageSettings
    {
        [field: SerializeField]
        public float GridWidth { get; private set; }

        [field: SerializeField]
        public Vector2 CardSize { get; private set; }
    }
}