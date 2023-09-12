using System.Text;
using System.Text.RegularExpressions;

namespace MatrixMultiplication;

/// <summary>
/// A class representing a matrix of rows and columns.
/// </summary>
public class Matrix
{
    private static readonly Random Rand = new Random();

    private readonly int[,] _matrixData;

    /// <summary>
    /// Number of rows.
    /// </summary>
    public int RowsCount { get; }

    /// <summary>
    /// Number of columns.
    /// </summary>
    public int ColumnsCount { get; }

    /// <summary>
    /// Standard constructor of Matrix.
    /// </summary>
    /// <param name="data"> Matrix elements. </param>
    public Matrix(int[,] data)
    {
        _matrixData = (int[,])data.Clone();
        RowsCount = data.GetLength(0);
        ColumnsCount = data.GetLength(1);
    }


    /// <summary>
    /// Get element of matrix by indexes.
    /// </summary>
    public int this[int x, int y] => _matrixData[x, y];


    /// <summary>
    /// Checks if matrices are equal by elements
    /// </summary>
    public static bool operator ==(Matrix firstMatrix, Matrix secondMatrix)
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
                if (firstMatrix._matrixData[i, j] != secondMatrix._matrixData[i, j])
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if matrices are not equal
    /// </summary>
    public static bool operator !=(Matrix firstMatrix, Matrix secondMatrix)
    {
        return !(firstMatrix == secondMatrix);
    }


    /// <summary>
    /// Creates Matrix object using file.
    /// </summary>
    /// <param name="path"> Path to file with matrix. </param>
    /// <exception cref="ArgumentException"> Given file does not contain matrix. </exception>
    /// <exception cref="FileNotFoundException"> There is no file at the specified path. </exception>
    public static Matrix ReadMatrixFromFile(string path)
    {
        using var stream = new StreamReader(path);

        var rows = new List<int[]>();
        var columnsCount = 0;

        while (stream.ReadLine() is { } line)
        {
            try
            {
                var row = new Regex("-?\\d+", RegexOptions.Compiled).Matches(line)
                    .Select(match => int.Parse(match.Value)).ToArray();

                if (row.Length == 0) continue;

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

        return new Matrix(matrixData);
    }


    /// <summary>
    /// Generate new matrix with random elements.
    /// </summary>
    /// <param name="rows"> Number of rows in generated matrix. </param>
    /// <param name="columns"> Number of columns in generated matrix. </param>
    /// <returns> New Matrix. </returns>
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


    /// <summary>
    /// Standard sequential matrix multiplication algorithm.
    ///
    /// To multiply matrices, the number of columns in the first matrix must equal the number of rows in the second.
    /// </summary>
    /// <param name="secondMatrix">The matrix by which the current matrix is multiplied. </param>
    /// <returns> New matrix - result of multiplication. </returns>
    /// <exception cref="ArgumentException"> The matrix sizes are not correlated for multiplication. </exception>
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
                    newMatrixData[i, j] += _matrixData[i, k] * secondMatrix._matrixData[k, j];
                }
            }
        }

        return new Matrix(newMatrixData);
    }

    /// <summary>
    /// Matrix multiplication algorithm using parallel computing.
    /// To multiply matrices, the number of columns in the first matrix must equal the number of rows in the second.
    /// </summary>
    /// <param name="secondMatrix"></param>
    /// <returns> New matrix - result of multiplication. </returns>
    /// <exception cref="ArgumentException"> The matrix sizes are not correlated for multiplication. </exception>
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
                                        (nestedThreadNumber < remainder
                                            ? nestedThreadNumber
                                            : remainder);
                var lastItemNumber = startedItemNumber + itemsForThread +
                                     (nestedThreadNumber < remainder
                                         ? 1
                                         : 0);

                for (var itemNumber = startedItemNumber; itemNumber < lastItemNumber; ++itemNumber)
                {
                    var row = itemNumber / secondMatrix.ColumnsCount;
                    var column = itemNumber % secondMatrix.ColumnsCount;

                    for (var i = 0; i < ColumnsCount; ++i)
                    {
                        newMatrixData[row, column] += _matrixData[row, i] * secondMatrix._matrixData[i, column];
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


    /// <summary>
    /// Save current matrix in file using standard representation. 
    /// </summary>
    /// <param name="path"> The path to save the matrix. </param>
    public void SaveToFile(string path)
    {
        using var writer = new StreamWriter(path);
        writer.Write(this);
    }


    /// <inheritdoc/>
    public override string ToString()
    {
        var builder = new StringBuilder();
        for (var i = 0; i < RowsCount; ++i)
        {
            for (var j = 0; j < ColumnsCount; ++j)
            {
                builder.Append($" {_matrixData[i, j]}");
            }

            builder.Append("\n");
        }

        return builder.ToString();
    }

    private bool MultiplicationMatching(Matrix secondMatrix) => ColumnsCount == secondMatrix.RowsCount;
}