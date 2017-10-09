using System.Net;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;

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

        public async Task<bool> IsSuspect(SessionEventArgs session)
        {
            if (session.WebSession.Request.Method == "POST" && session.WebSession.Request.HasBody)
            {
                var body = await session.GetRequestBodyAsString();
                body = WebUtility.UrlDecode(body);
                //basic check for SQL injection string:
                if (body.Contains("--") || body.Contains("/*"))
                {
                    return true;
                }
            }
            return false;
        }

        public async Task BlockRequest(SessionEventArgs session)
        {
            await session.GenericResponse(BLOCK_HTML, HttpStatusCode.BadRequest);
        }
    }
}
