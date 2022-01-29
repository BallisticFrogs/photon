using UnityEngine;

namespace DefaultNamespace
{
    public static class Utils
    {
        public static Vector2 Rotate(this Vector2 v, float angleInDegs)
        {
            float angleInRads = angleInDegs * Mathf.Deg2Rad;
            return new Vector2(
                v.x * Mathf.Cos(angleInRads) - v.y * Mathf.Sin(angleInRads),
                v.x * Mathf.Sin(angleInRads) + v.y * Mathf.Cos(angleInRads)
            );
        }
    }
}