namespace Dynamo.Ioc.ServiceLocator.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ServiceLocator = Microsoft.Practices.ServiceLocation.ServiceLocator;

    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void CanSetProvider()
        {
            var resolver = new IocContainer();
            var locator = new DynamoServiceLocator(resolver);
            ServiceLocator.SetLocatorProvider(() => locator);

            // No way to get provider from ServiceLocator.Current to verify!!
        }
    }
}
