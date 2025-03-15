using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace Projectiles
{
    public class Bullet : MonoBehaviour
    {
        public BulletModel bulletModel = null;
        [SerializeField] private LayerMask rayMask;
        [SerializeField] private int maxRaySegmentLength = 3;
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

            Vector3 startPos = transform.position;
            Vector3 movement = transform.forward * velocity * Time.deltaTime;
            Vector3 endPos = startPos + movement;

            int steps = Mathf.CeilToInt(movement.magnitude / maxRaySegmentLength);
            Vector3 stepSize = movement / steps;

            for (int i = 0; i < steps; i++)
            {
                if (Physics.Raycast(startPos, transform.forward, out hit, stepSize.magnitude, rayMask))
                {
                    hasHit = true;
                    if (flyTimeCoroutine != null)
                        StopCoroutine(flyTimeCoroutine);

                    HandleCollision().Forget();
                    return;
                }

                startPos += stepSize;
            }

            transform.position = endPos;
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

            if (trailRenderer != null)
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
