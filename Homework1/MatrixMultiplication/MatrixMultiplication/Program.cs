using System.Threading.Channels;
using MatrixMultiplication;


var firstMatrix = new Matrix(@"C:\Users\Егор\Desktop\matrix1.txt");
var secondMatrix = new Matrix(@"C:\Users\Егор\Desktop\matrix2.txt");

var resultMatrixData1 = firstMatrix.Multiply(secondMatrix);
var resultMatrixData2 = firstMatrix.ParallelMultiply(secondMatrix);

Console.WriteLine(resultMatrixData1);
Console.WriteLine(resultMatrixData2);