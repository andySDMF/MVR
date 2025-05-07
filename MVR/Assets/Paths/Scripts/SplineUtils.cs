using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVR.Paths
{
    public static class SplineUitls
    {
        public static class SplineHelp
        {
            /// <summary>Compute the value of a 4-point 3-dimensional bezier spline</summary>
            /// <param name="t">How far along the spline (0...1)</param>
            /// <param name="p0">First point</param>
            /// <param name="p1">First tangent</param>
            /// <param name="p2">Second point</param>
            /// <param name="p3">Second tangent</param>
            /// <returns>The spline value at t</returns>
            public static Vector3 Bezier3(
                float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
            {
                t = Mathf.Clamp01(t);
                float d = 1f - t;
                return d * d * d * p0 + 3f * d * d * t * p1
                    + 3f * d * t * t * p2 + t * t * t * p3;
            }

            /// <summary>Compute the tangent of a 4-point 3-dimensional bezier spline</summary>
            /// <param name="t">How far along the spline (0...1)</param>
            /// <param name="p0">First point</param>
            /// <param name="p1">First tangent</param>
            /// <param name="p2">Second point</param>
            /// <param name="p3">Second tangent</param>
            /// <returns>The spline tangent at t</returns>
            public static Vector3 BezierTangent3(
                float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
            {
                t = Mathf.Clamp01(t);
                return (-3f * p0 + 9f * p1 - 9f * p2 + 3f * p3) * (t * t)
                    + (6f * p0 - 12f * p1 + 6f * p2) * t
                    - 3f * p0 + 3f * p1;
            }

        }
    }
}