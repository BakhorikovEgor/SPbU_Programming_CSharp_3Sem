using System.Text;
using System.Text.RegularExpressions;

namespace MatrixMultiplication;

public partial class Matrix
{
    private static readonly Random Rand = new Random();
    public int[,] MatrixData { get; }
    public int RowsCount { get; }
    public int ColumnsCount { get; }

    public Matrix(int[,] data)
    {
        MatrixData = data;
        RowsCount = data.GetLength(0);
        ColumnsCount = data.GetLength(1);
    }


    public Matrix(string path)
    {
        using var stream = new StreamReader(path);

        var rows = new List<int[]>();
        var columnsCount = 0;

        while (stream.ReadLine() is { } line)
        {
            try
            {
                var row = MyRegex().Matches(line).Select(match => int.Parse(match.Value)).ToArray();
                if (columnsCount != 0 && row.Length != columnsCount)
                {
                    throw new ArgumentException("Given file does not contain matrix");
                }

                columnsCount = row.Length;
                rows.Add(row);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Given file does not contain matrix");
            }
        }

        var rowsCount = rows.Count;
        var matrixData = new int[rowsCount, columnsCount];
        for (var i = 0; i < rowsCount; ++i)
        {
            for (var j = 0; j < columnsCount; ++j)
            {
                matrixData[i, j] = rows[i][j];
            }
        }

        MatrixData = matrixData;
        RowsCount = rowsCount;
        ColumnsCount = columnsCount;
    }


    public static Matrix GenerateMatrix(int rows, int columns)
    {
        var newMatrixData = new int[rows, columns];
        for (var i = 0; i < rows; ++i)
        {
            for (var j = 0; j < columns; ++j)
            {
                newMatrixData[i, j] = Rand.Next(-100, 101);
            }
        }

        return new Matrix(newMatrixData);
    }


    public Matrix Multiply(Matrix secondMatrix)
    {
        if (!MultiplicationMatching(secondMatrix))
        {
            throw new ArgumentException("The dimensions of the matrices are not suitable for multiplying them");
        }

        var newMatrixData = new int[RowsCount, secondMatrix.ColumnsCount];
        for (var i = 0; i < RowsCount; ++i)
        {
            for (var j = 0; j < secondMatrix.ColumnsCount; ++j)
            {
                for (var k = 0; k < ColumnsCount; ++k)
                {
                    newMatrixData[i, j] += MatrixData[i, k] * secondMatrix.MatrixData[k, j];
                }
            }
        }

        return new Matrix(newMatrixData);
    }

    public Matrix ParallelMultiply(Matrix secondMatrix)
    {
        if (!MultiplicationMatching(secondMatrix))
        {
            throw new ArgumentException("The dimensions of the matrices are not suitable for multiplying them");
        }

        var newMatrixItemsCount = RowsCount * secondMatrix.ColumnsCount;
        var newMatrixData = new int[RowsCount, secondMatrix.ColumnsCount];

        var threadsCount = Environment.ProcessorCount;
        var threads = new Thread[threadsCount];

        var itemsForThread = newMatrixItemsCount / threadsCount;
        var remainder = newMatrixItemsCount % threadsCount;

        for (var threadNumber = 0; threadNumber < threadsCount; ++threadNumber)
        {
            var nestedThreadNumber = threadNumber;
            threads[threadNumber] = new Thread(() =>
            {
                var startedItemNumber = nestedThreadNumber * itemsForThread +
                    nestedThreadNumber < remainder
                        ? nestedThreadNumber
                        : remainder;
                var lastItemNumber = startedItemNumber + itemsForThread +
                    nestedThreadNumber < remainder
                        ? 1
                        : 0;

                for (var itemNumber = startedItemNumber; itemNumber < lastItemNumber; ++itemNumber)
                {
                    var row = itemNumber / secondMatrix.ColumnsCount;
                    var column = itemNumber % secondMatrix.ColumnsCount;

                    for (var i = 0; i < ColumnsCount; ++i)
                    {
                        newMatrixData[row, column] += MatrixData[row, i] * secondMatrix.MatrixData[i, column];
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

        return new Matrix(newMatrixData);
    }


    public void SaveToFile(string path)
    {
        using var writer = new StreamWriter(path);
        writer.Write(this);
    }


    public override string ToString()
    {
        var builder = new StringBuilder();
        for (var i = 0; i < RowsCount; ++i)
        {
            for (var j = 0; j < ColumnsCount; ++j)
            {
                builder.Append($" {MatrixData[i, j]}");
            }

            builder.Append("\n");
        }

        return builder.ToString();
    }


    private bool MultiplicationMatching(Matrix secondMatrix) => ColumnsCount == secondMatrix.RowsCount;

    
    [GeneratedRegex("-?\\d+")]
    private static partial Regex MyRegex();
}