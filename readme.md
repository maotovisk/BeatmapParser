# BeatmapParser

A lightweight C# library for decoding, editing, and re-encoding osu! `.osu` beatmap files. It models every major section of the file format so you can inspect or programmatically modify metadata, timing information, events, colours, and hit objects before writing the map back to disk.

## Features
- Parses osu! file format versions 14 and 128 into strongly typed section objects (`General`, `Metadata`, `Difficulty`, `TimingPoints`, `HitObjects`, etc.).
- Encodes a `Beatmap` back to a valid `.osu` document via `Beatmap.Encode()`.
- Section helpers to query timing points, BPM, volume, backgrounds, and specific hit objects.
- Nullability-aware API so optional sections (Editor, Colours, TimingPoints) are easy to detect and guard against.
- Pure library design with no runtime dependencies outside the .NET SDK.

## Requirements
- .NET SDK 10.0 (preview) or newer. The project’s `TargetFramework` is `net10.0`; earlier SDKs will fail to restore/build.

## Getting Started
Get it from NuGet:
```sh
dotnet add package MapWizard.BeatmapParser --version 1.0.0
```
## Usage
### Decode an existing beatmap
```csharp
using System.IO;
using BeatmapParser;

var beatmap = Beatmap.Decode(new FileInfo("/maps/example.osu"));
Console.WriteLine($"Title: {beatmap.MetadataSection.Title}");
Console.WriteLine($"HP: {beatmap.DifficultySection.HPDrainRate}");
```

### Inspect timing and events
```csharp
var bpm = beatmap.GetBpmAt(45_000);
var volume = beatmap.GetVolumeAt(60_000);
var bg = beatmap.GetBackgroundFilename();
Console.WriteLine($"BPM @45s: {bpm}, Volume @60s: {volume}%, Background: {bg}");
```

### Modify and re-encode
```csharp
beatmap.GeneralSection.AudioFilename = "remix.mp3";
beatmap.MetadataSection.Version = "Hard";
beatmap.SetBackgroundFileName("cool-bg.jpg");

var newContents = beatmap.Encode();
File.WriteAllText("/maps/example-remix.osu", newContents);
```

### Decoding from raw text
If you already have the `.osu` contents loaded, call `Beatmap.Decode(string beatmapString)` instead of the `FileInfo` overload.

## Project Structure
- `Beatmap.cs` – entry point with `Beatmap.Decode`/`Beatmap.Encode` plus convenience helpers.
- `Sections/` – strongly typed representations for each numbered section in an osu! file.
- `HitObjects/` – models for circles, sliders, spinners, mania holds, and helper classes to work with curve points and samples.
- `TimingPoints/` – handling for inherited/uninherited timing points and BPM/volume calculations.
- `Events/` & `Colours/` – storyboard primitives and combo colour definitions.

## Development Workflow
- `dotnet build` compiles the library.
- `dotnet test` runs the unit tests in `BeatmapParser.Tests`.
- Keep changes targeting `.NET 10` unless the `BeatmapParser.csproj` is updated to multi-target.

## Contributing
1. Fork and branch from `main`.
2. Keep PRs focused; add/adjust section decoders with accompanying unit tests where possible.
3. Run `dotnet build` and `dotnet test` before submitting.
4. Document new public APIs and update this README with additional usage notes if appropriate.

## Roadmap & Known Limitations
- Encoding is only guaranteed for format versions 14 and 128.

## License
A license file is not currently included. Please contact the repository maintainer before using the code in commercial projects.

