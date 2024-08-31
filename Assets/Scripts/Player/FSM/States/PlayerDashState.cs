using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Player
{
    public class PlayerDashState : PlayerState
    {
        [SerializeField] private PlayerState idleState = null;
        [SerializeField] private PlayerState movingState = null;
        [SerializeField] private PlayerState boostedSlideState = null;
        [SerializeField] private PlayerState fallingState = null;
        [Header("DashSlider")]
        [SerializeField] private Slider dashSlider = null;
        [SerializeField] private Image sliderBGImage = null;
        [SerializeField] private Color sliderDefaultBGColor = Color.black;
        [SerializeField] private Color sliderEmptyBGColor = Color.red;
        [SerializeField] private float sliderBGFlashFrequence = 2.0f;

        private InputAction slideAction = null;
        private Vector3 dashDirection = Vector3.zero;
        private float currentTime = 0.0f;
        private float targetTime = 0.0f;
        private UniTask sliderRefillTask;

        public bool dashReady { get; private set; } = true;

        public override void Init(PlayerCharacter player, PlayerStateController controller)
        {
            base.Init(player, controller);
            targetTime = player.playerModel.dashDistance / player.playerModel.dashSpeed;
            slideAction = player.controls.Player.Crouch;
            dashSlider.maxValue = player.playerModel.dashSliderSize;
            dashSlider.value = dashSlider.maxValue;
        }

        public override async UniTask Enter()
        {
            Debug.Log("DASH!");
            dashDirection = (player.movementDirection == Vector3.zero) ? player.transform.forward : player.movementDirection;
            currentTime = 0.0f;

            dashSlider.value = Mathf.Clamp(dashSlider.value - player.playerModel.dashUsagePrice, 0.0f, dashSlider.maxValue);
            if (sliderRefillTask.Status.IsCompleted())
                sliderRefillTask = DashRefill();

            await UniTask.NextFrame();
            
        }

        public override void Tick()
        {
            if (currentTime >= targetTime)
                return;

            if (slideAction.triggered && player.groundCheck.isGrounded)
            {
                controller.ChangeState(boostedSlideState).Forget();
                return;
            }

            currentTime += Time.deltaTime;

            if (currentTime >= targetTime)
            {
                if (player.groundCheck.isGrounded)
                    controller.ChangeState((player.movementDirection == Vector3.zero) ? idleState : movingState).Forget();
                else
                    controller.ChangeState(fallingState).Forget();

                return;
            }

            ((PlayerMovementStateController)controller).characterController.Move(dashDirection * player.playerModel.dashSpeed * Time.deltaTime);
        }

        private async UniTask DashRefill()
        {
            while (dashSlider.value < dashSlider.maxValue)
            {
                if (dashSlider.value == 0)
                {
                    dashReady = false;
                    await SliderBackgroundEmptyFlash(player.playerModel.dashSliderWaitTimeWhenZero);
                }

                dashSlider.value += player.playerModel.dashSliderRefillSpeed * Time.deltaTime;

                if (!dashReady)
                {

                    dashReady = true;
                }

                await UniTask.NextFrame();
            }

            dashSlider.value = dashSlider.maxValue;
        }

        private async UniTask SliderBackgroundEmptyFlash(float time)
        {
            float currentTime = 0.0f;
            float phase = 0.0f;

            while (currentTime < time)
            {
                phase = (Mathf.Sin(2 * Mathf.PI * sliderBGFlashFrequence * currentTime - Mathf.PI/2 ) + 1) / 2;         // - Mathf.PI/2 --> in this way the sin value will start from zero; without this, at time 0, the sin would evaluate to 0.5 once normalized
                sliderBGImage.color = Color.Lerp(sliderDefaultBGColor, sliderEmptyBGColor, phase);
                currentTime += Time.deltaTime;

                await UniTask.NextFrame();
            }

            sliderBGImage.color = sliderDefaultBGColor;
        }
    }
}
