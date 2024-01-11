namespace MyNUnit.Models;

public record TestClassReportModel(Type ClassType, TestReportModel[] Reports)
{
}

public record TestReportModel(string Result, long RuntTime)
{
}