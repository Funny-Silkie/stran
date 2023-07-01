using System;

namespace Stran.Logics
{
    /// <summary>
    /// 配列の変換方法を表します。
    /// </summary>
    [Serializable]
    public enum SequenceParsingOption : byte
    {
        /// <summary>
        /// 変換に失敗とします。
        /// </summary>
        Error = 0,

        /// <summary>
        /// 該当部分を無視します。
        /// </summary>
        Skip = 1,

        /// <summary>
        /// ギャップとして変換します。
        /// </summary>
        Gap = 2,

        /// <summary>
        /// Anyとして変換します。
        /// </summary>
        Any = 3,
    }
}
