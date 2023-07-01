using System.Collections.Generic;
using System.Collections.ObjectModel;

using NA = Stran.Logics.NucleotideBase;
using AA = Stran.Logics.AminoAcid;

namespace Stran.Logics
{
    public sealed partial class GeneticCodeTable
    {
        /// <summary>
        /// 既定の遺伝暗号を持つの遺伝コードテーブルのインスタンスを取得します。
        /// </summary>
        public static GeneticCodeTable Default => _default ??= CreateDefault();

        private static GeneticCodeTable? _default;

        /// <summary>
        /// <see cref="Default"/>となる<see cref="GeneticCodeTable"/>のインスタンスを生成します。
        /// </summary>
        /// <returns>既定の遺伝暗号を持つ<see cref="GeneticCodeTable"/>の新しいインスタンス</returns>
        private static GeneticCodeTable CreateDefault()
        {
            var dictionary = new Dictionary<Triplet, AA>(64)
            {
                [(NA.U, NA.U, NA.U)] = AA.F,
                [(NA.U, NA.U, NA.C)] = AA.F,
                [(NA.U, NA.U, NA.A)] = AA.L,
                [(NA.U, NA.U, NA.G)] = AA.L,
                [(NA.U, NA.C, NA.U)] = AA.S,
                [(NA.U, NA.C, NA.C)] = AA.S,
                [(NA.U, NA.C, NA.A)] = AA.S,
                [(NA.U, NA.C, NA.G)] = AA.S,
                [(NA.U, NA.A, NA.U)] = AA.Y,
                [(NA.U, NA.A, NA.C)] = AA.Y,
                [(NA.U, NA.A, NA.A)] = AA.End,
                [(NA.U, NA.A, NA.G)] = AA.End,
                [(NA.U, NA.G, NA.U)] = AA.C,
                [(NA.U, NA.G, NA.C)] = AA.C,
                [(NA.U, NA.G, NA.A)] = AA.End,
                [(NA.U, NA.G, NA.G)] = AA.W,

                [(NA.C, NA.U, NA.U)] = AA.L,
                [(NA.C, NA.U, NA.C)] = AA.L,
                [(NA.C, NA.U, NA.A)] = AA.L,
                [(NA.C, NA.U, NA.G)] = AA.L,
                [(NA.C, NA.C, NA.U)] = AA.P,
                [(NA.C, NA.C, NA.C)] = AA.P,
                [(NA.C, NA.C, NA.A)] = AA.P,
                [(NA.C, NA.C, NA.G)] = AA.P,
                [(NA.C, NA.A, NA.U)] = AA.H,
                [(NA.C, NA.A, NA.C)] = AA.H,
                [(NA.C, NA.A, NA.A)] = AA.Q,
                [(NA.C, NA.A, NA.G)] = AA.Q,
                [(NA.C, NA.G, NA.U)] = AA.R,
                [(NA.C, NA.G, NA.C)] = AA.R,
                [(NA.C, NA.G, NA.A)] = AA.R,
                [(NA.C, NA.G, NA.G)] = AA.R,

                [(NA.A, NA.U, NA.U)] = AA.I,
                [(NA.A, NA.U, NA.C)] = AA.I,
                [(NA.A, NA.U, NA.A)] = AA.I,
                [(NA.A, NA.U, NA.G)] = AA.M,
                [(NA.A, NA.C, NA.U)] = AA.T,
                [(NA.A, NA.C, NA.C)] = AA.T,
                [(NA.A, NA.C, NA.A)] = AA.T,
                [(NA.A, NA.C, NA.G)] = AA.T,
                [(NA.A, NA.A, NA.U)] = AA.N,
                [(NA.A, NA.A, NA.C)] = AA.N,
                [(NA.A, NA.A, NA.A)] = AA.K,
                [(NA.A, NA.A, NA.G)] = AA.K,
                [(NA.A, NA.G, NA.U)] = AA.S,
                [(NA.A, NA.G, NA.C)] = AA.S,
                [(NA.A, NA.G, NA.A)] = AA.R,
                [(NA.A, NA.G, NA.G)] = AA.R,

                [(NA.G, NA.U, NA.U)] = AA.V,
                [(NA.G, NA.U, NA.C)] = AA.V,
                [(NA.G, NA.U, NA.A)] = AA.V,
                [(NA.G, NA.U, NA.G)] = AA.V,
                [(NA.G, NA.C, NA.U)] = AA.A,
                [(NA.G, NA.C, NA.C)] = AA.A,
                [(NA.G, NA.C, NA.A)] = AA.A,
                [(NA.G, NA.C, NA.G)] = AA.A,
                [(NA.G, NA.A, NA.U)] = AA.D,
                [(NA.G, NA.A, NA.C)] = AA.D,
                [(NA.G, NA.A, NA.A)] = AA.E,
                [(NA.G, NA.A, NA.G)] = AA.E,
                [(NA.G, NA.G, NA.U)] = AA.G,
                [(NA.G, NA.G, NA.C)] = AA.G,
                [(NA.G, NA.G, NA.A)] = AA.G,
                [(NA.G, NA.G, NA.G)] = AA.G,
            };
            var result = new GeneticCodeTable(new ReadOnlyDictionary<Triplet, AA>(dictionary));
            result.Starts.Add(new Triplet(NA.A, NA.U, NA.G));
            return result;
        }
    }
}
