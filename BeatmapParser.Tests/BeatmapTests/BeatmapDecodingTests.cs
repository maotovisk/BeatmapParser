using NUnit.Framework;
using BeatmapParser;
using BeatmapParser.Sections;

namespace BeatmapParser.Tests.BeatmapTests;

public class BeatmapDecodingTests
{
    [Test]
    public void Decode_ValidBeatmap_ShouldParseSuccessfully()
    {
        // Act
        var beatmap = Beatmap.Decode(TestData.SampleBeatmapContent);

        // Assert
        Assert.That(beatmap, Is.Not.Null);
        Assert.That(beatmap.Version, Is.EqualTo(14));
        Assert.That(beatmap.MetadataSection, Is.Not.Null);
        Assert.That(beatmap.GeneralSection, Is.Not.Null);
        Assert.That(beatmap.DifficultySection, Is.Not.Null);
    }

    [Test]
    public void Decode_EmptyString_ShouldThrowException()
    {
        // Arrange
        var emptyContent = string.Empty;

        // Act & Assert
        Assert.Throws<Exception>(() => Beatmap.Decode(emptyContent));
    }

    [Test]
    public void Decode_InvalidFormat_ShouldThrowException()
    {
        // Arrange
        var invalidContent = "This is not a valid beatmap file";

        // Act & Assert
        Assert.Throws<Exception>(() => Beatmap.Decode(invalidContent));
    }

    [Test]
    public void Decode_BeatmapWithoutOptionalSections_ShouldHandleGracefully()
    {
        // Act
        var beatmap = Beatmap.Decode(TestData.MinimalBeatmapContent);

        // Assert
        Assert.That(beatmap, Is.Not.Null);
        Assert.That(beatmap.Colours, Is.Null);
        Assert.That(beatmap.Editor, Is.Null);
    }
}
