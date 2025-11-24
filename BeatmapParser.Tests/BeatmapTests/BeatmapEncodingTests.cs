using BeatmapParser;
using BeatmapParser.Sections;

namespace BeatmapParser.Tests.BeatmapTests;

public class BeatmapEncodingTests
{
    [Test]
    public void Encode_ValidBeatmap_ShouldGenerateString()
    {
        // Arrange
        var beatmap = new Beatmap
        {
            Version = 14,
            GeneralSection = new GeneralSection
            {
                AudioFilename = "audio.mp3",
                Mode = 0
            },
            MetadataSection = new MetadataSection
            {
                Title = "Test",
                Artist = "Test Artist",
                Creator = "Test Creator",
                Version = "Easy"
            },
            DifficultySection = new DifficultySection
            {
                HPDrainRate = 5,
                CircleSize = 4
            }
        };

        // Act
        var encoded = beatmap.Encode();

        // Assert
        Assert.That(encoded, Is.Not.Null);
        Assert.That(encoded, Does.Contain("osu file format v14"));
        Assert.That(encoded, Does.Contain("[General]"));
        Assert.That(encoded, Does.Contain("[Metadata]"));
        Assert.That(encoded, Does.Contain("AudioFilename: audio.mp3"));
    }

    [Test]
    public void Encode_Decode_RoundTrip_ShouldPreserveData()
    {
        // Arrange
        var originalBeatmap = Beatmap.Decode(TestData.SampleBeatmapContent);

        // Act
        var encoded = originalBeatmap.Encode();
        var decodedBeatmap = Beatmap.Decode(encoded);

        // Assert
        Assert.That(decodedBeatmap.MetadataSection.Title, Is.EqualTo(originalBeatmap.MetadataSection.Title));
        Assert.That(decodedBeatmap.MetadataSection.Artist, Is.EqualTo(originalBeatmap.MetadataSection.Artist));
        Assert.That(decodedBeatmap.GeneralSection.AudioFilename, Is.EqualTo(originalBeatmap.GeneralSection.AudioFilename));
        Assert.That(decodedBeatmap.DifficultySection.HPDrainRate, Is.EqualTo(originalBeatmap.DifficultySection.HPDrainRate));
    }

    [Test]
    public void ModifyAndEncode_ChangedValues_ShouldReflectInOutput()
    {
        // Arrange
        var beatmap = Beatmap.Decode(TestData.SampleBeatmapContent);
        var newAudioFilename = "new-audio.mp3";
        var newVersion = "Hard";

        // Act
        beatmap.GeneralSection.AudioFilename = newAudioFilename;
        beatmap.MetadataSection.Version = newVersion;
        var encoded = beatmap.Encode();

        // Assert
        Assert.That(encoded, Does.Contain($"AudioFilename: {newAudioFilename}"));
        Assert.That(encoded, Does.Contain($"Version:{newVersion}"));
    }
    
    [Test]
    public void Encode_ReallyComplexBeatmap_ShouldEncodeCorrectly()
    {
        var reallyComplexBeatmap = TestData.ReallyComplexBeatmap.Replace("\r\n", "\n");
        
        var encoded = Beatmap.Decode(reallyComplexBeatmap).Encode().Replace("\r\n", "\n");
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(encoded, Is.Not.Null);
            Assert.That(encoded, Does.Contain("osu file format v14"));
            Assert.That(reallyComplexBeatmap, Is.EqualTo(encoded));
        }
    }
}

