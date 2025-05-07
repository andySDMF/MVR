using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVR.Boats
{
    /// <summary>
    /// Class to store the default settings for buoyancy objects
    /// </summary>
    [System.Serializable]
    public class BuoyancyDefaults
    {
        public float underwaterDrag = 3f;
        public float underwaterAngularDrag = 1f;

        public float airDrag = 3f;
        public float airAngularDrag = 0.05f;

        public float floatingPower = 15f;
        public float waterHeight = 0f;
    }
}
