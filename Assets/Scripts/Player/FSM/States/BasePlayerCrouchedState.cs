using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class BasePlayerCrouchedState : PlayerState
    {
        protected float originalCameraHeightOffset = 0.0f;

        public override void Init(PlayerCharacter player, PlayerStateController controller)
        {
            base.Init(player, controller);
            originalCameraHeightOffset = player.fpCamera.heightOffset;
        }

        public override async UniTask Enter()
        {
            float targetCameraHeightOffset = originalCameraHeightOffset - (player.playerModel.defaultCharacterHeight - player.playerModel.crouchedCharacterHeight);
            await SmoothCameraOffset(originalCameraHeightOffset, targetCameraHeightOffset);

            ((PlayerMovementStateController)controller).characterController.height = player.playerModel.crouchedCharacterHeight;
            ((PlayerMovementStateController)controller).characterController.center = Vector3.up * player.playerModel.crouchedCharacterHeight / 2;
        }

        public override async UniTask Exit()
        {
            await SmoothCameraOffset(player.fpCamera.heightOffset, originalCameraHeightOffset);

            ((PlayerMovementStateController)controller).characterController.height = player.playerModel.defaultCharacterHeight;
            ((PlayerMovementStateController)controller).characterController.center = Vector3.up * player.playerModel.defaultCharacterHeight / 2;
        }

        protected async UniTask SmoothCameraOffset(float startValue, float targetValue)
        {
            float currentTime = 0.0f;
            float interpolation = 0.0f;
            while (true)
            {
                currentTime += Time.deltaTime;
                interpolation = currentTime / player.playerModel.crouchedTransitionTime;
                player.fpCamera.heightOffset = Mathf.Lerp(startValue, targetValue, interpolation);

                if (player.fpCamera.heightOffset == targetValue)
                    break;

                await UniTask.NextFrame();
            }
        }
    }
}
