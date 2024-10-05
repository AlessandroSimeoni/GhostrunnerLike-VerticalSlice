using Player;
using UnityEngine;

namespace Environment
{
    public class Trampoline : MonoBehaviour
    {
        [SerializeField] private TrampolineModel trampolineModel = null;
        [SerializeField, Min(0)] private int modelIndex = 0;

        private void OnTriggerEnter(Collider other)
        {
            PlayerCharacter player = other.GetComponent<PlayerCharacter>();

            if (player != null)
            {
                Vector3 direction = transform.up;
                direction.y = 0;
                player.TrampolineJump(trampolineModel.trampolineJumpModel[modelIndex], direction.normalized);
            }
        }
    }
}
