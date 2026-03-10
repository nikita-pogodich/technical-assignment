using Core.ModelProvider;
using Core.MVP;
using Core.SaveSystem;
using Core.WindowManager;
using Core.WindowViewProvider;
using Cysharp.Threading.Tasks;
using Features.CardsMatching;
using Settings;
using ViewInterfaces;

namespace Features.MainMenu
{
    public class MainMenuWindowFactory : IWindowFactory
    {
        private readonly IWindowViewProvider _windowViewProvider;
        private readonly IModelProvider _modelProvider;
        private readonly ILocalSettings _localSettings;
        private readonly IWindowManager _windowManager;
        private readonly ISaveSystem _saveSystem;
        private readonly CardsMatchingModel _cardsMatchingModel;

        public bool IsAllowMultipleInstances => false;
        public string ViewName => _localSettings.ViewNames.MainMenuWindow;

        public MainMenuWindowFactory(
            IWindowViewProvider windowViewProvider,
            IModelProvider modelProvider,
            ILocalSettings localSettings,
            IWindowManager windowManager,
            ISaveSystem saveSystem,
            CardsMatchingModel cardsMatchingModel)
        {
            _windowViewProvider = windowViewProvider;
            _modelProvider = modelProvider;
            _localSettings = localSettings;
            _windowManager = windowManager;
            _saveSystem = saveSystem;
            _cardsMatchingModel = cardsMatchingModel;
        }

        public async UniTask<IWindowPresenter> CreateAsync()
        {
            var model = new MainMenuWindowModel(_modelProvider.GetUniqueId());
            var view = await _windowViewProvider.GetAsync<IMainMenuWindowView>(ViewName, WindowType.Main);
            var presenter = new MainMenuWindowPresenter(
                _localSettings,
                _windowManager,
                _saveSystem,
                _cardsMatchingModel);

            presenter.Init(view, model);

            return presenter;
        }
    }
}