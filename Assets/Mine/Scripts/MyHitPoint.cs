using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mine
{
    public class MyHitPoint : MonoBehaviour
    {
        /// <summary>
        /// 目標の色
        /// </summary>
        private Color targetColor;
        
        /// <summary>
        /// Particles
        /// </summary>
        [SerializeField]
        private ParticleSystem[] particles;

        /// <summary>
        /// Light
        /// </summary>
        [SerializeField]
        private Light light;

        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
            light.color = Color.Lerp(light.color, targetColor, Time.deltaTime * 3);
        }

        /// <summary>
        /// Particleの色を変える。
        /// </summary>
        /// <param name="color">色</param>
        public void SetColor(Color color)
        {
            targetColor = color;
            
            foreach (var p in particles)
            {
                var module = p.main;
                var a = module.startColor.color.a;
                module.startColor = new ParticleSystem.MinMaxGradient(new Color(color.r, color.g, color.b, a));
            }
        }
    }
}