using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    [RequireComponent(typeof(Button))]
    public class QuitButton : MonoBehaviour
    {
        private void Start()
        {
            gameObject.GetComponent<Button>().onClick.AddListener(QuitGame);
        }

        public void QuitGame() => Application.Quit();
    }
}
