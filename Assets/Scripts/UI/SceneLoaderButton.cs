using SceneLoad;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    [RequireComponent(typeof(Button))]
    public class SceneLoaderButton : MonoBehaviour
    {
        [SerializeField, Min(0)] private int sceneBuildIndex = 0;

        private Button button = null;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void Start()
        {
            button.onClick.AddListener(LoadScene);
        }

        private void LoadScene()
        {
            SceneLoader.instance.LoadScene(sceneBuildIndex);
        }
    }
}
