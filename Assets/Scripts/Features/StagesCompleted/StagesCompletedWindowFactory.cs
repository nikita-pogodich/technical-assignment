using Core.ModelProvider;
using Core.MVP;
using Core.WindowManager;
using Core.WindowViewProvider;
using Cysharp.Threading.Tasks;
using Features.CardsMatching;
using Settings;
using ViewInterfaces;

namespace Features.StagesCompleted
{
    public class StagesCompletedWindowFactory : IWindowFactory
    {
        private readonly IWindowViewProvider _windowViewProvider;
        private readonly IModelProvider _modelProvider;
        private readonly ILocalSettings _localSettings;
        private readonly IWindowManager _windowManager;
        private readonly CardsMatchingModel _cardsMatchingModel;

        public bool IsAllowMultipleInstances => false;
        public string ViewName => _localSettings.ViewNames.StagesCompletedWindow;

        public StagesCompletedWindowFactory(
            IWindowViewProvider windowViewProvider,
            IModelProvider modelProvider,
            ILocalSettings localSettings,
            IWindowManager windowManager,
            CardsMatchingModel cardsMatchingModel)
        {
            _windowViewProvider = windowViewProvider;
            _modelProvider = modelProvider;
            _localSettings = localSettings;
            _windowManager = windowManager;
            _cardsMatchingModel = cardsMatchingModel;
        }

        public async UniTask<IWindowPresenter> CreateAsync()
        {
            var model = new StagesCompletedWindowModel(_modelProvider.GetUniqueId());
            var view = await _windowViewProvider.GetAsync<IStagesCompletedWindowView>(ViewName, WindowType.Popup);
            var presenter = new StagesCompletedWindowPresenter(_localSettings, _windowManager, _cardsMatchingModel);
            presenter.Init(view, model);

            return presenter;
        }
    }
}