using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "TurretEnemyAttackStateModel", menuName = "ScriptableObjects/Enemies/Turret/TurretEnemyAttackStateModel")]
    public class TurretEnemyAttackStateModel : ScriptableObject
    {
        [Header("Target following")]
        [Min(0.0f)] public float rotationSpeed = 90.0f;
        public float barrelTargetOffsetHeight = 1.75f;
        [Header("Fire")]
        [Min(0.0f)] public float fireFrequency = 3.0f;
        [Range(0.0f, 1.0f)] public float fireAlignmentThreshold = 0.9f;
    }
}
