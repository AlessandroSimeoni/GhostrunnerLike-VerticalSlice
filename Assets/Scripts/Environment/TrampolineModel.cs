using Player;
using System;
using UnityEngine;

namespace Environment
{
    [CreateAssetMenu(fileName = "TrampolineModel", menuName = "ScriptableObjects/Environment/Trampoline/TrampolineModel")]
    public class TrampolineModel : ScriptableObject
    {
        public ParabolicJumpConfig[] trampolineJumpModel = Array.Empty<ParabolicJumpConfig>();
    }
}
