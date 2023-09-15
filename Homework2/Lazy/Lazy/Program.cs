using Lazy;

var temp = new SimpleLazy<int>(() => 10);
Console.WriteLine(temp.Get());
Console.WriteLine(temp.Get());