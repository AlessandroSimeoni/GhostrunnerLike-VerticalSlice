using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerCharacter))]
    public class PlayerAttackStateController : PlayerStateController
    {
        public Sword sword = null;
    }
}
