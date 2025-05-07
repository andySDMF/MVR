using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVR.Paths
{
    public class PathFollow : MonoBehaviour
    {
        [SerializeField]
        private  float speed = 1;
        [SerializeField]
        private Path path;

        private float m_posiiton = 0.0f;

        private void Start()
        {
            if (path != null)
            {
                m_posiiton = path.SetObjectPositionAlongPath(m_posiiton, transform);
            }
        }

        private void Update()
        {
            if(path != null)
            {
                m_posiiton = path.SetObjectPositionAlongPath(m_posiiton + (speed / 1000) * Time.deltaTime, transform);
            }
        }

    }
}
