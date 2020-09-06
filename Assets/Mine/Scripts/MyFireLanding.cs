using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mine
{
    public class MyFireLanding : MonoBehaviour
    {
        /// <summary>
        /// L
        /// </summary>
        public Transform L;

        /// <summary>
        /// R
        /// </summary>
        public Transform R;

        /// <summary>
        /// Singleton
        /// </summary>
        public static MyFireLanding Instance { get; private set; }
        
        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// OnDestroy
        /// </summary>
        private void OnDestroy()
        {
            Instance = null;
        }
    }
}
