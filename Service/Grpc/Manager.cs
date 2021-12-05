using Grpc.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using Srv = Grpc.Core.Server;

namespace Service.Grpc
{
    public class Manager : ServiceManagerBase
    {
        Srv _server = null;

        public int ServicePort
        {
            get
            {
                if (_server != null)
                {
                    var p = _server.Ports.First();
                    return p.BoundPort;
                }
                else
                {
                    return -1;
                }
            }
        }

        public override void Dispose()
        {
            if (_server != null)
                _server.ShutdownAsync().Wait();
        }

        public override async Task<ServiceInfo> Up()
        {
            var t = await Task.Run(() =>
            {
                _server = new Srv()
                {
                    Ports = { new ServerPort("127.0.0.1", Network.GetAvailablePort(), ServerCredentials.Insecure) },
                    Services = { GameGrpcServiceV1.TicTacToeSrv.BindService(new Server()) }
                };

                try
                {
                    _server.Start();
                }
                catch (Exception ex)
                {
                    return new ServiceInfo()
                    {
                        Success = false,
                        Message = "Something has gone wrong. Exception details:" +
                        $"{Environment.NewLine}{ex}"
                    };
                }
                return new ServiceInfo()
                {
                    Success = true,
                    Message = $"Grpc service has been started successfully!"
                };
            });

            return t;
        }
    }
}
