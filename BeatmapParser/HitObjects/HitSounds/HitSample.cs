using System.Globalization;
using System.Text;
using BeatmapParser.Enums;

namespace BeatmapParser.HitObjects.HitSounds;
/// <summary>
///
/// </summary>
public class HitSample
{
    /// <summary>
    /// Sample set for hit normals.
    /// </summary>
    public SampleSet NormalSet { get; set; }

    /// <summary>
    /// Sample set for additions.
    /// </summary>
    public SampleSet AdditionSet { get; set; }

    /// <summary>
    /// Index of the hit sample.
    /// </summary>
    public uint? Index { get; set; }

    /// <summary>
    /// Volume of the hit sample.
    /// </summary>
    public uint? Volume { get; set; }

    /// <summary>
    /// Name of the file of the custom sample.
    /// </summary>
    public string? FileName { get; set; }

    ///  <summary>
    /// Initializes a new instance of the <see cref="HitSample"/> class.
    /// </summary>
    /// <param name="normalSet"></param>
    /// <param name="additionSet"></param>
    /// <param name="index"></param>
    /// <param name="volume"></param>
    /// <param name="fileName"></param>
    public HitSample(SampleSet normalSet, SampleSet additionSet, string? fileName, uint? index = 0, uint? volume = 0)
    {
        NormalSet = normalSet;
        AdditionSet = additionSet;
        Index = index;
        Volume = volume;
        FileName = fileName;
    }

    /// <summary>
    /// Initialization of HitSample class
    /// </summary>
    public HitSample()
    {
        NormalSet = SampleSet.Default;
        AdditionSet = SampleSet.Default;
        Index = 0;
        Volume = 0;
        FileName = string.Empty;
    }

    /// <summary>
    /// Converts the hit sample from a string to a HitSample object.
    /// </summary>
    /// <param name="data"></param>
    /// <returns><see cref="HitSample"/></returns>
    public static HitSample Decode(string data)
    {
        try
        {
            var split = data.Split(':', 5);
            return new HitSample(
                normalSet: ParseSampleSet(split, 0),
                additionSet: ParseSampleSet(split, 1),
                index: ParseOptionalUInt(split, 2),
                volume: ParseOptionalUInt(split, 3),
                fileName: split.Length > 4 ? split[4] : null
            );
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to parse HitSample {ex}");
        }
    }

    private static SampleSet ParseSampleSet(string[] split, int index)
    {
        if (split.Length <= index || string.IsNullOrWhiteSpace(split[index]))
            return SampleSet.Default;

        if (!int.TryParse(split[index], NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
            return SampleSet.Default;

        if (value < 0)
            return SampleSet.Default;

        return Enum.IsDefined(typeof(SampleSet), (uint)value)
            ? (SampleSet)(uint)value
            : SampleSet.Default;
    }

    private static uint? ParseOptionalUInt(string[] split, int index)
    {
        if (split.Length <= index || string.IsNullOrWhiteSpace(split[index]))
            return null;

        if (!long.TryParse(split[index], NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
            return 0;

        if (value < 0 || value > uint.MaxValue)
            return 0;

        return (uint)value;
    }

    /// <summary>
    /// Encodes the hit sample into a string.
    /// </summary>
    /// <returns></returns>
    public string Encode()
    {
        StringBuilder builder = new();

        // Always output normalSet and additionSet.
        builder.Append($"{(uint)NormalSet}:{(uint)AdditionSet}");
    
        // Always output index: if not set, default to 0.
        builder.Append($":{(Index.HasValue ? Index.Value.ToString() : "0")}");
    
        // Always output volume followed by a trailing colon; default to 0 if not provided.
        builder.Append($":{(Volume.HasValue ? Volume.Value.ToString() : "0")}:");
    
        // Append file name if it exists.
        if (!string.IsNullOrEmpty(FileName))
        {
            builder.Append(FileName);
        }

        return builder.ToString();
    }
}
