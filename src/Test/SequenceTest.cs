using Stran.Logics;
using System;
using System.IO;
using System.Linq;

namespace Test
{
    /// <summary>
    /// 配列の基本的な処理のテストを記述します。
    /// </summary>
    [TestFixture]
    public class SequenceTest
    {
        /// <summary>
        /// 核酸配列の構築をテストします。
        /// </summary>
        [Test]
        public void BuildNuc()
        {
            var builder = new SequenceBuilder<NucleotideSequence, NucleotideBase>();
            builder.Append(NucleotideBase.A);
            builder.Append(NucleotideBase.U);
            builder.Append(NucleotideBase.G);
            builder.Append(NucleotideBase.C);

            Assert.That(builder.ToString(), Is.EqualTo("AUGC"));

            builder.Clear();

            Assert.That(builder.ToString(), Is.EqualTo(string.Empty));

            builder.Append(new[] { NucleotideBase.Gap, NucleotideBase.A });
            builder.Append(new NucleotideSequence(new[] { NucleotideBase.C, NucleotideBase.G }));

            Assert.That(builder.ToString(), Is.EqualTo("-ACG"));
        }

        /// <summary>
        /// アミノ酸配列の構築をテストします。
        /// </summary>
        [Test]
        public void BuildAa()
        {
            var builder = new SequenceBuilder<ProteinSequence, AminoAcid>();
            builder.Append(AminoAcid.A);
            builder.Append(AminoAcid.B);
            builder.Append(AminoAcid.C);
            builder.Append(AminoAcid.D);

            Assert.That(builder.ToString(), Is.EqualTo("ABCD"));

            builder.Clear();

            Assert.That(builder.ToString(), Is.EqualTo(string.Empty));

            builder.Append(new[] { AminoAcid.Gap, AminoAcid.E });
            builder.Append(new ProteinSequence(new[] { AminoAcid.F, AminoAcid.End }));

            Assert.That(builder.ToString(), Is.EqualTo("-EF*"));
        }

        /// <summary>
        /// FASTAの読み込みをテストします。
        /// </summary>
        [Test]
        public void ReadFasta()
        {
            var fastaHandler = new FastaHandler();
            using var reader = new StreamReader(Util.GetDataFilePath(SR.File_SmallData));
            (ReadOnlyMemory<char> name, SequenceBuilder<NucleotideSequence, NucleotideBase> sequence)[] data = fastaHandler.LoadAndIterate(reader).ToArray();

            Assert.Multiple(() =>
            {
                Assert.That(data[0].name.ToString(), Is.EqualTo("Seq1"));
                Assert.That(data[0].sequence.ToString(), Is.EqualTo("AUGCAUGCAUGCAUGC"));
                Assert.That(data[1].name.ToString(), Is.EqualTo("Seq2"));
                Assert.That(data[1].sequence.ToString(), Is.EqualTo("AAAAUUUUGGGGCCCC"));
            });
        }

        /// <summary>
        /// 核酸配列→トリプレット変換をテストします。
        /// </summary>
        [Test]
        public void Nuc2Trp()
        {
            NucleotideSequence sequence = NucleotideSequence.Parse(
                "AAA" +
                "UUU" +
                "AUG" +
                "CCG" +
                "GUA" +
                "UGA" +
                "GGG" +
                "CCC");
            var comparison = new[]
            {
                NucleotideBase.A, NucleotideBase.A, NucleotideBase.A,
                NucleotideBase.U, NucleotideBase.U, NucleotideBase.U,
                NucleotideBase.A, NucleotideBase.U, NucleotideBase.G,
                NucleotideBase.C, NucleotideBase.C, NucleotideBase.G,
                NucleotideBase.G, NucleotideBase.U, NucleotideBase.A,
                NucleotideBase.U, NucleotideBase.G, NucleotideBase.A,
                NucleotideBase.G, NucleotideBase.G, NucleotideBase.G,
                NucleotideBase.C, NucleotideBase.C, NucleotideBase.C,
            };
            ReadOnlySpan<NucleotideBase> nucSpan = sequence.AsSpan();

            CollectionAssert.AreEquivalent(sequence, comparison);

            ReadOnlySpan<Triplet> trpSpan = nucSpan.ToTriplets();
#pragma warning disable NUnit2045 // Use Assert.Multiple
            Assert.That(trpSpan[0], Is.EqualTo(Triplet.Parse("AAA")));
            Assert.That(trpSpan[1], Is.EqualTo(Triplet.Parse("UUU")));
            Assert.That(trpSpan[2], Is.EqualTo(Triplet.Parse("AUG")));
            Assert.That(trpSpan[3], Is.EqualTo(Triplet.Parse("CCG")));
            Assert.That(trpSpan[4], Is.EqualTo(Triplet.Parse("GUA")));
            Assert.That(trpSpan[5], Is.EqualTo(Triplet.Parse("UGA")));
            Assert.That(trpSpan[6], Is.EqualTo(Triplet.Parse("GGG")));
            Assert.That(trpSpan[7], Is.EqualTo(Triplet.Parse("CCC")));
#pragma warning restore NUnit2045 // Use Assert.Multiple
        }
    }
}
