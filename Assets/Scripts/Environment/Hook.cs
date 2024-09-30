using UnityEngine;
using UnityEngine.UI;
using Player;
using System;

namespace Environment
{
    [RequireComponent(typeof(SphereCollider))]
    public class Hook : MonoBehaviour
    {

        [SerializeField] private Image QKeyImage = null;
        [SerializeField] private LayerMask rayMask;

        private PlayerCharacter player = null;
        private bool playerInRange = false;
        private bool hookVisible = false;
        private RaycastHit hit;
        private SphereCollider sc = null;
        private bool interacted = false;

        private const string PLAYER_LAYER = "Player";

        private void Start()
        {
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
            if (player != null)
                player.hookTransform = null;
            ToggleUIImage(false);
        }

        private void EnableHook()
        {
            if (player != null)
                player.hookTransform = transform;
            ToggleUIImage(true);
        }

        private void ToggleUIImage(bool value)
        {
            if (QKeyImage == null)
                return;

            QKeyImage.enabled = value;
        }
    }
}
