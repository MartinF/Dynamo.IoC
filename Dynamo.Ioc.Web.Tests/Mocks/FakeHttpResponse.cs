using System.Text;
using System.Web;

namespace Dynamo.Ioc.Web.Tests.Mocks
{
	public class FakeHttpResponse : HttpResponseBase
	{
		private StringBuilder sb = new StringBuilder();

		public override void Write(string s)
		{
			sb.Append(s);
		}

		public override string ToString()
		{
			return sb.ToString();
		}
	}
}