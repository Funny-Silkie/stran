using CuiLib.Commands;
using CuiLib.Options;

namespace Stran.Commands
{
    /// <summary>
    /// メインコマンドを表します。
    /// </summary>
    internal sealed class MainCommand : Command
    {
        #region Options

        private readonly FlagOption OptionHelp;
        private readonly FlagOption OptionVersion;

        #endregion Options

        /// <summary>
        /// <see cref="MainCommand"/>の新しいインスタンスを初期化します。
        /// </summary>
        public MainCommand() : base(Util.AppName)
        {
            Description = "塩基配列の翻訳を行います";

            OptionHelp = new FlagOption('h', "help")
            {
                Description = "ヘルプを表示します",
            }.AddTo(Options);
            OptionVersion = new FlagOption('v', "version")
            {
                Description = "バージョンを表示します",
            }.AddTo(Options);
        }

        /// <inheritdoc/>
        protected override void OnExecution()
        {
            if (OptionHelp.Value)
            {
                WriteHelp(SR.StdOut);
                return;
            }
            if (OptionVersion.Value)
            {
                SR.StdOut.WriteLine(Util.Version.ToString(3));
                return;
            }
        }
    }
}
