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
    public sealed class GeneticCodeTable : IEnumerable<KeyValuePair<Triplet, AminoAcid>>
    {
        private const int CompatibleSize = 64;

        private readonly IDictionary<Triplet, AminoAcid> table;

        /// <summary>
        /// 既定の遺伝暗号を持つの遺伝コードテーブルのインスタンスを取得します。
        /// </summary>
        public static GeneticCodeTable Default => _default ??= CreateDefault();

        private static GeneticCodeTable? _default;

        /// <summary>
        /// 全てのトリプレットがアミノ酸に対応しているかどうかを表す値を取得します。
        /// </summary>
        public bool IsCompatible => table.Count == CompatibleSize;

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
        /// <see cref="Default"/>となる<see cref="GeneticCodeTable"/>のインスタンスを生成します。
        /// </summary>
        /// <returns>既定の遺伝暗号を持つ<see cref="GeneticCodeTable"/>の新しいインスタンス</returns>
        private static GeneticCodeTable CreateDefault()
        {
            var dictionary = new Dictionary<Triplet, AminoAcid>(64)
            {
                [(NucleotideBase.U, NucleotideBase.U, NucleotideBase.U)] = AminoAcid.F,
                [(NucleotideBase.U, NucleotideBase.U, NucleotideBase.C)] = AminoAcid.F,
                [(NucleotideBase.U, NucleotideBase.U, NucleotideBase.A)] = AminoAcid.L,
                [(NucleotideBase.U, NucleotideBase.U, NucleotideBase.G)] = AminoAcid.L,
                [(NucleotideBase.U, NucleotideBase.C, NucleotideBase.U)] = AminoAcid.S,
                [(NucleotideBase.U, NucleotideBase.C, NucleotideBase.C)] = AminoAcid.S,
                [(NucleotideBase.U, NucleotideBase.C, NucleotideBase.A)] = AminoAcid.S,
                [(NucleotideBase.U, NucleotideBase.C, NucleotideBase.G)] = AminoAcid.S,
                [(NucleotideBase.U, NucleotideBase.A, NucleotideBase.U)] = AminoAcid.Y,
                [(NucleotideBase.U, NucleotideBase.A, NucleotideBase.C)] = AminoAcid.Y,
                [(NucleotideBase.U, NucleotideBase.A, NucleotideBase.A)] = AminoAcid.End,
                [(NucleotideBase.U, NucleotideBase.A, NucleotideBase.G)] = AminoAcid.End,
                [(NucleotideBase.U, NucleotideBase.G, NucleotideBase.U)] = AminoAcid.C,
                [(NucleotideBase.U, NucleotideBase.G, NucleotideBase.C)] = AminoAcid.C,
                [(NucleotideBase.U, NucleotideBase.G, NucleotideBase.A)] = AminoAcid.End,
                [(NucleotideBase.U, NucleotideBase.G, NucleotideBase.G)] = AminoAcid.W,

                [(NucleotideBase.C, NucleotideBase.U, NucleotideBase.U)] = AminoAcid.L,
                [(NucleotideBase.C, NucleotideBase.U, NucleotideBase.C)] = AminoAcid.L,
                [(NucleotideBase.C, NucleotideBase.U, NucleotideBase.A)] = AminoAcid.L,
                [(NucleotideBase.C, NucleotideBase.U, NucleotideBase.G)] = AminoAcid.L,
                [(NucleotideBase.C, NucleotideBase.C, NucleotideBase.U)] = AminoAcid.P,
                [(NucleotideBase.C, NucleotideBase.C, NucleotideBase.C)] = AminoAcid.P,
                [(NucleotideBase.C, NucleotideBase.C, NucleotideBase.A)] = AminoAcid.P,
                [(NucleotideBase.C, NucleotideBase.C, NucleotideBase.G)] = AminoAcid.P,
                [(NucleotideBase.C, NucleotideBase.A, NucleotideBase.U)] = AminoAcid.H,
                [(NucleotideBase.C, NucleotideBase.A, NucleotideBase.C)] = AminoAcid.H,
                [(NucleotideBase.C, NucleotideBase.A, NucleotideBase.A)] = AminoAcid.Q,
                [(NucleotideBase.C, NucleotideBase.A, NucleotideBase.G)] = AminoAcid.Q,
                [(NucleotideBase.C, NucleotideBase.G, NucleotideBase.U)] = AminoAcid.R,
                [(NucleotideBase.C, NucleotideBase.G, NucleotideBase.C)] = AminoAcid.R,
                [(NucleotideBase.C, NucleotideBase.G, NucleotideBase.A)] = AminoAcid.R,
                [(NucleotideBase.C, NucleotideBase.G, NucleotideBase.G)] = AminoAcid.R,

                [(NucleotideBase.A, NucleotideBase.U, NucleotideBase.U)] = AminoAcid.I,
                [(NucleotideBase.A, NucleotideBase.U, NucleotideBase.C)] = AminoAcid.I,
                [(NucleotideBase.A, NucleotideBase.U, NucleotideBase.A)] = AminoAcid.I,
                [(NucleotideBase.A, NucleotideBase.U, NucleotideBase.G)] = AminoAcid.M,
                [(NucleotideBase.A, NucleotideBase.C, NucleotideBase.U)] = AminoAcid.T,
                [(NucleotideBase.A, NucleotideBase.C, NucleotideBase.C)] = AminoAcid.T,
                [(NucleotideBase.A, NucleotideBase.C, NucleotideBase.A)] = AminoAcid.T,
                [(NucleotideBase.A, NucleotideBase.C, NucleotideBase.G)] = AminoAcid.T,
                [(NucleotideBase.A, NucleotideBase.A, NucleotideBase.U)] = AminoAcid.N,
                [(NucleotideBase.A, NucleotideBase.A, NucleotideBase.C)] = AminoAcid.N,
                [(NucleotideBase.A, NucleotideBase.A, NucleotideBase.A)] = AminoAcid.K,
                [(NucleotideBase.A, NucleotideBase.A, NucleotideBase.G)] = AminoAcid.K,
                [(NucleotideBase.A, NucleotideBase.G, NucleotideBase.U)] = AminoAcid.S,
                [(NucleotideBase.A, NucleotideBase.G, NucleotideBase.C)] = AminoAcid.S,
                [(NucleotideBase.A, NucleotideBase.G, NucleotideBase.A)] = AminoAcid.R,
                [(NucleotideBase.A, NucleotideBase.G, NucleotideBase.G)] = AminoAcid.R,

                [(NucleotideBase.G, NucleotideBase.U, NucleotideBase.U)] = AminoAcid.V,
                [(NucleotideBase.G, NucleotideBase.U, NucleotideBase.C)] = AminoAcid.V,
                [(NucleotideBase.G, NucleotideBase.U, NucleotideBase.A)] = AminoAcid.V,
                [(NucleotideBase.G, NucleotideBase.U, NucleotideBase.G)] = AminoAcid.V,
                [(NucleotideBase.G, NucleotideBase.C, NucleotideBase.U)] = AminoAcid.A,
                [(NucleotideBase.G, NucleotideBase.C, NucleotideBase.C)] = AminoAcid.A,
                [(NucleotideBase.G, NucleotideBase.C, NucleotideBase.A)] = AminoAcid.A,
                [(NucleotideBase.G, NucleotideBase.C, NucleotideBase.G)] = AminoAcid.A,
                [(NucleotideBase.G, NucleotideBase.A, NucleotideBase.U)] = AminoAcid.D,
                [(NucleotideBase.G, NucleotideBase.A, NucleotideBase.C)] = AminoAcid.D,
                [(NucleotideBase.G, NucleotideBase.A, NucleotideBase.A)] = AminoAcid.E,
                [(NucleotideBase.G, NucleotideBase.A, NucleotideBase.G)] = AminoAcid.E,
                [(NucleotideBase.G, NucleotideBase.G, NucleotideBase.U)] = AminoAcid.G,
                [(NucleotideBase.G, NucleotideBase.G, NucleotideBase.C)] = AminoAcid.G,
                [(NucleotideBase.G, NucleotideBase.G, NucleotideBase.A)] = AminoAcid.G,
                [(NucleotideBase.G, NucleotideBase.G, NucleotideBase.G)] = AminoAcid.G,
            };
            var result = new GeneticCodeTable(new ReadOnlyDictionary<Triplet, AminoAcid>(dictionary));
            result.Starts.Add(new Triplet(NucleotideBase.A, NucleotideBase.U, NucleotideBase.G));
            return result;
        }

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
