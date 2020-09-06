using System;
using System.Threading;
using UnityEngine;

namespace Mine
{
    /// <summary>
    /// 魔法使いの家
    /// </summary>
    public class MyMagician : MyLauncher
    {
        /// <summary>
        /// Audio
        /// </summary>
        private AudioSource _audio;
        
        /// <summary>
        /// House
        /// </summary>
        public GameObject house;
        
        /// <summary>
        /// Emitter
        /// </summary>
        [SerializeField]
        private ParticleSystem emitter;
        
        /// <summary>
        /// Instance
        /// </summary>
        public static MyMagician Instance { get; private set; }

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
        /// 発射
        /// </summary>
        public override MyNotes Launch(MyNoteData data)
        {
            var t = GetT(data.Side);
            var prefab = Resources.Load<MyNotes>($"Prefab/{data.Type}");
            var instance = Instantiate(prefab, t.position, Quaternion.identity, MyGameManager.Instance.transform);
            instance.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            instance.Init(data);
            return instance;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        private Transform GetT(MyNoteSide side)
        {
            switch (side)
            {
                case MyNoteSide.Left : return MyDoor.Instance.L;
                case MyNoteSide.Right : return MyDoor.Instance.R;
            }
            return null;
        }

        /// <summary>
        /// 魔法使いの家の表示切り替え
        /// </summary>
        /// <param name="isActive"></param>
        public void SwitchHouse(bool isActive, bool isEmitter = true)
        {
            emitter.Play();
            house.SetActive(isActive);
        }

        /// <summary>
        /// Bomb
        /// </summary>
        public void PlayBombAudio()
        {
            var clip = Resources.Load<AudioClip>("Sound/bomb1");
            _audio.PlayOneShot(clip);
        }
    }
}
