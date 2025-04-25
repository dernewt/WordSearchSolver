using CommunityToolkit.HighPerformance;
using System.Text;

namespace WordSearchSolver.Core;
public record Segment(int StartX, int StartY, int EndX, int EndY)
{
    public Segment(Point start, Point end)
        : this(start.X, start.Y, end.X, end.Y) { }
    public static Segment Reverse(Segment segment)
    {
        return new Segment(segment.EndX, segment.EndY, segment.StartX, segment.StartY);
    }
}
public record Point(int X, int Y);
public class LetterGrid
{
    protected static readonly char[] LineBreaks = ['\n', '\r'];
    protected char[,] Grid;

    public LetterGrid(string letters)
        : this(letters.Split(
            LineBreaks,
            StringSplitOptions.RemoveEmptyEntries
            & StringSplitOptions.TrimEntries))
    { }

    public LetterGrid(string[] letters)
    {
        var cleanLetters = letters.Select(row => row.Replace(" ", ""));
        var columns = cleanLetters.FirstOrDefault()?.Length ?? 0;
        if (columns == 0)
            throw new ArgumentException("Grid must not be empty");
        if (letters.Any(row => row.Length != columns))
            throw new ArgumentException("Grid must be not be jagged");

        Grid = new char[letters.Length, columns];
        var gridView = new Span2D<char>(Grid);
        for (int i = 0; i < letters.Length; i++)
        {
            var row = letters[i];
            row.AsSpan().CopyTo(gridView.GetRowSpan(i));
        }
    }

    public IEnumerable<Point> AllPoints()
    {
        for (int x = 0; x < Grid.GetLength(0); x++)
        {
            for (int y = 0; y < Grid.GetLength(1); y++)
            {
                yield return new Point(x, y);
            }
        }
    }

    public string StringAt(Segment line)
    {
        var word = new StringBuilder();

        var xStep = Math.Sign(line.EndX - line.StartX);
        var yStep = Math.Sign(line.EndY - line.StartY);

        var x = line.StartX switch
        {
            < 0 => 0,
            var xx when xx > Grid.GetLength(0) => Grid.GetLength(0) - 1,
            _ => line.StartX
        };
        var y = line.StartY switch
        {
            < 0 => 0,
            var yy when yy > Grid.GetLength(1) => Grid.GetLength(1) - 1,
            _ => line.StartY
        };

        var xCap = line.EndX switch
        {
            < 0 => 0,
            var xx when xx > Grid.GetLength(0) => Grid.GetLength(0) - 1,
            _ => line.EndX
        };
        var yCap = line.EndY switch
        {
            < 0 => 0,
            var yy when yy > Grid.GetLength(1) => Grid.GetLength(1) - 1,
            _ => line.EndY
        };

        word.Append(Grid[x, y]);

        while (x != xCap || y != yCap)
        {
            if (x != xCap)
                x += xStep;
            if (y != yCap)
                y += yStep;

            word.Append(Grid[x, y]);
        }

        return word.ToString();
    }

    public Segment? FindFirst(string word)
    {
        if(word == string.Empty)
            return null;

        foreach (var point in AllPoints())
        {
            var indexOffset = word.Length - 1;

            if (Grid[point.X, point.Y] == word[0])
            {
                if (word.Length == 1)
                    return new Segment(point, point);

                var across = new Segment(point, point with { Y = point.Y + indexOffset });
                if (StringAt(across) == word)
                    return across;

                var down = new Segment(point, point with { X = point.X + indexOffset });
                if (StringAt(down) == word)
                    return down;

                var diagonal = new Segment(point, point with { X = point.X + indexOffset, Y = point.Y + indexOffset });
                if (StringAt(diagonal) == word)
                    return diagonal;

                var diagonal2 = new Segment(point, point with { X = point.X - indexOffset, Y = point.Y + indexOffset });
                if (StringAt(diagonal2) == word)
                    return diagonal2;

            } else if(Grid[point.X, point.Y] == word[^1])
            {
                var acrossR = new Segment(point with { Y = point.Y + indexOffset }, point);
                if (StringAt(acrossR) == word)
                    return acrossR;

                var up = new Segment(point with { X = point.X + indexOffset }, point);
                if (StringAt(up) == word)
                    return up;

                var diagonalR = new Segment(point with { X = point.X + indexOffset, Y = point.Y + indexOffset }, point);
                if (StringAt(diagonalR) == word)
                    return diagonalR;

                var diagonal2R = new Segment(point with { X = point.X - indexOffset, Y = point.Y + indexOffset }, point);
                if (StringAt(diagonal2R) == word)
                    return diagonal2R;
            }
        }
        return null;
    }

}

