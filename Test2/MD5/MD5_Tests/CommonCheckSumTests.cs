using System.Text;
using System.Text.Encodings;

namespace MD5_Tests;

public class Tests
{
    public void CheckEmptyDir_ShouldReturnDirName(string path)
    {
        var name = Path.GetFileName(path);
        var single = Encoding.UTF8.GetString(CheckSumHelper.SingleThreadCheckSum(path));
        var multi = Encoding.UTF8.GetString(CheckSumHelper.MultiThreadCheckSum(path).Result);
        Assert.Multiple(() =>
        {
            Assert.That(single.Equals(name), Is.True);
            Assert.That(multi.Equals(name), Is.True);
        });
    }
}