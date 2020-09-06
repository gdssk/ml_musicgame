using System;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace Mine
{
    /// <summary>
    /// ハンドトラッキング管理
    /// </summary>
    public class MyHandManager : MonoBehaviour
    {
        /// <summary>
        /// Left
        /// </summary>
        public MyHand Left;
        
        /// <summary>
        /// Left
        /// </summary>
        public MyHand Right;
        
        /// <summary>
        /// Instance
        /// </summary>
        public static MyHandManager Instance { get; private set; }
        
        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            MLHandTracking.Start();
        }

        /// <summary>
        /// OnDestroy
        /// </summary>
        private void OnDestroy()
        {
            Instance = null;
            MLHandTracking.Stop();
        }
    }
}
