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

        public IViewNames ViewNames => _viewNames;
        public IResourceNames ResourceNames => _resourceNames;
    }
}