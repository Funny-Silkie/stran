using CuiLib;
using CuiLib.Commands;
using CuiLib.Options;
using Stran.Logics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stran.Cui.Commands
{
    /// <summary>
    /// メインコマンドを表します。
    /// </summary>
    internal sealed class MainCommand : Command
    {
        #region Options

        private readonly FlagOption OptionHelp;
        private readonly FlagOption OptionVersion;
        private readonly SingleValueOption<TextReader?> OptionIn;
        private readonly SingleValueOption<TextWriter?> OptionOut;
        private readonly MultipleValueOption<Triplet> OptionStarts;
        private readonly MultipleValueOption<Triplet> OptionAltStarts;
        private readonly SingleValueOption<string> OptionTable;
        private readonly FlagOption OptionOutputAllStarts;
        private readonly SingleValueOption<int> OptionThreads;

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
            OptionIn = new SingleValueOption<TextReader?>('i', "in")
            {
                Description = "読み込むFASTAファイル\n無指定で標準入力",
                Required = false,
                DefaultValue = null,
            }.AddTo(Options);
            OptionOut = new SingleValueOption<TextWriter?>('o', "out")
            {
                Description = "出力先FASTAファイル\n無指定で標準出力",
                Required = false,
                DefaultValue = null,
            }.AddTo(Options);
            OptionTable = new SingleValueOption<string>('t', "table")
            {
                Description = "遺伝暗号表のID (transl_table) またはファイルパス\n既定値：1",
                DefaultValue = "1",
            }.AddTo(Options);
            OptionStarts = new MultipleValueOption<Triplet>("start")
            {
                Description = "開始コドン（複数指定可能）\n既定値：AUG",
                Converter = ValueConverter.FromDelegate<string, Triplet>(Triplet.Parse),
                DefaultValue = new[] { new Triplet(NucleotideBase.A, NucleotideBase.U, NucleotideBase.G) },
            }.AddTo(Options);
            OptionAltStarts = new MultipleValueOption<Triplet>("alt-start")
            {
                Description = "Alternativeな開始コドン（複数指定可能）\n既定値：なし",
                Converter = ValueConverter.FromDelegate<string, Triplet>(Triplet.Parse),
            }.AddTo(Options);
            OptionOutputAllStarts = new FlagOption("output-all-starts")
            {
                Description = "Alternative startsで開始する配列を全て出力します",
            }.AddTo(Options);
            OptionThreads = new SingleValueOption<int>('T', "threads")
            {
                Description = $"スレッド数\n0で利用可能な全スレッド（{Util.GetAvailableThreads()}）\n既定値：1",
                Checker = ValueChecker.LargerOrEqual(0),
                DefaultValue = 1,
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

            using TextReader reader = OptionIn.Value ?? Console.In;
            //using TextReader reader = new StreamReader("random.fasta");
            using TextWriter writer = OptionOut.Value ?? Console.Out;
            string tableText = OptionTable.Value;
            int threads = OptionThreads.Value;

            GeneticCodeTable table;
            if (int.TryParse(tableText, out int ncbiIndex) && !File.Exists(tableText)) table = GeneticCodeTable.GetNcbiTable(ncbiIndex);
            else table = GeneticCodeTable.ReadText(tableText);

            var options = new TranslationOptions()
            {
                Start = new HashSet<Triplet>(OptionStarts.Value.SelectMany(x => x.AsAUGC())),
                AlternativeStart = new HashSet<Triplet>(OptionAltStarts.Value.SelectMany(x => x.AsAUGC())),
                OutputAllStarts = OptionOutputAllStarts.Value,
            };

            if (options.Start.Count == 0) throw new ArgumentAnalysisException("開始コドンを指定してください");
            if (!table.Starts.IsSupersetOf(options.Start)) throw new ArgumentAnalysisException($"開始コドンは[{string.Join(", ", table.Starts)}]に含まれる必要があります");
            if (!table.Starts.IsSupersetOf(options.AlternativeStart)) throw new ArgumentAnalysisException($"Alternativeな開始コドンは[{string.Join(", ", table.Starts)}]に含まれる必要があります");
            if (options.Start.Overlaps(options.AlternativeStart)) throw new ArgumentAnalysisException($"開始コドン[{string.Join(", ", options.Start)}]とAlternativeな開始コドン[{string.Join(", ", options.AlternativeStart)}]はオーバーラップして指定できません");

            var translator = new Translator(table, options);
            var fastaHandler = new FastaHandler();

            foreach ((ReadOnlyMemory<char> name, SequenceBuilder<NucleotideSequence, NucleotideBase> sequence) in fastaHandler.LoadAndIterate(reader))
            {
                int index = 0;
                ReadOnlySpan<char> title = fastaHandler.GetTitle(name.Span);
                foreach (OrfInfo orf in translator.Translate(sequence.AsMemory()))
                {
                    // >sequence1.p1 type:complete len:100 strand:(-) region:1-300 start-stop:XXX-XXX
                    int startIndex = orf.StartIndex < 0 ? 1 : orf.StartIndex + 1;
                    int endIndex = orf.EndIndex < 0 ? sequence.Length : orf.EndIndex + 1;
                    writer.WriteLine($">{title}.p{index++} type:{orf.State} offset:{orf.Offset} strand:(+) len:{orf.Sequence.Length} region:{startIndex}-{endIndex} start-stop:{GetCodonString(orf.StartCodon)}-{GetCodonString(orf.EndCodon)}");
                    foreach (AminoAcid aa in orf.Sequence.Span) writer.Write(aa);
                    writer.WriteLine();
                }
                foreach (OrfInfo orf in translator.Translate(sequence.ToSequence().GetReverseComplement().AsMemory()))
                {
                    int startIndex = orf.StartIndex < 0 ? 1 : orf.StartIndex + 1;
                    int endIndex = orf.EndIndex < 0 ? sequence.Length : orf.EndIndex + 1;
                    writer.WriteLine($">{title}.p{index++} type:{orf.State} offset:{orf.Offset} strand:(-) len:{orf.Sequence.Length} region:{startIndex}-{endIndex} start-stop:{GetCodonString(orf.StartCodon)}-{GetCodonString(orf.EndCodon)}");
                    foreach (AminoAcid aa in orf.Sequence.Span) writer.Write(aa);
                    writer.WriteLine();
                }
            }
            Console.ReadKey();
        }

        private static string GetCodonString(Triplet value)
        {
            return value == default ? "XXX" : value.ToString();
        }
    }
}
