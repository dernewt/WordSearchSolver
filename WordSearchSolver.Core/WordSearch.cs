namespace WordSearchSolver.Core;

public class WordSearch
{
    public required LetterGrid Letters { get; init; }
    public required List<string> WordBank { get; init; }
    public List<Segment> Solutions { get; init; } = [];
}