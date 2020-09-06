using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mine
{
    public class MyAniversary : MonoBehaviour
    {
        /// <summary>
        /// Singleton
        /// </summary>
        public static MyAniversary Instance { get; private set; }
        
        // Start is called before the first frame update
        void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
