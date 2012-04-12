using System;
using System.Web.Mvc;

namespace Dynamo.Ioc.Web
{
	public abstract class DynamoAreaRegistration : AreaRegistration
	{
		public override void RegisterArea(AreaRegistrationContext context)
		{
			var container = context.State as IIocContainer;

			if (container == null)
				throw new ArgumentException("State object in AreaRegistrationContext for Area: " + context.AreaName + " doesnt contain the IOC Container");

			RegisterDependencies(container);
		}

		protected abstract void RegisterDependencies(IIocContainer container);
	}
}
