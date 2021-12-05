using Grpc.Core;
using SrvClient = GameGrpcServiceV1.TicTacToeSrv.TicTacToeSrvClient;

namespace Client.Library
{
    public class Manager
    {
        DateTime Timeout { get { return DateTime.UtcNow.AddSeconds(2); } }

        Channel? _channel;
        SrvClient? _client;

        public  async Task<SrvClient?> SetConnection(string host, int port)
        {
            try
            {
                try
                {
                    _channel = new Channel(host, port, ChannelCredentials.Insecure);
                    await _channel.ConnectAsync(Timeout);
                }
                catch (Exception)
                {
                    _channel = new Channel("127.0.0.1", port, ChannelCredentials.Insecure);
                    await _channel.ConnectAsync(Timeout);
                }
                
                _client = new SrvClient(_channel);
                return _client;
            }
            catch (Exception)
            { return null; }
        }
    }
}
