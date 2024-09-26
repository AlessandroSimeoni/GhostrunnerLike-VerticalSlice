using UnityEngine;

namespace Utilities
{
    public static class MyUtility
    {
        public static bool SameDirection(Vector3 leftVector, Vector3 rightVector)
        {
            return Vector3.Dot(leftVector, rightVector) > 0;
        }
    }
}
