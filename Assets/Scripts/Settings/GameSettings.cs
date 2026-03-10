using System;
using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public class GameSettings : IGameSettings
    {
        [field: SerializeField]
        public List<string> CardResourceKeys { get; private set; }

        [field: SerializeField]
        public List<StageSetting> StageSettings { get; private set; }
    }
}