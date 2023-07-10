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
        Complete = 0b00,

        /// <summary>
        /// 5' partial
        /// </summary>
        Partial5 = 0b01,

        /// <summary>
        /// 3' partial
        /// </summary>
        Partial3 = 0b10,

        /// <summary>
        /// 5' partial 且つ 3' partial
        /// </summary>
        Internal = Partial5 | OrfState.Partial3,
    }
}
