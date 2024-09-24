using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "TurretModel", menuName = "ScriptableObjects/Enemies/Turret/TurretModel")]
    public class TurretModel : ScriptableObject
    {
        [Min(0.1f)] public float bulletVelocity = 10.0f;
        [Min(0.05f)] public float bulletMaxFlyTime = 10.0f;
    }
}
