using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Stran.Logics
{
    /// <summary>
    /// 配列のビルダーを表します。
    /// </summary>
    /// <typeparam name="TSequence">配列の型</typeparam>
    /// <typeparam name="TComponent">配列の要素の型</typeparam>
    [Serializable]
    public sealed class SequenceBuilder<TSequence, TComponent>
        where TSequence : ISequence<TSequence, TComponent>
        where TComponent : unmanaged, ISequenceComponent<TComponent>
    {
        internal const int DefaultLength = 4096;

        internal TComponent[] array;

        /// <summary>
        /// 容量を取得または設定します。
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">設定しようとした値が0未満または<see cref="Length"/>未満</exception>
        public int Capacity
        {
            get => array.Length;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "設定する容量が0未満です");
                if (value < Length) throw new ArgumentOutOfRangeException(nameof(value), "設定する容量が実際の長さ未満です");
                if (value != Capacity)
                {
                    TComponent[] newArray = GC.AllocateUninitializedArray<TComponent>(Capacity);
                    Array.Copy(array, newArray, array.Length);
                    array = newArray;
                }
            }
        }

        /// <summary>
        /// 配列長を取得します。
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// <see cref="SequenceBuilder{TSequence, TComponent}"/>の新しいインスタンスを初期化します。
        /// </summary>
        public SequenceBuilder()
        {
            array = GC.AllocateUninitializedArray<TComponent>(DefaultLength);
        }

        /// <summary>
        /// <see cref="SequenceBuilder{TSequence, TComponent}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="capacity">初期容量</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/>が0未満</exception>
        public SequenceBuilder(int capacity)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity), "設定する容量が0未満です");

            array = GC.AllocateUninitializedArray<TComponent>(capacity);
        }

        /// <summary>
        /// <see cref="SequenceBuilder{TSequence, TComponent}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="array">使用する配列</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/>が0未満または<paramref name="array"/>の長さを超える</exception>
        internal SequenceBuilder(TComponent[] array, int length)
        {
            ArgumentNullException.ThrowIfNull(array);
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length), "長さは0以上である必要があります");
            if (array.Length < length) throw new ArgumentOutOfRangeException(nameof(length), "ソース配列以上の長さは指定できません");

            this.array = array;
            Length = length;
        }

        /// <summary>
        /// <see cref="array"/>のサイズが足りない場合にリサイズします。
        /// </summary>
        /// <param name="required">必要サイズ</param>
        /// <exception cref="InvalidOperationException">必要サイズまで拡張できない</exception>
        private void ExpandIfShortSize(int required)
        {
            if (required <= Length) return;

            int newSize = Math.Min(Math.Max(array.Length * 4, required), Array.MaxLength);
            if (newSize < required) throw new InvalidOperationException("これ以上配列を長くできません");

            TComponent[] newArray = GC.AllocateUninitializedArray<TComponent>(newSize);
            Array.Copy(array, newArray, Length);
            array = newArray;
        }

        /// <summary>
        /// 末尾に要素を追加します。
        /// </summary>
        /// <param name="value">追加する要素</param>
        /// <returns>current instance</returns>
        public SequenceBuilder<TSequence, TComponent> Append(TComponent value)
        {
            ArgumentNullException.ThrowIfNull(value);

            return AppendPrivate(ref value, 1);
        }

        /// <summary>
        /// 末尾に配列を追加します。
        /// </summary>
        /// <param name="value">追加する配列</param>
        /// <returns>current instance</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/>がnull</exception>
        public SequenceBuilder<TSequence, TComponent> Append(TComponent[] value)
        {
            ArgumentNullException.ThrowIfNull(value);

            return Append(value.AsSpan());
        }

        /// <summary>
        /// 末尾に配列を追加します。
        /// </summary>
        /// <param name="value">追加する配列</param>
        /// <returns>current instance</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/>が0未満</exception>
        public unsafe SequenceBuilder<TSequence, TComponent> Append(TComponent* value, int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            return AppendPrivate(ref *value, count);
        }

        /// <summary>
        /// 末尾に配列を追加します。
        /// </summary>
        /// <param name="value">追加する配列</param>
        /// <returns>current instance</returns>
        public SequenceBuilder<TSequence, TComponent> Append(ReadOnlyMemory<TComponent> value) => Append(value.Span);

        /// <summary>
        /// 末尾に配列を追加します。
        /// </summary>
        /// <param name="value">追加する配列</param>
        /// <returns>current instance</returns>
        public SequenceBuilder<TSequence, TComponent> Append(ReadOnlySpan<TComponent> value)
        {
            ref TComponent appendRef = ref MemoryMarshal.GetReference(value);

            return AppendPrivate(ref appendRef, value.Length);
        }

        /// <summary>
        /// 末尾に配列を追加します。
        /// </summary>
        /// <param name="value">追加する配列</param>
        /// <returns>current instance</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/>がnull</exception>
        public SequenceBuilder<TSequence, TComponent> Append(TSequence value)
        {
            ArgumentNullException.ThrowIfNull(value);

            return Append(value.AsSpan());
        }

        /// <summary>
        /// 末尾に配列を追加します。
        /// </summary>
        /// <param name="value">追加する配列</param>
        /// <returns>current instance</returns>
        private SequenceBuilder<TSequence, TComponent> AppendPrivate(ref TComponent value, int appendCount)
        {
            if (appendCount == 0) return this;

            ExpandIfShortSize(Length + appendCount);

            ref TComponent destRef = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), Length);

            if (appendCount == 1)
            {
                destRef = value;
                Length++;
                return this;
            }

            // byte列に変換
            ref byte destBRef = ref Unsafe.As<TComponent, byte>(ref destRef);
            ref byte appendBRef = ref Unsafe.As<TComponent, byte>(ref value);
            // コピー
            Unsafe.CopyBlockUnaligned(ref destBRef, ref appendBRef, (uint)(appendCount * Unsafe.SizeOf<TComponent>() / Unsafe.SizeOf<byte>()));

            Length += appendCount;
            return this;
        }

        /// <summary>
        /// <see cref="ReadOnlyMemory{T}"/>に変換します。
        /// </summary>
        /// <returns><see cref="ReadOnlyMemory{T}"/>の新しいインスタンス</returns>
        public ReadOnlyMemory<TComponent> AsMemory() => new ReadOnlyMemory<TComponent>(array, 0, Length);

        /// <summary>
        /// <see cref="ReadOnlySpan{T}"/>に変換します。
        /// </summary>
        /// <returns><see cref="ReadOnlySpan{T}"/>の新しいインスタンス</returns>
        public ReadOnlySpan<TComponent> AsSpan() => array.AsSpan(0, Length);

        /// <summary>
        /// インスタンスの内部データを削除します。
        /// </summary>
        /// <returns>current instance</returns>
        public SequenceBuilder<TSequence, TComponent> Clear()
        {
            Length = 0;
            return this;
        }

        /// <summary>
        /// 配列に変換します。
        /// </summary>
        /// <returns>変換後の配列</returns>
        public TSequence ToSequence()
        {
            return TSequence.FromArray(array.AsSpan(0, Length).ToArray());
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var builder = new StringBuilder();
            for (int i = 0; i < Length; i++) builder.Append(array[i].ToString());
            return builder.ToString();
        }
    }
}
