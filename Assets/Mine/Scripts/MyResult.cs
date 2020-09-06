using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace Mine
{
    public class MyResult : MonoBehaviour
    {
        private AudioSource _audio;
        
        public Transform Logo;

        public MyResultScore Perfect;
        
        public MyResultScore Great;
        
        public MyResultScore Normal;
        
        public MyResultScore Bad;
        
        public MyResultScore Miss;

        public Transform Button;
        
        public static MyResult Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            _audio = GetComponent<AudioSource>();
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            Logo.DORotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360)
                .SetEase(Ease.InOutCubic)
                .SetLoops(-1, LoopType.Yoyo);
        }

        /// <summary>
        /// hide
        /// </summary>
        public async UniTask HideAsync(bool immediate, CancellationToken ct = default)
        {
            if (!immediate)
            {
                var clip = Resources.Load<AudioClip>("Sound/cancel2");
                _audio.PlayOneShot(clip);
                await transform.DOScale(new Vector3(.1f, 1f, 1), .25f).SetEase(Ease.InOutCubic).OnCompleteAsObservable().ToUniTask(cancellationToken: ct);
                _audio.PlayOneShot(clip);
                await transform.DOScale(new Vector3(.1f, .1f, 1), .25f).SetEase(Ease.InOutCubic).OnCompleteAsObservable().ToUniTask(cancellationToken: ct);   
            }
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 表示
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async UniTask ShowAsync(
            int perfect,
            int great,
            int normal,
            int bad,
            int miss,
            CancellationToken ct = default)
        {
            gameObject.SetActive(true);
            Perfect.Hide();
            Great.Hide();
            Normal.Hide();
            Bad.Hide();
            Miss.Hide();
            Button.gameObject.SetActive(false);

            // 拡縮
            {
                transform.localScale = new Vector3(.1f, .1f, 1);

                var clip = Resources.Load<AudioClip>("Sound/cancel2");
                _audio.PlayOneShot(clip);
                await transform.DOScale(new Vector3(1, .1f, 1), .25f).SetEase(Ease.InOutCubic).OnCompleteAsObservable()
                    .ToUniTask(cancellationToken: ct);
                _audio.PlayOneShot(clip);
                await transform.DOScale(new Vector3(1, 1, 1), .25f).SetEase(Ease.InOutCubic).OnCompleteAsObservable()
                    .ToUniTask(cancellationToken: ct);
            }

            // Score
            {
                var clip = Resources.Load<AudioClip>("Sound/papa1");
                
                // Perfect
                Perfect.Show(perfect);
                _audio.PlayOneShot(clip);
                await UniTask.Delay(TimeSpan.FromSeconds(.5f), cancellationToken: ct);

                // Great
                Great.Show(great);
                _audio.PlayOneShot(clip);
                await UniTask.Delay(TimeSpan.FromSeconds(.5f), cancellationToken: ct);

                // Normal
                Normal.Show(normal);
                _audio.PlayOneShot(clip);
                await UniTask.Delay(TimeSpan.FromSeconds(.5f), cancellationToken: ct);

                // Bad
                Bad.Show(bad);
                _audio.PlayOneShot(clip);
                await UniTask.Delay(TimeSpan.FromSeconds(.5f), cancellationToken: ct);

                // Miss
                Miss.Show(miss);
                _audio.PlayOneShot(clip);
                await UniTask.Delay(TimeSpan.FromSeconds(.5f), cancellationToken: ct);

                // Button
                Button.gameObject.SetActive(true);
                _audio.PlayOneShot(clip);
                await UniTask.Delay(TimeSpan.FromSeconds(.5f), cancellationToken: ct);
            }
            
            // 歓声
            {
                var clip = Resources.Load<AudioClip>("Sound/people-performance-cheer1");
                _audio.PlayOneShot(clip);
            }
            
            // 紙吹雪
            MyAniversary.Instance.Show();
            
            await UniTask.Delay(TimeSpan.FromSeconds(.5f), cancellationToken: ct);
            
            // ボタン押し待ち
            var l = MyHandManager.Instance.Left.TriggerEnterObservable;
            var r = MyHandManager.Instance.Right.TriggerEnterObservable;
            await Observable.Merge(l, r).Where(x =>
                {
                    var layer = LayerMask.LayerToName(x.gameObject.layer);
                    return layer == "EndButton";
                })
                .Where(x => x != null)
                .Take(1)
                .ToUniTask(cancellationToken: ct);

            {
                var clip = Resources.Load<AudioClip>("Sound/decision1");
                _audio.PlayOneShot(clip);
            }
        }
    }
}
