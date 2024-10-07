using UnityEngine;

namespace UserInterface
{
    public class GamePauseCanvas : MonoBehaviour
    {
        private void OnEnable() => Time.timeScale = 0;
        private void OnDisable() => Time.timeScale = 1;
    }
}
