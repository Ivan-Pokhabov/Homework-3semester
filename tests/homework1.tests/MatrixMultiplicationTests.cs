namespace homework.tests;

using homework1;

public class MatrixMultiplicationTests
{
    private static IEnumerable<TestCaseData> Files
        =>
    [
        new("../../../TestFiles/Empty.txt"),
        new("../../../TestFiles/WithEmptyLine.txt"),
        new("../../../TestFiles/NotRectangle.txt"),
    ];

    private static IEnumerable<TestCaseData> CorrectMatrices
        =>
    [
        new TestCaseData(
            new Matrix(new int[,]
            {
                {5, -2, 0},
                {11, 3, 3},
                {0, 56, 2}
            }),
            new Matrix(new int[,]
            {
                {4, 9, 2},
                {1, 1, 1},
                {-5, -2, -1}
            }),
            new Matrix(new int[,]
            {
                {18, 43, 8},
                {32, 96, 22},
                {46, 52, 54}
            })
        ),
        new TestCaseData(
            new Matrix(new int[,]
            {
                {5, -2, 0, 2},
                {11, 3, 3, 10},
                {0, 56, 2, -7}
            }),
            new Matrix(new int[,]
            {
                {4, 9},
                {1, 1},
                {-5, -2},
                {2, 2}
            }),
            new Matrix(new int[,]
            {
                {22, 47},
                {52, 116},
                {32, 38}
            })
        ),
    ];

    private static IEnumerable<TestCaseData> IncorrectMatrices
        =>
    [
        new TestCaseData(
            new Matrix(new int[,]
            {
                {5, -2, 0, 2},
                {11, 3, 3, 10},
                {0, 56, 2, -7}
            }),
            new Matrix(new int[,]
            {
                {4, 9, 2},
                {1, 1, 1},
                {-5, -2, -1}
            })
        ),
    ];

    [TestCaseSource(nameof(Files))]
    public void InitializeWith_IncorrectFile_ArgumentExceptionReturned(string filePath)
        => Assert.Throws<InvalidDataException>(() => new Matrix(filePath));

    [TestCaseSource(nameof(CorrectMatrices))]
    public void Multiply_CorrectMatrices_CorrectResultMatrix(Matrix firstMatrix, Matrix secondMatrix, Matrix expected)
        => Assert.That(Matrix.Multiply(firstMatrix, secondMatrix).IsEqual(expected));

    [TestCaseSource(nameof(CorrectMatrices))]
    public void MultiplyParallel_CorrectMatrices_CorrectResultMatrix(Matrix firstMatrix, Matrix secondMatrix, Matrix expected)
        => Assert.That(Matrix.ParallelMultiply(firstMatrix, secondMatrix).IsEqual(expected));

    [TestCaseSource(nameof(IncorrectMatrices))]
    public void Multiply_IncorrectMatrices_ArgumentExceptionReturned(Matrix firstMatrix, Matrix secondMatrix)
        => Assert.Throws<ArgumentException>(() => Matrix.Multiply(firstMatrix, secondMatrix));

    [TestCaseSource(nameof(IncorrectMatrices))]
    public void MultiplyParallel_IncorrectMatrices_ArgumentExceptionReturned(Matrix firstMatrix, Matrix secondMatrix)
        => Assert.Throws<ArgumentException>(() => Matrix.ParallelMultiply(firstMatrix, secondMatrix));
}