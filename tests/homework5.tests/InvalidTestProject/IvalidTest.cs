using homework5.Attributes;
using homework5.Assert;

namespace MyNUnitInvalidTestsProjectForTest;

public class InvalidTests
{

    [BeforeClass]
    public void NotStaticBeforeClass()
    {

    }
    
    [AfterClass]
    public void NotStaticAfterClass()
    {

    }
}