using System;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public class StageSetting
    {
        [field: SerializeField]
        public int CardsToMatch { get; private set; } = 2;

        [field: SerializeField]
        public int CardsAmount { get; private set; } = 4;

        [field: SerializeField]
        public float TimeToRememberCardsSeconds { get; private set; } = 1.5f;

        [field: SerializeField]
        public float GridWidth { get; private set; }

        [field: SerializeField]
        public Vector2 CardSize { get; private set; }
    }
}