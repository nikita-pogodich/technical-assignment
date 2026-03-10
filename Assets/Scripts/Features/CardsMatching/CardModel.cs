using Core.MVPImplementation;
using R3;

namespace Features.CardsMatching
{
    public class CardModel : BaseModel
    {
        public readonly int Index;
        public readonly int Position;
        public readonly string ResourceKey;

        public readonly ReactiveProperty<bool> IsFlipped = new();
        public readonly ReactiveProperty<bool> IsMatched = new();

        public CardModel(int position, int index, string resourceKey, int uniqueId) : base(uniqueId)
        {
            Position = position;
            Index = index;
            ResourceKey = resourceKey;
        }
    }
}