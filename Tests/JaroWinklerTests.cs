// SPDX-License-Identifier: MPL-2.0
namespace Emik.NRTs.Tests;

public sealed class JaroWinklerTests
{
    [Test]
    public void BothEmpty() => AreEqual(1, "".Jaro("", useWinkler: true));

    [Test]
    public void FirstEmpty() => AreEqual(0, "".Jaro("jaro-winkler", useWinkler: true));

    [Test]
    public void SecondEmpty() => AreEqual(0, "distance".Jaro("", useWinkler: true));

    [Test]
    public void Same() => AreEqual(1, "Jaro-Winkler".Jaro("Jaro-Winkler", useWinkler: true));

    [Test]
    public void Multibyte()
    {
        const string
            A = "testabctest",
            B = "testöঙ香test";

        AreEqual(0.89, A.Jaro(B, useWinkler: true), 0.001);
        AreEqual(0.89, B.Jaro(A, useWinkler: true), 0.001);
    }

    [Test]
    public void DiffShort()
    {
        const string
            A = "dixon",
            B = "dicksonx";

        AreEqual(0.813, A.Jaro(B, useWinkler: true), 0.001);
        AreEqual(0.813, B.Jaro(A, useWinkler: true), 0.001);
    }

    [Test]
    public void DiffOneCharacter() => AreEqual(0, "a".Jaro("b", useWinkler: true));

    [Test]
    public void SameOneCharacter() => AreEqual(1, "a".Jaro("a", useWinkler: true));

    [Test]
    public void DiffNoTransposition() => AreEqual(0.84, "dwayne".Jaro("duane", useWinkler: true), 0.001);

    [Test]
    public void DiffWithTransposition() => AreEqual(0.961, "martha".Jaro("marhta", useWinkler: true), 0.001);

    [Test]
    public void Names() => AreEqual(0.562, "Friedrich Nietzsche".Jaro("Fran-Paul Sartre", useWinkler: true), 0.001);

    [Test]
    public void LongPrefix() => AreEqual(0.911, "cheeseburger".Jaro("cheese fries", useWinkler: true), 0.001);

    [Test]
    public void MoreNames() => AreEqual(0.868, "Thorkel".Jaro("Thorgier", useWinkler: true), 0.001);

    [Test]
    public void LengthOfOne() => AreEqual(0.738, "Dinsdale".Jaro("D", useWinkler: true), 0.001);

    [Test]
    public void VeryLongPrefix() =>
        AreEqual(1, "thequickbrownfoxjumpedoverx".Jaro("thequickbrownfoxjumpedovery", useWinkler: true), 0.001);
}
