using UnityEngine;

namespace Projectiles
{
    [CreateAssetMenu(fileName = "BulletModel", menuName = "ScriptableObjects/Projectiles/BulletModel")]
    public class BulletModel : ScriptableObject
    {
        public float impactForce = 5.0f;
        public float playerStaminaConsume = 1.0f;
        [Header("Parry options")]
        [Min(0.0f)] public float velocityMultiplier = 1.0f;
    }
}
