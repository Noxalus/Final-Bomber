using System;
using System.Diagnostics;

namespace FBClient.Network
{
    public class Timer
    {
        readonly Stopwatch _timer;
        bool _isStarted = false;

        private int Ticks { get; set; }

        public bool IsStarted
        {
            get { return _isStarted; }
        }

        public Timer()
        {
            Ticks = 0;
            _timer = new Stopwatch();
        }

        public void Start()
        {
            if (!_isStarted)
            {
                _timer.Start();
                _isStarted = true;
            }
        }

        public void Stop()
        {
            if (_isStarted)
            {
                _timer.Stop();
                _isStarted = false;
            }
        }

        public void Reset()
        {
            if (_isStarted)
            {
                _timer.Reset();
                _timer.Start();
            }
        }

        public long ElapsedMilliseconds
        {
            get { return _timer.ElapsedMilliseconds; }
        }

        public bool Each(TimeSpan timeSpan)
        {
            if (_timer.ElapsedMilliseconds > timeSpan.TotalMilliseconds)
            {
                Reset();

                Ticks++;

                return true;
            }

            return false;
        }

        public bool Each(int time)
        {
            if (_timer.ElapsedMilliseconds > time)
            {
                Reset();

                Ticks++;
                
                return true;
            }

            return false;
        }
    }
}
