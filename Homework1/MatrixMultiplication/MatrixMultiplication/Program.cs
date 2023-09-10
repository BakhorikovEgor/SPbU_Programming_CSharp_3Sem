using MatrixMultiplication;


var firstMatrix = new Matrix(new int[2, 2] { { 1, 1 }, { 2, 2 } });
var secondMatrix = new Matrix(new int[2, 2] { { 3, 3 }, { 4, 4 } });

var resultMatrixData1 = firstMatrix.Multiply(secondMatrix).MatrixData;
var resultMatrixData2 = firstMatrix.ParallelMultiply(secondMatrix).MatrixData;


for (var i = 0; i < resultMatrixData1.GetLength(0); ++i)
{
    Console.WriteLine();
    for (var j = 0; j < resultMatrixData1.GetLength(1); ++j)
    {
        Console.Write(resultMatrixData1[i, j] + " ");
    }
}


for (var i = 0; i < resultMatrixData2.GetLength(0); ++i)
{
    Console.WriteLine();
    for (var j = 0; j < resultMatrixData2.GetLength(1); ++j)
    {
        Console.Write(resultMatrixData2[i, j] + " ");
    }
}