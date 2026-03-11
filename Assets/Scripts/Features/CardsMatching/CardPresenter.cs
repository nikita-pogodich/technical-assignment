using System.Threading;
using Core.AudioManager;
using Core.MVPImplementation;
using Cysharp.Threading.Tasks;
using R3;
using Settings;
using ViewInterfaces;

namespace Features.CardsMatching
{
    public class CardPresenter : BasePresenter<ICardView, CardModel>
    {
        private readonly ILocalSettings _localSettings;
        private readonly IAudioManager _audioManager;

        private CancellationTokenSource _cancellationTokenSource;

        public CardPresenter(ILocalSettings localSettings, IAudioManager audioManager)
        {
            _localSettings = localSettings;
            _audioManager = audioManager;
        }

        protected override void OnInit(ref DisposableBuilder disposableBuilder)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            View.Selected.Subscribe(OnSelected).AddTo(ref disposableBuilder);
            View.Position = Model.Position;

            Model.IsFlipped.Subscribe(OnIsFlippedChanged).AddTo(ref disposableBuilder);
            Model.IsMatched.Subscribe(View.SetMatched).AddTo(ref disposableBuilder);
        }

        protected override void OnDeinit()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
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
            if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            if (isFlipped)
            {
                _audioManager.PlaySound(_localSettings.ResourceNames.CardFlipSound);
            }

            View.SetFlippedAsync(isFlipped, _cancellationTokenSource.Token).Forget();
        }
    }
}