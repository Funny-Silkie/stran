using Stran.Logics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    /// <summary>
    /// 翻訳のテストを記述します。
    /// </summary>
    [TestFixture]
    public class TranslationTest
    {
        /// <summary>
        /// 遺伝暗号表の読み込みをテストします。
        /// </summary>
        [Test]
        public void ReadTable()
        {
            GeneticCodeTable table = GeneticCodeTable.ReadText(Util.GetDataFilePath(SR.File_Table));

            Assert.That(table, Is.Not.Null);

            var comparisonTable = new Dictionary<string, char>(64, StringComparer.Ordinal)
            {
                ["UUU"] = 'F',
                ["UUC"] = 'F',
                ["UUA"] = 'L',
                ["UUG"] = 'L',
                ["UCU"] = 'S',
                ["UCC"] = 'S',
                ["UCA"] = 'S',
                ["UCG"] = 'S',
                ["UAU"] = 'Y',
                ["UAC"] = 'Y',
                ["UAA"] = '*',
                ["UAG"] = '*',
                ["UGU"] = 'C',
                ["UGC"] = 'C',
                ["UGA"] = '*',
                ["UGG"] = 'W',
                ["CUU"] = 'L',
                ["CUC"] = 'L',
                ["CUA"] = 'L',
                ["CUG"] = 'L',
                ["CCU"] = 'P',
                ["CCC"] = 'P',
                ["CCA"] = 'P',
                ["CCG"] = 'P',
                ["CAU"] = 'H',
                ["CAC"] = 'H',
                ["CAA"] = 'Q',
                ["CAG"] = 'Q',
                ["CGU"] = 'R',
                ["CGC"] = 'R',
                ["CGA"] = 'R',
                ["CGG"] = 'R',
                ["AUU"] = 'I',
                ["AUC"] = 'I',
                ["AUA"] = 'I',
                ["AUG"] = 'M',
                ["ACU"] = 'T',
                ["ACC"] = 'T',
                ["ACA"] = 'T',
                ["ACG"] = 'T',
                ["AAU"] = 'N',
                ["AAC"] = 'N',
                ["AAA"] = 'K',
                ["AAG"] = 'K',
                ["AGU"] = 'S',
                ["AGC"] = 'S',
                ["AGA"] = 'R',
                ["AGG"] = 'R',
                ["GUU"] = 'V',
                ["GUC"] = 'V',
                ["GUA"] = 'V',
                ["GUG"] = 'V',
                ["GCU"] = 'A',
                ["GCC"] = 'A',
                ["GCA"] = 'A',
                ["GCG"] = 'A',
                ["GAU"] = 'D',
                ["GAC"] = 'D',
                ["GAA"] = 'E',
                ["GAG"] = 'E',
                ["GGU"] = 'G',
                ["GGC"] = 'G',
                ["GGA"] = 'G',
                ["GGG"] = 'G',
            };
            var comparisonStart = new HashSet<string>(3, StringComparer.Ordinal) { "UUG", "CUG", "AUG" };
            var comparisonEnd = new HashSet<string>(3, StringComparer.Ordinal) { "UAA", "UAG", "UGA" };

            foreach ((string trp, char aa) in comparisonTable) Assert.That(table[Triplet.Parse(trp)], Is.EqualTo(AminoAcid.Parse(aa)));
            CollectionAssert.AreEquivalent(comparisonStart, table.Starts.Select(x => x.ToString()));
            CollectionAssert.AreEquivalent(comparisonEnd, table.Ends.Select(x => x.ToString()));
        }

        /// <summary>
        /// <see cref="Translator"/>のインスタンスを生成します。
        /// </summary>
        /// <param name="starts">開始コドン一覧</param>
        /// <param name="altStarts">Alternativeな開始コドン一覧</param>
        /// <param name="outputAllStarts">全ての開始候補からの配列を出力するかどうか</param>
        /// <returns><see cref="Translator"/>の新しいインスタンス</returns>
        private Translator CreateTrasnlator(string[]? starts = null, string[]? altStarts = null, bool outputAllStarts = false)
        {
            GeneticCodeTable table = GeneticCodeTable.ReadText(Util.GetDataFilePath(SR.File_Table));

            starts ??= new[] { "AUG" };
            altStarts ??= Array.Empty<string>();

            var options = new TranslationOptions()
            {
                Start = starts.Select(x => Triplet.Parse(x)).ToHashSet(),
                AlternativeStart = altStarts.Select(x => Triplet.Parse(x)).ToHashSet(),
                OutputAllStarts = outputAllStarts,
            };

            return new Translator(table, options);
        }
    }
}
