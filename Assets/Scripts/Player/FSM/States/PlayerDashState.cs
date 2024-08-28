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
        public bool dashReady { get; private set; } = true;

        public override void Init(PlayerCharacter player, PlayerStateController controller)
        {
            base.Init(player, controller);
            targetTime = player.playerModel.dashDistance / player.playerModel.dashSpeed;
            slideAction = player.controls.Player.Crouch;
        }

        public override async UniTask Enter()
        {
            dashDirection = (player.movementDirection == Vector3.zero) ? player.transform.forward : player.movementDirection;
            currentTime = 0.0f;
            dashReady = false;
            dashSlider.value = 0.0f;
            await UniTask.NextFrame();
            DashCooldown(player.playerModel.dashCooldown).Forget();
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

        private async UniTask DashCooldown(float cooldownSeconds)
        {
            float remainingSeconds = cooldownSeconds;
            sliderFillImage.color = sliderChargingColor;

            while (remainingSeconds > 0)
            {
                dashSlider.value = 1 - (remainingSeconds / cooldownSeconds);
                remainingSeconds -= Time.deltaTime;
                await UniTask.NextFrame();
            }

            dashSlider.value = 1.0f;
            sliderFillImage.color = sliderFullColor;

            dashReady = true;
        }
    }
}
