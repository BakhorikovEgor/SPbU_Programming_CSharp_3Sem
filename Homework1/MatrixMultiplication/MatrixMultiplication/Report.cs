namespace MatrixMultiplication;

/// <summary>
/// A structure representing the result of collecting statistics on the
/// multiplication of two given matrices in different ways
/// </summary>
internal struct Report
{
    /// <summary>
    /// Number of rows in first matrix.
    /// </summary>
    public int FirstMatrixRows { get; }
    
    /// <summary>
    /// Number of columns in first matrix.
    /// </summary>
    public int FirstMatrixColumns { get; }
    
    /// <summary>
    /// Number of rows in second matrix.
    /// </summary>
    public int SecondMatrixRows { get; }
    
    /// <summary>
    /// Number of columns in second matrix.
    /// </summary>
    public int SecondMatrixColumns { get; }
    
    /// <summary>
    /// Mathematical expectation of the time to multiply matrices sequentially (milliseconds).
    /// </summary>
    public double MathExpectation { get; }
    
    /// <summary>
    /// Standard Deviation of the time to multiply matrices sequentially (milliseconds).
    /// </summary>
    public double StandardDeviation { get; }
    
    /// <summary>
    /// Mathematical expectation of the time to multiply matrices in parallel (milliseconds).
    /// </summary>
    public double ParallelMathExpectation { get; }
    
    /// <summary>
    /// Standard Deviation of the time to multiply matrices in parallel  (milliseconds).
    /// </summary>
    public double ParallelStandardDeviation { get; }
    

    /// <summary>
    /// Standard constructor.
    /// </summary>
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