using System.Globalization;
using BeatmapParser;
using BeatmapParser.HitObjects;
using BeatmapParser.TimingPoints;

var parseResult = CliOptions.TryParse(args, out var options, out var parseError);
if (!parseResult)
{
    Console.Error.WriteLine(parseError);
    PrintUsage();
    return 1;
}

if (options.ShowHelp)
{
    PrintUsage();
    return 0;
}

var inputPath = ResolveInputPath(options.InputPath);
if (inputPath == null)
{
    Console.Error.WriteLine("No input beatmap was provided and no default test file was found.");
    PrintUsage();
    return 1;
}

if (!File.Exists(inputPath))
{
    Console.Error.WriteLine($"Input file does not exist: {inputPath}");
    return 1;
}

Beatmap beatmap;
try
{
    beatmap = Beatmap.Decode(new FileInfo(inputPath));
}
catch (Exception ex)
{
    Console.Error.WriteLine("Failed to decode beatmap.");
    Console.Error.WriteLine(ex.Message);
    return 1;
}

PrintSummary(beatmap, inputPath);

if (options.TimeMs.HasValue)
{
    PrintTimingProbe(beatmap, options.TimeMs.Value, options.LeniencyMs);
}

if (!string.IsNullOrWhiteSpace(options.WritePath))
{
    var outputPath = Path.GetFullPath(options.WritePath!, Directory.GetCurrentDirectory());
    var outputDirectory = Path.GetDirectoryName(outputPath);
    if (!string.IsNullOrWhiteSpace(outputDirectory))
    {
        Directory.CreateDirectory(outputDirectory);
    }

    File.WriteAllText(outputPath, beatmap.Encode());
    Console.WriteLine();
    Console.WriteLine($"Encoded beatmap written to: {outputPath}");
}

return 0;

static void PrintSummary(Beatmap beatmap, string inputPath)
{
    var hitObjects = beatmap.HitObjects.Objects;
    var timingPoints = beatmap.TimingPoints?.TimingPointList ?? [];

    var circleCount = hitObjects.OfType<Circle>().Count();
    var sliderCount = hitObjects.OfType<Slider>().Count();
    var spinnerCount = hitObjects.OfType<Spinner>().Count();
    var maniaHoldCount = hitObjects.OfType<ManiaHold>().Count();

    var uninheritedCount = timingPoints.OfType<UninheritedTimingPoint>().Count();
    var inheritedCount = timingPoints.OfType<InheritedTimingPoint>().Count();
    var backgroundName = TryGetBackground(beatmap);

    Console.WriteLine("Beatmap debug summary");
    Console.WriteLine($"Input: {inputPath}");
    Console.WriteLine($"Format version: {beatmap.Version}");
    Console.WriteLine($"Title: {beatmap.MetadataSection.Artist} - {beatmap.MetadataSection.Title} [{beatmap.MetadataSection.Version}]");
    Console.WriteLine($"Mapper: {beatmap.MetadataSection.Creator}");
    Console.WriteLine($"Mode: {beatmap.GeneralSection.Mode}");
    Console.WriteLine($"Audio file: {beatmap.GeneralSection.AudioFilename}");
    Console.WriteLine($"Background: {backgroundName}");
    Console.WriteLine($"Hit objects: {hitObjects.Count} (Circle {circleCount}, Slider {sliderCount}, Spinner {spinnerCount}, Hold {maniaHoldCount})");
    Console.WriteLine($"Timing points: {timingPoints.Count} (Uninherited {uninheritedCount}, Inherited {inheritedCount})");
    Console.WriteLine($"Events: {beatmap.Events.EventList.Count}");
}

static void PrintTimingProbe(Beatmap beatmap, double timeMs, int leniencyMs)
{
    Console.WriteLine();
    Console.WriteLine($"Timing probe at {timeMs.ToString("0.###", CultureInfo.InvariantCulture)}ms (leniency {leniencyMs}ms)");
    Console.WriteLine($"BPM: {beatmap.GetBpmAt(timeMs).ToString("0.###", CultureInfo.InvariantCulture)}");
    Console.WriteLine($"Volume: {beatmap.GetVolumeAt(timeMs)}");

    var uninherited = beatmap.GetUninheritedTimingPointAt(timeMs);
    var inherited = beatmap.GetInheritedTimingPointAt(timeMs);

    if (uninherited != null)
    {
        Console.WriteLine($"Active red line at: {uninherited.Time.TotalMilliseconds.ToString("0.###", CultureInfo.InvariantCulture)}ms");
    }

    if (inherited != null)
    {
        Console.WriteLine($"Active green line at: {inherited.Time.TotalMilliseconds.ToString("0.###", CultureInfo.InvariantCulture)}ms (SV {inherited.SliderVelocity.ToString("0.###", CultureInfo.InvariantCulture)})");
    }

    var hitObject = beatmap.GetHitObjectAt(timeMs, leniencyMs);
    if (hitObject == null)
    {
        Console.WriteLine("Hit object: none in range");
        return;
    }

    Console.WriteLine(
        $"Hit object: {hitObject.Type} at {hitObject.Time.TotalMilliseconds.ToString("0.###", CultureInfo.InvariantCulture)}ms");
}

static string TryGetBackground(Beatmap beatmap)
{
    try
    {
        var value = beatmap.GetBackgroundFilename();
        return string.IsNullOrWhiteSpace(value) ? "<none>" : value;
    }
    catch
    {
        return "<none>";
    }
}

static string? ResolveInputPath(string? inputPath)
{
    if (!string.IsNullOrWhiteSpace(inputPath))
    {
        return Path.GetFullPath(inputPath, Directory.GetCurrentDirectory());
    }

    if (TryFindDefaultSample(out var samplePath))
    {
        Console.WriteLine($"No input path provided. Using test sample: {samplePath}");
        return samplePath;
    }

    return null;
}

static bool TryFindDefaultSample(out string samplePath)
{
    var candidates = new[]
    {
        Path.Combine("BeatmapParser.Tests", "Resources", "really_complex_beatmap.osu"),
        Path.Combine("BeatmapParser.Tests", "Resources", "lazer_beatmap.osu"),
    };

    var current = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (current != null)
    {
        foreach (var relativePath in candidates)
        {
            var fullPath = Path.Combine(current.FullName, relativePath);
            if (File.Exists(fullPath))
            {
                samplePath = fullPath;
                return true;
            }
        }

        current = current.Parent;
    }

    samplePath = string.Empty;
    return false;
}

static void PrintUsage()
{
    Console.WriteLine("BeatmapParser.DebugCli");
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run --project BeatmapParser.DebugCli -- [path/to/map.osu] [--time <ms>] [--leniency <ms>] [--write <output.osu>]");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  --time <ms>       Probe timing/BPM/volume and hitobject around a specific millisecond.");
    Console.WriteLine("  --leniency <ms>   Hitobject probe leniency (default: 2).");
    Console.WriteLine("  --write <path>    Re-encode decoded beatmap to a file.");
    Console.WriteLine("  -h, --help        Show this help.");
    Console.WriteLine();
    Console.WriteLine("If no input path is provided, the CLI tries test resources from BeatmapParser.Tests/Resources.");
}

internal sealed record CliOptions(
    string? InputPath,
    double? TimeMs,
    int LeniencyMs,
    string? WritePath,
    bool ShowHelp)
{
    public static bool TryParse(string[] args, out CliOptions options, out string? error)
    {
        error = null;
        string? inputPath = null;
        double? timeMs = null;
        var leniencyMs = 2;
        string? writePath = null;
        var showHelp = false;

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            switch (arg)
            {
                case "-h":
                case "--help":
                    showHelp = true;
                    break;
                case "--time":
                    if (!TryReadDouble(args, ref i, out var timeValue))
                    {
                        options = default!;
                        error = "Invalid value for --time.";
                        return false;
                    }

                    timeMs = timeValue;
                    break;
                case "--leniency":
                    if (!TryReadInt(args, ref i, out var leniencyValue))
                    {
                        options = default!;
                        error = "Invalid value for --leniency.";
                        return false;
                    }

                    if (leniencyValue < 0)
                    {
                        options = default!;
                        error = "--leniency must be >= 0.";
                        return false;
                    }

                    leniencyMs = leniencyValue;
                    break;
                case "--write":
                    if (!TryReadString(args, ref i, out var outputValue))
                    {
                        options = default!;
                        error = "Invalid value for --write.";
                        return false;
                    }

                    writePath = outputValue;
                    break;
                default:
                    if (arg.StartsWith('-'))
                    {
                        options = default!;
                        error = $"Unknown option: {arg}";
                        return false;
                    }

                    if (!string.IsNullOrWhiteSpace(inputPath))
                    {
                        options = default!;
                        error = "Only one input path can be provided.";
                        return false;
                    }

                    inputPath = arg;
                    break;
            }
        }

        options = new CliOptions(inputPath, timeMs, leniencyMs, writePath, showHelp);
        return true;
    }

    private static bool TryReadDouble(string[] args, ref int index, out double value)
    {
        value = default;
        if (index + 1 >= args.Length)
        {
            return false;
        }

        index++;
        return double.TryParse(args[index], NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    private static bool TryReadInt(string[] args, ref int index, out int value)
    {
        value = default;
        if (index + 1 >= args.Length)
        {
            return false;
        }

        index++;
        return int.TryParse(args[index], NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
    }

    private static bool TryReadString(string[] args, ref int index, out string value)
    {
        value = string.Empty;
        if (index + 1 >= args.Length)
        {
            return false;
        }

        index++;
        value = args[index];
        return !string.IsNullOrWhiteSpace(value);
    }
}
