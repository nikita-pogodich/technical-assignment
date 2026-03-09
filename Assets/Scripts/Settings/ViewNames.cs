using System;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public class ViewNames : IViewNames
    {
        [field: SerializeField]
        public string MainMenuWindow { get; private set; } = "MainMenuWindow";

        [field: SerializeField]
        public string GameHUDWindow { get; private set; } = "GameHUDWindow";

        [field: SerializeField]
        public string LoseWindow { get; private set; } = "LoseWindow";

        [field: SerializeField]
        public string ConfirmationWindow { get; private set; } = "ConfirmationWindow";
    }
}