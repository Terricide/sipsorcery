//-----------------------------------------------------------------------------
// Filename: PipedMemoryStream.cs
//
// Description: An in memory stream that pipes data from a sender to receiver.
//
// Author(s):
// Aaron Clauson (aaron@sipsorcery.com)
// 
// History:
// 04 Jul 2020	Aaron Clauson	Created.
//
// License: 
// BSD 3-Clause "New" or "Revised" License, see included LICENSE.md file.
//-----------------------------------------------------------------------------

using System.IO;
using System.Threading;

namespace SIPSorcery.Sys
{
    internal class PipedMemoryStream
    {
        private object thisLock = new object();
        private readonly MemoryStream _ms = new MemoryStream();
        private long _writePos = 0;
        private long _readPos = 0;
        private bool _isClosed = false;

        internal PipedMemoryStream()
        { }

        public void Close()
        {
            lock (thisLock)
            {
                _isClosed = true;
                Monitor.PulseAll(thisLock);
            }
        }

        public int Read(byte[] buffer, int offset, int count, int timeout)
        {
            lock (thisLock)
            {
                if (WaitForData(timeout))
                {
                    if (_ms.Position != _readPos)
                    {
                        _ms.Seek(_readPos, SeekOrigin.Begin);
                    }

                    int len = (int)System.Math.Min(count, _writePos - _readPos);
                    int bytesRead = _ms.Read(buffer, offset, len);
                    _readPos += bytesRead;
                    return bytesRead;
                }
                else
                {
                    return 0;
                }
            }
        }

        public void Write(byte[] buf, int off, int len)
        {
            lock (thisLock)
            {
                if (_ms.Position != _writePos)
                {
                    _ms.Seek(_writePos, SeekOrigin.Begin);
                }

                _ms.Write(buf, off, len);
                _writePos += len;

                Monitor.PulseAll(thisLock);
            }
        }

        private bool WaitForData(int timeout)
        {
            if (_readPos >= _writePos && !_isClosed)
            {
                return Monitor.Wait(thisLock, timeout);
            }
            else
            {
                return !_isClosed;
            }
        }
    }
}
