using System.Text;
using BeatmapParser.Enums;
using BeatmapParser.HitObjects;
using BeatmapParser.Sections;
using BeatmapParser.TimingPoints;

namespace BeatmapParser;

/// <summary>
/// Represents an osu! beatmap.
/// </summary>
public class Beatmap : IEncodable
{
    /// <summary>
    /// The format version of the beatmap.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// The metadata section of the beatmap.
    /// </summary>
    public MetadataSection MetadataSection { get; set; }
    /// <summary>
    /// The general section of the beatmap.
    /// </summary>
    public GeneralSection GeneralSection { get; set; }
    /// <summary>
    /// The editor section of the beatmap.
    /// </summary>
    public EditorSection? Editor { get; set; }
    /// <summary>
    /// The difficulty section of the beatmap.
    /// </summary>
    public DifficultySection DifficultySection { get; set; }
    /// <summary>
    /// The colours section of the beatmap.
    /// </summary>
    public ColoursSection? Colours { get; set; }
    /// <summary>
    /// The events section of the beatmap.
    /// </summary>
    public EventsSection Events { get; set; }
    /// <summary>
    /// The timing points section of the beatmap.
    /// </summary>
    public TimingPointsSection? TimingPoints { get; set; }
    /// <summary>
    /// The hit objects section of the beatmap.
    /// </summary>
    public HitObjectsSection HitObjects { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Beatmap"/> class.
    /// </summary>
    public Beatmap()
    {
        MetadataSection = new MetadataSection();
        GeneralSection = new GeneralSection();
        DifficultySection = new DifficultySection();
        Events = new EventsSection();
        HitObjects = new HitObjectsSection();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Beatmap"/> class with the specified parameters.
    /// </summary>
    /// <param name="version"></param>
    /// <param name="metadataSection"></param>
    /// <param name="generalSection"></param>
    /// <param name="editor"></param>
    /// <param name="difficultySection"></param>
    /// <param name="colours"></param>
    /// <param name="events"></param>
    /// <param name="timingPoints"></param>
    /// <param name="hitObjects"></param>
    private Beatmap(int version, MetadataSection metadataSection, GeneralSection generalSection, EditorSection? editor, DifficultySection difficultySection, ColoursSection? colours, EventsSection events, TimingPointsSection? timingPoints, HitObjectsSection hitObjects)
    {
        Version = version;
        MetadataSection = metadataSection;
        GeneralSection = generalSection;
        Editor = editor;
        DifficultySection = difficultySection;
        Colours = colours;
        Events = events;
        TimingPoints = timingPoints;
        HitObjects = hitObjects;
    }

    /// <summary>
    /// Decodes the contents of a .osu beatmap file into a <see cref="Beatmap"/> instance.
    /// Currently supports format version 14.
    /// </summary>
    /// <param name="beatmapString">The full textual contents of the .osu file.</param>
    /// <returns>A populated <see cref="Beatmap"/> object.</returns>
    /// <exception cref="Exception">Thrown if the input is empty, the format is invalid, or required sections are missing.</exception>
    public static Beatmap Decode(string beatmapString)
    {
        var lines = beatmapString.Split(["\r\n", "\n"], StringSplitOptions.None)
                           .Where(line => !string.IsNullOrEmpty(line.Trim()) || !string.IsNullOrWhiteSpace(line.Trim()))
                           .ToList();

        if (lines.Count == 0) throw new Exception("Beatmap is empty.");

        var sections = new Dictionary<string, List<string>>();
        var currentSection = string.Empty;

        if (!lines[0].Contains("file format")) throw new Exception("Invalid file format.");

        var formatVersion = int.Parse(lines[0].Split("v")[1].Trim());
        Helpers.Helper.FormatVersion = formatVersion == 128 ? 128 : 14;

        foreach (var line in lines[1..])
        {
            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                currentSection = line.Trim('[', ']');
                sections[currentSection] = [];
            }
            else
            {
                sections[currentSection].Add(currentSection == "Events" ? line : line.Trim());
            }
        }

        var general = GeneralSection.Decode(sections[$"{SectionType.General}"]);
        var editor = sections.ContainsKey($"{SectionType.Editor}") ? EditorSection.Decode(sections[$"{SectionType.Editor}"]) : null;
        var metadata = MetadataSection.Decode(sections[$"{SectionType.Metadata}"]);
        var difficulty = DifficultySection.Decode(sections[$"{SectionType.Difficulty}"]);
        var colours = sections.ContainsKey($"{SectionType.Colours}") ? ColoursSection.Decode(sections[$"{SectionType.Colours}"]) : null;
        var events = EventsSection.Decode(sections[$"{SectionType.Events}"]);
        var timingPoints = sections.ContainsKey($"{SectionType.TimingPoints}") ? TimingPointsSection.Decode(sections[$"{SectionType.TimingPoints}"]) : null;
        var hitObjects = HitObjectsSection.Decode(sections[$"{SectionType.HitObjects}"], timingPoints ?? new TimingPointsSection(), difficulty);

        return new Beatmap(
            formatVersion, metadata, general, editor, difficulty, colours, events, timingPoints, hitObjects
        );
    }

    /// <summary>
    /// Reads a .osu file from disk and decodes it into a <see cref="Beatmap"/> instance.
    /// </summary>
    /// <param name="path">The file path to the .osu beatmap.</param>
    /// <returns>A populated <see cref="Beatmap"/> object.</returns>
    /// <exception cref="Exception">Thrown if the file cannot be read or its contents are invalid.</exception>
    public static Beatmap Decode(FileInfo path) => Decode(File.ReadAllText(path.FullName));


    /// <summary>
    /// Encodes the current instance of the <see cref="Beatmap"/> into a string that conforms
    /// to the osu file format specification.
    /// </summary>
    /// <returns>
    /// A string representation of the beatmap in the osu file format.
    /// </returns>
    public string Encode()
    {
        StringBuilder builder = new();

        // INFO: we don't plan to support encoding for formats other than 14 and 128
        var headerVersion = Version == 128 ? 128 : 14;
        Helpers.Helper.FormatVersion = headerVersion;
        builder.AppendLine($"osu file format v{headerVersion}");
        builder.AppendLine();

        builder.AppendLine($"[{SectionType.General}]");
        builder.AppendLine(GeneralSection.Encode());

        if (Editor != null)
        {
            builder.AppendLine($"[{SectionType.Editor}]");
            builder.AppendLine(Editor.Encode());
        }

        builder.AppendLine($"[{SectionType.Metadata}]");
        builder.AppendLine(MetadataSection.Encode());
        
        builder.AppendLine($"[{SectionType.Difficulty}]");
        builder.AppendLine(DifficultySection.Encode());

        builder.AppendLine($"[{SectionType.Events}]");
        if (Events.EventList.Count > 0)
            builder.AppendLine(Events.Encode());
        
        if (Helpers.Helper.FormatVersion != 128)
            builder.AppendLine();

        if (TimingPoints != null)
        {
            builder.AppendLine($"[{SectionType.TimingPoints}]");
            if (TimingPoints.TimingPointList.Count > 0)
                builder.AppendLine(TimingPoints.Encode());
            builder.AppendLine();
        }

        if (Colours != null)
        {
            builder.AppendLine($"[{SectionType.Colours}]");
            builder.AppendLine(Colours.Encode());
        }

        builder.AppendLine($"[{SectionType.HitObjects}]");
        builder.Append(HitObjects.Encode());

        return builder.ToString();
    }

    /// <summary>
    /// Gets the uninherited timing point active at the specified time.
    /// </summary>
    /// <param name="time">The time in milliseconds.</param>
    /// <returns>The matching <see cref="TimingPointsSection.UninheritedTimingPoint"/> if one exists; otherwise, null.</returns>
    public UninheritedTimingPoint? GetUninheritedTimingPointAt(double time)
    {
        return TimingPoints?.GetUninheritedTimingPointAt(time);
    }

    /// <summary>
    /// Gets the inherited timing point (green line) active at the specified time.
    /// </summary>
    /// <param name="time">The time in milliseconds.</param>
    /// <returns>The matching <see cref="TimingPointsSection.InheritedTimingPoint"/> if one exists; otherwise, null.</returns>
    public InheritedTimingPoint? GetInheritedTimingPointAt(double time)
    {
        return TimingPoints?.GetInheritedTimingPointAt(time);
    }

    /// <summary>
    /// Gets the effective volume at the specified time, based on the active timing point.
    /// </summary>
    /// <param name="time">The time in milliseconds.</param>
    /// <returns>The volume as a percentage (0–100). Defaults to 100 if no timing point exists.</returns>
    public uint GetVolumeAt(double time)
    {
        return TimingPoints?.GetVolumeAt(time) ?? (uint)100;
    }

    /// <summary>
    /// Gets the effective BPM at the specified time.
    /// </summary>
    /// <param name="time">The time in milliseconds.</param>
    /// <returns>The BPM value as a double. Defaults to 120 if no timing point exists.</returns>
    public double GetBpmAt(double time)
    {
        return TimingPoints?.GetBpmAt(time) ?? 120;
    }

    /// <summary>
    /// Gets the hit object occurring at or near the specified time.
    /// </summary>
    /// <param name="time">The time in milliseconds.</param>
    /// <param name="leniency">The allowed time difference in milliseconds when matching the object (default is 2 ms).</param>
    /// <returns>The matching <see cref="HitObjectsSection.IHitObject"/> if found; otherwise, null.</returns>
    public IHitObject? GetHitObjectAt(double time, int leniency = 2)
    {
        return HitObjects.GetHitObjectAt(time, leniency);
    }
    
    /// <summary>
    /// Gets the current Background image name for the beatmap
    /// </summary>
    public string? GetBackgroundFilename()
    {
        return Events.GetBackgroundImage();
    }
    
    /// <summary>
    /// Sets the current background image name for the beatmap
    /// </summary>
    /// <param name="filename">Complete filename with extension of the file in the root folder of the beatmap.</param>
    public void SetBackgroundFileName(string filename)
    {
        Events.SetBackgroundImage(filename);
    }
}
