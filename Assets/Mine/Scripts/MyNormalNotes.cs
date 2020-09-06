using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mine
{
    /// <summary>
    /// 普通のノーツ
    /// </summary>
    public abstract class MyNormalNotes : MyNotes
    {
        /// <summary>
        /// 速度
        /// </summary>
        private Vector3 speed;

        /// <summary>
        /// OnInit
        /// </summary>
        /// <param name="data">Data</param>
        protected override void OnInit(MyNoteData data)
        {
            var goal = data.Side == MyNoteSide.Left
                ? MyPlayer.Instance.HitPointL.transform
                : MyPlayer.Instance.HitPointR.transform;
            
            speed = (goal.position - transform.position) / GoalTime;
        }

        protected override void OnMove(float time)
        {
            var t = transform;
            var p = t.position;
            p += speed * time;
            t.position = p;
        }

    }
}