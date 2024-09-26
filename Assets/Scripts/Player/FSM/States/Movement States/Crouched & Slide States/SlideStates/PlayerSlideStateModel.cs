using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerSlideStateModel", menuName = "ScriptableObjects/Player/PlayerStates/PlayerSlideState/PlayerSlideStateModel")]
    public class PlayerSlideStateModel : ScriptableObject
    {
        [Min(0.1f)] public float slideTime = 1.0f;
        [Min(0.01f)] public float maxSlideSpeed = 20.0f;
        [Min(0.01f)] public float minSlideSpeed = 5.0f;
        [Min(0.0f)] public float slideFriction = 5.0f;
    }
}
