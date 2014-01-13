using FBLibrary.Core;
using Lidgren.Network;

namespace FBServer.Host
{
    sealed partial class GameServer
    {
        void DataProcessing(NetIncomingMessage message, ref Client client)
        {
            switch (message.ReadByte())
            {
                case (byte)RMT.NeedMap:
                    Program.Log.Info("[Client #" + client.ClientId + "] Need map !");
                    ReceiveNeedMap(client);
                    break;
                case (byte)RMT.Ready:
                    Program.Log.Info("[Client #" + client.ClientId + "] Is ready !");
                    ReceiveReady(client, message.ReadString(), message.ReadString());
                    break;
                case (byte)RMT.MoveDown:
                    Program.Log.Info("[Client #" + client.ClientId + "] Want to move down !");
                    ReceiveMovePlayer(client, LookDirection.Down);
                    break;
                case (byte)RMT.MoveLeft:
                    Program.Log.Info("[Client #" + client.ClientId + "] Want to move left !");
                    ReceiveMovePlayer(client, LookDirection.Left);
                    break;
                case (byte)RMT.MoveRight:
                    Program.Log.Info("[Client #" + client.ClientId + "] Want to move right !");
                    ReceiveMovePlayer(client, LookDirection.Right);
                    break;
                case (byte)RMT.MoveUp:
                    Program.Log.Info("[Client #" + client.ClientId + "] Want to move up !");
                    ReceiveMovePlayer(client, LookDirection.Up);
                    break;
                case (byte)RMT.Standing:
                    Program.Log.Info("[Client #" + client.ClientId + "] Want to stay here !");
                    ReceiveMovePlayer(client, LookDirection.Idle);
                    break;
                case (byte)RMT.PlaceBomb:
                    Program.Log.Info("[Client #" + client.ClientId + "] Want to place bomb !");
                    ReceiveBombPlacing(client);
                    break;
            }
        }
    }
}
