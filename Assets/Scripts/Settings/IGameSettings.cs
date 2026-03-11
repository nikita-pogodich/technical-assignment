using System.Collections.Generic;

namespace Settings
{
    public interface IGameSettings
    {
        List<string> CardResourceKeys { get; }
        List<StageSetting> StageSettings { get; }
        int MatchScoreBonus { get; }
        int MismatchScorePenalty { get; }
        float TimeToResetMismatchedCardsSeconds { get; }
        float DelayBeforeStageCompletedSeconds { get; }
        string SavesFolderName { get; }
        string AutoSaveSlotName { get; }
        float DefaultMasterVolume { get; }
        float DefaultMusicVolume { get; }
        float DefaultSfxVolume { get; }
        int MaxComboMultiplier { get; }
        int CardViewsPreloadPoolSize { get; }
        float LandscapeGridWrapperOffset { get; }
        float PortraitGridWrapperOffset { get; }
        float LandscapeGridHeight { get; }
        float PortraitGridHeight { get; }
        float LandscapeCanvasScalerMatch { get; }
        float PortraitCanvasScalerMatch { get; }
    }
}