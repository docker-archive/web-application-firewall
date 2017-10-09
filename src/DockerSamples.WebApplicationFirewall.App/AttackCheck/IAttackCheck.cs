using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;

namespace DockerSamples.WebApplicationFirewall.App.AttackCheck
{
    public interface IAttackCheck
    {
        string AttackName { get; }

        Task<bool> IsSuspect(SessionEventArgs session);

        Task BlockRequest(SessionEventArgs session);
    }
}
