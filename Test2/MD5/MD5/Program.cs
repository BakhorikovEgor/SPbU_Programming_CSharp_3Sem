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

    Console.WriteLine($"SingleThread time : {watch.ElapsedMilliseconds}");
    Console.WriteLine($"SingleThread result : {BitConverter.ToString(a)}");
        
    watch.Reset();
    watch.Start();
    var multiThreadCheckSum = await CheckSumHelper.MultiThreadCheckSum(args[0]);
    watch.Stop();

    Console.WriteLine($"MultiThread time is {watch.ElapsedMilliseconds}");
    Console.WriteLine($"MultiThread result : {BitConverter.ToString(multiThreadCheckSum)}");

    Console.WriteLine($"Are results equal {a.SequenceEqual(multiThreadCheckSum)}");
}
catch (Exception e) when (e is ArgumentException or IOException)
{
    Console.WriteLine(e.Message);
}
