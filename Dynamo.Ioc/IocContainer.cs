using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Dynamo.Ioc.Compiler;
using Dynamo.Ioc.Index;

// Implement lock - not possible to change container after use?
	// Just implement it on Index so it is not possible to add to it at some point?

// Let Constructor take IContainer-Settings/Configuration DTO instead? factory, compilemode, compiler, more... ?

// Rename to DependencyContainer ?

namespace Dynamo.Ioc
{
	public class IocContainer : IIocContainer
	{
		#region Fields
		private IIndex _index;
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
			_index = index ?? new DirectIndex();
		}
		#endregion

		#region Properties
		public IIndex Index { get { return _index; } }
		IIndexReader IResolver.Index { get { return _index; } }
		public CompileMode DefaultCompileMode { get { return _defaultCompileMode; } }
		public Func<ILifetime> DefaultLifetimeFactory { get { return _defaultLifetimeFactory; } }
		#endregion

		#region Methods

		#region Register
		public IExpressionRegistration Register<T>(Expression<Func<IResolver, T>> expression, object key = null, ILifetime lifetime = null, CompileMode? compileMode = null)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");

			var lt = lifetime ?? _defaultLifetimeFactory();
			var cm = compileMode != null ? compileMode.Value : _defaultCompileMode;

			var registration = new ExpressionRegistration<T>(this, expression, lt, cm, key: key);

			_index.Add(registration);

			return registration;
		}

		// Auto
		public IExpressionRegistration Register(Type type, Type implType, object key = null, ILifetime lifetime = null, CompileMode? compileMode = null, bool includeInternalCtor = false, Func<ConstructorInfo[], ConstructorInfo> selector = null)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (implType == null)
				throw new ArgumentNullException("implType");

			if (!type.IsAssignableFrom(implType))
				throw new ArgumentException("Type: " + type.Name + " is not assignable from implementation type: " + implType.Name);

			// Forward call to Generic RegisterImpl<,> method using reflection
			Func<object, ILifetime, CompileMode?, bool, Func<ConstructorInfo[], ConstructorInfo>, IExpressionRegistration> pointer = Register<object, object>;
			var reg = ReflectionHelper.InvokeGenericMethod(this, pointer.Method, new Type[] { type, implType }, new Object[] { key, lifetime, compileMode, includeInternalCtor, selector });

			return (IExpressionRegistration)reg;
		}
		public IExpressionRegistration Register<TType, TImpl>(object key = null, ILifetime lifetime = null, CompileMode? compileMode = null, bool includeInternalCtor = false, Func<ConstructorInfo[], ConstructorInfo> selector = null)
			where TType : class
			where TImpl : class, TType
		{
			// Why create a Func<IResolver, T> Expression here when it is always turned into a Func<IResolver, object> later
			// Only to keep the constraints and work when calling Register<T> ?

			var exp = ExpressionHelper.CreateExpression<TType, TImpl>(includeInternalCtor: includeInternalCtor, selector: selector);
			return Register<TType>(exp, key, lifetime, compileMode);
		}

		// Instance
		public IRegistration RegisterInstance(Type type, object instance)
		{
			return RegisterInstance(type, instance, null);
		}
		public IRegistration RegisterInstance(Type type, object instance, object key)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (instance == null)
				throw new ArgumentNullException("instance");

			if (!type.IsAssignableFrom(instance.GetType()))
				throw new ArgumentException("Type: " + type.Name + " is not assignable from instance of type: " + instance.GetType().Name);

			// Forward call to Generic Register<,> methods using reflection
			Func<object, object, IRegistration> pointer = RegisterInstance<object>;
			var reg = ReflectionHelper.InvokeGenericMethod(this, pointer.Method, new Type[] { type }, new Object[] { instance, key });

			return (IRegistration)reg;
		}
		public IRegistration RegisterInstance<T>(T instance, object key = null)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			var registration = new InstanceRegistration<T>(instance, key: key);
			_index.Add(registration);

			return registration;
		}
		#endregion

		#region Resolve
		public object Resolve(Type type)
		{
			return _index.Get(type).GetInstance();
		}
		public object Resolve(Type type, object key)
		{
			return _index.Get(type, key).GetInstance();
		}
		public T Resolve<T>()
		{
			return (T)_index.Get(typeof(T)).GetInstance();
		}
		public T Resolve<T>(object key)
		{
			return (T)_index.Get(typeof(T), key).GetInstance();
		}
		#endregion
		
		#region TryResolve
		// Let generic version use non-generic version ?
		// And make key optional to remove 2 methods
		public bool TryResolve(Type type, out object instance)
		{
			IRegistration registration;
			if (_index.TryGet(type, out registration))
			{
				instance = registration.GetInstance();
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
				instance = registration.GetInstance();
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
				instance = (T)registration.GetInstance();
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
				instance = (T)registration.GetInstance();
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
				yield return registration.GetInstance();
			}
		}
		public IEnumerable<T> ResolveAll<T>()
		{
			foreach (var registration in _index.GetAll(typeof(T)))
			{
				yield return (T)registration.GetInstance();
			}
		}
		#endregion

		#region TryResolveAll
		public IEnumerable<object> TryResolveAll(Type type)
		{
			foreach (var registration in _index.TryGetAll(type))
			{
				yield return registration.GetInstance();
			}
		}
		public IEnumerable<T> TryResolveAll<T>()
		{
			foreach (var registration in _index.TryGetAll(typeof(T)))
			{
				yield return (T)registration.GetInstance();
			}
		}
		#endregion

		public void Compile()
		{
			var compiler = new ExpressionCompiler();
			compiler.Compile(this);
		}

		public void Verify()
		{
			// For ExpressionRegistrations it only verfies it can create instances - not if the lifetime is working like it should in the given context etc.

			// TODO: Rename to Pre-heat / Bootstrap / ? 

			foreach (var registration in _index)
			{
				Exception exception = null;

				try
				{
					if (!registration.Verify())
					{
						exception = new InvalidRegistrationException(registration);			
					}
				}
				catch (Exception e)
				{
					throw new InvalidRegistrationException(registration, e);
				}

				if (exception != null)
					throw exception;
			}
		}

		public void Dispose()
		{
			if (_index != null)
			{
				_index.Dispose();
				_index = null;			
			}
		}
		#endregion
	}
}
