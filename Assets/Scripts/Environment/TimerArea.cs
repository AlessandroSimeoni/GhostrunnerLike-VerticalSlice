using Player;
using UnityEngine;
using Utilities;

namespace Environment
{
    public class TimerArea : MonoBehaviour
    {
        [SerializeField] private bool stopTimer = false;
        [SerializeField] MyTimer timer = null;

        private void OnTriggerEnter(Collider other)
        {
            PlayerCharacter player = other.GetComponent<PlayerCharacter>();

            if (player != null)
            {
                if (stopTimer)
                    timer.StopTimer();
                else
                    timer.StartTimer();

                gameObject.SetActive(false);
            }
        }
    }
}
