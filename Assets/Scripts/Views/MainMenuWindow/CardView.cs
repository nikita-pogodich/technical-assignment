using Core.MVPImplementation;
using Core.ResourcesManager;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;
using ViewInterfaces;

namespace Views.MainMenuWindow
{
    public class CardView : BaseView, ICardView
    {
        [SerializeField]
        private Image _icon;

        [SerializeField]
        private CanvasGroup _wrapperCanvasGroup;

        [SerializeField]
        private CanvasGroup _backCanvasGroup;

        [SerializeField]
        private CanvasGroup _frontCanvasGroup;

        [SerializeField]
        private Button _selectButton;

        private readonly ReactiveCommand _selected = new();
        private IResourcesManager _resourcesManager;

        public ReactiveProperty<string> IconResourceKey { get; } = new();
        public Observable<Unit> Selected => _selected;

        public void InjectDependencies(IResourcesManager resourcesManager)
        {
            _resourcesManager = resourcesManager;

            if (string.IsNullOrEmpty(IconResourceKey.Value) == false)
            {
                OnIconResourceKeyChanged(IconResourceKey.Value);
            }
        }

        public void SetFlipped(bool isFlipped)
        {
            //TODO Animate flip

            _frontCanvasGroup.alpha = isFlipped ? 1.0f : 0.0f;
            _backCanvasGroup.alpha = isFlipped ? 0.0f : 1.0f;
        }

        public void SetMatched(bool isMatched)
        {
            _wrapperCanvasGroup.alpha = isMatched ? 0.0f : 1.0f;

            if (isMatched)
            {
                //TODO: Animate matching
            }
        }

        protected override void OnInit(ref DisposableBuilder disposableBuilder)
        {
            IconResourceKey.Subscribe(OnIconResourceKeyChanged).AddTo(ref disposableBuilder);
            _selectButton.OnClickAsObservable().Subscribe(OnSelect).AddTo(ref disposableBuilder);
        }

        private void OnIconResourceKeyChanged(string iconResourceKey)
        {
            if (_resourcesManager == null)
            {
                return;
            }

            LoadIconAsync(iconResourceKey).Forget();
        }

        private async UniTaskVoid LoadIconAsync(string iconResourceKey)
        {
            var iconSprite = await _resourcesManager.LoadAssetAsync<Sprite>(iconResourceKey);
            _icon.sprite = iconSprite;
        }

        private void OnSelect(Unit _)
        {
            _selected.Execute(Unit.Default);
        }
    }
}