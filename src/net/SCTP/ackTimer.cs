﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SIPSorcery.Net.Sctp
{
    public class ackTimer
    {
        private static readonly TimeSpan ackInterval = TimeSpan.FromMilliseconds(200);

        private IAckTimerObserver _ackTimerObserver;
        private TimeSpan interval;
        private System.Timers.Timer _timer;
        // newAckTimer creates a new acknowledgement timer used to enable delayed ack.
        public static ackTimer newAckTimer(IAckTimerObserver observer)
        {
            return new ackTimer()
            {
                _ackTimerObserver = observer,
                interval = ackInterval
            };
        }
        public void start()
        {
            _timer = new System.Timers.Timer();
            _timer.AutoReset = true;
            _timer.Interval = interval.TotalMilliseconds;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _ackTimerObserver?.onAckTimeout();
        }

        public void stop()
        {
            _timer?.Dispose();
            _timer = null;
            _ackTimerObserver?.onAckTimeout();
        }

        public void close()
        {
            stop();
        }

        public bool isRunning()
        {
            return _timer == null;
        }
    }

    // ackTimerObserver is the inteface to an ack timer observer.
    public interface IAckTimerObserver
    {
        void onAckTimeout();
    }
}
