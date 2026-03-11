using System;
using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public class GameSettings : IGameSettings
    {
        [field: SerializeField]
        public List<string> CardResourceKeys { get; private set; }

        [field: SerializeField]
        public float TimeToResetMismatchedCardsSeconds { get; private set; } = 1.0f;

        [field: SerializeField]
        public float DelayBeforeStageCompletedSeconds { get; private set; } = 1.0f;

        [field: SerializeField]
        public int MatchScoreBonus { get; private set; } = 100;

        [field: SerializeField]
        public int MismatchScorePenalty { get; private set; } = 10;

        [field: SerializeField]
        public string SavesFolderName { get; private set; } = "Saves";

        [field: SerializeField]
        public string AutoSaveSlotName { get; private set; } = "Slot_1";

        [SerializeField]
        private StagesSettings _stagesSettings;

        public List<StageSetting> StageSettings => _stagesSettings.StageSettings;
    }
}