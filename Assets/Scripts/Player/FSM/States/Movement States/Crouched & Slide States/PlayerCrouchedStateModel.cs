using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerCrouchedStateModel", menuName = "ScriptableObjects/PlayerStates/PlayerCrouchedStateModel")]
    public class PlayerCrouchedStateModel : ScriptableObject
    {
        [Min(0.01f)] public float defaultCharacterHeight = 2.0f;
        [Min(0.1f)] public float crouchedCharacterHeight = 1.0f;
        [Min(0.0f)] public float crouchedTransitionTime = 0.5f;
        [Min(0.05f)] public float crouchedMovementSpeed = 5.0f;
    }
}
