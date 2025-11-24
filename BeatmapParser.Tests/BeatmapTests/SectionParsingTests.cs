using BeatmapParser;

namespace BeatmapParser.Tests.BeatmapTests;

public class SectionParsingTests
{
    [Test]
    public void Decode_MetadataSection_ShouldParseCorrectly()
    {
        // Act
        var beatmap = Beatmap.Decode(TestData.SampleBeatmapContent);

        // Assert
        Assert.That(beatmap.MetadataSection.Title, Is.EqualTo("Test Song"));
        Assert.That(beatmap.MetadataSection.Artist, Is.EqualTo("Test Artist"));
        Assert.That(beatmap.MetadataSection.Creator, Is.EqualTo("Test Creator"));
        Assert.That(beatmap.MetadataSection.Version, Is.EqualTo("Normal"));
    }

    [Test]
    public void Decode_GeneralSection_ShouldParseCorrectly()
    {
        // Act
        var beatmap = Beatmap.Decode(TestData.SampleBeatmapContent);

        // Assert
        Assert.That(beatmap.GeneralSection.AudioFilename, Is.EqualTo("audio.mp3"));
        Assert.That(beatmap.GeneralSection.AudioLeadIn, Is.EqualTo(0));
        Assert.That(beatmap.GeneralSection.PreviewTime, Is.EqualTo(10000));
        Assert.That(beatmap.GeneralSection.Mode, Is.EqualTo(0));
        Assert.That(beatmap.GeneralSection.StackLeniency, Is.EqualTo(0.7));
    }

    [Test]
    public void Decode_DifficultySection_ShouldParseCorrectly()
    {
        // Act
        var beatmap = Beatmap.Decode(TestData.SampleBeatmapContent);

        // Assert
        Assert.That(beatmap.DifficultySection.HPDrainRate, Is.EqualTo(5));
        Assert.That(beatmap.DifficultySection.CircleSize, Is.EqualTo(4));
        Assert.That(beatmap.DifficultySection.OverallDifficulty, Is.EqualTo(5));
        Assert.That(beatmap.DifficultySection.ApproachRate, Is.EqualTo(5));
        Assert.That(beatmap.DifficultySection.SliderMultiplier, Is.EqualTo(1.4));
    }

    [Test]
    public void Decode_EditorSection_ShouldParseCorrectly()
    {
        // Act
        var beatmap = Beatmap.Decode(TestData.SampleBeatmapContent);

        // Assert
        Assert.That(beatmap.Editor, Is.Not.Null);
        Assert.That(beatmap.Editor!.BeatDivisor, Is.EqualTo(4));
        Assert.That(beatmap.Editor.DistanceSpacing, Is.EqualTo(1.2));
    }

    [Test]
    public void Decode_TimingPoints_ShouldParseCorrectly()
    {
        // Act
        var beatmap = Beatmap.Decode(TestData.SampleBeatmapContent);

        // Assert
        Assert.That(beatmap.TimingPoints, Is.Not.Null);
        Assert.That(beatmap.TimingPoints!.TimingPointList.Count, Is.EqualTo(2));
    }

    [Test]
    public void Decode_HitObjects_ShouldParseCorrectly()
    {
        // Act
        var beatmap = Beatmap.Decode(TestData.SampleBeatmapContent);

        // Assert
        Assert.That(beatmap.HitObjects, Is.Not.Null);
        Assert.That(beatmap.HitObjects.Objects.Count, Is.EqualTo(2));
    }
}

