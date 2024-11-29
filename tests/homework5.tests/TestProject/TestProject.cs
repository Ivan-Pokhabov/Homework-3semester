using homework5.Attributes;
using homework5.Assert;

namespace ProjectForTest;

public class CorrectTests
{
    public static volatile int beforeAndAfterClassInvokesAmount;

    public static volatile int beforeAndAfterInvokesAmount;
    
    [BeforeClass]
    public static void BeforeClass()
    {
        Interlocked.Add(ref beforeAndAfterClassInvokesAmount, 10);
    }

    [AfterClass]
    public static void AfterClass()
    {
        Interlocked.Increment(ref beforeAndAfterClassInvokesAmount);
    }

    [Before]
    public void Before()
    {
        Interlocked.Add(ref beforeAndAfterInvokesAmount, 10);
    }
    
    [After]
    public void After()
    {
        Interlocked.Increment(ref beforeAndAfterInvokesAmount);
    }
    
    [Test(typeof(InvalidDataException))]
    public void TestWithRightExceptionShouldBePassed()
    {
        throw new InvalidDataException();
    }

    [Test]
    public void TestWithTrueAssertionShouldBePassed()
    {
        const int temp = 2;
        Assert.That(2 == temp);
    }

    [Test]
    public void TestWithFalseAssertionShouldBeFailed()
    {
        const int temp = 2;
        Assert.That(1 == temp);
    }

    [Test("lol ne hochu zapuskat'")]
    public void TetsWithIgnoreMessageShouldBeIgnored()
    {
        Assert.That(false);
    }

    [Test(typeof(InvalidDataException))]
    public void TestWithIncorrectExceptionShouldBeFailed()
    {
        throw new ArgumentException();
    }
}