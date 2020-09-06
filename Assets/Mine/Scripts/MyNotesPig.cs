using UnityEngine;

namespace Mine
{
    /// <summary>
    /// Pigノーツ
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class MyNotesPig : MyNormalNotes
    {
        /// <summary>
        /// Animator
        /// </summary>
        private Animator _animator;

        /// <summary>
        /// エリアに到着する時間
        /// </summary>
        public override float GoalTime => 1.5f;

        /// <summary>
        /// Awake
        /// </summary>
        protected void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            _animator.SetInteger("animation", 2);
        }
    }
}
