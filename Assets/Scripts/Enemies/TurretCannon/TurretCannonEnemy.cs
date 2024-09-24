using Architecture;
using Cysharp.Threading.Tasks;
using Player;
using Projectiles;
using UnityEngine;
using UnityEngine.Pool;

namespace Enemy
{
    [RequireComponent(typeof(SphereCollider))]
    public class TurretCannonEnemy : MonoBehaviour
    {
        [SerializeField] private EnemyHitablePart[] enemyParts = new EnemyHitablePart[0];
        [SerializeField] private TurretCannonEnemyStateController stateController = null;
        [Header("Bullets")]
        [SerializeField] private Transform shootPointTransform = null;
        [SerializeField] private TurretModel turretModel = null;
        [SerializeField] private Transform bulletsParentTransform = null;
        [SerializeField] private Bullet bulletPrefab = null;
        [SerializeField, Min(1)] private int bulletsPoolDefaultSize = 20;
        [SerializeField, Min(1)] private int bulletsPoolMaxSize = 50;
        [Header("Views")]
        [SerializeField] private BulletDestructionView bulletDestructionView = null;
        [SerializeField] private CutDestructionView cutDestructionView = null;

        private IObjectPool<Bullet> bulletsPool;

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
            bulletsPool = new ObjectPool<Bullet>(CreateBullet, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, true, bulletsPoolDefaultSize, bulletsPoolMaxSize);
        }

        private void Start()
        {
            stateController.Init(this);

            foreach (EnemyHitablePart part in enemyParts)
            {
                part.OnHit += BulletHit;
                part.OnCutEvent += CutDestruction;
            }
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

        private Bullet CreateBullet()
        {
            Bullet bullet = Instantiate(bulletPrefab, bulletsParentTransform);
            bullet.objectPool = bulletsPool;
            return bullet;
        }

        private void OnGetFromPool(Bullet bullet)
        {
            bullet.gameObject.SetActive(true);
            bullet.transform.SetPositionAndRotation(shootPointTransform.position, shootPointTransform.rotation);
            bullet.returningFromPlayer = false;
            bullet.Shoot(turretModel.bulletVelocity, turretModel.bulletMaxFlyTime);
        }

        private void OnReleaseToPool(Bullet bullet)
        {
            bullet.gameObject.SetActive(false);
        }

        private void OnDestroyPooledObject(Bullet bullet)
        {
            Destroy(bullet.gameObject);
        }

        public void FireBullet() => bulletsPool.Get();

        public void BulletHit(Bullet bullet)
        {
            if (!bullet.returningFromPlayer)
                return;

            bulletDestructionView.ChangeView().Forget();
            stateController.Death();
        }

        public void CutDestruction()
        {
            cutDestructionView.ChangeView().Forget();
            stateController.Death();
        }
    }
}
