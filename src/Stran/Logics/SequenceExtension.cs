using System;
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
    }
}
