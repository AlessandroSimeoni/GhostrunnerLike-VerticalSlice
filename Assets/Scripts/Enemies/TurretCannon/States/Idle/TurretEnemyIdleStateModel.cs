using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "TurretEnemyIdleStateModel", menuName = "ScriptableObjects/Enemies/Turret/TurretEnemyIdleStateModel")]
    public class TurretEnemyIdleStateModel : ScriptableObject
    {
        public Vector3 idleRotation = Vector3.zero;
        public float toIdleRotationSpeed = 45.0f;
    }
}
