using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Final_BomberNetwork.GameServer
{
    public class Timer
    {
        int tmr;
        int ticks = 0;

        public Timer()
        {
            tmr = Environment.TickCount;
        }

        public void Reset()
        {
            tmr = Environment.TickCount;
        }

        public bool Each(int time)
        {
            int timeChecker = Environment.TickCount - tmr;
            if (timeChecker > time)
            {
                tmr = Environment.TickCount;
                ticks++;
                return true;
            }
            return false;
        }
    }
}
