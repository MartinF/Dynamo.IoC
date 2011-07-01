using System;
using System.Web.Mvc;

namespace Dynamo.Ioc.Web
{
	public abstract class DynamoAreaRegistration : AreaRegistration
	{
		public override void RegisterArea(AreaRegistrationContext context)
		{
			var container = context.State as IContainer;

			// What type of Exception to throw ?
			if (container == null)
				throw new ArgumentException("State object in AreaRegistrationContext for Area: " + context.AreaName + " doesnt contain the IOC Container");

			RegisterServices(container);
		}

		protected abstract void RegisterServices(IContainer container);
	}
}
