using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(
        fileName = "LocalSettings",
        menuName = "Config/LocalSettings",
        order = 0
    )]
    public class LocalSettings : ScriptableObject, ILocalSettings
    {
        [SerializeField]
        private ViewNames _viewNames;

        [SerializeField]
        private ResourceNames _resourceNames;

        [SerializeField]
        private GameSettings _gameSettings;

        public IViewNames ViewNames => _viewNames;
        public IResourceNames ResourceNames => _resourceNames;
        public IGameSettings GameSettings => _gameSettings;
    }
}