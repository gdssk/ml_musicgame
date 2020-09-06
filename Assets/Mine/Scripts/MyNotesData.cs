using System;

namespace Mine
{
    /// <summary>
    /// ノーツ種別
    /// </summary>
    public enum MyNotesType
    {
        /// <summary>
        /// 未指定
        /// </summary>
        None = -1,
        /// <summary>
        /// 豚
        /// </summary>
        Pig,
        /// <summary>
        /// ユニコーン
        /// </summary>
        Unicorn,
        /// <summary>
        /// 炎
        /// </summary>
        Fire,
    }

    /// <summary>
    /// ノートの出てくる位置
    /// </summary>
    public enum MyNoteSide
    {
        /// <summary>
        /// 未指定
        /// </summary>
        None = -1,
        /// <summary>
        /// 左
        /// </summary>
        Left,
        /// <summary>
        /// 右
        /// </summary>
        Right,
    }
    
    /// <summary>
    /// ノートデータ
    /// </summary>
    [Serializable]
    public struct MyNoteData
    {
        /// <summary>
        /// 種別
        /// </summary>
        public MyNotesType Type;

        /// <summary>
        /// 出てくる位置
        /// </summary>
        public MyNoteSide Side;

        /// <summary>
        /// 出現時間
        /// </summary>
        public float Time;

        /// <summary>
        /// 速度 per Second
        /// </summary>
        public const float Speed = 3.3f;
    }
}
