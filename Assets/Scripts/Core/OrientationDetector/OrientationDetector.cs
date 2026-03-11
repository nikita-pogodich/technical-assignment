using R3;
using UnityEngine;

namespace Core.OrientationDetector
{
    public class OrientationDetector : MonoBehaviour, IOrientationDetector
    {
        [SerializeField]
        private float _minAspectRatio = 0.5625f;

        private float _screenWidth;
        private float _screenHeight;

        private readonly ReactiveProperty<Orientation> _orientation = new();

        public ReadOnlyReactiveProperty<Orientation> Orientation => _orientation;

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
            if (aspectRatio > _minAspectRatio)
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