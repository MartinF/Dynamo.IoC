using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Dynamo.Ioc.Index;

// Make Registration type for each lifetime instead of attaching the lifetime to the ExpressionRegistration ?

// Set constraint on Register so it cant register a struct? - should use RegisterInstance for that ?

// Rename useInternalCtor to includeInternalCtor - and make so it includes internal constructors when selecting
// Should useInternalCtor be false per default ?

// Remove generic implementation again? (IRegistration<T> and IExpressionRegistration<T>)

// IDisposable - Container, Registration, Lifetime, Index ? - but index and registration can be shared ?

namespace Dynamo.Ioc
{
	public class IocContainer : IIocContainer
	{
		#region Fields
		private readonly IIndex _index;
		private readonly CompileMode _defaultCompileMode;
		private readonly Func<ILifetime> _defaultLifetimeFactory;
		#endregion

		#region Constructor
		public IocContainer(Func<ILifetime> defaultLifetimeFactory = null, CompileMode defaultCompileMode = CompileMode.Delegate, IIndex index = null)
		{
			if (defaultLifetimeFactory != null)
				_defaultLifetimeFactory = defaultLifetimeFactory;	// Test if the lifetime factory returns valid ILifetime?
			else
				_defaultLifetimeFactory = () => new TransientLifetime();
	
			if (!Enum.IsDefined(typeof(CompileMode), defaultCompileMode))
				throw new ArgumentException("Invalid CompileMode value");

			_defaultCompileMode = defaultCompileMode;
			_index = index ?? new DirectIndex();	// new GroupedIndex(); Which one is fastest now?
		}
		#endregion

		#region Properties
		public IIndex Index { get { return _index; } }
		public CompileMode DefaultCompileMode { get { return _defaultCompileMode; } }
		public Func<ILifetime> DefaultLifetimeFactory { get { return _defaultLifetimeFactory; } }
		#endregion



		// SET DirectIndex as the standard one ???? better now !?!?!??!




		// IResolvable + IResolveable<T> - instead of IRegistration ? can be used as a lazy resolve feature bascically 
		// IRegistration have all info about the registratration ? which shouldnt be available for IResolveable ?


		// set compileMode to none instead of using a nullable enum ?

		#region Methods

		#region Register
		// Remove Unneeded parameters - compileMode and lifetime ? - both for Register and Register Automatic
			// Set compileMode when creating registration, or set it afterwards (change it) with .SetCompileMode() ?
			// Set lifetime when creating registration, or set it afterwards (change it) with .SetLifetime() ?
		public IExpressionRegistration Register<T>(Expression<Func<IResolver, T>> expression, ILifetime lifetime = null, CompileMode? compileMode = null)
		{
			return RegisterImpl(expression, null, lifetime, compileMode);
		}
		public IExpressionRegistration Register<T>(Expression<Func<IResolver, T>> expression, object key, ILifetime lifetime = null, CompileMode? compileMode = null)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			return RegisterImpl(expression, key, lifetime, compileMode);
		}


		private IExpressionRegistration RegisterImpl<T>(Expression<Func<IResolver, T>> expression, object key, ILifetime lifetime, CompileMode? compileMode)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");
			
			lifetime = lifetime ?? _defaultLifetimeFactory();

			if (compileMode == null)
				compileMode = _defaultCompileMode;
			
			var registration = new ExpressionRegistration<T>(expression, lifetime, compileMode.Value);
			

			// SHOULD BE 2 DIFFERENT VERSIONS OF ADD - one for key and one without ... which have the null checks on the key

			_index.Add(registration, key);

			return registration;
		}
		#endregion

		#region Register - Automatic
		public IExpressionRegistration Register(Type type, Type implType, ILifetime lifetime = null, CompileMode? compileMode = null, bool includeInternalCtor = false, Func<ConstructorInfo[], ConstructorInfo> selector = null)
		{
			return RegisterAutoImpl(type, implType, null, lifetime, compileMode, includeInternalCtor, selector);
		}
		public IExpressionRegistration Register(Type type, Type implType, object key, ILifetime lifetime = null, CompileMode? compileMode = null, bool includeInternalCtor = false, Func<ConstructorInfo[], ConstructorInfo> selector = null)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			return RegisterAutoImpl(type, implType, key, lifetime, compileMode, includeInternalCtor, selector);
		}
		private IExpressionRegistration RegisterAutoImpl(Type type, Type implType, object key, ILifetime lifetime, CompileMode? compileMode, bool includeInternalCtor, Func<ConstructorInfo[], ConstructorInfo> selector)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (implType == null)
				throw new ArgumentNullException("implType");

			if (!type.IsAssignableFrom(implType))
				throw new ArgumentException("Type: " + type.Name + " is not assignable from implementation type: " + implType.Name);

			// Forward call to Generic RegisterImpl<,> method using reflection
			Func<object, ILifetime, CompileMode?, bool, Func<ConstructorInfo[], ConstructorInfo>, IExpressionRegistration> pointer = RegisterAutoImpl<object, object>;
			var reg = ReflectionHelper.InvokeGenericMethod(this, pointer.Method, new Type[] { type, implType }, new Object[] { key, lifetime, compileMode, includeInternalCtor, selector });

			return (IExpressionRegistration)reg;
		}

		// Generics
		public IExpressionRegistration Register<TType, TImpl>(ILifetime lifetime = null, CompileMode? compileMode = null, bool includeInternalCtor = false, Func<ConstructorInfo[], ConstructorInfo> selector = null)
			where TType : class
			where TImpl : class, TType
		{
			return RegisterAutoImpl<TType, TImpl>(null, lifetime, compileMode, includeInternalCtor, selector);
		}
		public IExpressionRegistration Register<TType, TImpl>(object key, ILifetime lifetime = null, CompileMode? compileMode = null, bool includeInternalCtor = false, Func<ConstructorInfo[], ConstructorInfo> selector = null)
			where TType : class
			where TImpl : class, TType
		{
			if (key == null)
				throw new ArgumentNullException("key");

			return RegisterAutoImpl<TType, TImpl>(key, lifetime, compileMode, includeInternalCtor, selector);
		}
		private IExpressionRegistration RegisterAutoImpl<TType, TImpl>(object key, ILifetime lifetime, CompileMode? compileMode, bool includeInternalCtor, Func<ConstructorInfo[], ConstructorInfo> selector)
			where TType : class
			where TImpl : class, TType
		{
			var exp = ExpressionHelper.CreateExpression<TType, TImpl>(includeInternalCtor: includeInternalCtor, selector: selector);
			return RegisterImpl<TType>(exp, key, lifetime, compileMode);
		}
		#endregion

		#region Register Instance
		public IRegistration RegisterInstance(Type type, object instance)
		{
			return RegisterInstanceImpl(type, instance, null);
		}
		public IRegistration RegisterInstance(Type type, object instance, object key)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			return RegisterInstanceImpl(type, instance, key);
		}
		private IRegistration RegisterInstanceImpl(Type type, object instance, object key)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (instance == null)
				throw new ArgumentNullException("instance");

			if (!type.IsAssignableFrom(instance.GetType()))
				throw new ArgumentException("Type: " + type.Name + " is not assignable from instance of type: " + instance.GetType().Name);

			// Forward call to Generic Register<,> methods using reflection
			Func<object, object, IRegistration> pointer = RegisterInstanceImpl<object>;
			var reg = ReflectionHelper.InvokeGenericMethod(this, pointer.Method, new Type[] { type }, new Object[] { instance, key });

			return (IRegistration)reg;
		}

		public IRegistration RegisterInstance<T>(T instance)
		{
			return RegisterInstanceImpl(instance, null);
		}
		public IRegistration RegisterInstance<T>(T instance, object key)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			return RegisterInstanceImpl(instance, key);
		}
		private IRegistration RegisterInstanceImpl<T>(T instance, object key)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");



			// Check for null where ? 
			// Split index in two add methods so check for null on key happens there instead of here?
			


			var registration = new InstanceRegistration<T>(instance);
			_index.Add(registration, key);

			return registration;
		}
		#endregion



		#region Resolve
		public object Resolve(Type type)
		{
			return _index.Get(type).GetInstance(this);
		}
		public object Resolve(Type type, object key)
		{
			return _index.Get(type, key).GetInstance(this);
		}
		public T Resolve<T>()
		{
			return (T)_index.Get(typeof(T)).GetInstance(this);	// Faster
			//return _index.Get<T>().GetInstance(this);			// Slower
		}
		public T Resolve<T>(object key)
		{
			return (T)_index.Get(typeof(T), key).GetInstance(this);
		}
		#endregion
		
		#region TryResolve

		// Let generic version use non-generic version ?
		
		public bool TryResolve(Type type, out object instance)
		{
			IRegistration registration;
			if (_index.TryGet(type, out registration))
			{
				instance = registration.GetInstance(this);
				return true;
			}

			instance = null;
			return false;
		}
		public bool TryResolve(Type type, object key, out object instance)
		{
			IRegistration registration;
			if (_index.TryGet(type, key, out registration))
			{
				instance = registration.GetInstance(this);
				return true;
			}

			instance = null;
			return false;
		}
		public bool TryResolve<T>(out T instance)
		{
			IRegistration registration;
			if (_index.TryGet(typeof(T), out registration))
			{
				instance = (T)registration.GetInstance(this);
				return true;
			}

			instance = default(T);
			return false;
		}
		public bool TryResolve<T>(object key, out T instance)
		{
			IRegistration registration;
			if (_index.TryGet(typeof(T), key, out registration))
			{
				instance = (T)registration.GetInstance(this);
				return true;
			}

			instance = default(T);
			return false;
		}
		#endregion

		#region ResolveAll
		public IEnumerable<object> ResolveAll(Type type)
		{
			foreach (var registration in _index.GetAll(type))
			{
				yield return registration.GetInstance(this);
			}
		}
		public IEnumerable<T> ResolveAll<T>()
		{
			foreach (var registration in _index.GetAll(typeof(T)))
			{
				yield return (T)registration.GetInstance(this);
			}
		}
		#endregion

		#region TryResolveAll
		public IEnumerable<object> TryResolveAll(Type type)
		{
			// Just  return or yield return ?
			//yield return TryResolveAll<object>();

			foreach (var registration in _index.TryGetAll(type))
			{
				yield return registration.GetInstance(this);
			}
		}
		public IEnumerable<T> TryResolveAll<T>()
		{
			foreach (var registration in _index.TryGetAll(typeof(T)))
			{
				yield return (T)registration.GetInstance(this);
			}
		}
		#endregion



		public void Compile()
		{
			// Verify before or/and after ? 
	
			foreach (var registration in _index)
			{
				var compilableRegistration = registration as ICompilableRegistration;
				if (compilableRegistration != null)
					compilableRegistration.Compile(this);
			}
		}


		// Clean / Clear All method - runs through all and casts to IExpressionRegistration and if true calls Clean/Clear on lifetime !



		/* Removed until problems are fixed
		 * 
		 * Implement Reset/Refresh/Clear method on ILifetime
		 * Only few methods needs to implement it - like Session/Request Lifetime ?
		 * 
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
		*/

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
