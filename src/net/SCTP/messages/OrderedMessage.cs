﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using SCTP4CS.Utils;
using SIPSorcery.Net.Sctp;
using SIPSorcery.Sys;

namespace SIPSorcery.net.SCTP.messages
{
    public class OrderedMessage
    {
        private static ILogger logger = Log.Logger;
        private ConcurrentDictionary<uint, DataChunk> Chunks = new ConcurrentDictionary<uint, DataChunk>();
        private DataChunk start;
        private DataChunk end;
        public int Number;
        public OrderedMessage(int num)
        {
            this.Number = num;
        }
        public OrderedMessage Add(DataChunk dc, int type)
        {
            switch (type)
            {
                case DataChunk.ENDFLAG:
                    //logger.LogDebug($"finished message no{this.Number} with  {dc.getTsn()}");
                    end = dc;
                    break;
                case DataChunk.BEGINFLAG:
                    start = dc;
                    //logger.LogDebug($"new message no{this.Number} starts with  {dc.getTsn()}");
                    break;
                default:
                    //logger.LogDebug($"continued message no{this.Number} with  {dc.getTsn()}");
                    break;
            }

            Chunks.AddOrUpdate(dc.getTsn(), dc, (a, b) =>
            {
                if (b != null)
                {
                    //logger.LogDebug($"duplicate message{this.Number} with  {b.getTsn()}");
                }
                return dc;
            });
            return this;
        }

        public void AddToList(List<DataChunk> array)
        {
            array.AddRange(Chunks.Values);
        }

        public bool IsReady
        {
            get
            {
                if (start == null || end == null)
                {
                    return false;
                }

                for (uint i = start.getTsn(); i <= end?.getTsn(); i++)
                {
                    if (!Chunks.ContainsKey(i))
                    {
                        return false;
                    }
                }

                logger.LogDebug($"isready message no{this.Number}");

                return true;
            }
        }

        public SortedArray<DataChunk> ToArray()
        {
            var list = new SortedArray<DataChunk>();
            for (uint i = start.getTsn(); i <= end?.getTsn(); i++)
            {
                list.Add(Chunks[i]);
            }
            return list;
        }
    }
}
