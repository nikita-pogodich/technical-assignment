using Core.ModelProvider;
using Core.MVP;
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
        private readonly CardsMatchingModel _cardsMatchingModel;

        public bool IsAllowMultipleInstances => false;
        public string ViewName => _localSettings.ViewNames.CardsMatchingWindow;

        public CardsMatchingWindowFactory(
            IWindowViewProvider windowViewProvider,
            ILocalSettings localSettings,
            IViewProvider viewProvider,
            IWindowManager windowManager,
            IResourcesManager resourcesManager,
            CardsMatchingModel cardsMatchingModel)
        {
            _windowViewProvider = windowViewProvider;
            _localSettings = localSettings;
            _viewProvider = viewProvider;
            _windowManager = windowManager;
            _resourcesManager = resourcesManager;
            _cardsMatchingModel = cardsMatchingModel;
        }

        public async UniTask<IWindowPresenter> CreateAsync()
        {
            var view = await _windowViewProvider.GetAsync<ICardsMatchingWindow>(ViewName, WindowType.Main);
            view.InjectDependencies(_localSettings);

            var presenter = new CardsMatchingWindowPresenter(
                _localSettings,
                _viewProvider,
                _windowManager,
                _resourcesManager);

            presenter.Init(view, _cardsMatchingModel);

            return presenter;
        }
    }
}