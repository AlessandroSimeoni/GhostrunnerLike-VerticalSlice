using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerDashStateModel", menuName = "ScriptableObjects/PlayerStates/PlayerDashStateModel")]
    public class PlayerDashStateModel : ScriptableObject
    {
        [Min(0.01f)] public float dashSpeed = 30.0f;
        [Min(0.05f)] public float dashDistance = 4.0f;
        [Header("Dash Cooldown & Slider")]
        [Min(0.5f)] public float dashSliderSize = 3.0f;
        [Min(0.0f)] public float dashSliderRefillSpeed = 1.0f;
        [Min(0.01f)] public float dashUsagePrice = 1.0f;
        [Min(0.0f)] public float dashSliderWaitTimeWhenZero = 2.0f;
    }
}
