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

        [field: SerializeField]
        public float DefaultMasterVolume { get; private set; } = 0.3f;

        [field: SerializeField]
        public float DefaultMusicVolume { get; private set; } = 0.15f;

        [field: SerializeField]
        public float DefaultSfxVolume { get; private set; } = 1.0f;

        [field: SerializeField]
        public int MaxComboMultiplier { get; private set; } = 4;

        [field: SerializeField]
        public int CardViewsPreloadPoolSize { get; private set; } = 32;

        [field: SerializeField]
        public float LandscapeGridWrapperOffset { get; private set; } = 400.0f;

        [field: SerializeField]
        public float PortraitGridWrapperOffset { get; private set; } = 0.0f;

        [field: SerializeField]
        public float LandscapeGridHeight { get; private set; } = 1080.0f;

        [field: SerializeField]
        public float PortraitGridHeight { get; private set; } = 1580.0f;

        [field: SerializeField]
        public float LandscapeCanvasScalerMatch { get; private set; } = 1.0f;

        [field: SerializeField]
        public float PortraitCanvasScalerMatch { get; private set; } = 0.5f;

        [SerializeField]
        private StagesSettings _stagesSettings;

        public List<StageSetting> StageSettings => _stagesSettings.StageSettings;
    }
}