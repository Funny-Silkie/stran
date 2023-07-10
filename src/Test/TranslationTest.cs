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

            VerifyOrf(FindOrf(orfs, SeqStrand.Plus, 0), "AUG", "UGA", OrfState.Complete, "MYTTHTRWYWNSCVKCSPVLKKHWYCLNGFGTQDTFDHWVAL*");
            VerifyOrf(FindOrf(orfs, SeqStrand.Plus, 1), "---", "UAA", OrfState.Partial5, "LSGFRCIRHTQGGIGILASNAALC*");
            VerifyOrf(FindOrf(orfs, SeqStrand.Plus, 2), "AUG", "---", OrfState.Partial3, "MQPCVKETLVLLKRVWNPGHVRPLGSAV");
            VerifyOrf(FindOrf(orfs, SeqStrand.Minus, 0), "---", "UAA", OrfState.Partial5, "SQRYPVVERVLGSKPV*");
            VerifyOrf(FindOrf(orfs, SeqStrand.Minus, 1), "---", "---", OrfState.Internal, "HSATQWSNVSWVPNPFKQYQCFFNTGLHLTQEFQYHLVCVVYIGNPT");
            VerifyOrf(FindOrf(orfs, SeqStrand.Minus, 2), "---", "UGA", OrfState.Partial5, "TALPSGRTCPGFQTRLSSTSVSLTQGCI*");
        }

        /// <summary>
        /// 翻訳をテストします。
        /// </summary>
        [Test]
        public void Translate2()
        {
            Translator translator = CreateTrasnlator(altStarts: new[] { "CUG", "UUG" }, outputAllStarts: false);
            (ReadOnlyMemory<char> _, SequenceBuilder<NucleotideSequence, NucleotideBase> seq) = LoadFasta(Util.GetDataFilePath(SR.File_TranslSrc1)).First();
            OrfInfo[] orfs = translator.Translate(seq).ToArray();

            VerifyOrf(FindOrf(orfs, SeqStrand.Plus, 0), "AUG", "UGA", OrfState.Complete, "MYTTHTRWYWNSCVKCSPVLKKHWYCLNGFGTQDTFDHWVAL*");

            OrfInfo[] orf1p = FindOrfs(orfs, SeqStrand.Plus, 1);
            Assert.That(orf1p, Has.Length.EqualTo(3));
            VerifyOrf(orf1p[0], "UUG", "UAA", OrfState.Complete, "LSGFRCIRHTQGGIGILASNAALC*");
            VerifyOrf(orf1p[1], "CUG", "UAA", OrfState.Complete, "LC*");
            VerifyOrf(orf1p[2], "UUG", "UAG", OrfState.Complete, "LEPRTRSTTG*");

            OrfInfo[] orf2p = FindOrfs(orfs, SeqStrand.Plus, 2);
            Assert.That(orf2p, Has.Length.EqualTo(3));
            VerifyOrf(orf2p[0], "UUG", "---", OrfState.Partial3, "LEFLRQMQPCVKETLVLLKRVWNPGHVRPLGSAV");
            VerifyOrf(orf2p[1], "UUG", "---", OrfState.Partial3, "LRQMQPCVKETLVLLKRVWNPGHVRPLGSAV");
            VerifyOrf(orf2p[2], "AUG", "---", OrfState.Partial3, "MQPCVKETLVLLKRVWNPGHVRPLGSAV");

            VerifyOrf(FindOrf(orfs, SeqStrand.Minus, 0), "CUG", "UAA", OrfState.Complete, "LGSKPV*");

            OrfInfo[] orf1m = FindOrfs(orfs, SeqStrand.Minus, 1);
            Assert.That(orf1m, Has.Length.EqualTo(2));
            VerifyOrf(orf1m[0], "CUG", "---", OrfState.Partial3, "LHLTQEFQYHLVCVVYIGNPT");
            VerifyOrf(orf1m[1], "UUG", "---", OrfState.Partial3, "LTQEFQYHLVCVVYIGNPT");

            OrfInfo[] orf2m = FindOrfs(orfs, SeqStrand.Minus, 2);
            Assert.That(orf2m, Has.Length.EqualTo(2));
            VerifyOrf(orf2m[0], "---", "UGA", OrfState.Partial5, "TALPSGRTCPGFQTRLSSTSVSLTQGCI*");
            VerifyOrf(orf2m[1], "UUG", "---", OrfState.Partial3, "LCVSYTSETRQ");
        }

        /// <summary>
        /// 翻訳をテストします。
        /// </summary>
        [Test]
        public void Translate3()
        {
            Translator translator = CreateTrasnlator(altStarts: new[] { "CUG", "UUG" }, outputAllStarts: true);
            (ReadOnlyMemory<char> _, SequenceBuilder<NucleotideSequence, NucleotideBase> seq) = LoadFasta(Util.GetDataFilePath(SR.File_TranslSrc1)).First();
            OrfInfo[] orfs = translator.Translate(seq).ToArray();

            OrfInfo[] orf0p = FindOrfs(orfs, SeqStrand.Plus, 0);
            Assert.That(orf0p, Has.Length.EqualTo(2));
            VerifyOrf(orf0p[0], "AUG", "UGA", OrfState.Complete, "MYTTHTRWYWNSCVKCSPVLKKHWYCLNGFGTQDTFDHWVAL*");
            VerifyOrf(orf0p[1], "CUG", "UGA", OrfState.Complete, "L*");

            OrfInfo[] orf1p = FindOrfs(orfs, SeqStrand.Plus, 1);
            Assert.That(orf1p, Has.Length.EqualTo(3));
            VerifyOrf(orf1p[0], "UUG", "UAA", OrfState.Complete, "LSGFRCIRHTQGGIGILASNAALC*");
            VerifyOrf(orf1p[1], "CUG", "UAA", OrfState.Complete, "LC*");
            VerifyOrf(orf1p[2], "UUG", "UAG", OrfState.Complete, "LEPRTRSTTG*");

            OrfInfo[] orf2p = FindOrfs(orfs, SeqStrand.Plus, 2);
            Assert.That(orf2p, Has.Length.EqualTo(6));
            VerifyOrf(orf2p[0], "UUG", "---", OrfState.Partial3, "LEFLRQMQPCVKETLVLLKRVWNPGHVRPLGSAV");
            VerifyOrf(orf2p[1], "UUG", "---", OrfState.Partial3, "LRQMQPCVKETLVLLKRVWNPGHVRPLGSAV");
            VerifyOrf(orf2p[2], "AUG", "---", OrfState.Partial3, "MQPCVKETLVLLKRVWNPGHVRPLGSAV");
            VerifyOrf(orf2p[3], "CUG", "---", OrfState.Partial3, "LVLLKRVWNPGHVRPLGSAV");
            VerifyOrf(orf2p[4], "CUG", "---", OrfState.Partial3, "LLKRVWNPGHVRPLGSAV");
            VerifyOrf(orf2p[5], "CUG", "---", OrfState.Partial3, "LGSAV");

            VerifyOrf(FindOrf(orfs, SeqStrand.Minus, 0), "CUG", "UAA", OrfState.Complete, "LGSKPV*");

            OrfInfo[] orf1m = FindOrfs(orfs, SeqStrand.Minus, 1);
            Assert.That(orf1m, Has.Length.EqualTo(2));
            VerifyOrf(orf1m[0], "CUG", "---", OrfState.Partial3, "LHLTQEFQYHLVCVVYIGNPT");
            VerifyOrf(orf1m[1], "UUG", "---", OrfState.Partial3, "LTQEFQYHLVCVVYIGNPT");

            OrfInfo[] orf2m = FindOrfs(orfs, SeqStrand.Minus, 2);
            Assert.That(orf2m, Has.Length.EqualTo(2));
            VerifyOrf(orf2m[0], "---", "UGA", OrfState.Partial5, "TALPSGRTCPGFQTRLSSTSVSLTQGCI*");
            VerifyOrf(orf2m[1], "UUG", "---", OrfState.Partial3, "LCVSYTSETRQ");
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
        /// <param name="strand">鎖</param>
        /// <param name="offSet">オフセット（0-2）</param>
        /// <returns>対応するORF</returns>
        private OrfInfo FindOrf(IEnumerable<OrfInfo> orfs, SeqStrand strand, int offSet)
        {
            OrfInfo[] array = FindOrfs(orfs, strand, offSet);
            Assert.That(array, Has.Length.EqualTo(1), "multiple ORFs are found");
            return array[0];
        }

        /// <summary>
        /// 指定した位置によるORFを取得します。
        /// </summary>
        /// <param name="orfs">ORF一覧</param>
        /// <param name="strand">鎖</param>
        /// <param name="offSet">オフセット（0-2）</param>
        /// <returns>対応するORF</returns>
        private OrfInfo[] FindOrfs(IEnumerable<OrfInfo> orfs, SeqStrand strand, int offSet)
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
