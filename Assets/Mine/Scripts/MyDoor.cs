using UnityEngine;

namespace Mine
{
    public class MyDoor : MonoBehaviour
    {
        /// <summary>
        /// L
        /// </summary>
        public Transform L;

        /// <summary>
        /// R
        /// </summary>
        public Transform R;

        /// <summary>
        /// Center
        /// </summary>
        public Transform C;

        /// <summary>
        /// Audio L
        /// </summary>
        public AudioSource audioL;
        
        /// <summary>
        /// Audio L
        /// </summary>
        public AudioSource audioR;
        
        /// <summary>
        /// Audio L
        /// </summary>
        public AudioClip summonClip;
        
        /// <summary>
        /// Singleton
        /// </summary>
        public static MyDoor Instance { get; private set; }

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// OnDestroy
        /// </summary>
        private void OnDestroy()
        {
            Instance = null;
        }

        /// <summary>
        /// 召喚音
        /// </summary>
        public void PlaySummonAudio(MyHand.MySide side)
        {
            AudioSource audio;
            switch (side)
            {
                case MyHand.MySide.Left:
                    audio = audioL;
                    break;
                case MyHand.MySide.Right:
                    audio = audioR;
                    break;
                default:
                    return;
            }
            audio.PlayOneShot(summonClip);
        }
    }
}
