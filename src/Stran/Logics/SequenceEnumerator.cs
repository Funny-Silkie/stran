using System;
using System.Collections;
using System.Collections.Generic;

namespace Stran.Logics
{
    /// <summary>
    /// <see cref="ISequence{TComponent}"/>の列挙をサポートする構造体です。
    /// </summary>
    /// <typeparam name="TSequence">配列の型</typeparam>
    /// <typeparam name="TComponent">配列を構成する要素の型</typeparam>
    [Serializable]
    public struct SequenceEnumerator<TSequence, TComponent> : IEnumerator<TComponent>
        where TSequence : ISequence<TSequence, TComponent>
        where TComponent : ISequenceComponent<TComponent>
    {
        private int index;
        private readonly TSequence sequence;

        /// <inheritdoc/>
        public TComponent Current { get; private set; }

        readonly object IEnumerator.Current
        {
            get
            {
                if (index == 0 || index == sequence.Length + 1) throw new InvalidOperationException("列挙が開始されていません");
                return Current;
            }
        }

        /// <summary>
        /// <see cref="SequenceEnumerator{TSequence, TComponent}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="sequence">列挙する<typeparamref name="TSequence"/>のインスタンス</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequence"/>が<see langword="null"/></exception>
        internal SequenceEnumerator(TSequence sequence)
        {
            this.sequence = sequence ?? throw new ArgumentNullException(nameof(sequence));
            index = 0;
            Current = default!;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public bool MoveNext()
        {
            if (index < sequence.Length)
            {
                Current = sequence[index++];
                return true;
            }
            Current = default!;
            index = sequence.Length + 1;
            return false;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }

        void IEnumerator.Reset()
        {
            Current = default!;
            index = 0;
        }
    }
}
