using GameGrpcServiceV1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Grpc
{
    internal class Server : TicTacToeSrv.TicTacToeSrvBase
    {
        static Storage Storage { get; set; }

        public Server()
        {
            Storage = new Storage();
        }

        public override Task<Response> CreateGame(Game request, ServerCallContext context)
        {
            if (request == null)
            {
                return Task.FromResult(
                    new Response()
                    {
                        Status = Response.Types.call_status.Err,
                        Message = "Request parameter can not be null"
                    }
                );
            }

            Storage.GameDict.Add(request.Id, request);
            Log.Prt($"CreateGame: {request.Id}", Log.LogType.Message);
            return Task.FromResult(new Response() { Status = Response.Types.call_status.Ok });
        }

        public override Task<Games> GetAllGames(Empty request, ServerCallContext context)
        {
            try
            {
                Games r = new Games();
                var games = Storage.GameDict.Values.Where(x => x.Status == Game.Types.GameStatus.WaitingForOpponent);
                r.GameList.AddRange(games);
                Log.Prt($"GetAllGames: WaitingForOpponent: Count: {games.Count()}", Log.LogType.Message);
                return Task.FromResult(r);
            }
            catch (Exception ex)
            {
                throw new RpcException(Status.DefaultCancelled, ex.ToString());
            }
        }

        public override async Task GameStream(IAsyncStreamReader<GameStreamRequest> requestStream, IServerStreamWriter<Game> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var req = requestStream.Current;
                if (Storage.GameDict.TryGetValue(req.Id, out var game))
                {
                    game.DateTime = DateTime.UtcNow.ToString();
                    await responseStream.WriteAsync(game);
                }
            }
        }

        public override Task<Response> RemoveGame(Game request, ServerCallContext context)
        {
            if (Storage.GameDict.TryGetValue(request.Id, out var game))
            {
                Storage.GameDict.Remove(request.Id);
                Log.Prt($"RemoveGame: {request.Id}", Log.LogType.Message);
                return Task.FromResult(new Response() { Status = Response.Types.call_status.Ok });
            }

            var msg = $"RemoveGame: {request.Id}: There is no such a game matching with given ID to remove";
            Log.Prt(msg, Log.LogType.Error);
            return Task.FromResult(new Response() { Status = Response.Types.call_status.Err, Message = msg });
        }

        public override Task<Response> UpdateGame(Game request, ServerCallContext context)
        {
            if (Storage.GameDict.TryGetValue(request.Id, out var game))
            {
                request.Turn = SetTurn(request);

                var n = 3; // n x n board
                var totalStep = request.GameSteps.Circle.Count + request.GameSteps.Cross.Count;
                request.WinningPlayer = SetWinningPlayer(request, n);

                if (totalStep == n * n || request.WinningPlayer != Game.Types.Winner.Draw)
                {
                    request.Status = Game.Types.GameStatus.Completed;
                    Log.Prt($"UpdateGame: {request.Id}: Game is over", Log.LogType.Message);
                }

                Storage.GameDict[request.Id] = request;
                Log.Prt($"UpdateGame: {request.Id}", Log.LogType.Message);
                return Task.FromResult(new Response() { Status = Response.Types.call_status.Ok });
            }

            var msg = $"UpdateGame: {request.Id}: Couldn't found available game in storage to update";
            Log.Prt(msg, Log.LogType.Message);
            return Task.FromResult(new Response() { Status = Response.Types.call_status.Err, Message = msg });
        }

        private Game.Types.Shape SetTurn(Game game)
        {
            if (game.GameSteps.Circle.Count > game.GameSteps.Cross.Count)
                return Game.Types.Shape.Cross;
            else
                return Game.Types.Shape.Circle;
        }

        /// <summary>Give relaviant <see cref="Game"/> and board size as (n x n)</summary>
        /// <param name="n">set size for board (n x n)</param>
        private Game.Types.Winner SetWinningPlayer(Game game, int n)
        {
            var arr = new uint[n][];
            var cmpArr1 = (new uint[n]).Select(x => (uint)1).ToArray();
            var cmpArr2 = (new uint[n]).Select(x => (uint)2).ToArray();

            #region FILL JAGGED ARRAY
            var c = (uint)1;
            for (int i = 0; i < n; i++)
            {
                var sub = new uint[n];
                for (int j = 0; j < n; j++)
                {
                    if (game.GameSteps.Circle.Contains(c))
                        sub[j] = 1;
                    else if (game.GameSteps.Cross.Contains(c))
                        sub[j] = 2;
                    else
                        sub[j] = 0;
                    ++c;
                }
                arr[i] = sub;
            }
            #endregion

            #region CHECKING ROW BY ROW
            for (int i = 0; i < n; i++)
            {
                if (arr[i].SequenceEqual(cmpArr1))
                    return Game.Types.Winner.PlayerCircle;
                if (arr[i].SequenceEqual(cmpArr2))
                    return Game.Types.Winner.PlayerCross;
            }
            #endregion

            #region CHECKING COLUMN BY COLUMN
            for (int i = 0; i < n; i++)
            {
                var sub = new uint[n];
                for (int j = 0; j < n; j++)
                    sub[j] = arr[j][i];

                if (sub.SequenceEqual(cmpArr1))
                    return Game.Types.Winner.PlayerCircle;

                if (sub.SequenceEqual(cmpArr2))
                    return Game.Types.Winner.PlayerCross;
            }
            #endregion

            #region CHECKING BY DIAGONALS
            var diagSub = new uint[n];
            for (int i = 0; i < n; i++)
                diagSub[i] = arr[i][i];

            if (diagSub.SequenceEqual(cmpArr1))
                return Game.Types.Winner.PlayerCircle;

            if (diagSub.SequenceEqual(cmpArr2))
                return Game.Types.Winner.PlayerCross;

            var k = -1;
            diagSub = new uint[n];
            var row = -1;
            var clm = n;
            while (k <= n)
                diagSub[++k] = arr[++row][--n];

            if (diagSub.SequenceEqual(cmpArr1))
                return Game.Types.Winner.PlayerCircle;

            if (diagSub.SequenceEqual(cmpArr2))
                return Game.Types.Winner.PlayerCross;
            #endregion

            return Game.Types.Winner.Draw;
        }
    }
}
