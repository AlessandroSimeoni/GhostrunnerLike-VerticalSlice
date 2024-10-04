using Player;
using UnityEngine;

namespace Environment
{
    [RequireComponent(typeof(Collider))]
    public class DeathArea : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            PlayerCharacter player = other.GetComponent<PlayerCharacter>();
            if (player != null)
                player.Death();
        }
    }
}
