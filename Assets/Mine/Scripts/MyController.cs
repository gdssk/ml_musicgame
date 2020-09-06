using System;
using UniRx;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace Mine
{
    /// <summary>
    /// コントローラー管理
    /// </summary>
    public class MyController : MonoBehaviour
    {
        /// <summary>
        /// RayCastSubject
        /// </summary>
        private readonly Subject<(Vector3 point, Vector3 normal, float confidence)> _raycastSubject =
            new Subject<(Vector3 point, Vector3 normal, float confidence)>();

        /// <summary>
        /// TriggerDownSubject
        /// </summary>
        private readonly Subject<(byte controllerId, float triggerValue)> _triggerDownSubject =
            new Subject<(byte controllerId, float triggerValue)>();
        
        /// <summary>
        /// TriggerDownSubject
        /// </summary>
        private readonly Subject<(byte controllerId, MLInput.Controller.Button button)> _controllerButtonDownSubject =
            new Subject<(byte controllerId, MLInput.Controller.Button button)>();

        /// <summary>
        /// Ray
        /// </summary>
        [SerializeField]
        private LineRenderer lineRenderer;

        /// <summary>
        /// RayCastObservable
        /// </summary>
        public IObservable<(Vector3 point, Vector3 normal, float confidence)> RaycastObservable =>
            _raycastSubject;

        /// <summary>
        /// TriggerDownObservable
        /// </summary>
        public IObservable<(byte controllerId, float triggerValue)> TriggerDownObservable =>
            _triggerDownSubject;

        /// <summary>
        /// ControllerButtonDownObservable
        /// </summary>
        public IObservable<(byte controllerId, MLInput.Controller.Button button)> ControllerButtonDownObservable =>
            _controllerButtonDownSubject;
        
        /// <summary>
        /// Instance
        /// </summary>
        public static MyController Instance { get; private set; }

        /// <summary>
        /// レイを表示するかどうか
        /// </summary>
        public bool IsViewRay;

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            Instance = this;
        }
        
        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            MLInput.Start();
            MLRaycast.Start();
            MLInput.OnTriggerDown += OnTriggerDown;
            MLInput.OnControllerButtonDown += OnControllerButtonDown;
        }

        /// <summary>
        /// OnDestroy
        /// </summary>
        private void OnDestroy()
        {
            Instance = null;
            MLInput.OnTriggerDown -= OnTriggerDown;
            MLInput.OnControllerButtonDown -= OnControllerButtonDown;
            MLInput.Stop();
            MLRaycast.Stop();
        }

        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
            lineRenderer.gameObject.SetActive(IsViewRay);
            lineRenderer.SetPosition(0, transform.position);
            var rayCastParams = new MLRaycast.QueryParams
            {
                Position = transform.position,
                Direction = transform.forward,
                UpVector = transform.up,
                Height = 1,
                Width = 1,
            };
            MLRaycast.Raycast(rayCastParams, HandleOnReceiveRaycast);
        }
        
        /// <summary>
        /// Raycastコールバック
        /// </summary>
        /// <param name="state"></param>
        /// <param name="point"></param>
        /// <param name="normal"></param>
        /// <param name="confidence"></param>
        private void HandleOnReceiveRaycast(
            MLRaycast.ResultState state,
            Vector3 point,
            Vector3 normal,
            float confidence)
        {
            if (state == MLRaycast.ResultState.HitObserved)
            {
                lineRenderer.SetPosition(1, point);
                _raycastSubject.OnNext((point, normal, confidence));
            }
        }

        /// <summary>
        /// トリガーを引いたときのコールバック
        /// </summary>
        private void OnTriggerDown(byte controllerId, float triggerValue)
        {
            _triggerDownSubject.OnNext((controllerId, triggerValue));
        }

        /// <summary>
        /// ボタンを押したときのコールバック
        /// </summary>
        private void OnControllerButtonDown(byte controllerId, MLInput.Controller.Button button)
        {
            _controllerButtonDownSubject.OnNext((controllerId, button));
        }
    }
}
