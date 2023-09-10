using static System.Console;
using MatrixMultiplication;

ForegroundColor = ConsoleColor.Green;
if (args.Length == 0 || args[0] == "-help")
{
    WriteLine("""
      
      This program for matrix multiplication using parallel computing.
      
      To multiply matrices from files:
      - - - - -- - - - - - - - - - - - - - - - - - - - - - - - - - - 
      dotnet run [first matrix file path] [second matrix file path]
      (If the path contains spaces -> enclose it in quotes)
      - - - - -- - - - - - - - - - - - - - - - - - - - - - - - - - - 
      
      """);

    return 0;
}

if (args.Length < 2)
{
    WriteLine("Two arguments are needed");
    WriteLine("For help, use the command: dotnet run -help"); 
}
else
{
    try
    {
        var firstMatrix = new Matrix(args[0]);
        var secondMatrix = new Matrix(args[1]);

        var resultMatrix = firstMatrix.ParallelMultiply(secondMatrix);
        
        using var writer = new StreamWriter("ResultMatrix.txt");
        writer.Write(resultMatrix);
    }
    catch (FileNotFoundException)
    {
        WriteLine("No such file");
        WriteLine("For help, use the command: dotnet run -help");
    }
    catch (ArgumentException)
    {
        WriteLine("Matrix dimensions are not suitable for multiplication");
        WriteLine("For help, use the command: dotnet run -help");
    }
}


return 0;


