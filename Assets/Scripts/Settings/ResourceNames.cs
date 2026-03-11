using System;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public class ResourceNames : IResourceNames
    {
        [field: SerializeField]
        public string WindowRoots { get; private set; } = "WindowRoots";

        [field: SerializeField]
        public string MainThemeSong { get; private set; } = "Main_Theme";

        [field: SerializeField]
        public string CardFlipSound { get; private set; } = "Card_Flip";

        [field: SerializeField]
        public string CardMatchedSound { get; private set; } = "Match";

        [field: SerializeField]
        public string CardMismatchedSound { get; private set; } = "Mismatch";

        [field: SerializeField]
        public string VictorySound { get; private set; } = "Victory";

        [field: SerializeField]
        public string CardDealSound { get; private set; } = "Card_Deal";

        [field: SerializeField]
        public string ComboMultiplierIncreased { get; private set; } = "ComboMultiplier_Increased";

        [field: SerializeField]
        public string ComboMultiplierLost { get; private set; } = "ComboMultiplier_Lost";
    }
}