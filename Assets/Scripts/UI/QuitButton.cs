using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    [RequireComponent(typeof(Button))]
    public class QuitButton : MonoBehaviour
    {
        public void QuitGame() => Application.Quit();
    }
}
