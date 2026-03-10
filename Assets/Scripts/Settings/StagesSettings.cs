using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(
        fileName = "StagesSettings",
        menuName = "Config/StagesSettings",
        order = 0
    )]
    public class StagesSettings : ScriptableObject 
    {
        [field: SerializeField]
        public List<StageSetting> StageSettings { get; private set; }
    }
}