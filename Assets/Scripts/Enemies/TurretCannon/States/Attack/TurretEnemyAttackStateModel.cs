using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "TurretEnemyAttackStateModel", menuName = "ScriptableObjects/Enemies/Turret/TurretEnemyAttackStateModel")]
    public class TurretEnemyAttackStateModel : ScriptableObject
    {
        public float rotationSpeed = 90.0f;
        public float barrelTargetOffsetHeight = 1.75f;
    }
}
