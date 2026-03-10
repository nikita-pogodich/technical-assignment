using Core.MVPImplementation;
using R3;
using ViewInterfaces;

namespace Features.CardsMatching
{
    public class CardPresenter : BasePresenter<ICardView, CardModel>
    {
        protected override void OnInit(ref DisposableBuilder disposableBuilder)
        {
            View.IconResourceKey.Value = Model.IconResourceKey;
            View.Selected.Subscribe(OnSelected).AddTo(ref disposableBuilder);

            Model.IsFlipped.Subscribe(View.SetFlipped).AddTo(ref disposableBuilder);
            Model.IsMatched.Subscribe(View.SetMatched).AddTo(ref disposableBuilder);
        }

        private void OnSelected(Unit _)
        {
            if (IsShown == false)
            {
                return;
            }

            Model.Select();
        }
    }
}