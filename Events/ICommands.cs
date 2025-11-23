namespace BeatmapParser.Events;

/// <summary>
/// Represents an entity that contains a collection of commands.
/// </summary>
public interface IHasCommands
{
    /// <summary>
    /// Gets or sets the collection of commands associated with the object.
    /// </summary>
    public List<Commands.ICommand> Commands { get; set; }
}