using DockerSamples.WebApplicationFirewall.App.AttackCheck;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace DockerSamples.WebApplicationFirewall.App
{
    public class ProxyController
    {
        private readonly IEnumerable<IAttackCheck> _attackChecks;
        private readonly IConfigurationRoot _config;
        private readonly ILogger<ProxyController> _logger; 
        private readonly ProxyServer _server = new ProxyServer();

        public ProxyController(IEnumerable<IAttackCheck> attackChecks, IConfigurationRoot config, ILogger<ProxyController> logger)
        {
            _attackChecks = attackChecks;
            _config = config;
            _logger = logger;
            _server.ExceptionFunc = ex => _logger.LogError(ex, "Proxy server errored");
            _server.ForwardToUpstreamGateway = true;
        }

        public void StartProxy()
        {
            var listenPort = _config["Proxy:ListenPort"];
            var targetServer = _config["Proxy:TargetServer"];
            var targetPort = _config["Proxy:TargetPort"];
            _logger.LogInformation($"Proxy config - listen port: {listenPort}, target server: {targetServer}, targetPort: {targetPort}");
                        
            var endPoint = new TransparentProxyEndPoint(IPAddress.Any, int.Parse(listenPort), false);            
            _server.AddEndPoint(endPoint);
            _server.BeforeRequest += OnRequest;
            _server.UpStreamHttpProxy = new ExternalProxy()
            {
                HostName = targetServer,
                Port = int.Parse(targetPort)
            };

            _server.Start();
            _logger.LogInformation("Proxy server listening");
        }

        public void Stop()
        {
            _server.BeforeRequest -= OnRequest;
            _server.Stop();

            _logger.LogInformation("Proxy stopped");
        }
        
        public async Task OnRequest(object sender, SessionEventArgs e)
        {
            _logger.LogDebug($"Proxying request URL: {e.WebSession.Request.Url}");

            foreach(var attackCheck in _attackChecks)
            {
                if (await attackCheck.IsSuspect(e))
                {
                    await attackCheck.BlockRequest(e);
                    _logger.LogWarning($"** Blocked potential attack, type: {attackCheck.AttackName} **");
                }
            }
        }
    }
}
