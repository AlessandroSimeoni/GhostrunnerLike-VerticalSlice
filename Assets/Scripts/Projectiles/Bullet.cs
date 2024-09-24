using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace Projectiles
{
    public class Bullet : MonoBehaviour
    {
        public BulletModel bulletModel = null;
        [Header("VFX")]
        [SerializeField] private ParticleSystem bulletParticle = null;
        [SerializeField] private TrailRenderer trailRenderer = null;
        [SerializeField] private ParticleSystem impactParticle = null;
        public IObjectPool<Bullet> objectPool { get; set; } = null;
        public bool parried { get; private set; } = false;
        public bool returningFromPlayer { get; set; } = false;

        private Coroutine flyTimeCoroutine = null;
        private float velocity = 1.0f;
        private float maxFlyTime = 0.0f;
        private RaycastHit hit;
        private bool hasHit = false;
        private Vector3 originPosition = Vector3.zero;

        private void Update()
        {
            if (hasHit) 
                return;

            transform.position += transform.forward * velocity * Time.deltaTime;

            if (Physics.Raycast(transform.position, transform.forward, out hit, 1.0f))
            {
                hasHit = true;
                if (flyTimeCoroutine != null)
                    StopCoroutine(flyTimeCoroutine);

                HandleCollision().Forget();
            }
        }

        public void Shoot(float velocity, float maxFlyTime)
        {
            trailRenderer.Clear();
            this.velocity = velocity;
            this.maxFlyTime = maxFlyTime;
            originPosition = transform.position;
            flyTimeCoroutine = StartCoroutine(MaxFlyTimeCoroutine());
            hasHit = false;
        }

        private IEnumerator MaxFlyTimeCoroutine()
        {
            bulletParticle.Play();
            yield return new WaitForSeconds(maxFlyTime);
            objectPool.Release(this);
        }

        private async UniTask HandleCollision()
        {
            transform.position = hit.point;

            Rigidbody targetRb = hit.transform.gameObject.GetComponent<Rigidbody>();
            if (targetRb != null)
                targetRb.AddForceAtPosition(transform.forward * bulletModel.impactForce, hit.point, ForceMode.Impulse);

            IHitable hitableTarget = (IHitable)hit.transform.gameObject.GetComponent(typeof(IHitable));
            if (hitableTarget != null)
                hitableTarget.Hit(this);

            if (parried)
            {
                parried = false;
                return;
            }
            
            bulletParticle.Stop();
            impactParticle.Play();

            await UniTask.WaitForSeconds(impactParticle.main.duration);
            trailRenderer.Clear();

            objectPool.Release(this);
        }

        public void Parry()
        {
            parried = true;

            if (flyTimeCoroutine != null)
                StopCoroutine(flyTimeCoroutine);

            transform.forward = originPosition - transform.position;
            Shoot(velocity * bulletModel.velocityMultiplier, maxFlyTime);
            returningFromPlayer = true;
        }
    }
}
