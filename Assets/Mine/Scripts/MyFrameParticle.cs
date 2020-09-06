using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mine
{
    public class MyFrameParticle : MonoBehaviour
    {
        /// <summary>
        /// root
        /// </summary>
        [SerializeField]
        private GameObject root;
        
        /// <summary>
        /// OnParticleSystemStopped
        /// </summary>
        private void OnParticleSystemStopped()
        {
            Destroy(root);
        }
    }
}
