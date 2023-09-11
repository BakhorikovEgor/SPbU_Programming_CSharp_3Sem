using static System.Console;
using Aspose.Pdf;
using MatrixMultiplication;
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
        var reports = ReportsCreator.CreateReports();

        var document = new Document();
        var table = new Table();

        var page = document.Pages.Add();
        page.Paragraphs.Add(table);

        table.Border = new Aspose.Pdf.BorderInfo(Aspose.Pdf.BorderSide.All, .5f,
            Aspose.Pdf.Color.FromRgb(System.Drawing.Color.LightGray));
        table.DefaultCellBorder = new Aspose.Pdf.BorderInfo(Aspose.Pdf.BorderSide.All, .5f,
            Aspose.Pdf.Color.FromRgb(System.Drawing.Color.LightGray));

        var mainRow = table.Rows.Add();
        mainRow.Cells.Add("First matrix sizes");
        mainRow.Cells.Add("Second matrix sizes");
        mainRow.Cells.Add("Math expectation (milliseс) (sequentially/parallel) ");
        mainRow.Cells.Add("Standard Deviation (milliseс) (sequentially/parallel)");

        foreach (var report in reports)
        {
            var row = table.Rows.Add();
            row.Cells.Add($"{report.FirstMatrixRows} \u00d7 {report.FirstMatrixColumns}");
            row.Cells.Add($"{report.SecondMatrixRows} \u00d7 {report.SecondMatrixColumns}");
            row.Cells.Add($"{Math.Round(report.MathExpectation, 2, MidpointRounding.AwayFromZero)} / " +
                          $"{Math.Round(report.ParallelMathExpectation, 2, MidpointRounding.AwayFromZero)}");
            row.Cells.Add($"{Math.Round(report.StandardDeviation, 2, MidpointRounding.AwayFromZero)} / " +
                          $"{Math.Round(report.ParallelStandardDeviation, 2, MidpointRounding.AwayFromZero)}");
        }

        try
        {
            document.Save("Reports.pdf");
            WriteLine("Done !");
            WriteLine("Check Reports.pdf file.");
        }
        catch (IOException)
        {
            WriteLine("Failed.");
            WriteLine("File Reports.pdf in use");
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