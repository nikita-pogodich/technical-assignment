using Core.AudioManager;
using Core.MVPImplementation;
using R3;
using Settings;
using ViewInterfaces;

namespace Features.CardsMatching
{
    public class CardPresenter : BasePresenter<ICardView, CardModel>
    {
        private readonly ILocalSettings _localSettings;
        private readonly IAudioManager _audioManager;

        public CardPresenter(ILocalSettings localSettings, IAudioManager audioManager)
        {
            _localSettings = localSettings;
            _audioManager = audioManager;
        }

        protected override void OnInit(ref DisposableBuilder disposableBuilder)
        {
            View.Selected.Subscribe(OnSelected).AddTo(ref disposableBuilder);
            View.Position = Model.Position;

            Model.IsFlipped.Subscribe(OnIsFlippedChanged).AddTo(ref disposableBuilder);
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

        private void OnIsFlippedChanged(bool isFlipped)
        {
            if (isFlipped)
            {
                _audioManager.PlaySound(_localSettings.ResourceNames.CardFlipSound);
            }
            
            View.SetFlipped(isFlipped);
        }
    }
}