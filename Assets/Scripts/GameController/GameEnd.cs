using Cysharp.Threading.Tasks;
using InputControls;
using SceneLoad;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameController
{
    public class GameEnd : MonoBehaviour
    {
        [Header("Canvas transition")]
        [SerializeField] private Canvas hudCanvas = null;
        [SerializeField] private CanvasGroup gameEndCanvasGroup = null;
        [SerializeField, Min(0.0f)] private float speed = 2.0f;
        [Header("Target scene")]
        [SerializeField, Min(0)] private int targetSceneIndex = 0;

        private Controls controls = null;

        private void Start()
        {
            controls = new Controls();
            controls.GameEnd.Continue.performed += ProcessContinue;
        }

        public void EnterGameEnd() => GameEndProcedure().Forget();

        private async UniTask GameEndProcedure()
        {
            hudCanvas.enabled = false;
            gameEndCanvasGroup.alpha = 0.0f;
            gameEndCanvasGroup.GetComponent<Canvas>().enabled = true;
            controls.GameEnd.Enable();

            while (Time.timeScale > 0.01f)
            {
                if (gameEndCanvasGroup == null)
                    return;

                Time.timeScale -= Time.deltaTime * speed;
                gameEndCanvasGroup.alpha += Time.deltaTime * speed;
                await UniTask.NextFrame();
            }

            Time.timeScale = 0.0f;
            gameEndCanvasGroup.alpha = 1.0f;
        }

        private void ProcessContinue(InputAction.CallbackContext context)
        {
            SceneLoader.instance.LoadScene(targetSceneIndex);
            controls.GameEnd.Disable();
        }
    }
}
