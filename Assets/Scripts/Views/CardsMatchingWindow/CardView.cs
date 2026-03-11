using Core.MVPImplementation;
using Core.ResourcesManager;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using R3;
using UnityEngine;
using UnityEngine.UI;
using ViewInterfaces;

namespace Views.CardsMatchingWindow
{
    public class CardView : BaseView, ICardView
    {
        [SerializeField]
        private Image _icon;

        [SerializeField]
        private CanvasGroup _wrapperCanvasGroup;

        [SerializeField]
        private RectTransform _wrapperRectTransform;

        [SerializeField]
        private CanvasGroup _backCanvasGroup;

        [SerializeField]
        private CanvasGroup _frontCanvasGroup;

        [SerializeField]
        private Button _selectButton;

        [SerializeField]
        private float _halfFlipDuration;

        [SerializeField]
        private Ease _flipEase = Ease.Linear;

        [SerializeField]
        private float _matchedScale = 0.8f;

        [SerializeField]
        private Vector3 _matchedRotation = new(0.0f, 0.0f, 12.0f);

        [SerializeField]
        private float _matchedDuration;

        [SerializeField]
        private float _delayBeforeHideMatched;

        [SerializeField]
        private Ease _matchedEase = Ease.Linear;

        private readonly ReactiveCommand _selected = new();
        private IResourcesManager _resourcesManager;

        public Observable<Unit> Selected => _selected;

        private Sequence _flipSequence;
        private Sequence _matchedSequence;
        private bool _isShown;

        public override void SetShown(bool isShown)
        {
            _isShown = isShown;
            _wrapperCanvasGroup.alpha = isShown ? 1.0f : 0.0f;
        }

        public void InjectDependencies(IResourcesManager resourcesManager)
        {
            _resourcesManager = resourcesManager;
        }

        public void SetFlipped(bool isFlipped, bool isInstantly = false)
        {
            _flipSequence?.Kill();
            _flipSequence = DOTween.Sequence();

            if (_isShown == false)
            {
                return;
            }

            if (isInstantly)
            {
                _frontCanvasGroup.alpha = isFlipped ? 1.0f : 0.0f;
                _backCanvasGroup.alpha = isFlipped ? 0.0f : 1.0f;
                _wrapperRectTransform.localEulerAngles = isFlipped ? new Vector3(0.0f, 180.0f, 0.0f) : Vector3.zero;
                return;
            }

            if (isFlipped)
            {
                TweenerCore<Quaternion, Vector3, QuaternionOptions> firstHalfRotationTween = _wrapperRectTransform
                    .DOLocalRotate(new Vector3(0.0f, 90.0f, 0.0f), _halfFlipDuration)
                    .SetEase(_flipEase);

                TweenerCore<Quaternion, Vector3, QuaternionOptions> secondHalfRotationTween = _wrapperRectTransform
                    .DOLocalRotate(new Vector3(0.0f, 180.0f, 0.0f), _halfFlipDuration)
                    .SetEase(_flipEase);

                _flipSequence
                    .Insert(0.0f, firstHalfRotationTween)
                    .InsertCallback(_halfFlipDuration, SwitchSides)
                    .Insert(_halfFlipDuration, secondHalfRotationTween);
            }
            else
            {
                TweenerCore<Quaternion, Vector3, QuaternionOptions> firstHalfRotationTween = _wrapperRectTransform
                    .DOLocalRotate(new Vector3(0.0f, 90.0f, 0.0f), _halfFlipDuration)
                    .SetEase(_flipEase);

                TweenerCore<Quaternion, Vector3, QuaternionOptions> secondHalfRotationTween = _wrapperRectTransform
                    .DOLocalRotate(new Vector3(0.0f, 0.0f, 0.0f), _halfFlipDuration)
                    .SetEase(_flipEase);

                _flipSequence
                    .Insert(0.0f, firstHalfRotationTween)
                    .InsertCallback(_halfFlipDuration, SwitchSides)
                    .Insert(_halfFlipDuration, secondHalfRotationTween);
            }

            void SwitchSides()
            {
                _frontCanvasGroup.alpha = isFlipped ? 1.0f : 0.0f;
                _backCanvasGroup.alpha = isFlipped ? 0.0f : 1.0f;
            }
        }

        public void SetMatched(bool isMatched)
        {
            _matchedSequence?.Kill();
            _matchedSequence = DOTween.Sequence();

            if (isMatched)
            {
                TweenerCore<Quaternion, Vector3, QuaternionOptions> rotationTween = _wrapperRectTransform
                    .DOLocalRotate(_matchedRotation, _matchedDuration)
                    .SetEase(_matchedEase);

                TweenerCore<Vector3, Vector3, VectorOptions> scaleTween = _wrapperRectTransform
                    .DOScale(_matchedScale, _matchedDuration)
                    .SetEase(_matchedEase);

                TweenerCore<float, float, FloatOptions> fadeTween = _wrapperCanvasGroup
                    .DOFade(0.0f, _matchedDuration)
                    .SetEase(_matchedEase);

                _matchedSequence
                    .Insert(_delayBeforeHideMatched, rotationTween)
                    .Insert(_delayBeforeHideMatched, scaleTween)
                    .Insert(_delayBeforeHideMatched, fadeTween);
            }
            else
            {
                if (_isShown)
                {
                    _wrapperCanvasGroup.alpha = 1.0f;
                }

                _wrapperRectTransform.localScale = Vector3.one;
                _wrapperRectTransform.localRotation = Quaternion.identity;
            }
        }

        public async UniTask LoadIconAsync(string iconResourceKey)
        {
            var iconSprite = await _resourcesManager.LoadAssetAsync<Sprite>(iconResourceKey);
            _icon.sprite = iconSprite;
        }

        protected override void OnInit(ref DisposableBuilder disposableBuilder)
        {
            _selectButton.OnClickAsObservable().Subscribe(OnSelect).AddTo(ref disposableBuilder);
        }

        private void OnSelect(Unit _)
        {
            _selected.Execute(Unit.Default);
        }
    }
}