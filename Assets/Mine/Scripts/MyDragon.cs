using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mine
{
    /// <summary>
    /// Dragon
    /// </summary>
    public class MyDragon : MyLauncher
    {
        /// <summary>
        /// Particle
        /// </summary>
        private ParticleSystem frame;

        /// <summary>
        /// AudioSource
        /// </summary>
        private AudioSource _audio;
        
        /// <summary>
        /// 移動アニメーター
        /// </summary>
        [SerializeField]
        private Animator moveAnimator;

        /// <summary>
        /// ドラゴンアニメーター
        /// </summary>
        [SerializeField]
        private Animator dragonAnimator;

        /// <summary>
        /// 炎音
        /// </summary>
        [SerializeField]
        private AudioClip frameClip;
        
        /// <summary>
        /// ドラゴン音
        /// </summary>
        [SerializeField]
        private AudioClip dragonClip;

        /// <summary>
        /// ドラゴンの口元原点
        /// </summary>
        public Transform dragonMouseOrigin;
        
        /// <summary>
        /// Subject
        /// </summary>
        private readonly Subject<string> subject = new Subject<string>();

        /// <summary>
        /// AnimationId
        /// </summary>
        private static readonly int AnimationId = Animator.StringToHash("Animation");

        /// <summary>
        /// Observable
        /// </summary>
        public IObservable<string> Observable => subject;
        
        /// <summary>
        /// Singleton
        /// </summary>
        public static MyDragon Instance { get; private set; }

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
        /// 待機
        /// </summary>
        public void Idle()
        {
            moveAnimator.Play("Idle");
        }
        
        /// <summary>
        /// Show
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Hide
        /// </summary>
        public void Hide()
        {
            if (frame != null)
            {
                Destroy(frame.gameObject);
            }
            gameObject.SetActive(false);
        }

        /// <summary>
        /// シークエンス
        /// </summary>
        public async UniTask SequenceAsync(CancellationToken ct = default)
        {
            gameObject.SetActive(true);
            
            // 出現してから目の前に来る
            dragonAnimator.SetInteger(AnimationId, 0);
            moveAnimator.Play("Appear");
            await Observable.Where(x => x == "appear").First().ToUniTask(cancellationToken: ct);
            dragonAnimator.SetInteger(AnimationId, 2);
            _audio.PlayOneShot(dragonClip);

            // Delay
            await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: ct);

            // 移動して攻撃地点に行く
            moveAnimator.Play("Move1");
            dragonAnimator.SetInteger(AnimationId, 0);
            await Observable.Where(x => x == "move1").First().ToUniTask(cancellationToken: ct);
            dragonAnimator.SetInteger(AnimationId, 2);

            // 攻撃モーションを再生しつつ、炎オブジェクトを飛ばす
            {
                var prefab = Resources.Load<Transform>("Prefab/Frame");
                for (int i = 0; i < 3; i++)
                {
                    var o = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                    var p = dragonMouseOrigin.position + RandomVec3(0.05f);
                    o.transform.position = p;
                    var t = MyMagician.Instance.house.transform.position + RandomVec3(0.05f);
                    o.DOMove(t, 1.5f).OnComplete(() =>
                    {
                        o.GetComponent<MyFrame>().Explosion();
                    });
                    _audio.PlayOneShot(frameClip);
                    await UniTask.Delay(TimeSpan.FromSeconds(.2f), cancellationToken: ct);
                }
                dragonAnimator.SetInteger(AnimationId, 3);
                await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: ct);
            }
            
            // 炎パーティクルを再生
            {
                if (frame != null)
                {
                    Destroy(frame.gameObject);
                }
                var prefab = Resources.Load<ParticleSystem>("Prefab/MyFire");
                frame = Instantiate(prefab, MyMagician.Instance.house.transform.position, Quaternion.identity);
                frame.Play();
                MyMagician.Instance.SwitchHouse(false);
                MyMagician.Instance.PlayBombAudio();
            }
          
            // 攻撃地点から指定位置に行く
            moveAnimator.Play("Move2");
            dragonAnimator.SetInteger(AnimationId, 0);
            await Observable.Where(x => x == "move2").First().ToUniTask(cancellationToken: ct);
            dragonAnimator.SetInteger(AnimationId, 2);
            _audio.PlayOneShot(dragonClip);
            
            // Delay
            await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: ct);
            
            // Idle
            dragonAnimator.SetInteger(AnimationId, 0);

            // Random
            Vector3 RandomVec3(float v)
            {
                return new Vector3(Random.Range(-v, +v), Random.Range(-v, +v), Random.Range(-v, +v));
            }
        }

        /// <summary>
        /// ノーツの発射
        /// </summary>
        /// <param name="data">データ</param>
        /// <returns>ノーツ</returns>
        public override MyNotes Launch(MyNoteData data)
        {
            dragonAnimator.SetTrigger("Fire");
            {
                var prefab = Resources.Load<ParticleSystem>("Prefab/small_fire"); 
                Instantiate(prefab, dragonMouseOrigin.position, Quaternion.identity);
            }
            {
                var prefab = Resources.Load<MyNotes>($"Prefab/Fire");
                var instance = Instantiate(prefab, MyDoor.Instance.C.position, Quaternion.identity,
                    MyGameManager.Instance.transform);
                instance.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                instance.Init(data);
                return instance;
            }
        }

        /// <summary>
        /// Animation
        /// </summary>
        private void OnEndAnimation(string s)
        {
            subject.OnNext(s);
        }

        /// <summary>
        /// 炎発射音
        /// </summary>
        public void PlayFrameSound()
        {
            _audio.PlayOneShot(frameClip);
        }
    }
}
