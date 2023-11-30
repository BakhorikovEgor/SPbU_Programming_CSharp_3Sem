using MD5;
using System.Diagnostics;

const string helpMessage = "How to run: dotnet run (string){path to directory of file}";
if (args.Length != 1)
{
    Console.WriteLine(helpMessage);
    return;
}

try
{
    var watch = new Stopwatch();

    watch.Start();
    var a = CheckSumHelper.SingleThreadCheckSum(args[0]);
    watch.Stop();

    Console.WriteLine($"SingleThread time is {watch.ElapsedMilliseconds}");

    watch.Reset();
    watch.Start();
    var b = await CheckSumHelper.MultiThreadCheckSum(args[0]);
    watch.Stop();

    Console.WriteLine($"MultiThread time is {watch.ElapsedMilliseconds}");

    Console.WriteLine(a.SequenceEqual(b));
}
catch (ArgumentException e)
{
    Console.WriteLine(e.Message);
}
catch (IOException)
{
    Console.WriteLine("File in use");
}