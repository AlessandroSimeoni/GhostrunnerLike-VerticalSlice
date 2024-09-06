using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class BasePlayerCrouchedState : PlayerState
    {
        protected float originalCameraHeightOffset = 0.0f;
        protected float defaultCharacterHeight = 0.0f;
        protected float crouchedCharacterHeight = 0.0f;
        protected float crouchedTransitionTime = 0.0f;

        public override void Init<T>(T entity, AStateController controller)
        {
            base.Init(entity, controller);
            originalCameraHeightOffset = player.fpCamera.heightOffset;
            defaultCharacterHeight = ((PlayerCrouchedStateModel)stateModel).defaultCharacterHeight;
            crouchedCharacterHeight = ((PlayerCrouchedStateModel)stateModel).crouchedCharacterHeight;
            crouchedTransitionTime = ((PlayerCrouchedStateModel)stateModel).crouchedTransitionTime;
        }

        public override async UniTask Enter()
        {
            float targetCameraHeightOffset = originalCameraHeightOffset - (defaultCharacterHeight - crouchedCharacterHeight);
            await SmoothCameraOffset(originalCameraHeightOffset, targetCameraHeightOffset);

            player.characterController.height = crouchedCharacterHeight;
            player.characterController.center = Vector3.up * crouchedCharacterHeight / 2;
        }

        public override async UniTask Exit()
        {
            await SmoothCameraOffset(player.fpCamera.heightOffset, originalCameraHeightOffset);

            player.characterController.height = defaultCharacterHeight;
            player.characterController.center = Vector3.up * defaultCharacterHeight / 2;
        }

        protected async UniTask SmoothCameraOffset(float startValue, float targetValue)
        {
            float currentTime = 0.0f;
            float interpolation = 0.0f;
            while (true)
            {
                currentTime += Time.deltaTime;
                interpolation = currentTime / crouchedTransitionTime;
                player.fpCamera.heightOffset = Mathf.Lerp(startValue, targetValue, interpolation);

                if (player.fpCamera.heightOffset == targetValue)
                    break;

                await UniTask.NextFrame();
            }
        }
    }
}
