using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerModel", menuName = "ScriptableObjects/Player/PlayerModel")]
    public class PlayerModel : ScriptableObject
    {
        public int hp = 1;
        public float movementSpeed = 5.0f;
        [Header("Jump")]
        public float jumpHeight = 1.5f;
        public float gravityMultiplier = 2.0f;
        public float midAirSpeed = 2.5f;
    }
}
