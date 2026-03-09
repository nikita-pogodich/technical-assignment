using System;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public class ResourceNames : IResourceNames
    {
        [field: SerializeField]
        public string WindowRoots { get; private set; } = "WindowRoots";
    }
}