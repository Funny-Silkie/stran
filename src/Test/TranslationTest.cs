using Stran.Logics;
using System;
using System.Collections.Generic;
using System.IO;
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
        /// 翻訳をテストします。
        /// </summary>
        [Test]
        public void Translate1()
        {
            Translator translator = CreateTrasnlator();
            (ReadOnlyMemory<char> _, SequenceBuilder<NucleotideSequence, NucleotideBase> seq) = LoadFasta(Util.GetDataFilePath(SR.File_TranslSrc1)).First();
            OrfInfo[] orfs = translator.Translate(seq).ToArray();

            VerifyOrf(FindOrf(orfs, 0, SeqStrand.Plus), "AUG", "UGA", OrfState.Complete, "MYTTHTRWYWNSCVKCSPVLKKHWYCLNGFGTQDTFDHWVAL*");
            VerifyOrf(FindOrf(orfs, 1, SeqStrand.Plus), "---", "UAA", OrfState.Partial5, "LSGFRCIRHTQGGIGILASNAALC*");
            VerifyOrf(FindOrf(orfs, 2, SeqStrand.Plus), "AUG", "---", OrfState.Partial3, "MQPCVKETLVLLKRVWNPGHVRPLGSAV");
            VerifyOrf(FindOrf(orfs, 0, SeqStrand.Minus), "---", "UAA", OrfState.Partial5, "SQRYPVVERVLGSKPV*");
            VerifyOrf(FindOrf(orfs, 1, SeqStrand.Minus), "---", "---", OrfState.Internal, "HSATQWSNVSWVPNPFKQYQCFFNTGLHLTQEFQYHLVCVVYIGNPT");
            VerifyOrf(FindOrf(orfs, 2, SeqStrand.Minus), "---", "UGA", OrfState.Partial5, "TALPSGRTCPGFQTRLSSTSVSLTQGCI*");
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

        /// <summary>
        /// FASTAを読み込みます。
        /// </summary>
        /// <param name="path">読み込むファイルパス</param>
        /// <returns>FASTAの内容</returns>
        private IEnumerable<(ReadOnlyMemory<char> name, SequenceBuilder<NucleotideSequence, NucleotideBase> seq)> LoadFasta(string path)
        {
            using var reader = new StreamReader(path);
            var fastaHandler = new FastaHandler();
            foreach ((ReadOnlyMemory<char> name, SequenceBuilder<NucleotideSequence, NucleotideBase> sequence) current in fastaHandler.LoadAndIterate(reader)) yield return current;
        }

        /// <summary>
        /// 指定した位置によるORFを取得します。
        /// </summary>
        /// <param name="orfs">ORF一覧</param>
        /// <param name="offSet">オフセット（0-2）</param>
        /// <param name="strand">鎖</param>
        /// <returns>対応するORF</returns>
        private OrfInfo FindOrf(IEnumerable<OrfInfo> orfs, int offSet, SeqStrand strand)
        {
            OrfInfo[] array = FindOrfs(orfs, offSet, strand);
            Assert.That(array, Has.Length.EqualTo(1), "multiple ORFs are found");
            return array[0];
        }

        /// <summary>
        /// 指定した位置によるORFを取得します。
        /// </summary>
        /// <param name="orfs">ORF一覧</param>
        /// <param name="offSet">オフセット（0-2）</param>
        /// <param name="strand">鎖</param>
        /// <returns>対応するORF</returns>
        private OrfInfo[] FindOrfs(IEnumerable<OrfInfo> orfs, int offSet, SeqStrand strand)
        {
            return orfs.Where(x => x.Offset == offSet && x.Strand == strand)
                       .ToArray();
        }

        /// <summary>
        /// <see cref="OrfInfo"/>を検証します。
        /// </summary>
        /// <param name="orf">検証するインスタンス</param>
        /// <param name="startCodon"><see cref="OrfInfo.StartCodon"/></param>
        /// <param name="endCodon"><see cref="OrfInfo.EndCodon"/></param>
        /// <param name="state"><see cref="OrfInfo.State"/></param>
        /// <param name="sequence"><see cref="OrfInfo.Sequence"/></param>
        private void VerifyOrf(OrfInfo orf, string startCodon, string endCodon, OrfState state, string sequence)
        {
            Assert.Multiple(() =>
            {
                Assert.That(startCodon, Is.EqualTo(orf.StartCodon.ToString()));
                Assert.That(endCodon, Is.EqualTo(orf.EndCodon.ToString()));
                Assert.That(state, Is.EqualTo(orf.State));
                Assert.That(sequence, Is.EqualTo(new ProteinSequence(orf.Sequence.Span).ToString()));
            });
        }

        /// <summary>
        /// <see cref="OrfInfo"/>を検証します。
        /// </summary>
        /// <param name="orf">検証するインスタンス</param>
        /// <param name="startCodon"><see cref="OrfInfo.StartCodon"/></param>
        /// <param name="startIndex"><see cref="OrfInfo.StartIndex"/></param>
        /// <param name="endCodon"><see cref="OrfInfo.EndCodon"/></param>
        /// <param name="endIndex"><see cref="OrfInfo.EndIndex"/></param>
        /// <param name="state"><see cref="OrfInfo.State"/></param>
        /// <param name="sequence"><see cref="OrfInfo.Sequence"/></param>
        private void VerifyOrf(OrfInfo orf, string startCodon, int startIndex, string endCodon, int endIndex, OrfState state, string sequence)
        {
            Assert.Multiple(() =>
            {
                Assert.That(startIndex, Is.EqualTo(orf.StartIndex));
                Assert.That(endIndex, Is.EqualTo(orf.EndIndex));
                Assert.That(startCodon, Is.EqualTo(orf.StartCodon.ToString()));
                Assert.That(endCodon, Is.EqualTo(orf.EndCodon.ToString()));
                Assert.That(state, Is.EqualTo(orf.State));
                Assert.That(sequence, Is.EqualTo(new ProteinSequence(orf.Sequence.Span).ToString()));
            });
        }
    }
}
