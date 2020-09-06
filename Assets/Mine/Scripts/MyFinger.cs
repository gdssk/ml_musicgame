using System;
using UniRx;
using UnityEngine;

namespace Mine
{
    [RequireComponent(typeof(Rigidbody))]
    public class MyFinger : MonoBehaviour
    {
        /// <summary>
        /// Enter
        /// </summary>
        private readonly Subject<Collider> _triggerEnterSubject = new Subject<Collider>();
        
        /// <summary>
        /// Exit
        /// </summary>
        private readonly Subject<Collider> _triggerExitSubject = new Subject<Collider>();

        /// <summary>
        /// Enter
        /// </summary>
        public IObservable<Collider> TriggerEnterObservable => _triggerEnterSubject;
        
        /// <summary>
        /// Exit
        /// </summary>
        public IObservable<Collider> TriggerExitObservable => _triggerExitSubject;

        /// <summary>
        /// OnTriggerEnter
        /// </summary>
        /// <param name="other">Collider</param>
        private void OnTriggerEnter(Collider other)
        {
            _triggerEnterSubject.OnNext(other);
        }
        
        /// <summary>
        /// OnTriggerExit
        /// </summary>
        /// <param name="other">Collider</param>
        private void OnTriggerExit(Collider other)
        {
            _triggerExitSubject.OnNext(other);
        }
    }
}
