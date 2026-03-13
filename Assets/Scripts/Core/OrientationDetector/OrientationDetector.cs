using R3;
using UnityEngine;

namespace Core.OrientationDetector
{
    public class OrientationDetector : MonoBehaviour, IOrientationDetector
    {
        [SerializeField]
        private float _portraitAspectRatio = 0.5625f;

        [SerializeField]
        private float _wideScreenAspectRatio = 1.777778f;

        private float _screenWidth;
        private float _screenHeight;

        private readonly ReactiveProperty<Orientation> _orientation = new();
        private readonly ReactiveProperty<bool> _isWideScreen = new();

        public ReadOnlyReactiveProperty<Orientation> Orientation => _orientation;
        public ReadOnlyReactiveProperty<bool> IsWideScreen => _isWideScreen;

        private void Start()
        {
            UpdateOrientation();
        }

        private void Update()
        {
            TryUpdateOrientation();
        }

        private void TryUpdateOrientation()
        {
            if (!Mathf.Approximately(_screenWidth, Screen.width) ||
                !Mathf.Approximately(_screenHeight, Screen.height))
            {
                UpdateOrientation();
            }
        }

        private void UpdateOrientation()
        {
            _screenWidth = Screen.width;
            _screenHeight = Screen.height;

            float aspectRatio = _screenWidth / _screenHeight;
            _isWideScreen.Value = aspectRatio > _wideScreenAspectRatio;

            if (aspectRatio > _portraitAspectRatio)
            {
                _orientation.Value = Core.OrientationDetector.Orientation.Landscape;
            }
            else
            {
                _orientation.Value = Core.OrientationDetector.Orientation.Portrait;
            }
        }
    }
}