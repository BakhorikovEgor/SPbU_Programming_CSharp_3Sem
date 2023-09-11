namespace MatrixMultiplication.Tests;

public class MatrixTests
{
    private const string SavedMatrixPath = "../../../TestData/SavedMatrix.txt";
    private static bool AreMatricesEqual(Matrix firstMatrix, Matrix secondMatrix)
    {
        if (firstMatrix.RowsCount != secondMatrix.RowsCount ||
            firstMatrix.ColumnsCount != secondMatrix.ColumnsCount)
        {
            return false;
        }

        for (var i = 0; i < firstMatrix.RowsCount; ++i)
        {
            for (var j = 0; j < firstMatrix.ColumnsCount; ++j)
            {
                if (firstMatrix.MatrixData[i, j] != secondMatrix.MatrixData[i, j])
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static IEnumerable<TestCaseData> SourceForReadingFromFileTest
    {
        get
        {
            yield return new TestCaseData("../../../TestData/SimpleMatrix.txt",
                new Matrix(new int[,]
                {
                    { 1, 2, 3 },
                    { -2, 3, 5 },
                    { 1, 8, 7 }
                }));
            yield return new TestCaseData("../../../TestData/VectorMatrix.txt",
                new Matrix(new int[,]
                {
                    { 1 },
                    { 2 },
                    { 3 }
                }));
            yield return new TestCaseData("../../../TestData/ManySpacesMatrix.txt",
                new Matrix(new int[,]
                {
                    { 1, 2, 4 },
                    { 5, 6, 7 },
                    { 9, 1, 8 }
                }));
            yield return new TestCaseData("../../../TestData/EmptyMatrix.txt",
                new Matrix(new int[,] { }));
        }
    }


    private static IEnumerable<TestCaseData> SourceForCorrectMultiplication
    {
        get
        {
            yield return new TestCaseData(
                new Matrix(new int[,]
                {
                    { 1, 1 },
                    { 2, 2 }
                }),
                new Matrix(new int[,]
                {
                    { 3, 3 },
                    { 4, 4 }
                }),
                new Matrix(new int[,]
                {
                    { 7, 7 },
                    { 14, 14 }
                }));

            yield return new TestCaseData(
                new Matrix(new int[,]
                {
                    { -10 },
                    { 10 }
                }),
                new Matrix(new int[,]
                {
                    { 1, 0 }
                }),
                new Matrix(new int[,]
                {
                    { -10, 0 },
                    { 10, 0 }
                }));
            yield return new TestCaseData(
                new Matrix(new int[,]
                {
                    { 9, -1 },
                    { 2, 2 },
                    { 4, -8 }
                }),
                new Matrix(new int[,]
                {
                    { 3, 3, 7 },
                    { -4, 7, 12 }
                }),
                new Matrix(new int[,]
                {
                    { 31, 20, 51 },
                    { -2, 20, 38 },
                    { 44, -44, -68 }
                }));
            yield return new TestCaseData(
                new Matrix(new int[,] { }),
                new Matrix(new int[,] { }),
                new Matrix(new int[,] { }));
        }
    }


    private static IEnumerable<TestCaseData> SourceForIncorrectMultiplication
    {
        get
        {
            yield return new TestCaseData(
                new Matrix(new int[,]
                {
                    { 1, 1 },
                    { 2, 2 }
                }),
                new Matrix(new int[,]
                {
                    { 3, 3 }
                }));

            yield return new TestCaseData(
                new Matrix(new int[,]
                {
                    { -10 },
                    { 10 }
                }),
                new Matrix(new int[,] { }));
        }
    }

    private static IEnumerable<TestCaseData> SourceForSavingMatrices
    {
        get
        {
            yield return new TestCaseData(
                new Matrix(new int[,]
                {
                    { 1, 2 },
                    { 3, 4 }
                }));

            yield return new TestCaseData(
                new Matrix(new int[,]
                {
                    { 1 },
                    { -1 }
                }));
            yield return new TestCaseData( new Matrix(new int[,] { }));
        }
    }
    
    [TestCaseSource(nameof(SourceForReadingFromFileTest))]
    public void ReadCorrectMatrixDataFromFile_ShouldCreateCorrectMatrix(string path, Matrix correctMatrix)
        => Assert.That(AreMatricesEqual(Matrix.ReadMatrixFromFile(path), correctMatrix), Is.True);


    [TestCase("../../../TestData/NotMatrix.txt")]
    [TestCase("../../../TestData/NotMatrix2.txt")]
    [TestCase("../../../TestData/WrongItemsMatrix.txt")]
    public void ReadIncorrectMatrixDataFromFile_ShouldThrowArgumentException(string path)
        => Assert.Throws<ArgumentException>(() => Matrix.ReadMatrixFromFile(path));


    [TestCase("../../../TestData/NotExistingFile.txt")]
    public void ReadMatrixDataFromNotExistingFile_ShouldThrowFileNotFoundException(string path)
        => Assert.Throws<FileNotFoundException>(() => Matrix.ReadMatrixFromFile(path));


    [TestCase(10, 10)]
    [TestCase(0, 0)]
    [TestCase(1000, 1000)]
    public void GenerateMatrixWithGivenSizes_ShouldCreateMatrixWithGivenSizes(int rows, int columns)
    {
        var matrix = Matrix.GenerateMatrix(rows, columns);
        Assert.Multiple(() =>
        {
            Assert.That(matrix.RowsCount, Is.EqualTo(rows));
            Assert.That(matrix.ColumnsCount, Is.EqualTo(columns));
        });
    }


    [TestCaseSource(nameof(SourceForCorrectMultiplication))]
    public void MultiplyCorrectMatrices_ShouldReturnCorrectResult(Matrix first, Matrix second, Matrix correctResult)
        => Assert.That(AreMatricesEqual(first.Multiply(second), correctResult), Is.True);


    [TestCaseSource(nameof(SourceForIncorrectMultiplication))]
    public void MultiplyIncorrectMatrices_ShouldThrowArgumentException(Matrix first, Matrix second)
        => Assert.Throws<ArgumentException>(() => first.Multiply(second));


    [TestCaseSource(nameof(SourceForCorrectMultiplication))]
    public void ParallelMultiplyCorrectMatrices_ShouldReturnCorrectResult(Matrix first, Matrix second, Matrix correctResult)
        => Assert.That(AreMatricesEqual(first.ParallelMultiply(second), correctResult), Is.True);
    
    
    [TestCaseSource(nameof(SourceForIncorrectMultiplication))]
    public void ParallelMultiplyIncorrectMatrices_ShouldThrowArgumentException(Matrix first, Matrix second)
        => Assert.Throws<ArgumentException>(() => first.ParallelMultiply(second));

    //it also checks .ToString
    [TestCaseSource(nameof(SourceForSavingMatrices))]
    public void SaveMatrixToFileAndReadFromFile_ResultMatrixShouldEqualToStarted(Matrix matrix)
    {
        matrix.SaveToFile(SavedMatrixPath);
        Assert.That(AreMatricesEqual(matrix, Matrix.ReadMatrixFromFile(SavedMatrixPath)), Is.True);
    }

}