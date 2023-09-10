namespace MatrixMultiplication;

public class Matrix
{
    private static readonly Random rand = new Random();
    public int[,] MatrixData { get; }
    public int Rows { get; }
    public int Columns { get; }
    
    public Matrix(int[,] data) 
    {
        MatrixData = data;
        Rows = data.GetLength(0);
        Columns = data.GetLength(1);
    }

    
    public static Matrix GenerateMatrix(int rows, int columns)
    {
        var newMatrixData = new int[rows, columns];
        for (var i = 0; i < rows; ++i)
        {
            for (var j = 0; j < columns; ++j)
            {
                newMatrixData[i, j] = rand.Next(-100, 101);
            }
        }

        return new Matrix(newMatrixData);
    }

    
    public Matrix Multiply(Matrix secondMatrix)
    {
        if (!MultiplicationMatching(secondMatrix))
        {
            throw new ArgumentException();
        }
        
        var newMatrixData = new int[Rows, secondMatrix.Columns];
        for (var i = 0; i < Rows; ++i)
        {
            for (var j = 0; j < secondMatrix.Columns; ++j)
            {
                for (var k = 0; k < Columns; ++k)
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
            throw new ArgumentException();
        }

        var newMatrixItemsCount = Rows * secondMatrix.Columns;
        var newMatrixData = new int[Rows, secondMatrix.Columns];

        var threadsCount = Environment.ProcessorCount;
        var threads = new Thread[threadsCount];

        var itemsForThread = (newMatrixItemsCount) / threadsCount;
        var remainder = (newMatrixItemsCount) % threadsCount;

        for (var threadNumber = 0; threadNumber < threadsCount; ++threadNumber)
        {
            var nestedThreadNumber = threadNumber;
            threads[threadNumber] = new Thread(() =>
            {
                var startedItemNumber = nestedThreadNumber * itemsForThread + Math.Min(nestedThreadNumber, remainder);
                var lastItemNumber = startedItemNumber + itemsForThread;

                if (nestedThreadNumber < remainder)
                {
                    lastItemNumber++;
                }

                for (var itemNumber = startedItemNumber; itemNumber < lastItemNumber; ++itemNumber)
                {
                    var row = itemNumber / Rows;
                    var column = itemNumber % secondMatrix.Columns;

                    for (var i = 0; i < Columns; ++i)
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

    
    private bool MultiplicationMatching(Matrix secondMatrix) => Columns == secondMatrix.Rows;
}