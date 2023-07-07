using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Stran.Logics
{
    /// <summary>
    /// 配列の拡張を記述します。
    /// </summary>
    public static class SequenceExtension
    {
        /// <summary>
        /// テキストとして出力します。
        /// </summary>
        /// <param name="sequence">配列データ</param>
        /// <param name="writer">出力先</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequence"/>または<paramref name="writer"/>が<see langword="null" /></exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        /// <exception cref="IOException">I/Oエラーが発生した</exception>
        public static void WriteTo<TSequence, TComponent>(this TSequence sequence, TextWriter writer)
            where TSequence : ISequence<TSequence, TComponent>
            where TComponent : unmanaged, ISequenceComponent<TComponent>
        {
            ArgumentNullException.ThrowIfNull(sequence);
            ArgumentNullException.ThrowIfNull(writer);

            ReadOnlySpan<TComponent> span = sequence.AsSpan();
            for (int i = 0; i < span.Length; i++) writer.Write(span[i].SingleName);
        }

        /// <summary>
        /// テキストとして出力します。
        /// </summary>
        /// <param name="builder">配列データ</param>
        /// <param name="writer">出力先</param>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/>または<paramref name="writer"/>が<see langword="null" /></exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        /// <exception cref="IOException">I/Oエラーが発生した</exception>
        public static void WriteTo<TSequence, TComponent>(this SequenceBuilder<TSequence, TComponent> builder, TextWriter writer)
            where TSequence : ISequence<TSequence, TComponent>
            where TComponent : unmanaged, ISequenceComponent<TComponent>
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(writer);

            for (int i = 0; i < builder.Length; i++) writer.Write(builder.array[i].SingleName);
        }

        /// <summary>
        /// 配列データをテキストとして出力します。
        /// </summary>
        /// <param name="orf">配列データ</param>
        /// <param name="writer">出力先</param>
        /// <exception cref="ArgumentNullException"><paramref name="orf"/>または<paramref name="writer"/>が<see langword="null" /></exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        /// <exception cref="IOException">I/Oエラーが発生した</exception>
        public static void WriteSequence(this OrfInfo orf, TextWriter writer)
        {
            ArgumentNullException.ThrowIfNull(orf);
            ArgumentNullException.ThrowIfNull(writer);

            if (orf.Length == 0) return;

            ReadOnlySpan<AminoAcid> span = orf.Sequence.Span;
            for (int i = 0; i < orf.Sequence.Length - 1; i++) writer.Write(span[i].SingleName);
            if (!orf.State.HasFlag(OrfState.Partial3)) writer.Write(AminoAcid.End.SingleName);
            else writer.Write(span[^1].SingleName);
        }

        /// <summary>
        /// トリプレットを取得します。
        /// </summary>
        /// <param name="offset">読み取り開始座位</param>
        /// <returns><paramref name="offset"/>から読み取り開始した際のトリプレット</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/>が0未満</exception>
        public static unsafe ReadOnlySpan<Triplet> ToTriplets(this ReadOnlySpan<NucleotideBase> source, int offset = 0)
        {
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));

            int length = (source.Length - offset) / 3;
            if (length == 0) return Array.Empty<Triplet>();
            fixed (NucleotideBase* ptr = source)
            {
                Triplet* usedPtr = (Triplet*)(ptr + offset);
                ref Triplet rf = ref Unsafe.AsRef<Triplet>(usedPtr);
                ReadOnlySpan<Triplet> span = MemoryMarshal.CreateReadOnlySpan(ref rf, length);
                return span;
            }
        }

        /// <summary>
        /// 逆順の相補鎖を取得します。
        /// </summary>
        /// <param name="builder">配列データ</param>
        /// <returns>逆順の相補鎖</returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/>が<see langword="null"/></exception>
        public static SequenceBuilder<NucleotideSequence, NucleotideBase> GetReverseComplement(this SequenceBuilder<NucleotideSequence, NucleotideBase> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            if (builder.Length == 0) return new SequenceBuilder<NucleotideSequence, NucleotideBase>();
            var array = (NucleotideBase[])builder.array.Clone();
            Array.Reverse(array, 0, builder.Length);
            for (int i = 0; i < builder.Length; i++) array[i] = array[i].Complement;
            return new SequenceBuilder<NucleotideSequence, NucleotideBase>(array, builder.Length);
        }
    }
}
