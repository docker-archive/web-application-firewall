using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;

namespace DockerSamples.WebApplicationFirewall.App.AttackCheck
{
    public interface IAttackCheck
    {
        string AttackName { get; }

        bool IsSuspect(Request incomingRequest);

        Task BlockRequest(SessionEventArgs session);
    }
}
