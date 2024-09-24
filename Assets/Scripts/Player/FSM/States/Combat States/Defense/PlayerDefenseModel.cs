using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerDefenseModel", menuName = "ScriptableObjects/Player/PlayerStates/PlayerDefenseModel")]
    public class PlayerDefenseModel : ScriptableObject
    {
        [Tooltip("The visual deviation in degree from the hitting target in order to successfully defend the player"), Range(0.0f, 90.0f)] 
        public float defenseDegreeThreshold = 45.0f;
        [Header("Parry")]
        [Tooltip("The parry window in seconds starting from the enter defense state"), Min(0.0f)] 
        public float parryWindow = 0.3f;
        [Min(0.0f)] public float minRepositioningRadius = 0.2f;
        [Min(0.0f)] public float maxRepositioningRadius = 0.2f;
    }
}
