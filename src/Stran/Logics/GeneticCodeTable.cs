using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
        /// トリプレットとアミノ酸の組み合わせを追加します。
        /// </summary>
        /// <param name="triplet">追加するトリプレット</param>
        /// <param name="aminoAcid">追加するアミノ酸</param>
        /// <exception cref="ArgumentException"><paramref name="triplet"/>が既に存在する</exception>
        public void Add(Triplet triplet, AminoAcid aminoAcid) => table.Add(triplet, aminoAcid);

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
    }
}
