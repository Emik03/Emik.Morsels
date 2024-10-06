// SPDX-License-Identifier: MPL-2.0
namespace Emik.Morsels.Tests;

public sealed class JaroWinklerTests
{
    [Test]
    public void BothEmpty() => AreEqual(1, "".JaroWinkler(""));

    [Test]
    public void FirstEmpty() => AreEqual(0, "".JaroWinkler("jaro-winkler"));

    [Test]
    public void SecondEmpty() => AreEqual(0, "distance".JaroWinkler(""));

    [Test]
    public void Same() => AreEqual(1, "Jaro-Winkler".JaroWinkler("Jaro-Winkler"));

    [Test]
    public void Multibyte()
    {
        const string
            A = "testabctest",
            B = "testöঙ香test";

        AreEqual(0.89, A.JaroWinkler(B), 0.001);
        AreEqual(0.89, B.JaroWinkler(A), 0.001);
    }

    [Test]
    public void DiffShort()
    {
        const string
            A = "dixon",
            B = "dicksonx";

        AreEqual(0.813, A.JaroWinkler(B), 0.001);
        AreEqual(0.813, B.JaroWinkler(A), 0.001);
    }

    [Test]
    public void DiffOneCharacter() => AreEqual(0, "a".JaroWinkler("b"));

    [Test]
    public void SameOneCharacter() => AreEqual(1, "a".JaroWinkler("a"));

    [Test]
    public void DiffNoTransposition() => AreEqual(0.84, "dwayne".JaroWinkler("duane"), 0.001);

    [Test]
    public void DiffWithTransposition() => AreEqual(0.961, "martha".JaroWinkler("marhta"), 0.001);

    [Test]
    public void Names() => AreEqual(0.562, "Friedrich Nietzsche".JaroWinkler("Fran-Paul Sartre"), 0.001);

    [Test]
    public void LongPrefix() => AreEqual(0.911, "cheeseburger".JaroWinkler("cheese fries"), 0.001);

    [Test]
    public void MoreNames() => AreEqual(0.868, "Thorkel".JaroWinkler("Thorgier"), 0.001);

    [Test]
    public void LengthOfOne() => AreEqual(0.738, "Dinsdale".JaroWinkler("D"), 0.001);

    [Test]
    public void VeryLongPrefix() =>
        AreEqual(1, "thequickbrownfoxjumpedoverx".JaroWinkler("thequickbrownfoxjumpedovery"), 0.001);
}
