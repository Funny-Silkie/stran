using CuiLib.Options;
using System;

namespace Stran
{
    /// <summary>
    /// コマンドシステムの拡張を記述します。
    /// </summary>
    internal static class CommandExtension
    {
        /// <summary>
        /// 指定したコレクションにオプションを追加します。
        /// </summary>
        /// <typeparam name="TOption">オプションの型</typeparam>
        /// <param name="option">追加するオプション</param>
        /// <param name="options">追加先</param>
        /// <exception cref="ArgumentNullException"><paramref name="option"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="option"/>が既に追加されている</exception>
        /// <returns><paramref name="option"/></returns>
        public static TOption AddTo<TOption>(this TOption option, OptionCollection options) where TOption : Option
        {
            options.Add(option);
            return option;
        }

        /// <summary>
        /// 指定したコレクションにパラメータを追加します。
        /// </summary>
        /// <typeparam name="TParameter">パラメータの型</typeparam>
        /// <param name="option">追加するパラメータ</param>
        /// <param name="options">追加先</param>
        /// <exception cref="ArgumentNullException"><paramref name="option"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="option"/>が既に追加されている</exception>
        /// <returns><paramref name="option"/></returns>
        public static TParameter AddTo<TParameter>(this TParameter option, ParameterCollection options) where TParameter : Parameter
        {
            options.Add(option);
            return option;
        }
    }
}
