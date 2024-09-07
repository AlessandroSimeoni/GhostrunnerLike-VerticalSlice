using UnityEngine;

namespace Enemy
{
    public class TurretCannonEnemy : BaseEnemy
    {
        [SerializeField] private TurretCannonEnemyStateController stateController = null;

        public delegate void PlayerRangeEvent();
        public event PlayerRangeEvent OnPlayerRange = null;
        public event PlayerRangeEvent OnPlayerNotInRange = null;

        public Transform playerTransform { get; private set; } = null;

        private void Start()
        {
            stateController.Init(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                playerTransform = other.transform;
                OnPlayerRange?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
                OnPlayerNotInRange?.Invoke();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 30.0f);
        }
    }
}
