using System;

namespace Stran.Logics
{
    /// <summary>
    /// ORFの状態を表します。
    /// </summary>
    [Flags]
    [Serializable]
    public enum OrfState
    {
        /// <summary>
        /// 全長が存在
        /// </summary>
        Complete,

        /// <summary>
        /// 5' partial
        /// </summary>
        Partial5,

        /// <summary>
        /// 3' partial
        /// </summary>
        Partial3,

        /// <summary>
        /// 5' partial 且つ 3' partial
        /// </summary>
        Internal,
    }
}
