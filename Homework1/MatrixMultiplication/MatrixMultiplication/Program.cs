﻿using MatrixMultiplication;
using static System.Console;
using Matrix = MatrixMultiplication.Matrix;


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

              To create/update Reports.pdf file:
              - - - - -- - - - - - - - - - - - - - - - - - - - - - - - - - -
              dotnet run -reports
              - - - - -- - - - - - - - - - - - - - - - - - - - - - - - - - -

              """);

    return 0;
}


if (args.Length == 1)
{
    if (args[0] == "-reports")
    {
        WriteLine("Evaluating....");
        var reports = ReportsHandler.CreateReports();
        try
        {
            ReportsHandler.SaveReportsToFile(reports, "Reports.pdf");
            WriteLine("Done !");
            WriteLine("Check Reports.pdf file.");
        }
        catch (IOException e)
        {
            WriteLine("Failed.");
            WriteLine(e.Message);
        }
    }
    else
    {
        WriteLine("Unknown command");
        WriteLine("For help, use the command: dotnet run -help");
    }
}
else
{
    try
    {
        var firstMatrix = Matrix.ReadMatrixFromFile(args[0]);
        var secondMatrix = Matrix.ReadMatrixFromFile(args[1]);

        var resultMatrix = firstMatrix.ParallelMultiply(secondMatrix);
        resultMatrix.SaveToFile("ResultMatrix.txt");
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