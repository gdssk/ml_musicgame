using System;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace Mine
{
    /// <summary>
    /// 手の管理をするコンポーネント
    /// </summary>
    public class MyHand : MonoBehaviour
    {
        /// <summary>
        /// どっち
        /// </summary>
        public enum MySide
        {
            /// <summary>
            /// 未指定
            /// </summary>
            None,
            /// <summary>
            /// 左
            /// </summary>
            Left,
            /// <summary>
            /// 右
            /// </summary>
            Right
        }

        /// <summary>
        /// どっちの手か
        /// </summary>
        public MySide Side;
        
        /// <summary>
        /// 人差し指先
        /// </summary>
        [SerializeField]
        private MyFinger index3;
        
        /// <summary>
        /// Enter
        /// </summary>
        public IObservable<Collider> TriggerEnterObservable =>
            index3.TriggerEnterObservable;
        
        /// <summary>
        /// Exit
        /// </summary>
        public IObservable<Collider> TriggerExitObservable =>
            index3.TriggerExitObservable;

        /// <summary>
        /// 更新
        /// </summary>
        private void Update()
        {
            if (Side == MySide.None)
            {
                return;
            }
            // 手の情報取得
            var hand = Side == MySide.Left ? MLHandTracking.Left : MLHandTracking.Right;
            // Transformに反映させる
            index3.transform.position = hand.Index.KeyPoints[2].Position;
        }
    }
}
