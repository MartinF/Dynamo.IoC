
namespace Dynamo.Ioc
{
	public abstract class LifetimeBase : ILifetime
	{
		protected LifetimeBase()
		{
		}

		public abstract object GetInstance(IInstanceFactory factory, IResolver resolver);

		// Optional
		public virtual void Init(IRegistrationInfo key)
		{
		}

		// Dispose ?
	}
}