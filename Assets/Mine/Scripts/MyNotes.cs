using System;
using UnityEngine;

namespace Mine
{
    /// <summary>
    /// Note
    /// </summary>
    public abstract class MyNotes : MonoBehaviour
    {
        /// <summary>
        /// 発射してからの時間
        /// </summary>
        public float ElapsedTime { get; private set; }
        
        /// <summary>
        /// 紐づくデータ
        /// </summary>
        public MyNoteData Data { get; private set; }
        
        /// <summary>
        /// エリアに到着する時間
        /// </summary>
        public abstract float GoalTime { get; }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="data"></param>
        protected virtual void OnInit(MyNoteData data) {}

        /// <summary>
        /// 移動
        /// </summary>
        /// <param name="time"></param>
        protected abstract void OnMove(float time);

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="data">データ</param>
        public void Init(MyNoteData data)
        {
            Data = data;
            OnInit(data);
        }

        /// <summary>
        /// 移動
        /// </summary>
        /// <param name="time"></param>
        public void Move(float time)
        {
            ElapsedTime += time;
            OnMove(time);
        }

        /// <summary>
        /// ノートの消滅
        /// </summary>
        public virtual void Dismiss()
        {
            // particle
            {
                var prefab = Resources.Load<ParticleSystem>("Prefab/Emitter");
                Instantiate(prefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
        
        /// <summary>
        /// OnTriggerEnter
        /// </summary>
        /// <param name="other">Collider</param>
        private void OnTriggerEnter(Collider other)
        {
            var cLayer = LayerMask.LayerToName(other.gameObject.layer);
            if (cLayer == "Wall")
            {
                MyGameManager.Instance.DismissNote(this, MyGameManager.NotesResult.Miss);
            }
        }
    }
}
