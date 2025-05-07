using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MVR.Boats
{
    [RequireComponent(typeof(Collider))]
    public class Docking : MonoBehaviour
    {
        [Header("NOTE:: Docking is based relative to the FWD direction of this transform")]

        [SerializeField]
        private float tolerance = 20.0f;

        [SerializeField]
        private DockingSide dockSide = DockingSide.Port;

        [SerializeField]
        private  UnityEvent onComplete;


        private Coroutine m_checker;

        private void Start()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            BoatController boat = other.GetComponentInParent<BoatController>();

            if (boat != null)
            {
                m_checker = StartCoroutine(CheckDocking(boat));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(m_checker != null)
            {
                //we must stop the dock checker co-routine when boat exists to release from memeory/main thread
                StopCoroutine(m_checker);
            }
        }

        private IEnumerator CheckDocking(BoatController boat)
        {
            while(boat.Velocity > 0.5f)
            {
                yield return null;
            }

          
            if(dockSide == DockingSide.Port)
            {
                float angle = Vector3.Angle(boat.ForwardDirection, transform.forward);

                if (angle <= tolerance)
                {
                    onComplete.Invoke();
                }
            }
            else
            {
                float angle = Vector3.Angle(boat.ForwardDirection, transform.forward);

                if (angle <= tolerance + 180)
                {
                    onComplete.Invoke();
                }
            }

        }

        [System.Serializable]
        private enum DockingSide { Port, Starbaord }
    }
}
