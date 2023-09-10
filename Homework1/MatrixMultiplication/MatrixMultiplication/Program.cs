using MatrixMultiplication;


var firstMatrix = Matrix.GenerateMatrix(10, 10);
var secondMatrix = Matrix.GenerateMatrix(10, 10);

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