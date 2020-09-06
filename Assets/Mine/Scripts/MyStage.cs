using UnityEngine;

namespace Mine
{
    /// <summary>
    /// Stage
    /// </summary>
    public class MyStage : MonoBehaviour
    {
        /// <summary>
        /// Singleton
        /// </summary>
        public static MyStage Instance { get; private set; }

        [SerializeField] private AudioSource leftAudio;
        [SerializeField] private AudioSource rightAudio;
        [SerializeField] private AudioSource magicianAudio;
        [SerializeField] private AudioSource doorLAudio;
        [SerializeField] private AudioSource doorRAudio;
        
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
        /// Show
        /// </summary>
        /// <param name="isShow"></param>
        /// <param name="immediate"></param>
        public void Show(bool isShow, bool immediate = true)
        {
            gameObject.SetActive(isShow);

            if (!immediate)
            {
                var anim = GetComponent<Animator>();
                string state = null;
                if (isShow)
                {
                    state = "Show";
                }
                else
                {
                    state = "Hide";
                }
                anim.Play(state);
            }
        }

        
        /// <summary>
        /// Pon
        /// </summary>
        public void PlayPonOneShot(string key)
        {
            AudioSource a = null;

            switch (key)
            {
                case "left" :
                    a = leftAudio;
                    break;
                case "right" :
                    a = rightAudio;
                    break;
                case "doorL" :
                    a = doorLAudio;
                    break;
                case "doorR" :
                    a = doorRAudio;
                    break;
                case "magician" :
                    a = magicianAudio;
                    break;
            }

            if (a == null) return;
            
            var clip = Resources.Load<AudioClip>("Sound/papa1");
            a.PlayOneShot(clip);
        }
    }
}