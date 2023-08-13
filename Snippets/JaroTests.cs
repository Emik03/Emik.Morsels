// SPDX-License-Identifier: MPL-2.0
namespace Emik.NRTs.Tests;

public sealed class JaroTests
{
    [Test]
    public void BothEmpty() => AreEqual(1, "".Jaro(""));

    [Test]
    public void FirstEmpty() => AreEqual(0, "".Jaro("jaro"));

    [Test]
    public void SecondEmpty() => AreEqual(0, "distance".Jaro(""));

    [Test]
    public void Same() => AreEqual(1, "jaro".Jaro("jaro"));

    [Test]
    public void Multibyte()
    {
        const string
            A = "testabctest",
            B = "testöঙ香test";

        AreEqual(0.818, A.Jaro(B), 0.001);
        AreEqual(0.818, B.Jaro(A), 0.001);
    }

    [Test]
    public void DiffShort() => AreEqual(0.767, "dixon".Jaro("dicksonx"), 0.001);

    [Test]
    public void DiffOneCharacter() => AreEqual(0, "a".Jaro("b"));

    [Test]
    public void SameOneCharacter() => AreEqual(1, "a".Jaro("a"));

    [Test]
    public void Generic() => AreEqual(0, new[] { 1, 2 }.Jaro(new[] { 3, 4 }));

    [Test]
    public void DiffOneAndTwo() => AreEqual(0.83, "a".Jaro("ab"), 0.01);

    [Test]
    public void DiffTwoAndOne() => AreEqual(0.83, "ab".Jaro("a"), 0.01);

    [Test]
    public void DiffNoTransposition() => AreEqual(0.822, "dwayne".Jaro("duane"), 0.001);

    [Test]
    public void DiffWithTransposition() => AreEqual(0.944, "martha".Jaro("marhta"), 0.001);

    [Test]
    public void Names() => AreEqual(0.392, "Friedrich Nietzsche".Jaro("Jean-Paul Sartre"), 0.001);
}
