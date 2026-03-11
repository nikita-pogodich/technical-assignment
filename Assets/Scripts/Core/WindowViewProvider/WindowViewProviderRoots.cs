using System;
using System.Collections.Generic;
using Core.OrientationDetector;
using R3;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Core.WindowViewProvider
{
    public class WindowViewProviderRoots : MonoBehaviour
    {
        private IOrientationDetector _orientationDetector;
        private IDisposable _reactiveDisposable;
        private ILocalSettings _localSettings;

        [SerializeField]
        private CanvasScaler _canvasScaler;

        [field: SerializeField]
        public List<WindowTypeRoot> WindowTypeRoots { get; private set; } = new();

        [field: SerializeField]
        public WindowTypeRoot DefaultWindowTypeRoot { get; private set; }

        public void InjectDependencies(ILocalSettings localSettings, IOrientationDetector orientationDetector)
        {
            _localSettings = localSettings;
            _orientationDetector = orientationDetector;
        }

        public void Init()
        {
            DisposableBuilder disposableBuilder = Disposable.CreateBuilder();

            _orientationDetector.Orientation.Subscribe(OnOrientationChanged).AddTo(ref disposableBuilder);
            _reactiveDisposable = disposableBuilder.Build();
        }

        public void Deinit()
        {
            _reactiveDisposable?.Dispose();
        }

        private void OnOrientationChanged(Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.Landscape:
                    _canvasScaler.matchWidthOrHeight = _localSettings.GameSettings.LandscapeCanvasScalerMatch;
                    break;
                case Orientation.Portrait:
                    _canvasScaler.matchWidthOrHeight = _localSettings.GameSettings.PortraitCanvasScalerMatch;
                    break;
            }
        }
    }
}