using WordSearchSolver.Core;

namespace WordSearchSolver.Core.Test;

public class WordGridTest
{
    [Test]
    public void EmptyWordGridThrowsError()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var grid = new LetterGrid(string.Empty);
        });
    }

    [Test]
    public void JaggedWordGridThrowsError()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var grid = new LetterGrid(["loooooooongRow", "shortRow"]);
        });
    }

    public record GridLineWord(LetterGrid grid, Segment line, string word);
    public static IEnumerable<Func<GridLineWord>> AllDirections()
    {
        LetterGrid grid = new(
            [
            "123",
            "456",
            "789"
            ]);

        yield return () => new(grid, new(0, 0, 0, 0), "1"); //point
        yield return () => new(grid, new(0, 2, 0, 2), "3"); //point
        yield return () => new(grid, new(0, 0, 0, 2), "123"); //top row 
        yield return () => new(grid, new(0, 2, 0, 0), "321"); //top row reverse
        yield return () => new(grid, new(0, 2, 2, 2), "369"); //right column
        yield return () => new(grid, new(2, 2, 0, 2), "963"); //right column reverse
        yield return () => new(grid, new(0, 0, 2, 2), "159"); //diagonal (top left to bottom right)
        yield return () => new(grid, new(2, 2, 0, 0), "951"); //diagonal reverse
        yield return () => new(grid, new(2, 0, 0, 2), "753"); //diagonal2 (bottom left to top right)
        yield return () => new(grid, new(0, 2, 2, 0), "357"); //diagonal2 reverse
    }

    [Test]
    [MethodDataSource(nameof(AllDirections))]
    public async Task StringAtAndFindTest(GridLineWord test)
    {
        var wordResult = test.grid.StringAt(test.line);
        await Assert.That(wordResult).IsEqualTo(test.word);
        var lineResult = test.grid.FindFirst(test.word);
        await Assert.That(lineResult).IsEqualTo(test.line);
    }
}