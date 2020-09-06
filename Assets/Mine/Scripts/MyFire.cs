using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mine
{
    public class MyFire : MyNotes
    {
        /// <summary>
        /// 地面に着弾するまで
        /// </summary>
        protected Vector3 speed1;
        
        /// <summary>
        /// 着弾してから
        /// </summary>
        protected Vector3 speed2;

        /// <summary>
        /// 着弾レート
        /// </summary>
        //private const float landingRate = 0.5145f;
        private const float landingRate = 0.5f;
        
        /// <summary>
        /// エリアに到着する時間
        /// </summary>
        // public override float GoalTime => 1.53779f;
        public override float GoalTime => 1.5f;
        
        /// <summary>
        /// OnInit
        /// </summary>
        /// <param name="data">Data</param>
        protected override void OnInit(MyNoteData data)
        {
            var landing = data.Side == MyNoteSide.Left ? MyFireLanding.Instance.L : MyFireLanding.Instance.R;
            var goal = data.Side == MyNoteSide.Left ? MyPlayer.Instance.HitPointL.transform : MyPlayer.Instance.HitPointR.transform;
            
            speed1 = (landing.position - transform.position) / (GoalTime * landingRate);
            speed2 = (goal.position - landing.position) / (GoalTime * (1f - landingRate));
        }

        /// <summary>
        /// 移動
        /// </summary>
        /// <param name="time"></param>
        protected override void OnMove(float time)
        {
            var t = transform;
            var p = t.position;

            Vector3 speed;
            if ((ElapsedTime / GoalTime) <= landingRate)
            {
                speed = speed1;
            }
            else
            {
                speed = speed2;
            }
            p += speed * time;
            t.position = p;
        }
    }
}
