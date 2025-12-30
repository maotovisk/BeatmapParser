using System.Globalization;
using System.Numerics;
using System.Text;
using BeatmapParser.Enums;
using BeatmapParser.HitObjects.HitSounds;

namespace BeatmapParser.HitObjects;

/// <summary>
/// Represents a hold object in a mania beatmap.
/// </summary>
public class ManiaHold : HitObject
{
    private double _endMilliseconds;

    /// <summary>
    /// Gets or sets the end time of the hold object.
    /// </summary>
    public TimeSpan End
    {
        get => TimeSpan.FromMilliseconds(_endMilliseconds);
        set => _endMilliseconds = value.TotalMilliseconds;
    }

    internal double EndMilliseconds
    {
        get => _endMilliseconds;
        set => _endMilliseconds = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ManiaHold"/> class.
    /// </summary>
    /// <param name="coordinates">The coordinates of the hold object.</param>
    /// <param name="timeMilliseconds">The time of the hold object.</param>
    /// <param name="type">The type of the hold object.</param>
    /// <param name="hitSounds">The list of hit sounds for the hold object.</param>
    /// <param name="newCombo">A value indicating whether the hold object starts a new combo.</param>
    /// <param name="comboOffset">The color of the combo for the hold object.</param>
    /// <param name="endMilliseconds">The end time of the hold object in milliseconds.</param>
    public ManiaHold(Vector2 coordinates, double timeMilliseconds, HitObjectType type, (HitSample, List<HitSound>) hitSounds, bool newCombo, uint comboOffset, double endMilliseconds)
        : base(coordinates, timeMilliseconds, type, hitSounds, newCombo, comboOffset)
    {
        _endMilliseconds = endMilliseconds;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ManiaHold"/> class.
    /// </summary>
    public ManiaHold()
    {
        Coordinates = new Vector2();
        Time = new TimeSpan();
        HitSounds = (new HitSample(), new List<HitSound>());
        NewCombo = false;
        ComboOffset = 0;
        _endMilliseconds = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ManiaHold"/> class.
    /// </summary>
    /// <param name="baseObject"></param>
    public ManiaHold(HitObject baseObject) : base(baseObject)
    {
        _endMilliseconds = 0;
    }

    /// <summary>
    /// Converts a list of strings into a <see cref="ManiaHold"/> object.
    /// </summary>
    /// <param name="splitData">The list of strings containing the hold object data, split by commas.</param>
    /// <returns>A <see cref="ManiaHold"/> instance representing the parsed hold object.</returns>
    public new static ManiaHold Decode(List<string> splitData)
    {
        try
        {
            var hasHitSample = splitData.Last().Contains(":");
            var timeMilliseconds = double.Parse(splitData[2], CultureInfo.InvariantCulture);
            var endMilliseconds = double.Parse(splitData[5], CultureInfo.InvariantCulture);

            return new ManiaHold(
                coordinates: new Vector2(float.Parse(splitData[0], CultureInfo.InvariantCulture), float.Parse(splitData[1], CultureInfo.InvariantCulture)),
                timeMilliseconds: timeMilliseconds,
                type: Helpers.Helper.ParseHitObjectType(int.Parse(splitData[3])),
                hitSounds: !hasHitSample ? (new HitSample(), Helpers.Helper.ParseHitSounds(int.Parse(splitData[4]))) : (HitSample.Decode(splitData.Last()), Helpers.Helper.ParseHitSounds(int.Parse(splitData[4]))),
                newCombo: (int.Parse(splitData[3]) & (1 << 2)) != 0,
                comboOffset: (uint)((int.Parse(splitData[3]) & (1 << 4 | 1 << 5 | 1 << 6)) >> 4),
                endMilliseconds: endMilliseconds
            );
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to parse ManiaHold {ex}");
        }
    }

    /// <summary>
    /// Encodes the mania hold hit object into a string.
    /// </summary>
    /// <returns>A string representation of the mania hold hit object.</returns>
    public new string Encode()
    {
        StringBuilder builder = new();

        builder.Append($"{Helpers.Helper.FormatCoord(Coordinates.X)},{Helpers.Helper.FormatCoord(Coordinates.Y)},");
        builder.Append($"{Helpers.Helper.FormatTime(TimeMilliseconds)},");

        var type = (int)Type;

        if (NewCombo)
        {
            type |= 1 << 2;
        }

        type |= (int)ComboOffset << 4;

        builder.Append($"{type},");
        builder.Append($"{Helpers.Helper.EncodeHitSounds(HitSounds.Sounds)},");

        builder.Append($"{Helpers.Helper.FormatTime(_endMilliseconds)},");

        builder.Append($"{HitSounds.SampleData.Encode()}");

        return builder.ToString();
    }
}
