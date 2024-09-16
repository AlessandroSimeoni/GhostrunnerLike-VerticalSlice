using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "StaminaModel", menuName = "ScriptableObjects/Player/StaminaModel")]
    public class StaminaModel : ScriptableObject
    {
        [Min(0.5f)] public float maxStamina = 3.0f;
        [Min(0.0f)] public float staminaRegenSpeed = 1.0f;
        [Min(0.0f)] public float staminaRegenDelayWhenZero = 2.0f;
    }
}
