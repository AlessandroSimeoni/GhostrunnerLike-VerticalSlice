using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "TurretCannonBulletModel", menuName = "ScriptableObjects/Enemies/Turret/TurretCannonBulletModel")]
    public class TurretCannonBulletModel : ScriptableObject
    {
        [Min(0.1f)] public float velocity = 10.0f;
        [Min(0.05f)] public float maxFlyTime = 10.0f;
    }
}
