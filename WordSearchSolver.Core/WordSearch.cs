using System.Diagnostics.CodeAnalysis;

namespace WordSearchSolver.Core;

public class WordSearch
{
    public required LetterGrid Letters { get; init; }
    public required string[] WordBank { get; init; }
    public List<Segment> Solutions { get; init; } = [];
}