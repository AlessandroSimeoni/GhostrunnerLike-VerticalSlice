using InputControls;
using Player;
using SceneLoad;
using UnityEngine;
using UnityEngine.InputSystem;
using UserInterface;

namespace GameController
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] protected PlayerCharacter player = null;
        [SerializeField] protected GameEnd gameOver = null;
        [SerializeField] protected GameEnd win = null;
        [SerializeField] protected GamePauseCanvas pauseCanvas = null;

        private Controls controls = null;

        protected virtual void Start()
        {
            controls = new Controls();
            SceneLoader.instance.OnLoadingCompleted += player.EnableControls;
            SceneLoader.instance.OnLoadingCompleted += EnablePauseControls;
            player.OnDeath += HandleGameOver;
        }

        protected void EnablePauseControls()
        {
            controls.GamePause.Enable();
            controls.GamePause.Pause.performed += EnablePause;
        }

        protected virtual void HandleGameOver()
        {
            controls.GamePause.Disable();
            gameOver.EnterGameEnd();
        }

        protected virtual void HandleWin()
        {
            controls.GamePause.Disable();
        }

        protected void EnablePause(InputAction.CallbackContext context)
        {
            pauseCanvas.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
            player.DisableControls();
            controls.GamePause.Pause.performed -= EnablePause;
            controls.GamePause.Pause.performed += DisablePause;
        }

        protected void DisablePause(InputAction.CallbackContext context)
        {
            pauseCanvas.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            player.EnableControls();
            controls.GamePause.Pause.performed -= DisablePause;
            controls.GamePause.Pause.performed += EnablePause;
        }

        protected void OnDestroy()
        {
            controls.GamePause.Pause.performed -= EnablePause;
            controls.GamePause.Pause.performed -= DisablePause;
            SceneLoader.instance.OnLoadingCompleted -= player.EnableControls;
            SceneLoader.instance.OnLoadingCompleted -= EnablePauseControls;
            controls.GamePause.Disable();
        }
    }
}
