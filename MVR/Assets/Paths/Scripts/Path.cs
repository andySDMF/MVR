using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MVR.Paths;

namespace MVR.Paths
{
    public class Path : MonoBehaviour
    {
        [SerializeField]
        protected Transform startPosition;

        [SerializeField]
        protected List<Transform> points;

        [SerializeField]
        protected Transform endPosition;

        [SerializeField]
        protected List<Vector3> tangents = new List<Vector3>();

        [SerializeField]
        protected bool closePath = false;

        protected Transform[] m_waypoints;

        private void Awake()
        {
            m_waypoints = new Transform[points.Count + 2];

            for(int i = 0; i < m_waypoints.Length; i++)
            {
                if (i == 0)
                {
                    m_waypoints[i] = startPosition;
                }
                else if (i == m_waypoints.Length - 1)
                {
                    m_waypoints[i] = endPosition;
                }
                else
                {
                    m_waypoints[i] = points[i - 1];
                }
            }
        }

        /// <summary>
        /// Returns max number of positions along the path
        /// </summary>
        public virtual float MaxPos
        {
            get
            {
                int count = 0;

                if (!Application.isPlaying)
                {
                    count = tangents.Count - 1;
                    if (count < 1)
                        return 0;

                    return closePath ? count + 1 : count;
                }

                count = m_waypoints.Length - 1;
                if (count < 1)
                    return 0;

                return closePath ? count + 1 : count;
            }
        }

        /// <summary>
        /// Called to place and object along the spline at a given point
        /// Max number of points is the paht length
        /// </summary>
        /// <param name="distanceAlongPath"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public float SetObjectPositionAlongPath(float distanceAlongPath, Transform obj)
        {
            float pos = 0f;

            if (obj != null)
            {
                pos = StandardizePathDistance(distanceAlongPath, m_waypoints);
                obj.position = EvaluatePosition(pos, m_waypoints, false);
                obj.rotation = EvaluateOrientation(pos, m_waypoints);
            }

            return pos;
        }

#if UNITY_EDITOR

        public void OnDrawGizmosSelected()
        {
            OnDrawGizmosEvent();
        }

        protected void OnDrawGizmosEvent()
        {
            //this should now draw the line based on points & tagent
            if (startPosition != null && endPosition != null)
            {
                Transform[] trans = new Transform[points.Count + 2];

                for (int i = 0; i < trans.Length; i++)
                {
                    if (i == 0)
                    {
                        trans[i] = startPosition;
                    }
                    else if (i == trans.Length - 1)
                    {
                        trans[i] = endPosition;
                    }
                    else
                    {
                        trans[i] = points[i - 1];
                    }
                }

                float step = 1f / 20.0f;
                Vector3 lastPos = EvaluatePosition(0.0f, trans);
                float tEnd = MaxPos + step / 2;
                for (float t = 0.0f + step; t <= tEnd; t += step)
                {
                    Gizmos.color = Color.red;
                    Vector3 p = EvaluatePosition(t, trans);
                    Gizmos.DrawLine(p, lastPos);

                    lastPos = p;
                }
            }

        }
#endif

        /// <summary>Get the orientation the curve at a point along the path.</summary>
        /// <param name="pos">Postion along the path.  Need not be normalized.</param>
        /// <returns>World-space orientation of the path, as defined by tangent, up, and roll.</returns>
        protected virtual Quaternion EvaluateOrientation(float pos, Transform[] waypoints, bool toWorldSpace = false)
        {
            Quaternion result = transform.rotation;

            if (waypoints.Length > 0)
            {
                float roll = 0;
                int indexA, indexB;
                pos = GetBoundingIndices(pos, out indexA, out indexB, waypoints);
                if (indexA == indexB)
                    roll = 0.0f;
                else
                {
                    float rollA = 0.0f;
                    float rollB = 0.0f;
                    if (indexB == 0)
                    {
                        // Special handling at the wraparound - cancel the spins
                        rollA = rollA % 360;
                        rollB = rollB % 360;
                    }
                    roll = Mathf.Lerp(rollA, rollB, pos - indexA);
                }

                Vector3 fwd = EvaluateTangent(pos, waypoints, toWorldSpace);

                if (!fwd.AlmostZero())
                {
                    Vector3 up = transform.rotation * Vector3.up;
                    Quaternion q = Quaternion.LookRotation(fwd, up);
                    result = q * Quaternion.AngleAxis(roll, Vector3.forward);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a standard distance along a path between 0 and max number of points
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="waypoints"></param>
        /// <returns></returns>
        protected float StandardizePathDistance(float distance, Transform[] waypoints)
        {
            float length = MaxPos;

            if (length < Vector3.kEpsilon)
                return 0;
            if (closePath)
            {
                distance = distance % length;
                if (distance < 0)
                    distance += length;
            }

            if (length < Vector3.kEpsilon)
                return 0;

            return Mathf.Clamp(distance, 0, length);
        }

        /// <summary>
        /// Returns the modular position along the path between 0 and max numbe rof points
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="waypoints"></param>
        /// <returns></returns>
        protected float StandardizePos(float pos, Transform[] waypoints)
        {
            if (closePath && MaxPos > 0)
            {
                pos = pos % MaxPos;
                if (pos < 0)
                    pos += MaxPos;
                return pos;
            }

            return Mathf.Clamp(pos, 0, MaxPos);
        }

        /// <summary>
        /// Wraps a position wihtin the bounding area of the spline
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="indexA"></param>
        /// <param name="indexB"></param>
        /// <param name="waypoints"></param>
        /// <returns></returns>
        protected float GetBoundingIndices(float pos, out int indexA, out int indexB, Transform[] waypoints)
        {
            pos = StandardizePos(pos, waypoints);
            int rounded = Mathf.RoundToInt(pos);
            if (Mathf.Abs(pos - rounded) < UnityVectorExtensions.Epsilon)
                indexA = indexB = (rounded == waypoints.Length) ? 0 : rounded;
            else
            {
                indexA = Mathf.FloorToInt(pos);
                if (indexA >= waypoints.Length)
                {
                    pos -= waypoints.Length - 1;
                    indexA = 0;
                }
                indexB = Mathf.CeilToInt(pos);
                if (indexB >= waypoints.Length)
                    indexB = 0;
            }
            return pos;
        }

        /// <summary>Get a worldspace position of a point along the path</summary>
        /// <param name="pos">Postion along the path.  Need not be normalized.</param>
        /// <returns>World-space position of the point along at path at pos</returns>
        protected Vector3 EvaluatePosition(float pos, Transform[] waypoints, bool toWorldSpace = false)
        {
            Vector3 result = new Vector3();
            if (waypoints.Length == 0)
                result = transform.position;
            else
            {
                int indexA, indexB;
                pos = GetBoundingIndices(pos, out indexA, out indexB, waypoints);
                if (indexA == indexB)
                    result = waypoints[indexA].position;
                else
                {
                    // interpolate
                    Transform wpA = waypoints[indexA];
                    Transform wpB = waypoints[indexB];
                    result = SplineUitls.SplineHelp.Bezier3(pos - indexA,
                        waypoints[indexA].position, wpA.position + tangents[indexA],
                        wpB.position - tangents[indexB], wpB.position);
                }
            }

            return !toWorldSpace ? result : transform.TransformPoint(result);
        }

        /// <summary>Get the tangent of the curve at a point along the path.</summary>
        /// <param name="pos">Postion along the path.  Need not be normalized.</param>
        /// <returns>World-space direction of the path tangent.
        /// Length of the vector represents the tangent strength</returns>
        protected Vector3 EvaluateTangent(float pos, Transform[] waypoints, bool toWorldSpace = false)
        {
            Vector3 result = new Vector3();
            if (waypoints.Length == 0)
                result = transform.rotation * Vector3.forward;
            else
            {
                int indexA, indexB;
                pos = GetBoundingIndices(pos, out indexA, out indexB, waypoints);
                if (indexA == indexB)
                    result = Vector3.zero;
                else
                {
                    Transform wpA = waypoints[indexA];
                    Transform wpB = waypoints[indexB];

                    result = SplineUitls.SplineHelp.BezierTangent3(pos - indexA,
                        waypoints[indexA].position, wpA.position + tangents[indexA],
                        wpB.position - tangents[indexB], wpB.position);
                }
            }

            return !toWorldSpace ? result : transform.TransformDirection(result);
        }
    }
}
