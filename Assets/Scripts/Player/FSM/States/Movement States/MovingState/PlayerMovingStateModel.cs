using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerMovingStateModel", menuName = "ScriptableObjects/PlayerStates/PlayerMovingStateModel")]
    public class PlayerMovingStateModel : ScriptableObject
    {
        [Min(0.01f)] public float movementSpeed = 15.0f;
    }
}
