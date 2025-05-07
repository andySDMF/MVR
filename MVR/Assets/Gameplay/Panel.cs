using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVR.Gameplay
{
    public class Panel : MonoBehaviour
    {
        [SerializeField]
        private bool startVisible = false;

        private void Awake()
        {
            SetVisible(startVisible);
        }

        /// <summary>
        /// Called to set this panel visibility
        /// </summary>
        /// <param name="isVisible"></param>
        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }
    }

}