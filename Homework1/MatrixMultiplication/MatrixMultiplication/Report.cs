namespace MatrixMultiplication;

public struct Report
{
    public int FirstMatrixRows { get; }
    public int FirstMatrixColumns { get; }
    public int SecondMatrixRows { get; }
    public int SecondMatrixColumns { get; }
    public double MathExpectation { get; }
    public double StandardDeviation { get; }
    public double ParallelMathExpectation { get; }
    public double ParallelStandardDeviation { get; }
    

    public Report  (int firstMatrixRows, int firstMatrixColumNs, int secondMatrixRows, 
                    int secondMatrixColumns, double mathExpectation, double standardDeviation,
                    double parallelMathExpectation, double parallelStandardDeviation)
    {
        FirstMatrixRows = firstMatrixRows;
        FirstMatrixColumns = firstMatrixColumNs;
        SecondMatrixRows = secondMatrixRows;
        SecondMatrixColumns = secondMatrixColumns;
        MathExpectation = mathExpectation;
        StandardDeviation = standardDeviation;
        ParallelMathExpectation = parallelMathExpectation;
        ParallelStandardDeviation = parallelStandardDeviation;
    }
}