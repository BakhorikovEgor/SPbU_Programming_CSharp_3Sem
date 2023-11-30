using System.Text;

namespace MD5_Tests;

public class Tests
{
    public Tests()
    {
        Environment.CurrentDirectory = "../../../";
    }
    
    [TestCase("TestDirs/EmptyDir")]
    public void CheckEmptyDir_ShouldReturnDirName(string path)
    {
        var name = Path.GetFileName(path);
        var single = Encoding.UTF8.GetString(CheckSumHelper.SingleThreadCheckSum(path));
        var multi = Encoding.UTF8.GetString(CheckSumHelper.MultiThreadCheckSum(path).Result);
        Assert.Multiple(() =>
        {
            Assert.That(single, Is.EqualTo(name));
            Assert.That(multi, Is.EqualTo(name));
        });
    }


    [TestCase("TestDirs/NestedDir")]
    [TestCase("TestDirs/FileDir")]
    public void CheckBySingleAndMultiThreadHelpers_ShouldReturnSameResult(string path)
    {
        var single = CheckSumHelper.SingleThreadCheckSum(path);
        var multi = CheckSumHelper.MultiThreadCheckSum(path).Result;
        
        Assert.That(single.SequenceEqual(multi), Is.True);
    }

    
    [TestCase("TestDirs/NestedDir")]
    [TestCase("TestDirs/EmptyDir")]
    [TestCase("TestDirs/FileDir")]
    public void CheckMoreThanOneTime_ShouldReturnSameResult(string path)
    {
        var prevSingle = CheckSumHelper.SingleThreadCheckSum(path);
        var prevMulti = CheckSumHelper.MultiThreadCheckSum(path).Result;

        byte[] newSingle;
        byte[] newMulti;
        for (var i = 0; i < 10; ++i)
        {
            newSingle = CheckSumHelper.SingleThreadCheckSum(path);
            newMulti = CheckSumHelper.MultiThreadCheckSum(path).Result;
            Assert.Multiple(() =>
            {
                Assert.That(newSingle.SequenceEqual(prevSingle), Is.True);
                Assert.That(newMulti.SequenceEqual(prevMulti), Is.True);
            });
        }
    }
}