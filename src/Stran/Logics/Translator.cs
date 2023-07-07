using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Stran.Logics
{
    /// <summary>
    /// 翻訳を行うクラスです。
    /// </summary>
    public class Translator
    {
        private readonly GeneticCodeTable codonTable;
        private readonly TranslationOptions options;

        /// <summary>
        /// <see cref="Translator"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="codonTable">遺伝暗号表</param>
        /// <param name="options">オプション</param>
        public Translator(GeneticCodeTable codonTable, TranslationOptions options)
        {
            this.codonTable = codonTable;
            this.options = options;
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
        /// 核酸配列をアミノ酸配列に変換します。
        /// </summary>
        /// <param name="nucSeq">核酸配列</param>
        /// <param name="offset">オフセット</param>
        /// <returns>変換後のアミノ酸配列</returns>
        private SequenceBuilder<ProteinSequence, AminoAcid> TranslateAll(ReadOnlySpan<NucleotideBase> nucSeq, int offset)
        {
            ReadOnlySpan<Triplet> triplets = nucSeq.ToTriplets(offset);
            var builder = new SequenceBuilder<ProteinSequence, AminoAcid>(nucSeq.Length / 3);
            for (int i = 0; i < triplets.Length; i++)
            {
                AminoAcid aa = Translate(triplets[i]);
                builder.Append(aa);
            }
            return builder;
        }

        /// <summary>
        /// 核酸配列から指定したインデックスのトリプレットを取得します。
        /// </summary>
        /// <param name="seq">核酸配列</param>
        /// <param name="index">インデックス</param>
        /// <returns><paramref name="index"/>に対応したトリプレット</returns>
        private Triplet SeqToTriplet(ReadOnlyMemory<NucleotideBase> seq, int index)
        {
            ReadOnlySpan<NucleotideBase> span = seq.Span.Slice(index, 3);
            return MemoryMarshal.Cast<NucleotideBase, Triplet>(span)[0];
        }

        /// <summary>
        /// 0-2間のオフセットと±鎖で核酸配列の翻訳を行います。
        /// </summary>
        /// <param name="nucSeq">核酸配列</param>
        /// <returns>翻訳後のORF一覧</returns>
        public IEnumerable<OrfInfo> Translate(SequenceBuilder<NucleotideSequence, NucleotideBase> nucSeq)
        {
            return new[] {
                (nucSeq, SeqStrand.Plus),
                (nucSeq.GetReverseComplement(), SeqStrand.Minus)
            }.SelectMany(
                x => Enumerable.Range(0, 3)
                               .SelectMany(y => TranslatePrivate(x.Item1.AsMemory(), y, x.Item2))
            );
        }

        /// <summary>
        /// 核酸配列の翻訳を行います。
        /// </summary>
        /// <param name="nucSeq">核酸配列</param>
        /// <param name="offset">オフセット（0-2）</param>
        /// <param name="strand">strand情報</param>
        /// <returns>翻訳後のORF一覧</returns>
        private IEnumerable<OrfInfo> TranslatePrivate(ReadOnlyMemory<NucleotideBase> nucSeq, int offset, SeqStrand strand)
        {
            SequenceBuilder<ProteinSequence, AminoAcid> builder = TranslateAll(nucSeq.Span, offset); // アミノ酸配列（生データ）
            ReadOnlyMemory<AminoAcid> memory = builder.AsMemory();                                   // アミノ酸配列（Memory）
            var starts = new SortedSet<int>();                                                       // 開始コドンのインデックス一覧
            bool is5Partial = true;                                                                  // 開始コドンが一度も見つかっていないかどうか
            for (int aaIndex = 0; aaIndex < builder.Length; aaIndex++)
            {
                Triplet currentCodon = SeqToTriplet(nucSeq, aaIndex * 3 + offset);
                AminoAcid currentAa = memory.Span[aaIndex];

                // 開始コドンに当たった
                if (options.Start.Contains(currentCodon)) starts.Add(aaIndex);
                // 終止コドンに当たった
                else if (codonTable.Ends.Contains(currentCodon))
                {
                    // 開始コドンが見つかっていない場合は 5' partial
                    if (starts.Count == 0 && is5Partial)
                    {
                        yield return new OrfInfo()
                        {
                            Offset = offset,
                            Strand = strand,
                            StartCodon = default,
                            StartIndex = -1,
                            EndCodon = currentCodon,
                            EndIndex = aaIndex * 3 + offset + 2,
                            Sequence = memory[..(aaIndex + 1)],
                            State = OrfState.Partial5,
                        };
                        if (currentAa == AminoAcid.End) is5Partial = false;
                    }
                    else
                    {
                        // 開始コドンが見つかっている場合は complete
                        foreach (int startIndex in starts)
                        {
                            Triplet startCodon = SeqToTriplet(nucSeq, startIndex * 3 + offset);
                            yield return new OrfInfo()
                            {
                                Offset = offset,
                                Strand = strand,
                                StartCodon = startCodon,
                                StartIndex = startIndex * 3 + offset,
                                EndCodon = currentCodon,
                                EndIndex = aaIndex * 3 + offset + 2,
                                Sequence = memory[startIndex..(aaIndex + 1)],
                                State = OrfState.Complete,
                            };
                            if (currentAa == AminoAcid.End) is5Partial = false;
                            // 優先的な開始コドンの場合はここで切り上げ
                            if (options.Start.Contains(startCodon) && !options.OutputAllStarts)
                            {
                                // 確実に終始コドンの場合は開始コドン情報をクリア
                                if (currentAa == AminoAcid.End) starts.Clear();
                                break;
                            }
                        }
                        // 確実に終始コドンの場合は開始コドン情報をクリア
                        if (currentAa == AminoAcid.End) starts.Clear();
                    }
                }
            }

            // yield return していない残りの部分

            // 開始コドンが見つかっている場合は 3' partial
            if (starts.Count > 0)
            {
                foreach (int startIndex in starts)
                {
                    Triplet startCodon = SeqToTriplet(nucSeq, startIndex * 3 + offset);
                    yield return new OrfInfo()
                    {
                        Offset = offset,
                        Strand = strand,
                        StartCodon = startCodon,
                        StartIndex = startIndex * 3 + offset,
                        EndCodon = default,
                        EndIndex = -1,
                        Sequence = memory[startIndex..],
                        State = OrfState.Partial3,
                    };
                    // 優先的な開始コドンの場合はここで切り上げ
                    if (options.Start.Contains(startCodon) && !options.OutputAllStarts) break;
                }
            }
            // 開始コドンが見つかっておらずこれまで一切ORFを見つけていない場合は internal
            else
            {
                if (is5Partial)
                    yield return new OrfInfo()
                    {
                        Offset = offset,
                        Strand = strand,
                        StartCodon = default,
                        StartIndex = -1,
                        EndCodon = default,
                        EndIndex = -1,
                        Sequence = memory,
                        State = OrfState.Internal,
                    };
            }
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
    }
}
