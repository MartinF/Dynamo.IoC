using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dynamo.Ioc.Index;

// The container is basically just a wrapper around the IIndex
// It creates registrations and adds them to the index, and when resolving it gets the registration from the index and calls GetInstance() on it

namespace Dynamo.Ioc
{
	public class Container : IContainer
	{
		#region Fields
		private readonly IIndex _index = new GroupedIndex();
		private readonly Func<ILifetime> _defaultLifetimeFactory = () => new TransientLifetime();
		private readonly CompileMode _defaultCompileMode;
		#endregion

		#region Constructor
		public Container(Func<ILifetime> defaultLifetimeFactory = null, CompileMode compileMode = CompileMode.Dynamic, IIndex index = null)
		{
			if (index != null)
				_index = index;

			if (defaultLifetimeFactory != null)
			{
				// Check that the func returns a ILifetime
				if (defaultLifetimeFactory() == null)
					throw new ArgumentException("Default Lifetime Factory returns null. It must return a ILifetime.");

				_defaultLifetimeFactory = defaultLifetimeFactory;
			}

			_defaultCompileMode = compileMode;
		}
		#endregion

		#region Properties
		public IIndex Index { get { return _index; } }
		public ILifetimeInfo DefaultLifetime { get { return _defaultLifetimeFactory(); } }
		public CompileMode DefaultCompileMode { get { return _defaultCompileMode; } }
		#endregion

		#region Methods

		#region Register
		public IConfigurableRegistration Register(Type type, Expression<Func<IResolver, object>> expression)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (expression == null)
				throw new ArgumentNullException("expression");

			return RegisterImpl(type, null, expression);
		}
		public IConfigurableRegistration Register(Type type, object key, Expression<Func<IResolver, object>> expression)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (key == null)
				throw new ArgumentNullException("key");
			if (expression == null)
				throw new ArgumentNullException("expression");

			return RegisterImpl(type, key, expression);
		}
		private IConfigurableRegistration RegisterImpl(Type type, object key, Expression<Func<IResolver, object>> expression)
		{
			// Skip this check ?
			// Problem here if struct - use RegisterInstance ?
			// Not possible to use CompileMode.Dynamic if expression reference a variable - but else it works
			Type returnType = type.IsValueType ? ((UnaryExpression)expression.Body).Operand.Type : expression.Body.Type;
			if (!type.IsAssignableFrom(returnType))
				throw new RegistrationException(type, key, "Lambda Expression doesnt return an object of the expected type: " + type.Name);

			var lifetime = _defaultLifetimeFactory();
			var registration = new ExpressionRegistration(type, expression, lifetime, key, _defaultCompileMode);
			_index.Add(registration);

			return registration;
		}
		#endregion

		#region Register Instance
		public IRegistration RegisterInstance(Type type, object instance)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (instance == null)
				throw new ArgumentNullException("instance");

			return RegisterInstanceImpl(type, null, instance);
		}
		public IRegistration RegisterInstance(Type type, object key, object instance)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (key == null)
				throw new ArgumentNullException("key");
			if (instance == null)
				throw new ArgumentNullException("instance");

			return RegisterInstanceImpl(type, key, instance);
		}
		private IRegistration RegisterInstanceImpl(Type type, object key, object instance)
		{
			// Keep this check ?
			if (!type.IsAssignableFrom(instance.GetType()))
				throw new RegistrationException(type, key, "Instance being registered is not assignable from Type: " + type.Name);

			var registration = new InstanceRegistration(type, instance, key);
			_index.Add(registration);

			return registration;
		}
		#endregion

		#region Try- Resolve
		public object Resolve(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return _index.Get(type).GetInstance(this);
		}
		public object Resolve(Type type, object key)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (key == null)
				throw new ArgumentNullException("key");

			return _index.Get(type, key).GetInstance(this);
		}
		public bool TryResolve(Type type, out object obj)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			IRegistration registration;
			if (_index.TryGet(type, out registration))
			{
				obj = registration.GetInstance(this);
				return true;
			}

			obj = null;
			return false;
		}
		public bool TryResolve(Type type, object key, out object obj)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (key == null)
				throw new ArgumentNullException("key");

			IRegistration registration;
			if (_index.TryGet(type, key, out registration))
			{
				obj = registration.GetInstance(this);
				return true;
			}

			obj = null;
			return false;
		}
		#endregion

		#region Try- ResolveAll
		public IEnumerable<object> ResolveAll(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			foreach (var registration in _index.GetAll(type))
			{
				yield return registration.GetInstance(this);
			}
		}
		public IEnumerable<object> TryResolveAll(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			foreach (var registration in _index.TryGetAll(type))
			{
				yield return registration.GetInstance(this);
			}
		}
		#endregion



		public void Compile()
		{
			// Verify before or/and after ? 
	
			foreach (var registration in _index)
			{
				var compilableRegistration = registration as ICompilable;
				if (compilableRegistration != null)
					compilableRegistration.Compile(_index);
			}
		}
		
		public void Verify()
		{
			// This doesnt really work as intended currently.

			// Right now it just tries to resolve all - and if this is done in Application_Start of a web app etc and it is registered to be disposed on RequestEnd it will never happen because the event is never fired.
			// Currently it remindes more of just a init/warmup method.
			// Call Reset() on all ExpressionRegistrations or only their Lifetimes after Verify have been run so they can clean up themselves? or let them stay "initialized"?

			// Create both a Verify and a TryVerify ? 

			foreach (var registration in _index)
		    {
		        object instance = null;
		        try
		        {
		            instance = registration.GetInstance(this);
		        }
		        catch (Exception e)
		        {
		            throw new RegistrationException(registration, e);	// VerifyException ? What kind of Exception ?
		        }

		        // If registration exists it should not be null
		        if (instance == null)
		            throw new RegistrationException(registration, "Resolved instance was null.");	// Exception type ?
		    }
		}

		public void Dispose()
		{
			// The big question ? 

			// Support disposing all registrations etc. or require registrations to implement a desctructor and handle it themselves ? 
			// Lifetime of the container is usually from the start of the app and to the end.
			// If keeping, implement Desctructor/Finalizer ?

			// Call Dispose() on the _index or just call Clear() ? 
			// Let the IIndex be responsible for disposing all the registrations?

			// Currently no unmanaged resources are used.
		}

		#endregion
	}
}
