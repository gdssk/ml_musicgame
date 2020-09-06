using UnityEngine;

namespace Mine
{
    /// <summary>
    /// Player
    /// </summary>
    public class MyPlayer : MonoBehaviour
    {
        /// <summary>
        /// Audio
        /// </summary>
        private AudioSource _audio;
        
        /// <summary>
        /// L
        /// </summary>
        public MyHitPoint HitPointL;
        
        /// <summary>
        /// R
        /// </summary>
        public MyHitPoint HitPointR;

        /// <summary>
        /// C
        /// </summary>
        public GameObject HitPointC;

        /// <summary>
        /// 判定L
        /// </summary>
        [SerializeField]
        private ParticleSystem resultL;
        
        /// <summary>
        /// 判定R
        /// </summary>
        [SerializeField]
        private ParticleSystem resultR;
        
        /// <summary>
        /// オーディオC
        /// </summary>
        [SerializeField]
        private AudioSource audioC;

        /// <summary>
        /// 判定 L
        /// </summary>
        [SerializeField]
        private Material resultMatL;
        
        /// <summary>
        /// 判定 R
        /// </summary>
        [SerializeField]
        private Material resultMatR;
        
        /// <summary>
        /// Instance
        /// </summary>
        public static MyPlayer Instance { get; private set; }

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            Instance = this;

            _audio = GetComponent<AudioSource>();
        }

        /// <summary>
        /// OnDestroy
        /// </summary>
        private void OnDestroy()
        {
            Instance = null;
        }

        /// <summary>
        /// 判定結果パーティクル
        /// </summary>
        /// <param name="side">方向</param>
        /// <param name="result">結果</param>
        public void PlayResult(MyNoteSide side, MyGameManager.NotesResult result)
        {
            var material = side == MyNoteSide.Left ? resultMatL : resultMatR;
            var texture = Resources.Load<Texture>($"Texture/{result}");
            if (texture == null)
            {
                return;
            }
            material.SetTexture("_MainTex", texture);
            
            var particle = side == MyNoteSide.Left ? resultL : resultR;
            particle.Play();
        }

        public void ToPlay(bool isPlay)
        {
            HitPointL.gameObject.SetActive(isPlay);
            HitPointR.gameObject.SetActive(isPlay);
            HitPointC.gameObject.SetActive(!isPlay);
        }

        /// <summary>
        /// キラ音
        /// </summary>
        public void PlayKira()
        {
            var clip = Resources.Load<AudioClip>("Sound/kira1");
            _audio.PlayOneShot(clip);
        }

        /// <summary>
        /// CLOOP
        /// </summary>
        public void PlayCLoop(bool isPlay)
        {
            if (isPlay)
            {
                audioC.Play();
            }
            else
            {
                audioC.Stop();
            }
        }
    }
}
