using FBLibrary.Core;
using Final_BomberServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_BomberServer.Host
{
    partial class GameServer
    {
        void DataProcessing(byte data, ref Client client)
        {
            switch (data)
            {
                case (byte)RMT.NeedMap:
                    ReceiveNeedMap(client);
                    break;
                case (byte)RMT.Ready:
                    Program.Log.Info("[Client #" + client.ClientId + "]Ready !");
                    ReceiveReady(client, buffer.ReadString(), buffer.ReadString());
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
