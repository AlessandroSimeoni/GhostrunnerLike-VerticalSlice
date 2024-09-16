using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerDashStateModel", menuName = "ScriptableObjects/PlayerStates/PlayerDashStateModel")]
    public class PlayerDashStateModel : ScriptableObject
    {
        [Min(0.01f)] public float dashSpeed = 30.0f;
        [Min(0.05f)] public float dashDistance = 4.0f;
        [Min(0.01f)] public float dashStaminaCost = 1.0f;
    }
}
