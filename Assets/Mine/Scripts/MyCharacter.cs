using UnityEngine;
using Random = UnityEngine.Random;

namespace Mine
{
    /// <summary>
    /// Character
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class MyCharacter : MonoBehaviour
    {
        /// <summary>
        /// Animator
        /// </summary>
        private Animator _animator;

        /// <summary>
        /// AudioSource
        /// </summary>
        private AudioSource _audio;

        /// <summary>
        /// Shot
        /// </summary>
        [SerializeField]
        private AudioClip shotClip;
        
        /// <summary>
        /// Beam
        /// </summary>
        [SerializeField]
        private AudioClip beamClip;
        
        /// <summary>
        /// Singleton
        /// </summary>
        public static MyCharacter Instance { get; private set; }

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            Instance = this;
            _animator = GetComponent<Animator>();
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
        /// Attack
        /// </summary>
        public void PlayAttack()
        {
            switch (Random.Range(0, 3))
            {
                case 0:
                    _audio.PlayOneShot(shotClip);
                    _animator.SetTrigger("attack");
                    break;
                case 1:
                    _audio.PlayOneShot(shotClip);
                    _animator.SetTrigger("skill01");
                    break;
                case 2:
                    _audio.PlayOneShot(shotClip);
                    _animator.SetTrigger("skill02");
                    break;
            }
        }

        /// <summary>
        /// Idle
        /// </summary>
        public void PlayIdle()
        {
            _animator.Play("idle");
        }
        
        /// <summary>
        /// Win
        /// </summary>
        public void PlayWin()
        {
            _animator.Play("win");
        }
        
        /// <summary>
        /// Jump
        /// </summary>
        public void PlayJump()
        {
            _animator.SetTrigger("jump");
        }
        
        /// <summary>
        /// Run
        /// </summary>
        public void PlayRun()
        {
            _animator.SetTrigger("run");
        }
        
        /// <summary>
        /// Animation Event
        /// </summary>
        private void OnEventFx(GameObject InEffect)
        {
            var newSpell = Instantiate(InEffect);
            newSpell.transform.position = transform.position;
            newSpell.transform.rotation = transform.rotation;
            newSpell.transform.localScale = new Vector3(.075f, .075f, .075f);
            Destroy(newSpell, 1.0f);
        }
    }
}
