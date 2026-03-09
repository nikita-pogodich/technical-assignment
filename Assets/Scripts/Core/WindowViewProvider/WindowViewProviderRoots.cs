using System.Collections.Generic;
using UnityEngine;

namespace Core.WindowViewProvider
{
    public class WindowViewProviderRoots : MonoBehaviour
    {
        [field: SerializeField]
        public List<WindowTypeRoot> WindowTypeRoots { get; private set; } = new();

        [field: SerializeField]
        public WindowTypeRoot DefaultWindowTypeRoot { get; private set; }
    }
}