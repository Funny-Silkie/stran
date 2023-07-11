using System;
using System.Collections.Generic;
using System.IO;

namespace Stran.Logics
{
    /// <summary>
    /// FASTAを制御するクラスです。
    /// </summary>
    public class FastaHandler
    {
        /// <summary>
        /// FASTAの内容を読み取って列挙します。
        /// </summary>
        /// <param name="reader">テキストリーダーのインスタンス</param>
        /// <returns>配列名と核酸配列の情報</returns>
        public IEnumerable<(ReadOnlyMemory<char> name, SequenceBuilder<NucleotideSequence, NucleotideBase> sequence)> LoadAndIterate(TextReader reader)
        {
            int defaultSize = SequenceBuilder<NucleotideSequence, NucleotideBase>.DefaultLength;

            ReadOnlyMemory<char> header = null;
            string? line;
            SequenceBuilder<NucleotideSequence, NucleotideBase>? builder = null;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrEmpty(line)) continue;
                if (line.StartsWith('>'))
                {
                    if (builder is not null)
                    {
                        yield return (header, builder);
                        defaultSize = Math.Max(defaultSize, builder.Length);
                    }

                    header = line.AsMemory()[1..].Trim();
                    builder = new SequenceBuilder<NucleotideSequence, NucleotideBase>(defaultSize);
                    continue;
                }
                builder?.Append(NucleotideSequence.Parse(line.AsSpan().Trim()));
            }
            if ((builder?.Length ?? 0) > 0)
            {
                yield return (header, builder!);
            }
        }

        /// <summary>
        /// FASTAの配列ヘッダーのタイトル部分を抽出します。
        /// </summary>
        /// <param name="name">ヘッダー文字列</param>
        /// <returns><paramref name="name"/>のタイトル部分</returns>
        public ReadOnlySpan<char> GetTitle(ReadOnlySpan<char> name)
        {
            int index = name.IndexOf(' ');
            if (index < 0) return name;
            return name[..index];
        }
    }
}
