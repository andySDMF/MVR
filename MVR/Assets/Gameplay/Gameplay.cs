using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MVR.Gameplay
{
    public class Gameplay : MonoBehaviour
    {
        /// <summary>
        /// Called to restart the game
        /// </summary>
        public void RestartGame()
        {
            Scene scene = SceneManager.GetActiveScene();

            SceneManager.LoadScene(scene.name);
        }
    }
}
