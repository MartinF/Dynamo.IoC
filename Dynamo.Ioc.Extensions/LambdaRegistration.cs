using System;

// Expose IResolver by interface or returning LambdaRegistration from Register in LambdaRegistrationExtensions
// Could share interface somehow with IExpressionRegistration ?

namespace Dynamo.Ioc
{
	public class LambdaRegistration<T> : LifetimeRegistrationBase<T>
		where T : class
	{
		private readonly Func<IResolver, object> _lambda;
		private readonly IResolver _resolver;

		public LambdaRegistration(IResolver resolver, Func<IResolver, T> lambda, ILifetime lifetime, object key = null)
			: base(lifetime, key)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");
			if (lambda == null)
				throw new ArgumentNullException("lambda");

			_resolver = resolver;
			_lambda = lambda;
		}

		public IResolver Resolver 
		{
			get { return _resolver; }
		}

		public override object GetInstance()
		{
			return Lifetime.GetInstance(this);
		}

		public override object CreateInstance()
		{
			return _lambda(_resolver);
		}
	}
}
