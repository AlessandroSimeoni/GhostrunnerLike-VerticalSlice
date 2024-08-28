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
        [Header("DashSlider")]
        [SerializeField] private Slider dashSlider = null;
        [SerializeField] private Image sliderFillImage = null;
        [SerializeField] private Color sliderFullColor = Color.white;
        [SerializeField] private Color sliderChargingColor = Color.gray;

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
                controller.ChangeState((player.movementDirection == Vector3.zero) ? idleState : movingState).Forget();
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
                    await UniTask.WaitForSeconds(player.playerModel.dashSliderWaitTimeWhenZero);
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
    }
}
