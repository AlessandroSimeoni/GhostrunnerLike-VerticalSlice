using UnityEngine;
using UnityEngine.UI;
using Player;

namespace Environment
{
    [RequireComponent(typeof(SphereCollider))]
    public class Hook : MonoBehaviour
    {
        [SerializeField] private Image QKeyImage = null;
        [SerializeField] private LayerMask rayMask;
        [SerializeField] private MeshRenderer meshRenderer = null;
        [SerializeField] private Material inactiveHookMaterial = null;
        [SerializeField] private Material activeHookMaterial = null;
        [SerializeField] private GameObject activeHookLight = null;

        private PlayerCharacter player = null;
        private bool playerInRange = false;
        private bool hookVisible = false;
        private RaycastHit hit;
        private SphereCollider sc = null;
        private bool interacted = false;

        private const string PLAYER_LAYER = "Player";

        private void Start()
        {
            DisableHook();
            sc = GetComponent<SphereCollider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer(PLAYER_LAYER))
            {
                if (player == null)
                    player = other.GetComponent<PlayerCharacter>();

                playerInRange = true;
                interacted = false;
                player.OnGrapplingHookUsed += HandleHookInteraction;
            }

            ToggleHook();
        }

        private void HandleHookInteraction()
        {
            interacted = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(PLAYER_LAYER))
            {
                playerInRange = false;
                player.OnGrapplingHookUsed -= HandleHookInteraction;
            }

            ToggleHook();
        }

        private void Update()
        {
            if (playerInRange)
            {
                if (!interacted
                    && hookVisible
                    && Physics.Raycast(transform.position, (player.transform.position + player.transform.up * player.characterController.center.y) - transform.position, out hit, sc.radius, rayMask)
                    && hit.transform.gameObject.layer == LayerMask.NameToLayer(PLAYER_LAYER))
                    EnableHook();
                else
                    DisableHook();
            }
        }

        private void OnBecameVisible()
        {
            hookVisible = true;
            interacted = false;
            ToggleHook();
        }

        private void OnBecameInvisible()
        {
            hookVisible = false;
            ToggleHook();
        }

        private void ToggleHook()
        {
            if (playerInRange && hookVisible)
                EnableHook();
            else
                DisableHook();
        }

        private void DisableHook()
        {
            if (player != null && player.hookTransform == transform)
                player.hookTransform = null;
            ToggleUIImage(false);
            meshRenderer.material = inactiveHookMaterial;
            activeHookLight.SetActive(false);
        }

        private void EnableHook()
        {
            if (player != null)
                player.hookTransform = transform;
            ToggleUIImage(true);
            meshRenderer.material = activeHookMaterial;
            activeHookLight.SetActive(true);
        }

        private void ToggleUIImage(bool value)
        {
            if (QKeyImage == null)
                return;

            QKeyImage.enabled = value;
        }
    }
}
