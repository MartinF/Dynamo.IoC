namespace Dynamo.Ioc.ServiceLocator
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Practices.ServiceLocation;

    public class DynamoServiceLocator : ServiceLocatorImplBase
    {
        private readonly IResolver _resolver;

        public DynamoServiceLocator(IResolver resolver)
        {
            _resolver = resolver;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (!string.IsNullOrEmpty(key))
                return _resolver.Resolve(serviceType, key);

            return _resolver.Resolve(serviceType);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return _resolver.ResolveAll(serviceType);
        }
    }
}