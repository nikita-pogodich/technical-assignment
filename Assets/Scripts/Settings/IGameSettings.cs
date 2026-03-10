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
    }
}