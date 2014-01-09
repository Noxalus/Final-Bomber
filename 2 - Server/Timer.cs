using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FBServer
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

        public Timer(bool start)
        {
            timer = new Stopwatch();
            if (start)
                Start();
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
            if (isStarted)
            {
                if (timer.ElapsedMilliseconds > time)
                {
                    Reset();
                    ticks++;
                    return true;
                }
            }
            return false;
        }
    }
}
