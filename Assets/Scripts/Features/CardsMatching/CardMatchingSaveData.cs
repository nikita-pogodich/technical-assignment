using System;

namespace Features.CardsMatching
{
    [Serializable]
    public class CardMatchingSaveData
    {
        public int StageIndex;
        public int Score;

        public CardMatchingSaveData(int stageIndex, int score)
        {
            StageIndex = stageIndex;
            Score = score;
        }
    }
}