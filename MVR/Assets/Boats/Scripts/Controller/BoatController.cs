using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVR.Boats
{
    [RequireComponent(typeof(Rigidbody))]
    public class BoatController : MonoBehaviour
    {
        public Transform motor;
        public float steerPower = 500f;
        public float power = 500f;

        protected Rigidbody m_rigidbody;
        protected Quaternion m_startRotation;

        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_startRotation = motor.rotation;
        }

        void FixedUpdate()
        {
            var forceDirection = transform.forward;
            var steer = 0;


            if(Input.GetKey(KeyCode.A))
            {
                steer = 1;
            }

            if (Input.GetKey(KeyCode.D))
            {
                steer = -1;
            }

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

            m_rigidbody.AddForceAtPosition(movemnt * transform.forward * boatPower, motor.position);
        }
    }
}
