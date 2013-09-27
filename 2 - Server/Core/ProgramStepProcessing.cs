using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_BomberServer.Core
{
    partial class GameServerHandler
    {
        //H.U.D.Timer t_couldntConnect = new H.U.D.Timer(false);

        private void ProgramStepProccesing()
        {
            ConnectedGameProccesing();
        }

        private void ConnectedGameProccesing()
        {
            if (!game.HasStarted)
            {
                game.Initialize();
            }
        }
    }
}
