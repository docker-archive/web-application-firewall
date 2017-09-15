using System.Net;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;

namespace DockerSamples.WebApplicationFirewall.App.AttackCheck
{
    public class SqlInjectionAttackCheck : IAttackCheck
    {
        private const string BLOCK_HTML = "<!DOCTYPE html>" +
                      "<html><body><h1>" +
                      "No you don't!" +
                      "</h1>" +
                      "<h2>Go try your SQL injection attack somewhere else, chummy.</h2>" +
                      "</body>" +
                      "</html>";

        public string AttackName
        {
            get { return "SQL Injection"; }
        }

        public bool IsSuspect(Request request)
        {
            //TODO - rule implementation, POST type and values contain SQL operators
            return request.RequestUri.PathAndQuery.Contains("sql");
        }

        public async Task BlockRequest(SessionEventArgs session)
        {
            await session.GenericResponse(BLOCK_HTML, HttpStatusCode.BadRequest);
        }
    }
}
