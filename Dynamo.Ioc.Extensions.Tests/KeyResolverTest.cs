using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Extensions.Tests
{
	[TestClass]
	public class KeyResolverTest
	{
		[TestMethod]
		public void KeyResolverWorks()
		{
			using (var container = new IocContainer())
			{
				var reg1 = container.Register<IDeviceState>(x => new FooConnection(true), DeviceState.Online);
				var reg2 = container.Register<IDeviceState>(x => new FooConnection(false), DeviceState.Offline);
				var reg3 = container.Register<IHardwareDevice>(x => new Modem(x.GetKeyResolver<IDeviceState, DeviceState>()));

				var instance = container.Resolve<IHardwareDevice>();

				Assert.IsTrue(instance.State.Connected);

				instance.SwitchOff();

				Assert.IsFalse(instance.State.Connected);
			}
		}
	}

	#region Stubs
	public enum DeviceState
	{
		Online, 
		Offline
	}
	public interface IDeviceState
	{
		bool Connected { get; }
	}
	public class FooConnection : IDeviceState
	{
		public FooConnection(bool connected)
		{
			Connected = connected;
		}

		public bool Connected { get; private set; }
	}
	public interface IHardwareDevice
	{
		IDeviceState State { get; }
		void SwitchOn();
		void SwitchOff();
	}
	public class Modem : IHardwareDevice
	{
		private readonly KeyResolver<IDeviceState, DeviceState> _states;
		private IDeviceState _currentState;

		public Modem(KeyResolver<IDeviceState, DeviceState> states)
		{
			_states = states; 
			SwitchOn();
		} 
		
		public void SwitchOn()
		{
			_currentState = _states[DeviceState.Online];
		}

		public void SwitchOff()
		{
			_currentState = _states[DeviceState.Offline];
		}

		public IDeviceState State
		{
			get { return _currentState; }
		}
	}
	#endregion
}