namespace homework1;

using System.Diagnostics;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

/// <summary>
/// Class that analyze average time and standart deviation of multiplying matrices.
/// </summary>
public static class MultiplyingPerfomanceAnalyzer
{
    private static readonly Stopwatch Stopwatch = new();

    /// <summary>
    /// Method for analyze average time and standart deviation of multiplying matrices.
    /// </summary>
    public static void Analyze()
    {
        var dimensions = new (int, int)[] { (100, 100), (200, 200), (500, 500), (1000, 1000) };
        var parallelResults = new List<(double, double)>();
        var commonResults = new List<(double, double)>();

        foreach (var elem in dimensions)
        {
            parallelResults.Add(AnalyzeMultiplying(elem, elem, true));
        }

        foreach (var elem in dimensions)
        {
            commonResults.Add(AnalyzeMultiplying(elem, elem, false));
        }

        WriteResultToPdf(dimensions, [.. parallelResults], [.. commonResults]);
    }

    private static (double, double) AnalyzeMultiplying((int, int) firstMatrixSize, (int, int) secondMatrixSize, bool isParallel)
    {
        Stopwatch.Reset();

        const int startsNumber = 10;
        const int decimalNumber = 3;

        var firstMatrix = Matrix.GenerateRandom(firstMatrixSize.Item1, firstMatrixSize.Item2);
        var secondMatrix = Matrix.GenerateRandom(secondMatrixSize.Item1, secondMatrixSize.Item2);

        var results = new double[startsNumber];

        for (var i = 0; i < startsNumber; ++i)
        {
            if (isParallel)
            {
                Stopwatch.Start();
                Matrix.ParallelMultiply(firstMatrix, secondMatrix);
                Stopwatch.Stop();
            }
            else
            {
                Stopwatch.Start();
                Matrix.Multiply(firstMatrix, secondMatrix);
                Stopwatch.Stop();
            }

            results[i] = Stopwatch.ElapsedMilliseconds;
            Stopwatch.Reset();
        }

        var average = Math.Round(results.Average(), decimalNumber);
        var confidenceInterval = Math.Round(FindStandartDeviation(results), decimalNumber) * 2;

        return (average, confidenceInterval);
    }

    private static void WriteResultToPdf((int, int)[] dimensions, (double, double)[] parallelResults, (double, double)[] commonResults, string filePath = "Result.pdf")
    {
        var document = new PdfDocument();
        document.Info.Title = "Matrix Performance Analysis Results";

        var page = document.AddPage();
        var gfx = XGraphics.FromPdfPage(page);
        var font = new XFont("Verdana", 10, XFontStyleEx.Regular);

        var yPosition = 50;
        var rowHeight = 20;
        var col1Width = 130;
        var col2Width = 100;
        var col3Width = 130;
        var col4Width = 150;

        DrawCell(gfx, font, "Size", 40, yPosition, col1Width, rowHeight);
        DrawCell(gfx, font, "Method", 40 + col1Width, yPosition, col2Width, rowHeight);
        DrawCell(gfx, font, "Average Time (ms)", 40 + col1Width + col2Width, yPosition, col3Width, rowHeight);
        DrawCell(gfx, font, "Standard Deviation (ms)", 40 + col1Width + col2Width + col3Width, yPosition, col4Width, rowHeight);

        yPosition += rowHeight;

        for (var i = 0; i < dimensions.Length; ++i)
        {
            DrawCell(gfx, font, $"{dimensions[i].Item1}X{dimensions[i].Item1}", 40, yPosition, col1Width, rowHeight);
            DrawCell(gfx, font, "Parallel", 40 + col1Width, yPosition, col2Width, rowHeight);
            DrawCell(gfx, font, $"{parallelResults[i].Item1}", 40 + col1Width + col2Width, yPosition, col3Width, rowHeight);
            DrawCell(gfx, font, $"{parallelResults[i].Item2}", 40 + col1Width + col2Width + col3Width, yPosition, col4Width, rowHeight);

            yPosition += rowHeight;

            DrawCell(gfx, font, $"{dimensions[i].Item1}X{dimensions[i].Item1}", 40, yPosition, col1Width, rowHeight);
            DrawCell(gfx, font, "Common", 40 + col1Width, yPosition, col2Width, rowHeight);
            DrawCell(gfx, font, $"{commonResults[i].Item1}", 40 + col1Width + col2Width, yPosition, col3Width, rowHeight);
            DrawCell(gfx, font, $"{commonResults[i].Item2}", 40 + col1Width + col2Width + col3Width, yPosition, col4Width, rowHeight);

            yPosition += rowHeight;
        }

        document.Save(filePath);
    }

    private static void DrawCell(XGraphics gfx, XFont font, string text, int x, int y, int width, int height)
    {
        gfx.DrawRectangle(XPens.Black, x, y, width, height);
        gfx.DrawString(text, font, XBrushes.Black, new XRect(x, y, width, height), XStringFormats.Center);
    }

    private static double FindStandartDeviation(double[] times)
    {
        var average = times.Average();

        var result = times.Sum(time => (average - time) * (average - time));

        return Math.Sqrt(result / (times.Length - 1)) * (1 / Math.Sqrt(times.Length));
    }
}