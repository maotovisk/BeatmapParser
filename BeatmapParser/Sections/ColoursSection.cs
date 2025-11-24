using System.Drawing;
using System.Text;
using BeatmapParser.Colours;

namespace BeatmapParser.Sections;

/// <summary>
/// Represents the colours section of a <see cref="Beatmap"/>.
/// </summary>
public class ColoursSection
{
    /// <summary>
    /// The slider border colour of the beatmap.
    /// </summary>
    public Color? SliderBorder { get; set; }

    /// <summary>
    /// The additive slider track colour of the beatmap.
    /// </summary>
    public Color? SliderTrackOverride { get; set; }

    /// <summary>
    /// The list of combo colours of the beatmap.
    /// </summary>
    public List<ComboColour> Combos { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColoursSection"/> class with the specified parameters.
    /// </summary>
    public ColoursSection(Color sliderBorder, Color sliderTrackOverride, List<ComboColour> combos)
    {
        SliderBorder = sliderBorder;
        SliderTrackOverride = sliderTrackOverride;
        Combos = combos;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColoursSection"/> class.
    /// </summary>
    private ColoursSection()
    {
        Combos = [];
    }

    /// <summary>
    /// Parses a list of Colours lines into a new <see cref="ColoursSection"/> class.
    /// </summary>
    /// <param name="section"></param>
    /// <returns>A populated <see cref="ColoursSection"/> instance parsed from the provided section lines.</returns>
    public static ColoursSection Decode(List<string> section)
    {
        ColoursSection result = new();
        try
        {
            section.ForEach(sectionLine =>
            {
                if (sectionLine.StartsWith("SliderBorder"))
                {
                    var split = sectionLine.Split(':', 2);
                    result.SliderBorder = Helpers.Helper.ParseColor(split[1].Trim());
                }
                else if (sectionLine.StartsWith("SliderTrackOverride"))
                {
                    var split = sectionLine.Split(':', 2);
                    result.SliderTrackOverride = Helpers.Helper.ParseColor(split[1].Trim());
                }
                else if (sectionLine.StartsWith("Combo"))
                {
                    result.Combos.Add(ComboColour.Decode(sectionLine));
                }
            });

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to parse Colours section.", ex);
        }
    }

    /// <summary>
    /// Encodes the <see cref="ColoursSection"/> class into a string.
    /// </summary>
    /// <returns>A string containing the encoded Colours section lines, each terminated with a newline.</returns>
    public string Encode()
    {
        StringBuilder builder = new();
        
        var separator = Helpers.Helper.FormatVersion == 128 ? ": " : " : ";
        
        var alpha = Helpers.Helper.FormatVersion != 128 ? "" : ",255";

        Combos.ForEach(combo =>
        {
            builder.AppendLine($"Combo{combo.Number}{separator}{combo.Colour.R},{combo.Colour.G},{combo.Colour.B}{alpha}");
        });
        
        if (SliderBorder.HasValue)
        {
            builder.AppendLine($"SliderBorder{separator}{SliderBorder.Value.R},{SliderBorder.Value.G},{SliderBorder.Value.B}{alpha}");
        }
        
        if (SliderTrackOverride.HasValue)
        {
            builder.AppendLine($"SliderTrackOverride{separator}{SliderTrackOverride.Value.R},{SliderTrackOverride.Value.G},{SliderTrackOverride.Value.B}{alpha}");
            
        }
        
        return builder.ToString();
    }

}