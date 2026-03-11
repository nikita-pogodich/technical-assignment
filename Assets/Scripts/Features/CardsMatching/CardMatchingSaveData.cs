using System;

namespace Features.CardsMatching
{
    [Serializable]
    public class CardMatchingSaveData
    {
        public int StageIndex;
        public int Score;
        public int ComboMultiplier;

        public CardMatchingSaveData(int stageIndex, int score, int comboMultiplier)
        {
            StageIndex = stageIndex;
            Score = score;
            ComboMultiplier = comboMultiplier;
        }
    }
}