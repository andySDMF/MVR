using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVR.Boats
{
    [RequireComponent(typeof(Rigidbody))]
    public class Buoyancy : MonoBehaviour
    {
        public BuoyancyDefaults buoyancyControl;

        [Header("Float Data")]
        public bool includeThisTransform = true;
        public List<Transform> floaters;

        Rigidbody m_rigidbody;
        bool isUnderwater = false;
        int floatersUnderwater;

        void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();

            if(includeThisTransform && !floaters.Contains(transform))
            {
                floaters.Add(transform);
            }
        }

        void FixedUpdate()
        {
            floatersUnderwater = 0;

            for(int i = 0; i < floaters.Count; i++)
            {
                float difference = floaters[i].position.y - buoyancyControl.waterHeight;

                if (difference < 0)
                {
                    m_rigidbody.AddForceAtPosition(Vector3.up * buoyancyControl.floatingPower * Mathf.Abs(difference), floaters[i].position, ForceMode.Force);

                    floatersUnderwater += 1;

                    if (!isUnderwater)
                    {
                        isUnderwater = true;
                        SwitchWaterState(true);
                    }
                }
            }

            if (isUnderwater && floatersUnderwater == 0)
            {
                isUnderwater = false;
                SwitchWaterState(false);
            }

        }

        void SwitchWaterState(bool underwater)
        {
            if(underwater)
            {
                m_rigidbody.drag = buoyancyControl.underwaterDrag;
                m_rigidbody.angularDrag = buoyancyControl.underwaterAngularDrag;
            }
            else
            {
                m_rigidbody.drag = buoyancyControl.airDrag;
                m_rigidbody.angularDrag = buoyancyControl.airAngularDrag;
            }
        }
    }
}
