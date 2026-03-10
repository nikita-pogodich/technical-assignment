using Core.MVPImplementation;
using R3;

namespace Features.CardsMatching
{
    public class CardModel : BaseModel
    {
        public readonly int Index;
        public readonly int Position;
        public readonly string IconResourceKey;
        public readonly ReactiveProperty<bool> IsFlipped = new();
        public readonly ReactiveProperty<bool> IsMatched = new();

        private readonly ReactiveCommand<int> _selected = new();

        public Observable<int> Selected => _selected;

        public CardModel(int position, int index, string iconResourceKey, int uniqueId) : base(uniqueId)
        {
            Position = position;
            Index = index;
            IconResourceKey = iconResourceKey;
        }

        public void Select()
        {
            _selected.Execute(Position);
        }
    }
}