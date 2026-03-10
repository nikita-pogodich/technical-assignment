using Core.MVPImplementation;

namespace Features.StagesCompleted
{
    public class StagesCompletedWindowModel : BaseModel
    {
        public int Score { get; private set; }
        
        public StagesCompletedWindowModel(int uniqueId) : base(uniqueId)
        {
        }

        public void SetScore(int score)
        {
            Score = score;
        }
    }
}