using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.RegularExpressions;

namespace Stran.Logics
{
    /// <summary>
    /// 遺伝コードテーブルのクラスです。
    /// </summary>
    [Serializable]
    public sealed partial class GeneticCodeTable : IEnumerable<KeyValuePair<Triplet, AminoAcid>>
    {
        private const int CompatibleSize = 64;

        private readonly IDictionary<Triplet, AminoAcid> table;

        /// <summary>
        /// 全てのトリプレットがアミノ酸に対応しているかどうかを表す値を取得します。
        /// </summary>
        public bool HasCompleteSet => table.Count == CompatibleSize;

        /// <summary>
        /// 開始遺伝コード一覧を取得します。
        /// </summary>
        /// <remarks>空の場合は全ての遺伝コードを開始遺伝コードとする</remarks>
        public HashSet<Triplet> Starts { get; }

        /// <summary>
        /// <see cref="GeneticCodeTable"/>の新しいインスタンスを初期化します。
        /// </summary>
        public GeneticCodeTable()
        {
            table = new Dictionary<Triplet, AminoAcid>(CompatibleSize);
            Starts = new HashSet<Triplet>();
        }

        /// <summary>
        /// <see cref="GeneticCodeTable"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="source">コピー元のコレクション</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="source"/>のトリプレットが重複している</exception>
        public GeneticCodeTable(IEnumerable<KeyValuePair<Triplet, AminoAcid>> source)
        {
            table = source switch
            {
                null => throw new ArgumentNullException(nameof(source)),
                ReadOnlyDictionary<Triplet, AminoAcid> dic => dic,
                _ => new Dictionary<Triplet, AminoAcid>(source),
            };
            Starts = new HashSet<Triplet>();
        }

        /// <summary>
        /// トリプレットに応じたアミノ酸を取得します。
        /// </summary>
        /// <param name="triplet">使用するトリプレット</param>
        /// <returns><paramref name="triplet"/>に応じた<see cref="AminoAcid"/>のインスタンス</returns>
        public AminoAcid this[Triplet triplet] => table[triplet];

        /// <summary>
        /// NCBIで定義されているテーブルを取得します。
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="id"/>が無効な値</exception>
        public static partial GeneticCodeTable GetNcbiTable(int id);

        /// <summary>
        /// トリプレットとアミノ酸の組み合わせを追加します。
        /// </summary>
        /// <param name="triplet">追加するトリプレット</param>
        /// <param name="aminoAcid">追加するアミノ酸</param>
        /// <exception cref="ArgumentException"><paramref name="triplet"/>が既に存在する</exception>
        public void Add(Triplet triplet, AminoAcid aminoAcid)
        {
            foreach (Triplet trpAUG in triplet.AsAUGC()) table.Add(trpAUG, aminoAcid);
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<Triplet, AminoAcid>> GetEnumerator() => table.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// 指定したトリプレットが含まれているかどうかを検証します。
        /// </summary>
        /// <param name="triplet">検索するトリプレット</param>
        /// <returns><paramref name="triplet"/>が含まれていたら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public bool HasTriplet(Triplet triplet) => table.ContainsKey(triplet);

        /// <summary>
        /// 指定したアミノ酸に対応する遺伝コードを取得します。
        /// </summary>
        /// <param name="aminoAcid">検索するアミノ酸</param>
        /// <returns><paramref name="aminoAcid"/>に対応する遺伝コードのリスト</returns>
        public List<Triplet> ReversalGet(AminoAcid aminoAcid)
        {
            var result = new List<Triplet>();
            foreach ((Triplet k, AminoAcid v) in table)
                if (v == aminoAcid)
                    result.Add(k);
            return result;
        }

        /// <summary>
        /// トリプレットに応じたアミノ酸を取得します。
        /// </summary>
        /// <param name="triplet">使用するトリプレット</param>
        /// <param name="aminoAcid"><paramref name="triplet"/>に応じた<see cref="AminoAcid"/>のインスタンス</param>
        /// <returns><paramref name="aminoAcid"/>を取得できたら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public bool TryGetAminoAcid(Triplet triplet, out AminoAcid aminoAcid) => table.TryGetValue(triplet, out aminoAcid);

        #region Read From Text File

        private const string LabelAas = "AAs";
        private const string LabelStarts = "Starts";
        private const string LabelBase1 = "Base1";
        private const string LabelBase2 = "Base2";
        private const string LabelBase3 = "Base3";

        [GeneratedRegex("(?<=^\\s*)\\S+(?=\\s*=.+)")]
        private static partial Regex GetTextLabelRegex();

        [GeneratedRegex(@"(?<=^.+=\s*)[ATGC]{64}")]
        private static partial Regex GetTextBaseRegex();

        [GeneratedRegex(@"(?<=^.+=\s*)[-*M]{64}")]
        private static partial Regex GetTextStartRegex();

        [GeneratedRegex(@"(?<=^.+=\s*)[A-Z*]{64}")]
        private static partial Regex GetTextAaRegex();

        /// <summary>
        /// 行のラベルに応じて正規表現オブジェクトを返します。
        /// </summary>
        /// <param name="label">行ラベル</param>
        /// <returns><paramref name="label"/>に対応したコンテンツ抽出正規表現</returns>
        /// <exception cref="ArgumentException"><paramref name="label"/>が無効</exception>
        private static Regex GetRegexFromLabel(string label)
        {
            return label switch
            {
                LabelAas => GetTextAaRegex(),
                LabelStarts => GetTextStartRegex(),
                LabelBase1 or LabelBase2 or LabelBase3 => GetTextBaseRegex(),
                _ => throw new ArgumentException("無効な行名です", nameof(label)),
            };
        }

        /// <summary>
        /// テキストから遺伝暗号表を読み込みます。
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <returns>遺伝暗号表</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/>が<see langword="null"/></exception>
        /// <exception cref="FormatException">フォーマットが無効</exception>
        public static GeneticCodeTable ReadText(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            return ReadText(stream);
        }

        /// <summary>
        /// テキストから遺伝暗号表を読み込みます。
        /// </summary>
        /// <param name="stream">ストリームオブジェクト</param>
        /// <returns>遺伝暗号表</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/>が<see langword="null"/></exception>
        /// <exception cref="FormatException">フォーマットが無効</exception>
        public static GeneticCodeTable ReadText(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);

            var result = new GeneticCodeTable();

            using var reader = new StreamReader(stream);
            var dictionary = new Dictionary<string, string>(CompatibleSize, StringComparer.Ordinal);
            try
            {
                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line) || line.StartsWith('#')) continue;
                    string label = GetTextLabelRegex().Extract(line);
                    string value = GetRegexFromLabel(label).Extract(line);
                    if (value.Length != CompatibleSize) throw new FormatException();
                    dictionary.Add(label, value);
                }
                string labelAas = dictionary[LabelAas];
                string labelStarts = dictionary[LabelStarts];
                string labelBase1 = dictionary[LabelBase1];
                string labelBase2 = dictionary[LabelBase2];
                string labelBase3 = dictionary[LabelBase3];

                for (int i = 0; i < 64; i++)
                {
                    AminoAcid aa = AminoAcid.Parse(labelAas[i]);
                    NucleotideBase b1 = NucleotideBase.Parse(labelBase1[i]);
                    NucleotideBase b2 = NucleotideBase.Parse(labelBase2[i]);
                    NucleotideBase b3 = NucleotideBase.Parse(labelBase3[i]);
                    var trp = new Triplet(b1, b2, b3);
                    if (labelStarts[i] == 'M') result.Starts.Add(trp);
                    result.Add(trp, aa);
                }
            }
            catch (Exception e) when (e is ArgumentException or FormatException)
            {
                throw new FormatException("遺伝暗号表の書式が無効です", e);
            }
            return result;
        }

        #endregion Read From Text File
    }
}
