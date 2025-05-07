using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVR.Boats
{
    [RequireComponent(typeof(Rigidbody))]
    public class BoatController : MonoBehaviour
    {
        [SerializeField]
        protected Transform motor;
        [SerializeField]
        protected float steerPower = 500f;
        [SerializeField]
        protected float power = 500f;

        protected Rigidbody m_rigidbody;
        protected Quaternion m_startRotation;

        public float Velocity { get { return m_rigidbody.velocity.magnitude; } }

        public Vector3 ForwardDirection { get { return transform.forward; } }

        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_startRotation = motor.rotation;
        }

        private void FixedUpdate()
        {
            var steer = 0;

            if(m_rigidbody.velocity.magnitude > 0.5f)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    steer = 1;
                }

                if (Input.GetKey(KeyCode.D))
                {
                    steer = -1;
                }
            }

            // add force to the rotation of the boat
            m_rigidbody.AddForceAtPosition(steer * transform.right * steerPower / 100f, motor.position);

            var movemnt = 0;
            var boatPower = power; 

            if (Input.GetKey(KeyCode.W))
            {
                movemnt = 1;
            }

            if (Input.GetKey(KeyCode.S))
            {
                movemnt = -1;
                boatPower = 100;
            }

            // add force to the boats FWD direction
            m_rigidbody.AddForceAtPosition(movemnt * transform.forward * boatPower, motor.position);

            // sets rotation of the motor visuals
            motor.SetPositionAndRotation(motor.position, transform.rotation * m_startRotation * Quaternion.Euler(0, 30.0f * steer, 0));
        }
    }
}
