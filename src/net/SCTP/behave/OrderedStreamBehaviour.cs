/*
 * Copyright 2017 pi.pe gmbh .
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
// Modified by Andrés Leone Gámez

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using SCTP4CS.Utils;
using SIPSorcery.Sys;

/**
 *
 * @author tim
 */
namespace SIPSorcery.Net.Sctp
{
    internal class OrderedStreamBehaviour : SCTPStreamBehaviour
    {
        private ConcurrentDictionary<int, PacketOrder> queue = new ConcurrentDictionary<int, PacketOrder>();
        private static ILogger logger = Log.Logger;

        protected bool _ordered = true;

        public void deliver(SCTPStream s, SortedArray<DataChunk> stash, SCTPStreamListener l)
        {
            //stash is the list of all DataChunks that have not yet been turned into whole messages
            //we assume that it is sorted by stream sequence number.
            List<DataChunk> delivered = new List<DataChunk>();
            PacketOrder message = null;
            if (stash.Count == 0)
            {
                return; // I'm not fond of these early returns 
            }
            long expectedTsn = stash.First.getTsn(); // This ignores gaps - but _hopefully_ messageNo will catch any
                                                     // gaps we care about - ie gaps in the sequence for _this_ stream 
                                                     // we can deliver ordered messages on this stream even if earlier messages from other streams are missing
                                                     // - this does assume that the tsn's of a message are contiguous -which is odd.


            foreach (DataChunk dc in stash)
            {
                int messageNo = s.getNextMessageSeqIn();

                int flags = dc.getFlags() & DataChunk.SINGLEFLAG; // mask to the bits we want
                long tsn = dc.getTsn();
                bool lookingForOrderedMessages = _ordered || (message != null);
                // which is to say for unordered messages we can tolerate gaps _between_ messages
                // but not within them
                //if (lookingForOrderedMessages && (tsn != expectedTsn))
                //{
                //    logger.LogDebug("Hole in chunk sequence  " + tsn + " expected " + expectedTsn);
                //    break;
                //}
                switch (flags)
                {
                    case DataChunk.SINGLEFLAG:
                        // singles are easy - just dispatch.
                        if (_ordered && (messageNo != dc.getSSeqNo()))
                        {
                            logger.LogDebug("Hole (single) in message sequence  " + dc.getSSeqNo() + " expected " + messageNo);
                            break; // not the message we are looking for...
                        }
                        SCTPMessage single = new SCTPMessage(s, dc);
                        if (single.deliver(l))
                        {
                            delivered.Add(dc);
                            messageNo++;
                            s.setNextMessageSeqIn(messageNo);
                        }
                        break;
                    case DataChunk.BEGINFLAG:
                        //if (_ordered && (messageNo != dc.getSSeqNo()))
                        //{
                        //    logger.LogDebug("Hole (begin) in message sequence  " + dc.getSSeqNo() + " expected " + messageNo);
                        //    break; // not the message we are looking for...
                        //}
                        message = GetPacketOrder(dc.getSSeqNo());
                        message.Add(dc, flags);
                        //logger.LogDebug("new message no" + dc.getSSeqNo() + " starts with  " + dc.getTsn());
                        if (message.IsReady)
                        {
                            messageNo = FinishMessage(s, l, message, dc, messageNo);
                        }
                        break;
                    case DataChunk.CONTINUEFLAG: // middle 
                        message = GetPacketOrder(dc.getSSeqNo());
                        message.Add(dc, flags);
                        //logger.LogDebug("continued message no" + dc.getSSeqNo() + " with  " + dc.getTsn());
                        if (message.IsReady)
                        {
                            messageNo = FinishMessage(s, l, message, dc, messageNo);
                        }
                        break;
                    case DataChunk.ENDFLAG:
                        message = GetPacketOrder(dc.getSSeqNo());
                        message.Add(dc, flags);
                        if (message.IsReady)
                        {
                            messageNo = FinishMessage(s, l, message, dc, messageNo);
                            message.AddToList(delivered);
                        }
                        break;
                    default:
                        throw new Exception("[IllegalStateException] Impossible value in stream logic");
                }
                expectedTsn = tsn + 1;
            }
            stash.RemoveWhere((dc) => { return delivered.Contains(dc); });
        }

        private int FinishMessage(SCTPStream s, SCTPStreamListener l, PacketOrder message, DataChunk dc, int messageNo)
        {
            //logger.LogDebug("finished message no" + dc.getSSeqNo() + " with  " + dc.getTsn());
            SCTPMessage deliverable = new SCTPMessage(s, message.ToArray());
            if (deliverable.deliver(l))
            {
                queue.TryRemove(message.Number, out var val);
                //message.AddToList(delivered);
                messageNo++;
                s.setNextMessageSeqIn(messageNo);
            }

            return messageNo;
        }

        public Chunk[] respond(SCTPStream a)
        {
            return null;
        }

        public PacketOrder GetPacketOrder(int num)
        {
            if (!queue.TryGetValue(num, out var message))
            {
                lock (queue)
                {
                    if (!queue.TryGetValue(num, out message))
                    {
                        message = new PacketOrder(num);
                        queue.AddOrUpdate(num, message, (a, b) => message);
                    }
                }
            }
            return message;
        }
    }

    public class PacketOrder
    {
        public ConcurrentDictionary<uint, DataChunk> Chunks = new ConcurrentDictionary<uint, DataChunk>();
        public bool HasStart { get; set; }
        public bool HasEnd { get; set; }
        private DataChunk start;
        private DataChunk end;
        public int Number;
        public PacketOrder(int num)
        {
            this.Number = num;
        }
        public void Add(DataChunk chunk, int type)
        {
            switch(type)
            {
                case DataChunk.ENDFLAG:
                    HasEnd = true;
                    end = chunk;
                    break;
                case DataChunk.BEGINFLAG:
                    HasStart = true;
                    start = chunk;
                    break;
            }

            Chunks.AddOrUpdate(chunk.getTsn(), chunk, (a,b) => chunk);
        }

        public void AddToList(List<DataChunk> array)
        {
            array.AddRange(Chunks.Values);
        }

        public bool IsReady
        {
            get
            {
                if (!HasStart)
                {
                    return false;
                }
                if (!HasEnd)
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
