using Player;
using UnityEngine;
using UnityEngine.Pool;

namespace Enemy
{
    [RequireComponent(typeof(SphereCollider))]
    public class TurretCannonEnemy : BaseEnemy
    {
        [SerializeField] private TurretCannonEnemyStateController stateController = null;
        [Header("Bullets")]
        [SerializeField] private Transform shootPointTransform = null;
        [SerializeField] private TurretCannonBulletModel bulletModel = null;
        [SerializeField] private Transform bulletsParentTransform = null;
        [SerializeField] private TurretCannonBullet bulletPrefab = null;
        [SerializeField, Min(1)] private int bulletsPoolDefaultSize = 20;
        [SerializeField, Min(1)] private int bulletsPoolMaxSize = 50;

        private IObjectPool<TurretCannonBullet> bulletsPool;

        public delegate void PlayerRangeEvent();
        public event PlayerRangeEvent OnPlayerRange = null;
        public event PlayerRangeEvent OnPlayerNotInRange = null;

        public PlayerCharacter player { get; private set; } = null;
        public SphereCollider sphereCollider { get; private set; } = null;

        public const string PLAYER_LAYER = "Player";

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 30.0f);
        }

        private void Awake()
        {
            sphereCollider = GetComponent<SphereCollider>();
            bulletsPool = new ObjectPool<TurretCannonBullet>(CreateBullet, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, true, bulletsPoolDefaultSize, bulletsPoolMaxSize);
        }

        private void Start()
        {
            stateController.Init(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(PLAYER_LAYER))
            {
                player = other.GetComponent<PlayerCharacter>();
                OnPlayerRange?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(PLAYER_LAYER))
            {
                player = null;
                OnPlayerNotInRange?.Invoke();
            }
        }

        private TurretCannonBullet CreateBullet()
        {
            TurretCannonBullet bullet = Instantiate(bulletPrefab, bulletsParentTransform);
            bullet.objectPool = bulletsPool;
            return bullet;
        }

        private void OnGetFromPool(TurretCannonBullet pooledObject)
        {
            pooledObject.gameObject.SetActive(true);
        }

        private void OnReleaseToPool(TurretCannonBullet pooledObject)
        {
            pooledObject.gameObject.SetActive(false);
        }

        private void OnDestroyPooledObject(TurretCannonBullet pooledObject)
        {
            Destroy(pooledObject.gameObject);
        }

        public void FireBullet()
        {
            TurretCannonBullet bullet = bulletsPool.Get();
            bullet.transform.SetPositionAndRotation(shootPointTransform.position, shootPointTransform.rotation);
            bullet.rb.AddForce(bullet.transform.forward * bulletModel.velocity, ForceMode.VelocityChange);
            bullet.StartFlyTime(bulletModel.maxFlyTime);
        }
    }
}
