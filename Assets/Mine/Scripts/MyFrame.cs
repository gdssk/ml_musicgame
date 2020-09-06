using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mine
{
    public class MyFrame : MonoBehaviour
    {
        /// <summary>
        /// 爆発
        /// </summary>
        public void Explosion()
        {
            var prefab = Resources.Load<ParticleSystem>("Prefab/Explosion");
            var instance = Instantiate(prefab, transform.position, transform.rotation);
            instance.Play();
            Destroy(gameObject);
        }
    }
}
