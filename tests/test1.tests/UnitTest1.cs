namespace test1;

public class CheckSumCalculatorTests
{
    
    [Test]
    public void MultithreadingCalculate_WithCorrectFolder_ShouldReturnExpectedResult()
    {
        var path = "../../../";
        CheckSumCalculator.Calculate(path);
        var expectedResult = CheckSumCalculator.GetResult;
        CheckSumCalculator.Clear();

        CheckSumCalculator.MultithreadingCalculate(path);
        var result = CheckSumCalculator.GetResult;


        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void MultithreadingCalculate_WithCorrectFile_ShouldReturnExpectedResult()
    {
        var path = "../../../test1.tests.sln";
        CheckSumCalculator.Calculate(path);
        var expectedResult = CheckSumCalculator.GetResult;
        CheckSumCalculator.Clear();
        

        CheckSumCalculator.MultithreadingCalculate(path);
        var result = CheckSumCalculator.GetResult;


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