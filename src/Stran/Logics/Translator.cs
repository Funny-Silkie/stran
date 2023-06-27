using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stran.Logics
{
    /// <summary>
    /// 翻訳を行うクラスです。
    /// </summary>
    public class Translator
    {
        private readonly GeneticCodeTable codonTable;

        /// <summary>
        /// <see cref="Translator"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="codonTable">遺伝暗号表</param>
        public Translator(GeneticCodeTable codonTable)
        {
            this.codonTable = codonTable;
        }

        /// <summary>
        /// トリプレットの翻訳を行います。
        /// </summary>
        /// <param name="triplet">翻訳する核酸</param>
        /// <returns>翻訳後のアミノ酸</returns>
        public AminoAcid Translate(Triplet triplet)
        {
            if (codonTable.TryGetAminoAcid(triplet, out AminoAcid result)) return result;
            result = IterateAsAUGC(triplet).Distinct()
                                           .Select(x => codonTable[x])
                                           .Aggregate(AminoAcid.Gap, CombineAminoAcid);
            return result;
        }

        /// <summary>
        /// アミノ酸情報を結合します。
        /// </summary>
        /// <param name="left">結合するアミノ酸1</param>
        /// <param name="right">結合するアミノ酸2</param>
        /// <returns><paramref name="left"/>と<paramref name="right"/>を結合したもの</returns>
        /// <example>
        /// Gap + A = A
        ///   E + Q = Z
        ///   A + B = X
        /// </example>
        private AminoAcid CombineAminoAcid(AminoAcid left, AminoAcid right)
        {
            if (left == right) return left;
            if (left == AminoAcid.X || right == AminoAcid.X) return AminoAcid.X;
            (AminoAcid min, AminoAcid max) = Sort(left, right);
            if (min == AminoAcid.Gap) return max;

            if (min == AminoAcid.D && max == AminoAcid.N) return AminoAcid.B;
            if (min == AminoAcid.I && max == AminoAcid.L) return AminoAcid.J;
            if (min == AminoAcid.E && max == AminoAcid.Q) return AminoAcid.Z;
            return AminoAcid.X;

            static (AminoAcid min, AminoAcid max) Sort(AminoAcid left, AminoAcid right)
            {
                if (left.CompareTo(right) > 0) return (right, left);
                return (left, right);
            }
        }

        /// <summary>
        /// トリプレットの各塩基をAUGCに分解して列挙します。
        /// </summary>
        /// <param name="triplet">分解するトリプレット</param>
        /// <returns>分解されたトリプレット一覧</returns>
        private IEnumerable<Triplet> IterateAsAUGC(Triplet triplet)
        {
            NucleotideBase[] firstArray = triplet[0].AsAUGC();
            NucleotideBase[] secondArray = triplet[1].AsAUGC();
            NucleotideBase[] thirdArray = triplet[2].AsAUGC();
            for (int i1 = 0; i1 < firstArray.Length; i1++)
                for (int i2 = 0; i2 < secondArray.Length; i2++)
                    for (int i3 = 0; i3 < thirdArray.Length; i3++)
                        yield return new Triplet(firstArray[i1], secondArray[i2], thirdArray[i3]);
        }

        /// <summary>
        /// FASTAの内容を読み取って列挙します。
        /// </summary>
        /// <param name="reader">テキストリーダーのインスタンス</param>
        /// <returns>配列名と核酸配列の情報</returns>
        public IEnumerable<(ReadOnlyMemory<char> name, SequenceBuilder<NucleotideSequence, NucleotideBase> sequence)> IterateNucFastaEntries(TextReader reader)
        {
            ReadOnlyMemory<char> header = null;
            string? line;
            SequenceBuilder<NucleotideSequence, NucleotideBase>? builder = null;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrEmpty(line)) continue;
                if (line.StartsWith('>'))
                {
                    if (builder is not null) yield return (header, builder);

                    header = line.AsMemory()[1..].Trim();
                    builder = new SequenceBuilder<NucleotideSequence, NucleotideBase>();
                    continue;
                }
                builder?.Append(NucleotideSequence.Parse(line.AsSpan().Trim()));
            }
            if ((builder?.Length ?? 0) > 0)
            {
                yield return (header, builder!);
            }
        }
    }
}
