using System;
using System.Collections.Specialized;
using System.Web;

namespace Dynamo.Ioc.Web.Tests.Mocks
{
	public class FakeHttpRequest : HttpRequestBase
	{
		private readonly string _relativeUrl;
		private readonly NameValueCollection _formParams;
		private readonly NameValueCollection _queryStringParams;
		private readonly HttpCookieCollection _cookies;

		public FakeHttpRequest(string relativeUrl, NameValueCollection formParams, NameValueCollection queryStringParams, HttpCookieCollection cookies)
		{
			_relativeUrl = relativeUrl;
			_formParams = formParams;
			_queryStringParams = queryStringParams;
			_cookies = cookies;
		}

		public override NameValueCollection Form
		{
			get
			{
				return _formParams;
			}
		}

		public override NameValueCollection QueryString
		{
			get
			{
				return _queryStringParams;
			}
		}

		public override HttpCookieCollection Cookies
		{
			get
			{
				return _cookies;
			}
		}

		public override string AppRelativeCurrentExecutionFilePath
		{
			get { return _relativeUrl; }
		}

		public override string PathInfo
		{
			get { return String.Empty; }
		}
	}
}