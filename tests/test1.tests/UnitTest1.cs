namespace test1;

public class CheckSumCalculatorTests
{
    
    [Test]
    public void MultithreadingCalculate_WithCorrectFolder_ShouldReturnExpectedResult()
    {
        var path = "../";
        var expectedResult = CheckSumCalculator.Calculate(path);


        var result = CheckSumCalculator.MultithreadingCalculate(path);


        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void MultithreadingCalculate_WithCorrectFile_ShouldReturnExpectedResult()
    {
        var path = "../../../test1.tests.sln";
        var expectedResult = CheckSumCalculator.Calculate(path);


        var result = CheckSumCalculator.MultithreadingCalculate(path);


        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void Calculate_WithNonexistedFolderOrFile_ShouldThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => CheckSumCalculator.Calculate("../NonExist.cs"));
        Assert.Throws<ArgumentException>(() => CheckSumCalculator.Calculate("../NonExist"));
    }

    [Test]
    public void MultithreadingCalculate_WithNonexistedFolderOrFile_ShouldThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => CheckSumCalculator.MultithreadingCalculate("../NonExist.cs"));
        Assert.Throws<ArgumentException>(() => CheckSumCalculator.MultithreadingCalculate("../NonExist"));
    }
}