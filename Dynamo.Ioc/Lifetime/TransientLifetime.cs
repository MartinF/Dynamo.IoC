
namespace Dynamo.Ioc
{
	public sealed class TransientLifetime : LifetimeBase
	{
		public override object GetInstance(IInstanceFactory factory, IResolver resolver)
		{
			return factory.CreateInstance(resolver);
		}
	}
}