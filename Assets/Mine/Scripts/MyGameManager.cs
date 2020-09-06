using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MagicLeap;
using TMPro;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;

namespace Mine
{
    /// <summary>
    /// GameManager
    /// </summary>
    public class MyGameManager : MonoBehaviour
    {
        /// <summary>
        /// ゲームの進行状態
        /// </summary>
        public enum State
        {
            /// <summary>
            /// 未指定
            /// </summary>
            None = -1,
            /// <summary>
            /// 平面検出
            /// </summary>
            Plane,
            /// <summary>
            /// ゲーム開始
            /// </summary>
            Game,
            /// <summary>
            /// 結果
            /// </summary>
            Result,
        }

        /// <summary>
        /// ノーツ判定
        /// </summary>
        public enum NotesResult
        {
            /// <summary>
            /// 未指定
            /// </summary>
            None = -1,
            /// <summary>
            /// ミス
            /// </summary>
            Miss,
            /// <summary>
            /// 悪い
            /// </summary>
            Bad,
            /// <summary>
            /// まぁまぁ
            /// </summary>
            Good,
            /// <summary>
            /// 良い
            /// </summary>
            Great,
            /// <summary>
            /// 完璧
            /// </summary>
            Perfect
        }
        
        /// <summary>
        /// 発射したノート数
        /// </summary>
        private int notesCount = 0;
        
        /// <summary>
        /// AudioSource
        /// </summary>
        private AudioSource audioSource;

        /// <summary>
        /// ドラゴンに切り替えたか
        /// </summary>
        private bool isDragon;

        /// <summary>
        /// ゲームが終わったか
        /// </summary>
        private bool isEndGame;
    
        /// <summary>
        /// CompositeDisposable
        /// </summary>
        private CompositeDisposable cd = new CompositeDisposable();
        
        /// <summary>
        /// ドラゴンが出てくる時間
        /// </summary>
        [SerializeField]
        private float dragonTime;

        /// <summary>
        /// MyMeshingVisualizer
        /// </summary>
        [SerializeField]
        private MyMeshingVisualizer meshingVisualizer;

        /// <summary>
        /// planeText
        /// </summary>
        [SerializeField]
        private GameObject planeText;
        
        /// <summary>
        /// ノートデータ
        /// </summary>
        [SerializeField]
        private MyNoteData[] noteData;

        /// <summary>
        /// Number
        /// </summary>
        [SerializeField]
        private SpriteRenderer number;
        
        /// <summary>
        /// Number
        /// </summary>
        [SerializeField]
        private AudioSource numberAudio;

        /// <summary>
        /// 現在場に出ているノート
        /// </summary>
        private List<MyNotes> activeNotes = new List<MyNotes>();

        /// <summary>
        /// ステート
        /// </summary>
        [HideInInspector]
        public State Current = State.None;

        /// <summary>
        /// スコア
        /// </summary>
        public Dictionary<NotesResult, int> Score { get; } = new Dictionary<NotesResult, int>();
        
        /// <summary>
        /// コンボ数
        /// </summary>
        public int Combo { get; private set; } = 0;
        
        /// <summary>
        /// 音源の現在時間
        /// </summary>
        public float Time => audioSource.time;
        
        /// <summary>
        /// 音源の長さ
        /// </summary>
        public float Length => audioSource.clip.length;

        /// <summary>
        /// 音源の進行度
        /// </summary>
        public float Ratio => Time / Length;

        /// <summary>
        /// Perfect
        /// </summary>
        public const int PerfectFrame = 5;
        
        /// <summary>
        /// Great
        /// </summary>
        public const int GreatFrame = 10;
        
        /// <summary>
        /// Good
        /// </summary>
        public const int GoodFrame = 15;
        
        /// <summary>
        /// Bad
        /// </summary>
        public const int BadFrame = 20;
        
        /// <summary>
        /// Miss
        /// </summary>
        public const int MissFrame = 25;
        
        /// <summary>
        /// Instance
        /// </summary>
        public static MyGameManager Instance { get; private set; }

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            DOTween.Init();

            Instance = this;
            Application.targetFrameRate = 60;
            audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// OnDestroy
        /// </summary>
        private void OnDestroy()
        {
            Instance = null;
        }

        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            planeText.SetActive(true);
            ChangeState(State.Plane);   
        }

        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
            switch (Current)
            {
                case State.Game:
                    if (isEndGame) break;
                    LaunchNotes();
                    UpdateNotes();
                    if (!isDragon && dragonTime <= Time)
                    {
                        MyDragon.Instance.SequenceAsync().Forget();
                        isDragon = true;
                    }
                    if (Ratio >= 0.99f)
                    {
                        isEndGame = true;
                        Debug.Log("EndGame");
                        EndGameAsync().Forget();
                    }
                    break;
            }
        }

        /// <summary>
        /// ステートの変更
        /// </summary>
        /// <param name="state">state</param>
        private void ChangeState(State state)
        {
            // 今のステートの終了処理
            switch (Current)
            {
                // Plane
                case State.Plane:
                    meshingVisualizer.StopMeshHandle();
                    meshingVisualizer.DestroyMesh();
                    break;
            }

            cd.Clear();
            Current = state;

            // 次のステートの開始処理
            switch (Current)
            {
                // Plane
                case State.Plane:
                    MyResult.Instance.HideAsync(true).Forget();
                    MyDragon.Instance.Hide();
                    MyStage.Instance.Show(false);
                    meshingVisualizer.StartMeshHandle();
                    MyPlayer.Instance.ToPlay(false);
                    MyAniversary.Instance.Hide();
                    MyCharacter.Instance.PlayIdle();
                    StartPlane();
                    break;
                
                // Game
                case State.Game:
                    GameInitAsync().Forget();
                    break;
            }
        }

        /// <summary>
        /// ゲーム初期化
        /// </summary>
        /// <returns></returns>
        private async UniTask GameInitAsync()
        {
            isDragon = false;
            isEndGame = false;
            notesCount = Combo = 0;
            Score.Clear();
            ObserveNoteTap();
            ObserveTapState();
            MyPlayer.Instance.ToPlay(true);
            MyCharacter.Instance.PlayJump();
            MyResult.Instance.HideAsync(true).Forget();

            // 1秒待機
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            
            MyStage.Instance.Show(true, false);
            MyMagician.Instance.SwitchHouse(true, false);
            
            // 1秒待機
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            
            // カウントダウン
            number.gameObject.SetActive(true);
            int count = 3;
            Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1))
                .Select(x => (int) (count - x))
                .TakeWhile(x => x >= 0)
                .Subscribe(x =>
                {
                    switch (x)
                    {
                        case 3:
                            Countdown(x);
                            break;
                        case 2:
                            Countdown(x);
                            break;
                        case 1:
                            Countdown(x);
                            break;
                        case 0:
                            Countdown(x);
                            Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
                            {
                                number.gameObject.SetActive(false);
                                audioSource.Play();
                            }).AddTo(cd);
                            break;
                    }
                }).AddTo(cd);
        }

        private void StartPlane()
        {
            var position = Vector3.zero;

            // RayCast
            MyController.Instance.RaycastObservable
                .Subscribe(x =>
                {
                    position = x.point;
                }).AddTo(cd);

            // Bumper
            MyController.Instance.ControllerButtonDownObservable
                .Where(x => x.button == MLInput.Controller.Button.Bumper)
                .Subscribe(x =>
                {
                    planeText.SetActive(false);
                    transform.position = position;
                    transform.localEulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);
                }).AddTo(cd);

            MyPlayer.Instance.PlayCLoop(true);
            
            // Hand
            var l = MyHandManager.Instance.Left.TriggerEnterObservable;
            var r = MyHandManager.Instance.Right.TriggerEnterObservable;
            Observable.Merge(l, r).Where(x =>
                {
                    var layer = LayerMask.LayerToName(x.gameObject.layer);
                    return layer == "HitC";
                })
                .Where(x => x != null)
                .Take(1)
                .Subscribe(x =>
                {
                    MyPlayer.Instance.PlayCLoop(false);
                    MyPlayer.Instance.PlayKira();
                    ChangeState(State.Game);
                }).AddTo(cd);
        }

        /// <summary>
        /// Countdown
        /// </summary>
        /// <param name="v"></param>
        private void Countdown(int v)
        {
            switch (v)
            {
                case 3:
                    number.sprite = Resources.Load<Sprite>("Texture/3");
                    numberAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/papa1"));
                    rotate();
                    break;
                case 2:
                    number.sprite = Resources.Load<Sprite>("Texture/2");
                    numberAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/papa1"));
                    rotate();
                    break;
                case 1:
                    number.sprite = Resources.Load<Sprite>("Texture/1");
                    numberAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/papa1"));
                    rotate();
                    break;
                case 0:
                    number.sprite = Resources.Load<Sprite>("Texture/Start");
                    numberAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/whistle"));
                    scale();
                    break;
            }

            // rotate
            void rotate()
            {
                number.transform.localScale = Vector3.zero;
                number.transform.DOScale(new Vector3(.03f, .03f, .03f), .5f).SetEase(Ease.InOutCubic);
                number.transform.DORotate(new Vector3(0, 0, 720f), .5f, RotateMode.LocalAxisAdd).SetEase(Ease.InOutCubic);
            }

            // scale
            void scale()
            {
                number.transform.localScale = Vector3.zero;
                number.transform.DOScale(new Vector3(.03f, .03f, .03f), .5f).SetEase(Ease.InOutCubic);
            }
        }

        /// <summary>
        /// ノートの発射
        /// </summary>
        private void LaunchNotes()
        {
            bool isShotL = false, isShotR = false, isShotD = false;
            while (true)
            {
                if (IsLaunchedLastNote())
                {
                    break;
                }
                
                var data = noteData[notesCount];
                if (data.Time <= Time)
                {
                    MyLauncher launcher;
                    switch (data.Type)
                    {
                        case MyNotesType.Fire:
                            launcher = MyDragon.Instance;
                            isShotD = true;
                            break;
                        default:
                            launcher = MyMagician.Instance;
                            if (data.Side == MyNoteSide.Left) isShotL = true;
                            if (data.Side == MyNoteSide.Right) isShotR = true;
                            break;
                    }
                    activeNotes.Add(launcher.Launch(data));
                    notesCount++;
                }
                else
                {
                    break;
                }
            }
            if (isShotD)
            {
                MyDragon.Instance.PlayFrameSound();
            }
        }

        /// <summary>
        /// ノートの更新
        /// </summary>
        private void UpdateNotes()
        {
            foreach (var n in activeNotes)
            {
                n.Move(UnityEngine.Time.deltaTime);
            }
        }

        /// <summary>
        /// 最後のノートを発射したか
        /// </summary>
        /// <returns>最後のノートを発射した場合、真</returns>
        private bool IsLaunchedLastNote()
        {
            return noteData.Length == notesCount;
        }

        /// <summary>
        /// ノーツをタップしたときの挙動
        /// </summary>
        private void ObserveNoteTap()
        {
            // 左
            MyHandManager.Instance.Left.TriggerEnterObservable
                .Where(x => LayerMask.LayerToName(x.gameObject.layer) == "HitL")
                .Select(_ => GetLatestNotes(MyNoteSide.Left))
                .Where(x => x != null)
                .Subscribe(Exec)
                .AddTo(cd);
            
            // 右
            MyHandManager.Instance.Right.TriggerEnterObservable
                .Where(x => LayerMask.LayerToName(x.gameObject.layer) == "HitR")
                .Select(_ => GetLatestNotes(MyNoteSide.Right))
                .Where(x => x != null)
                .Subscribe(Exec)
                .AddTo(cd);
            
            // 指定したサイドで一番最新のノーツを取る
            MyNotes GetLatestNotes(MyNoteSide side)
            {
                return activeNotes.Where(x => x.Data.Side == side)
                    .Where(x => x.ElapsedTime <= (x.GoalTime + (float)MissFrame / Application.targetFrameRate))
                    .OrderByDescending(x => x.ElapsedTime)
                    .FirstOrDefault();
            }

            // ノーツを処理する
            void Exec(MyNotes notes)
            {
                var result = Judge(notes);
                if (result > NotesResult.Miss)
                {
                    MyCharacter.Instance.PlayAttack();
                    MyPlayer.Instance.PlayResult(notes.Data.Side, result);
                    DismissNote(notes, result);
                }
            }
            
            // ノート判定
            NotesResult Judge(MyNotes notes)
            {
                var diff = Mathf.Abs(notes.GoalTime - notes.ElapsedTime);
                var frame = diff * Application.targetFrameRate;

                // ノーツ判定
                if (frame < PerfectFrame)
                {
                    return NotesResult.Perfect;
                }
                if (frame < GreatFrame)
                {
                    return NotesResult.Great;
                }
                if (frame < GoodFrame)
                {
                    return NotesResult.Good;
                }
                if (frame < BadFrame)
                {
                    return NotesResult.Bad;
                }
                if (frame < MissFrame)
                {
                    return NotesResult.Miss;
                }
                
                return NotesResult.None;
            }
        }

        /// <summary>
        /// ノートを消す。
        /// </summary>
        /// <param name="notes">notes</param>
        /// <param name="result">判定結果</param>
        public void DismissNote(MyNotes notes, NotesResult result)
        {
            AddScore(result);
            if (result >= NotesResult.Good)
            {
                Combo++;
            }
            else
            {
                Combo = 0;
            }
            
            activeNotes.Remove(notes);
            notes.Dismiss();
        }

        /// <summary>
        /// スコアデータ追加
        /// </summary>
        /// <param name="result"></param>
        private void AddScore(NotesResult result)
        {
            if (!Score.ContainsKey(result))
            {
                Score.Add(result, 0);
            }
            else
            {
                Score[result] = Score[result] + 1;
            }
        }

        /// <summary>
        /// タップの状態を可視化する。
        /// </summary>
        private void ObserveTapState()
        {
            // left - enter
            MyHandManager.Instance.Left.TriggerEnterObservable
                .Where(x => LayerMask.LayerToName(x.gameObject.layer) == "HitL")
                .Subscribe(_ =>
                {
                    MyPlayer.Instance.HitPointL.SetColor(Color.green);
                }).AddTo(cd);
            
            // left - exit
            MyHandManager.Instance.Left.TriggerExitObservable
                .Where(x => LayerMask.LayerToName(x.gameObject.layer) == "HitL")
                .Subscribe(_ =>
                {
                    MyPlayer.Instance.HitPointL.SetColor(Color.white);
                }).AddTo(cd);
            
            // right - enter
            MyHandManager.Instance.Right.TriggerEnterObservable
                .Where(x => LayerMask.LayerToName(x.gameObject.layer) == "HitR")
                .Subscribe(_ =>
                {
                    MyPlayer.Instance.HitPointR.SetColor(Color.green);
                }).AddTo(cd);
            
            // right - exit
            MyHandManager.Instance.Right.TriggerExitObservable
                .Where(x => LayerMask.LayerToName(x.gameObject.layer) == "HitR")
                .Subscribe(_ =>
                {
                    MyPlayer.Instance.HitPointR.SetColor(Color.white);
                }).AddTo(cd);
        }

        /// <summary>
        /// EndGame
        /// </summary>
        private async UniTask EndGameAsync()
        {
            // whistle
            number.sprite = Resources.Load<Sprite>("Texture/Finish");
            numberAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/whistle"));
            
            // finish
            number.gameObject.SetActive(true);
            number.transform.localScale = Vector3.zero;
            number.transform.DOScale(new Vector3(.03f, .03f, .03f), .5f).SetEase(Ease.InOutCubic);
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            number.gameObject.SetActive(false);
            
            // win
            MyCharacter.Instance.PlayWin();
            
            // ダイアログ
            {
                var perfect = Score.ContainsKey(NotesResult.Perfect) ? Score[NotesResult.Perfect] : 0;
                var great = Score.ContainsKey(NotesResult.Great) ? Score[NotesResult.Great] : 0;
                var good = Score.ContainsKey(NotesResult.Good) ? Score[NotesResult.Good] : 0;
                var bad = Score.ContainsKey(NotesResult.Bad) ? Score[NotesResult.Bad] : 0;
                var miss = Score.ContainsKey(NotesResult.Miss) ? Score[NotesResult.Miss] : 0;
                await MyResult.Instance.ShowAsync(perfect, great, good, bad, miss);
                await MyResult.Instance.HideAsync(false);
            }
            
            
            MyAniversary.Instance.Hide();
            
            // 終了
            ChangeState(State.Plane);
        }
    }
}
