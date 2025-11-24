namespace BeatmapParser.Tests;

public static class TestData
{
    public const string SampleBeatmapContent = "osu file format v14\n\n[General]\nAudioFilename: audio.mp3\nAudioLeadIn: 0\nPreviewTime: 10000\nCountdown: 0\nSampleSet: Normal\nStackLeniency: 0.7\nMode: 0\nLetterboxInBreaks: 0\nWidescreenStoryboard: 0\n\n[Editor]\nBookmarks: 10000,20000\nDistanceSpacing: 1.2\nBeatDivisor: 4\nGridSize: 4\nTimelineZoom: 1\n\n[Metadata]\nTitle:Test Song\nTitleUnicode:Test Song\nArtist:Test Artist\nArtistUnicode:Test Artist\nCreator:Test Creator\nVersion:Normal\nSource:\nTags:test beatmap\nBeatmapID:0\nBeatmapSetID:-1\n\n[Difficulty]\nHPDrainRate:5\nCircleSize:4\nOverallDifficulty:5\nApproachRate:5\nSliderMultiplier:1.4\nSliderTickRate:1\n\n[Events]\n//Background and Video events\n0,0,\"bg.jpg\",0,0\n\n[TimingPoints]\n0,500,4,2,0,50,1,0\n10000,500,4,2,0,50,1,0\n\n[HitObjects]\n256,192,1000,1,0,0:0:0:0:\n100,100,2000,1,0,0:0:0:0:\n";

    public const string MinimalBeatmapContent = "osu file format v14\n\n[General]\nAudioFilename: audio.mp3\n\n[Metadata]\nTitle:Test\nArtist:Test\nCreator:Test\nVersion:Test\n\n[Difficulty]\nHPDrainRate:5\nCircleSize:4\nOverallDifficulty:5\nApproachRate:5\nSliderMultiplier:1.4\nSliderTickRate:1\n\n[Events]\n\n[HitObjects]\n";

    public static readonly string ReallyComplexBeatmap = LoadResourceAsset("really_complex_beatmap.osu");

    public static readonly string LazerBeatmap = LoadResourceAsset("lazer_beatmap.osu");

    private static string LoadResourceAsset(string assetName)
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), assetName);
        if (File.Exists(path))
            return File.ReadAllText(path);

        var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
        if (!string.IsNullOrEmpty(assemblyLocation))
        {
            var dir = Path.GetDirectoryName(assemblyLocation)!;
            path = Path.Combine(dir, assetName);
            if (File.Exists(path))
                return File.ReadAllText(path);

            path = Path.Combine(dir, "Resources", assetName);
            if (File.Exists(path))
                return File.ReadAllText(path);
        }

        path = Path.Combine("Resources", assetName);
        if (File.Exists(path))
            return File.ReadAllText(path);

        return string.Empty;
    }
}
