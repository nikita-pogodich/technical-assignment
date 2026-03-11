using Core.AudioManager;
using Core.MVP;
using Core.OrientationDetector;
using Core.ResourcesManager;
using Core.ViewProvider;
using Core.WindowManager;
using Core.WindowViewProvider;
using Cysharp.Threading.Tasks;
using Settings;
using ViewInterfaces;

namespace Features.CardsMatching
{
    public class CardsMatchingWindowFactory : IWindowFactory
    {
        private readonly IWindowViewProvider _windowViewProvider;
        private readonly ILocalSettings _localSettings;
        private readonly IViewProvider _viewProvider;
        private readonly IWindowManager _windowManager;
        private readonly IResourcesManager _resourcesManager;
        private readonly IAudioManager _audioManager;
        private readonly IOrientationDetector _orientationDetector;
        private readonly CardsMatchingModel _cardsMatchingModel;

        public bool IsAllowMultipleInstances => false;
        public string ViewName => _localSettings.ViewNames.CardsMatchingWindow;

        public CardsMatchingWindowFactory(
            IWindowViewProvider windowViewProvider,
            ILocalSettings localSettings,
            IViewProvider viewProvider,
            IWindowManager windowManager,
            IResourcesManager resourcesManager,
            CardsMatchingModel cardsMatchingModel,
            IAudioManager audioManager,
            IOrientationDetector orientationDetector)
        {
            _windowViewProvider = windowViewProvider;
            _localSettings = localSettings;
            _viewProvider = viewProvider;
            _windowManager = windowManager;
            _resourcesManager = resourcesManager;
            _cardsMatchingModel = cardsMatchingModel;
            _audioManager = audioManager;
            _orientationDetector = orientationDetector;
        }

        public async UniTask<IWindowPresenter> CreateAsync()
        {
            var view = await _windowViewProvider.GetAsync<ICardsMatchingWindow>(
                ViewName,
                WindowType.Main,
                isInitView: false);

            view.InjectDependencies(_localSettings, _orientationDetector);
            view.Init(ViewName);

            var presenter = new CardsMatchingWindowPresenter(
                _localSettings,
                _viewProvider,
                _windowManager,
                _resourcesManager,
                _audioManager);

            presenter.Init(view, _cardsMatchingModel);

            return presenter;
        }
    }
}