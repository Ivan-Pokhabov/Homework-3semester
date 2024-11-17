using System.Reflection;
using homework5.Assert.AssertionException;
using homework5.TestRunner;
using homework5.Utils;
using ProjectForTest;

namespace MyNUnitTests;

public class MyNUnitTests
{
    private Assembly assembly = null!;

    private TestClassResult testResult = null!;
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        assembly = Assembly.LoadFrom("../../../../TestProject/bin/Debug/net8.0/TestProject.dll");
        
        testResult = (await MyNUnitUtils.RunTestsAsync("../../../../TestProject/bin/Debug/net8.0/TestProject.dll"))[0];
    }
    
    [Test]
    public void BeforeAndAfter_InvokesNumber_ShouldBeExpected()
    {
        const int expectedNumber = 55;

        var successTestsType = assembly.GetTypes().First(t => t.Name == "CorrectTests");
        var actualResult = successTestsType.GetField("beforeAndAfterInvokesAmount")!.GetValue(null);

        Assert.That(actualResult, Is.EqualTo(expectedNumber));
    }
    
    [Test]
    public void BeforeAndAfterClass_InvokesNumber_ShouldBeExpected()
    {
        const int expectedNumber = 11;

        var successTestsType = assembly.GetTypes().First(t => t.Name == "CorrectTests");
        var actualResult = successTestsType.GetField("beforeAndAfterClassInvokesAmount")!.GetValue(null);
        
        Assert.That(actualResult, Is.EqualTo(expectedNumber));
    }

    [Test]
    public void TestResults_ShouldBeExpected()
    {
        var results = new TestResult[5]
        {
            new (methodName: "TestWithRightExceptionShouldBePassed", testException: null, time: 0, ignoreMessage: null),
            new (methodName: "TestWithTrueAssertionShouldBePassed", testException: null, time: 0, ignoreMessage: null),
            new (methodName: "TestWithFalseAssertionShouldBeFailed", testException: typeof(AssertionFailedException), time: 0, ignoreMessage: null),
            new (methodName: "TetsWithIgnoreMessageShouldBeIgnored", testException: null, time: 0, ignoreMessage: "lol ne hochu zapuskat'"),
            new (methodName: "TestWithIncorrectExceptionShouldBeFailed", testException: typeof(ArgumentException), time: 0, ignoreMessage: null)
        };
        var expected = new TestClassResult(typeof(CorrectTests), results);

        Assert.Multiple(() =>
        {
            Assert.That(testResult.results, Is.EqualTo(expected.results));
            Assert.That(testResult.classType, Is.EqualTo(expected.classType));
        });
    }

    [Test]
    public void Test_WithNonStaticBeforeAndAfterClassAttribute_ShouldThrowException()
    {
        static TestClassResult[] func() => MyNUnitUtils.RunTestsAsync("../../../../InvalidTestProject/bin/Debug/net8.0/InvalidTestProject.dll").Result;
        try
        {
            func();
        }
        catch (AggregateException e)
        {
            Assert.Throws<InvalidDataException>(() => throw e.GetBaseException());
        }
    }
}