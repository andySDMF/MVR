using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVR.Boats
{
    [RequireComponent(typeof(Rigidbody))]
    public class Buoyancy : MonoBehaviour
    {
        [SerializeField]
        protected BuoyancyDefaults buoyancyControl;

        [Header("Float Data")]
        [SerializeField]
        protected bool includeThisTransform = true;
        [SerializeField]
        protected List<Transform> floaters;

        private Rigidbody m_rigidbody;
        private bool isUnderwater = false;
        private int floatersUnderwater;

        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();

            if(includeThisTransform && !floaters.Contains(transform))
            {
                floaters.Add(transform);
            }
        }

        private void FixedUpdate()
        {
            floatersUnderwater = 0;

            //loop through all the floaters
            for(int i = 0; i < floaters.Count; i++)
            {
                float difference = floaters[i].position.y - buoyancyControl.waterHeight;

                if (difference < 0)
                {
                    // add for to the floater if below water
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

        private  void SwitchWaterState(bool underwater)
        {
            // change rigidbody drags based on underwater state
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
