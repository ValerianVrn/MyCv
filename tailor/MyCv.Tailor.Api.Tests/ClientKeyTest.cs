using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;
using System.Security.Claims;

namespace MyCv.Tailor.Api.Tests
{
    [TestClass]
    public sealed class ClientKeyTest
    {
        private sealed class FakeRequest(FunctionContext context, HttpHeadersCollection headers) : HttpRequestData(context)
        {
            public override Stream Body { get; } = new MemoryStream();
            public override HttpHeadersCollection Headers { get; } = headers;
            public override IReadOnlyCollection<IHttpCookie> Cookies { get; } = [];
            public override Uri Url { get; } = new("https://localhost/api/tailor");
            public override IEnumerable<ClaimsIdentity> Identities { get; } = [];
            public override string Method { get; } = "POST";
            public override HttpResponseData CreateResponse() => throw new NotSupportedException();
        }

        [TestMethod]
        [DataRow("1.2.3.4:5678", "1.2.3.4")]
        [DataRow("1.2.3.4", "1.2.3.4")]
        [DataRow("1.2.3.4:5678, 10.0.0.1", "1.2.3.4")]
        [DataRow("[2001:db8::1]:443", "2001:db8::1")]
        [DataRow("2001:db8::1", "2001:db8::1")]
        public void GetClientKeyParsesForwardedFor(string header, string expected)
        {
            var headers = new HttpHeadersCollection { { "X-Forwarded-For", header } };
            var request = new FakeRequest(new Mock<FunctionContext>().Object, headers);

            Assert.AreEqual(expected, TailorFunction.GetClientKey(request));
        }

        [TestMethod]
        public void GetClientKeyWithoutHeaderReturnsUnknown()
        {
            var request = new FakeRequest(new Mock<FunctionContext>().Object, []);

            Assert.AreEqual("unknown", TailorFunction.GetClientKey(request));
        }
    }
}
