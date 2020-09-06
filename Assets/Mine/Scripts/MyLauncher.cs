using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mine
{
    /// <summary>
    /// 発射台
    /// </summary>
    public abstract class MyLauncher : MonoBehaviour
    {
        public abstract MyNotes Launch(MyNoteData data);
    }
}
