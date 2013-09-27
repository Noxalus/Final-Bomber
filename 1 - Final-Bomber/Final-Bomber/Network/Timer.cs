using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Final_Bomber.Network
{
    public class Timer
    {
        int ticks = 0;
        Stopwatch timer;
        bool isStarted = false;

        public bool IsStarted
        {
            get { return isStarted; }
        }

        public Timer()
        {
            timer = new Stopwatch();
        }

        public void Start()
        {
            if (!isStarted)
            {
                timer.Start();
                isStarted = true;
            }
        }

        public void Stop()
        {
            if (isStarted)
            {
                timer.Stop();
                isStarted = false;
            }
        }

        public void Reset()
        {
            if (isStarted)
            {
                timer.Reset();
                timer.Start();
            }
        }

        public long ElapsedMilliseconds
        {
            get { return timer.ElapsedMilliseconds; }
        }

        public bool Each(int time)
        {
            if (timer.ElapsedMilliseconds > time)
            {
                timer.Reset();
                timer.Start();
                ticks++;
                return true;
            }
            return false;
        }
    }
}
