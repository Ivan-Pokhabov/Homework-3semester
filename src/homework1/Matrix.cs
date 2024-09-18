namespace homework1;

/// <summary>
/// Class implementation of math matrix.
/// </summary>
public class Matrix
{
    private readonly int[,] matrixElements;

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix"/> class.
    /// </summary>
    /// <param name="elements">Elements of matrix.</param>
    public Matrix(int[,] elements)
    {
        ArgumentNullException.ThrowIfNull(elements);
        matrixElements = (int[,])elements.Clone();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix"/> class.
    /// </summary>
    /// <param name="filePath">Path to the file that contains matrix.</param>
    public Matrix(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(filePath);
        }

        var lines = File.ReadAllLines(filePath);
        if (lines.Length == 0)
        {
            throw new InvalidDataException("Empty matrix");
        }

        matrixElements = new int[lines.Length, lines[0].Split(' ').Length];

        for (var i = 0; i < lines.Length; ++i)
        {
            if (lines[i] == string.Empty)
            {
                throw new InvalidDataException("Empty line");
            }

            var line = lines[i].Split(' ').Select(int.Parse).ToArray();

            if (line.Length != matrixElements.GetLength(1))
            {
                throw new InvalidDataException("The matrix is incorrect");
            }

            for (var j = 0; j < line.Length; ++j)
            {
                matrixElements[i, j] = line[j];
            }
        }
    }

    /// <summary>
    /// Gets number of lines in matrix.
    /// </summary>
    public int Lines => matrixElements.GetLength(0);

    /// <summary>
    /// Gets number of coloumns in matrix.
    /// </summary>
    public int Columns => matrixElements.GetLength(1);

    /// <summary>
    /// Gets or sets elements in matrix.
    /// </summary>
    /// <param name="lineIndex">Index of line in matrix.</param>
    /// <param name="columnIndex">Index of coloumn in matrix.</param>
    /// <returns>Element of matrix by position.</returns>
    public int this[int lineIndex, int columnIndex]
    {
        get => matrixElements[lineIndex, columnIndex];
        set => matrixElements[lineIndex, columnIndex] = value;
    }

    /// <summary>
    /// Generate matrix with random elements by size.
    /// </summary>
    /// <param name="lines">Number of lines.</param>
    /// <param name="columns">Number of columns.</param>
    /// <returns>Matrix with random numbers.</returns>
    /// <exception cref="ArgumentException">Size should be correct.</exception>
    public static Matrix GenerateRandom(int lines, int columns)
    {
        if (lines <= 0 || columns <= 0)
        {
            throw new ArgumentException("Number of lines and columns should be >0");
        }

        var matrix = new int[lines, columns];

        var rand = new Random();

        for (var i = 0; i < lines; ++i)
        {
            for (var j = 0; j < columns; ++j)
            {
                matrix[i, j] = rand.Next();
            }
        }

        return new Matrix(matrix);
    }

    /// <summary>
    /// Method that multiply matrix using multithreading.
    /// </summary>
    /// <param name="matrix1">Matrix that comes first in product.</param>
    /// <param name="matrix2">Matrix that comes second in product.</param>
    /// <returns>Matrix that is a result of production.</returns>
    /// <exception cref="ArgumentException">Threads number should be between 2 and 19 and multiplying should be correct by math rules.</exception>
    public static Matrix ParallelMultiply(Matrix matrix1, Matrix matrix2)
    {
        ArgumentNullException.ThrowIfNull(matrix1);
        ArgumentNullException.ThrowIfNull(matrix2);

        var threadsNumber = Environment.ProcessorCount;

        if (matrix1.Columns != matrix2.Lines)
        {
            throw new ArgumentException("Invalid matrices' dimensions");
        }

        var resultMatrix = new int[matrix1.Lines, matrix2.Columns];

        var threads = new Thread[threadsNumber];
        var chunkSize = (resultMatrix.Length / threadsNumber) + 1;

        for (var i = 0; i < threadsNumber; ++i)
        {
            var localI = i;
            threads[i] = new Thread(() =>
            {
                for (var j = localI * chunkSize; j < (localI + 1) * chunkSize && j < resultMatrix.Length; ++j)
                {
                    var lineIndex = j / matrix2.Columns;
                    var columnIndex = j % matrix2.Columns;
                    for (var k = 0; k < matrix1.Columns; ++k)
                    {
                        resultMatrix[lineIndex, columnIndex] += matrix1[lineIndex, k] * matrix2[k, columnIndex];
                    }
                }
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return new Matrix(resultMatrix);
    }

    /// <summary>
    /// Method that multiply matrix using multithreading.
    /// </summary>
    /// <param name="matrix1">Matrix that comes first in product.</param>
    /// <param name="matrix2">Matrix that comes second in product.</param>
    /// <returns>Matrix that is a result of production.</returns>
    /// <exception cref="ArgumentException">Multiplying should be correct by math rules.</exception>
    public static Matrix Multiply(Matrix matrix1, Matrix matrix2)
    {
        ArgumentNullException.ThrowIfNull(matrix1);
        ArgumentNullException.ThrowIfNull(matrix2);

        if (matrix1.Columns != matrix2.Lines)
        {
            throw new ArgumentException("Invalid matrices' dimensions");
        }

        var resultMatrix = new int[matrix1.Lines, matrix2.Columns];

        for (var i = 0; i < matrix1.Lines; ++i)
        {
            for (var j = 0; j < matrix2.Columns; ++j)
            {
                for (var k = 0; k < matrix1.Columns; ++k)
                {
                    resultMatrix[i, j] += matrix1[i, k] * matrix2[k, j];
                }
            }
        }

        return new Matrix(resultMatrix);
    }

    /// <summary>
    /// Method that write matrix in file.
    /// </summary>
    /// <param name="filePath">Path to file.</param>
    public void WriteInFile(string filePath)
    {
        using var writer = new StreamWriter(filePath);
        for (var i = 0; i < Lines; i++)
        {
            for (var j = 0; j < Columns; j++)
            {
                writer.Write($"{matrixElements[i, j]} ");
            }

            writer.Write('\n');
        }
    }

    /// <summary>
    /// Method to check is two matrices equal.
    /// </summary>
    /// <param name="matrix">Matrix that need to be checked for equality with current one.</param>
    /// <exception cref="ArgumentNullException">Throws if <see cref="matrix"/> is null.</exception>
    /// <returns>True if matrices are equal else false.</returns>
    public bool IsEqual(Matrix matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);

        if (Lines != matrix.Lines || Columns != matrix.Columns)
        {
            return false;
        }

        for (var i = 0; i < Lines; ++i)
        {
            for (var j = 0; j < Columns; ++j)
            {
                if (matrixElements[i, j] != matrix[i, j])
                {
                    return false;
                }
            }
        }

        return true;
    }
}
