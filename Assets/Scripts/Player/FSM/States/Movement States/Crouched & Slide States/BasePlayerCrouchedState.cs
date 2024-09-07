using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class BasePlayerCrouchedState : PlayerState
    {
        [SerializeField] protected PlayerCrouchedStateModel crouchedModel = null;

        protected float originalCameraHeightOffset = 0.0f;

        public override void Init<T>(T entity, AStateController controller)
        {
            base.Init(entity, controller);
            originalCameraHeightOffset = player.fpCamera.heightOffset;
        }

        public override async UniTask Enter()
        {
            float targetCameraHeightOffset = originalCameraHeightOffset - (crouchedModel.defaultCharacterHeight - crouchedModel.crouchedCharacterHeight);
            await SmoothCameraOffset(originalCameraHeightOffset, targetCameraHeightOffset);

            player.characterController.height = crouchedModel.crouchedCharacterHeight;
            player.characterController.center = Vector3.up * crouchedModel.crouchedCharacterHeight / 2;
        }

        public override async UniTask Exit()
        {
            await SmoothCameraOffset(player.fpCamera.heightOffset, originalCameraHeightOffset);

            player.characterController.height = crouchedModel.defaultCharacterHeight;
            player.characterController.center = Vector3.up * crouchedModel.defaultCharacterHeight / 2;
        }

        protected async UniTask SmoothCameraOffset(float startValue, float targetValue)
        {
            float currentTime = 0.0f;
            float interpolation = 0.0f;
            while (true)
            {
                currentTime += Time.deltaTime;
                interpolation = currentTime / crouchedModel.crouchedTransitionTime;
                player.fpCamera.heightOffset = Mathf.Lerp(startValue, targetValue, interpolation);

                if (player.fpCamera.heightOffset == targetValue)
                    break;

                await UniTask.NextFrame();
            }
        }
    }
}
