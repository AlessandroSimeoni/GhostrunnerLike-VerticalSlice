using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerWallRunStateModel", menuName = "ScriptableObjects/Player/PlayerStates/PlayerWallRunStateModel")]
    public class PlayerWallRunStateModel : ScriptableObject
    {
        [Min(0.01f)] public float wallRunSpeed = 10.0f;
        [Range(0.0f, 1.0f)] public float forwardDotFallThreshold = 0.5f;
        [Min(0.0f)] public float minJumpDirectionAngle = 30.0f;
        [Range(0.0f, 45.0f)] public float cameraTiltAngle = 15.0f;
        [Min(0.0f)] public float cameraTiltChangeSpeed = 0.5f;
        [Header("Wall Check")]
        public LayerMask wallCheckLayers = new LayerMask();
        [Min(0.05f)] public float wallRayLenght = 1.0f;
        [Min(0.0f)] public float wallRayHeightOffset = 1.0f;
    }
}
