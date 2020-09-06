using UnityEngine;

namespace Mine
{
    /// <summary>
    /// Emitter
    /// </summary>
    public class MyEmitter : MonoBehaviour
    {
        /// <summary>
        /// OnParticleSystemStopped
        /// </summary>
        private void OnParticleSystemStopped()
        {
            Destroy(gameObject);
        }
    }
}
