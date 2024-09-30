using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerGrapplingHookStateModel", menuName = "ScriptableObjects/Player/PlayerStates/PlayerGrapplingHookStateModel")]
    public class PlayerGrapplingHookStateModel : ScriptableObject
    {
        [Min(0.0f)] public float maxSpeed = 80.0f;
        [Min(0.0f)] public float minSpeed = 20.0f;
        [Min(0.0f), Tooltip("Under this distance will be used the min speed")] public float minSpeedDistance = 3.0f;
        [Min(0.0f)] public float maxSpeedDistance = 30.0f;
        [Range(0.0f, 1.0f), Tooltip("Falling will begin once the player covers this percentage of distance")] public float fallRatioOverDistance = 0.7f;
        [Min(0.0f)] public float gravityMultiplier = 3.0f;
        [Min(0.0f)] public float forwardMovementWeight = 0.3f;
        [Min(0.0f)] public float midAirMovementSpeed = 7.0f;
        [Space]
        [Header("Line Renderer Animation")]
        [Min(2)] public int lineRendererVertexCount = 300;
        [Min(0.0f)] public float lineSpeed = 20.0f;
        [Min(0.0f)] public float sinWaveAmplitude = 2.0f;
        [Min(0.0f)] public float sinFrequency = 80.0f;
        [Min(0.0f)] public float cosWaveAmplitude = 2.0f;
        [Min(0.0f)] public float cosFrequency = 80.0f;
        [Min(0.0f)] public float waveDamping = 3.0f;
        [Min(1.0f)] public float timeSpeed = 3.0f;
    }
}
