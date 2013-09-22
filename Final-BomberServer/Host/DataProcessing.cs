using FBLibrary.Core;
using Lidgren.Network;

namespace Final_BomberServer.Host
{
    partial class GameServer
    {
        void DataProcessing(NetIncomingMessage message, ref Client client)
        {
            switch (message.ReadByte())
            {
                case (byte)RMT.NeedMap:
                    ReceiveNeedMap(client);
                    break;
                case (byte)RMT.Ready:
                    Program.Log.Info("[Client #" + client.ClientId + "]Ready !");
                    ReceiveReady(client, message.ReadString(), message.ReadString());
                    break;
                case (byte)RMT.MoveDown:
                    Program.Log.Info("[Client #" + client.ClientId + "]Move down !");
                    ReceiveMovePlayer(client, LookDirection.Down);
                    break;
                case (byte)RMT.MoveLeft:
                    Program.Log.Info("[Client #" + client.ClientId + "]Move left !");
                    ReceiveMovePlayer(client, LookDirection.Left);
                    break;
                case (byte)RMT.MoveRight:
                    Program.Log.Info("[Client #" + client.ClientId + "]Move right !");
                    ReceiveMovePlayer(client, LookDirection.Right);
                    break;
                case (byte)RMT.MoveUp:
                    Program.Log.Info("[Client #" + client.ClientId + "]Move up !");
                    ReceiveMovePlayer(client, LookDirection.Up);
                    break;
                case (byte)RMT.Standing:
                    Program.Log.Info("[Client #" + client.ClientId + "]Standing !");
                    ReceiveMovePlayer(client, LookDirection.Idle);
                    break;
                case (byte)RMT.PlaceBomb:
                    ReceiveBombPlacing(client);
                    break;

            }
        }
    }
}
