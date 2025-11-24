using BeatmapParser;

namespace BeatmapParser.Tests.BeatmapTests;

public class BeatmapUtilityTests
{
    [Test]
    public void GetBpmAt_WithTimingPoints_ShouldReturnCorrectBpm()
    {
        // Arrange
        var beatmap = Beatmap.Decode(TestData.SampleBeatmapContent);

        // Act
        var bpm = beatmap.GetBpmAt(5000);

        // Assert
        Assert.That(bpm, Is.GreaterThan(0));
    }

    [Test]
    public void GetVolumeAt_WithTimingPoints_ShouldReturnVolume()
    {
        // Arrange
        var beatmap = Beatmap.Decode(TestData.SampleBeatmapContent);

        // Act
        var volume = beatmap.GetVolumeAt(5000);

        // Assert
        Assert.That(volume, Is.GreaterThanOrEqualTo(0));
        Assert.That(volume, Is.LessThanOrEqualTo(100));
    }

    [Test]
    public void Constructor_DefaultBeatmap_ShouldInitializeSections()
    {
        // Act
        var beatmap = new Beatmap();

        // Assert
        Assert.That(beatmap.MetadataSection, Is.Not.Null);
        Assert.That(beatmap.GeneralSection, Is.Not.Null);
        Assert.That(beatmap.DifficultySection, Is.Not.Null);
        Assert.That(beatmap.Events, Is.Not.Null);
        Assert.That(beatmap.HitObjects, Is.Not.Null);
    }
}

