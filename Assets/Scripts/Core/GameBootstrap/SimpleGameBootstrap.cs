using System.Threading;
using Core.ModelProvider;
using Core.SaveSystem;
using Cysharp.Threading.Tasks;
using Features.CardsMatching;
using Features.MainMenu;
using Features.StagesCompleted;
using Settings;
using UnityEngine;
using ViewInterfaces;

namespace Core.GameBootstrap
{
    public class SimpleGameBootstrap : MonoBehaviour
    {
        [SerializeField]
        private LocalSettings _localSettings;

        private readonly WindowManager.WindowManager _windowManager = new();
        private readonly IModelProvider _modelProvider = new SimpleModelProvider();
        private readonly ResourcesManager.ResourcesManager _resourcesManager = new();

        private CancellationTokenSource _destroyCancellationTokenSource = new();
        private ViewProvider.ViewProvider _viewProvider;
        private WindowViewProvider.WindowViewProvider _windowViewProvider;
        private CardsMatchingModel _cardsMatchingModel;
        private ISaveSystem _saveSystem;

        private void Start()
        {
            InitAsync().Forget();
        }

        private void OnDestroy()
        {
            _destroyCancellationTokenSource.Cancel();
            _destroyCancellationTokenSource.Dispose();
            _destroyCancellationTokenSource = null;
            _windowManager.Dispose();
            _cardsMatchingModel?.Deinit();
        }

        private async UniTaskVoid InitAsync()
        {
            string savePath = System.IO.Path.Combine(
                Application.persistentDataPath,
                _localSettings.GameSettings.SavesFolderName);

            _saveSystem = new JsonSlotSaveSystem(savePath, new NewtonsoftJsonSerializer());

            await InitResourcesManager();
            InitViewProvider();
            await InitWindowViewProvider();

            if (_destroyCancellationTokenSource == null || _destroyCancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            RegisterWindowFactories();

            ShowStartWindowAsync().Forget();
        }

        private void RegisterWindowFactories()
        {
            _cardsMatchingModel = new CardsMatchingModel(
                _localSettings,
                _modelProvider,
                _saveSystem,
                _modelProvider.GetUniqueId());

            _cardsMatchingModel.InitAsync().Forget();

            var mainMenuWindowFactory = new MainMenuWindowFactory(
                _windowViewProvider,
                _modelProvider,
                _localSettings,
                _windowManager,
                _saveSystem,
                _cardsMatchingModel);

            _windowManager.RegisterWindowFactory(mainMenuWindowFactory);

            var cardsMatchingWindowFactory = new CardsMatchingWindowFactory(
                _windowViewProvider,
                _localSettings,
                _viewProvider,
                _windowManager,
                _resourcesManager,
                _cardsMatchingModel);

            _windowManager.RegisterWindowFactory(cardsMatchingWindowFactory);

            var stagesCompletedWindowFactory = new StagesCompletedWindowFactory(
                _windowViewProvider,
                _modelProvider,
                _localSettings,
                _windowManager,
                _cardsMatchingModel);

            _windowManager.RegisterWindowFactory(stagesCompletedWindowFactory);
        }

        private async UniTask InitResourcesManager()
        {
            if (_destroyCancellationTokenSource == null || _destroyCancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            await _resourcesManager.InitializeAsync(_destroyCancellationTokenSource.Token);
        }

        private void InitViewProvider()
        {
            if (_destroyCancellationTokenSource == null || _destroyCancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            _viewProvider = new ViewProvider.ViewProvider(_resourcesManager);
        }

        private async UniTask InitWindowViewProvider()
        {
            if (_destroyCancellationTokenSource == null || _destroyCancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            _windowViewProvider = new WindowViewProvider.WindowViewProvider(
                _localSettings,
                _resourcesManager,
                _viewProvider);

            await _windowViewProvider.InitializeAsync(_destroyCancellationTokenSource.Token);
        }

        private async UniTask ShowStartWindowAsync()
        {
            await _windowManager.ShowWindowAsync<IMainMenuWindowView, MainMenuWindowModel>(
                _localSettings.ViewNames.MainMenuWindow);
        }
    }
}