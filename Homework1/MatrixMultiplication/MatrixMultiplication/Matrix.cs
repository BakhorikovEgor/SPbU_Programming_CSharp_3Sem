namespace MatrixMultiplication;

public class Matrix
{
    public int[,] MatrixData { get; private set; }
    public int Rows { get; private set; }
    public int Columns { get; private set; }

    public Matrix(int[,] data)
    {
        MatrixData = data;
        Rows = data.GetLength(0);
        Columns = data.GetLength(1);
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

        var newMatrix = new Matrix(newMatrixData);
        return newMatrix;
    }

    public Matrix ParallelMultiply(Matrix secondMatrix)
    {
        if (!MultiplicationMatching(secondMatrix))
        {
            throw new ArgumentException();
        }

        var newMatrixData = new int[Rows, secondMatrix.Columns];
        var newMatrixItemsCount = Rows * secondMatrix.Columns;

        var threadsCount = Environment.ProcessorCount;
        var threads = new Thread[threadsCount];

        var itemsForThread = (newMatrixItemsCount) / threadsCount;
        var remainder = (newMatrixItemsCount) % threadsCount;

        for (var threadNumber = 0; threadNumber < threadsCount; ++threadNumber)
        {
            var nestedThreadNumber = threadNumber;
            threads[threadNumber] = new Thread(() =>
            {
                var startedItemNumber = threadsCount * itemsForThread;
                var lastItemNumber = startedItemNumber + itemsForThread;

                if (nestedThreadNumber < remainder)
                {
                    startedItemNumber += nestedThreadNumber;
                    lastItemNumber += nestedThreadNumber + 1;
                }
                else
                {
                    startedItemNumber += remainder;
                    lastItemNumber += remainder;
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
        
        var newMatrix = new Matrix(newMatrixData);
        return newMatrix;
    }
    
    private bool MultiplicationMatching(Matrix secondMatrix) => Columns == secondMatrix.Rows;
}