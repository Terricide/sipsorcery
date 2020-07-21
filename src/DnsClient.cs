using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DnsClient;
using DnsClient.Protocol;

namespace DnsClient
{
    public class LookupClient
    {
        public LookupClient(LookupClientOptions options)
        {

        }

        public LookupClient()
        {
        }

        internal async Task<ServiceHostEntry[]> ResolveServiceAsync(string host, string v1, string v2)
        {
            return null;
        }

        internal IDnsQueryResponse Query(string host, QueryType queryType)
        {
            return null;
        }

        internal async Task<IDnsQueryResponse> QueryAsync(string address, QueryType a)
        {
            throw new NotImplementedException();
        }
    }

    public class ServiceHostEntry
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public int Priority { get; set; }
        public int Weight { get; set; }
    }


    public interface IDnsQueryResponse
    {
        IReadOnlyList<DnsResourceRecord> Answers
        {
            get;
        }
    }

    public class DnsQueryResponse : IDnsQueryResponse
    {
        public IReadOnlyList<DnsResourceRecord> Answers { get; }
    }
   
    public class DnsResourceRecord
    {

    }

    public class AddressRecord : DnsResourceRecord
    {
        public IPAddress Address { get; set; }
    }

    public class AaaaRecord : AddressRecord
    {
        
    }

    public class ARecord : AddressRecord
    {

    }

    public class LookupClientOptions
    {
        public int Retries;
        public TimeSpan Timeout;
        public bool UseCache;
    }

    public enum QueryType
    {
        A,
        AAAA
    }
}

namespace DnsClient.Protocol
{
    
}

namespace System.Linq
{
    public static class RecordCollectionExtension
    {
        //
        // Summary:
        //     Filters the elements of an System.Collections.Generic.IEnumerable`1 to return
        //     DnsClient.Protocol.ARecords only.
        //
        // Parameters:
        //   records:
        //     The records.
        //
        // Returns:
        //     The list of DnsClient.Protocol.ARecord.
        public static IEnumerable<AddressRecord> AddressRecords(this IEnumerable<DnsResourceRecord> records)
        {
            return records.OfType<AddressRecord>();
        }

        //
        // Summary:
        //     Filters the elements of an System.Collections.Generic.IEnumerable`1 to return
        //     DnsClient.Protocol.ARecords only.
        //
        // Parameters:
        //   records:
        //     The records.
        //
        // Returns:
        //     The list of DnsClient.Protocol.ARecord.
        public static IEnumerable<ARecord> ARecords(this IEnumerable<DnsResourceRecord> records)
        {
            return records.OfType<ARecord>();
        }

        //
        // Summary:
        //     Filters the elements of an System.Collections.Generic.IEnumerable`1 to return
        //     DnsClient.Protocol.NsRecords only.
        //
        // Parameters:
        //   records:
        //     The records.
        //
        // Returns:
        ////     The list of DnsClient.Protocol.NsRecord.
        //public static IEnumerable<NsRecord> NsRecords(this IEnumerable<DnsResourceRecord> records)
        //{
        //    return records.OfType<NsRecord>();
        //}

        ////
        //// Summary:
        ////     Filters the elements of an System.Collections.Generic.IEnumerable`1 to return
        ////     DnsClient.Protocol.CNameRecords only.
        ////
        //// Parameters:
        ////   records:
        ////     The records.
        ////
        //// Returns:
        ////     The list of DnsClient.Protocol.CNameRecord.
        //public static IEnumerable<CNameRecord> CnameRecords(this IEnumerable<DnsResourceRecord> records)
        //{
        //    return records.OfType<CNameRecord>();
        //}

        ////
        //// Summary:
        ////     Filters the elements of an System.Collections.Generic.IEnumerable`1 to return
        ////     DnsClient.Protocol.SoaRecords only.
        ////
        //// Parameters:
        ////   records:
        ////     The records.
        ////
        //// Returns:
        ////     The list of DnsClient.Protocol.SoaRecord.
        //[CLSCompliant(false)]
        //public static IEnumerable<SoaRecord> SoaRecords(this IEnumerable<DnsResourceRecord> records)
        //{
        //    return records.OfType<SoaRecord>();
        //}

        //
        // Summary:
        //     Filters the elements of an System.Collections.Generic.IEnumerable`1 to return
        //     DnsClient.Protocol.MbRecords only.
        //
        // Parameters:
        //   records:
        //     The records.
        //
        // Returns:
        //     The list of DnsClient.Protocol.MbRecord.
        //public static IEnumerable<MbRecord> MbRecords(this IEnumerable<DnsResourceRecord> records)
        //{
        //    return records.OfType<MbRecord>();
        //}

        ////
        //// Summary:
        ////     Filters the elements of an System.Collections.Generic.IEnumerable`1 to return
        ////     DnsClient.Protocol.MgRecords only.
        ////
        //// Parameters:
        ////   records:
        ////     The records.
        ////
        //// Returns:
        ////     The list of DnsClient.Protocol.MgRecord.
        //public static IEnumerable<MgRecord> MgRecords(this IEnumerable<DnsResourceRecord> records)
        //{
        //    return records.OfType<MgRecord>();
        //}

        //
        // Summary:
        //     Filters the elements of an System.Collections.Generic.IEnumerable`1 to return
        //     DnsClient.Protocol.MrRecords only.
        //
        // Parameters:
        //   records:
        //     The records.
        //
        // Returns:
        //     The list of DnsClient.Protocol.MrRecord.
        //public static IEnumerable<MrRecord> MrRecords(this IEnumerable<DnsResourceRecord> records)
        //{
        //    return records.OfType<MrRecord>();
        //}

        //
        // Summary:
        //     Filters the elements of an System.Collections.Generic.IEnumerable`1 to return
        //     DnsClient.Protocol.NullRecords only.
        //
        // Parameters:
        //   records:
        //     The records.
        //
        // Returns:
        //     The list of DnsClient.Protocol.NullRecord.
        //public static IEnumerable<NullRecord> NullRecords(this IEnumerable<DnsResourceRecord> records)
        //{
        //    return records.OfType<NullRecord>();
        //}

        ////
        //// Summary:
        ////     Filters the elements of an System.Collections.Generic.IEnumerable`1 to return
        ////     DnsClient.Protocol.WksRecords only.
        ////
        //// Parameters:
        ////   records:
        ////     The records.
        ////
        //// Returns:
        ////     The list of DnsClient.Protocol.WksRecord.
        //public static IEnumerable<WksRecord> WksRecords(this IEnumerable<DnsResourceRecord> records)
        //{
        //    return records.OfType<WksRecord>();
        //}

        //
        // Summary:
        //     Filters the elements of an System.Collections.Generic.IEnumerable`1 to return
        //     DnsClient.Protocol.PtrRecords only.
        //
        // Parameters:
        //   records:
        //     The records.
        //
        // Returns:
        //     The list of DnsClient.Protocol.PtrRecord.
        //public static IEnumerable<PtrRecord> PtrRecords(this IEnumerable<DnsResourceRecord> records)
        //{
        //    return records.OfType<PtrRecord>();
        //}

        ////
        //// Summary:
        ////     Filters the elements of an System.Collections.Generic.IEnumerable`1 to return
        ////     DnsClient.Protocol.HInfoRecords only.
        ////
        //// Parameters:
        ////   records:
        ////     The records.
        ////
        //// Returns:
        ////     The list of DnsClient.Protocol.HInfoRecord.
        //public static IEnumerable<HInfoRecord> HInfoRecords(this IEnumerable<DnsResourceRecord> records)
        //{
        //    return records.OfType<HInfoRecord>();
        //}

        //
        // Summary:
        //     Filters the elements of an System.Collections.Generic.IEnumerable`1 to return
        //     DnsClient.Protocol.MxRecords only.
        //
        // Parameters:
        //   records:
        //     The records.
        //
        // Returns:
        //     The list of DnsClient.Protocol.MxRecord.
        //[CLSCompliant(false)]
        //public static IEnumerable<MxRecord> MxRecords(this IEnumerable<DnsResourceRecord> records)
        //{
        //    return records.OfType<MxRecord>();
        //}

        ////
        //// Summary:
        ////     Filters the elements of an System.Collections.Generic.IEnumerable`1 to return
        ////     DnsClient.Protocol.TxtRecords only.
        ////
        //// Parameters:
        ////   records:
        ////     The records.
        ////
        //// Returns:
        ////     The list of DnsClient.Protocol.TxtRecord.
        //public static IEnumerable<TxtRecord> TxtRecords(this IEnumerable<DnsResourceRecord> records)
        //{
        //    return records.OfType<TxtRecord>();
        //}

        //
        // Summary:
        //     Filters the elements of an System.Collections.Generic.IEnumerable`1 to return
        //     DnsClient.Protocol.RpRecords only.
        //
        // Parameters:
        //   records:
        //     The records.
        //
        // Returns:
        //     The list of DnsClient.Protocol.RpRecord.
        //public static IEnumerable<RpRecord> RpRecords(this IEnumerable<DnsResourceRecord> records)
        //{
        //    return records.OfType<RpRecord>();
        //}

        //
        // Summary:
        //     Filters the elements of an System.Collections.Generic.IEnumerable`1 to return
        //     DnsClient.Protocol.AfsDbRecords only.
        //
        // Parameters:
        //   records:
        //     The records.
        //
        // Returns:
        //     The list of DnsClient.Protocol.AfsDbRecord.
        //public static IEnumerable<AfsDbRecord> AfsDbRecords(this IEnumerable<DnsResourceRecord> records)
        //{
        //    return records.OfType<AfsDbRecord>();
        //}

        //
        // Summary:
        //     Filters the elements of an System.Collections.Generic.IEnumerable`1 to return
        //     DnsClient.Protocol.AaaaRecords only.
        //
        // Parameters:
        //   records:
        //     The records.
        //
        // Returns:
        //     The list of DnsClient.Protocol.AaaaRecord.
        public static IEnumerable<AaaaRecord> AaaaRecords(this IEnumerable<DnsResourceRecord> records)
        {
            return records.OfType<AaaaRecord>();
        }

        //
        // Summary:
        //     Filters the elements of an System.Collections.Generic.IEnumerable`1 to return
        //     DnsClient.Protocol.SrvRecords only.
        //
        // Parameters:
        //   records:
        //     The records.
        //
        // Returns:
        //     The list of DnsClient.Protocol.SrvRecord.
        //[CLSCompliant(false)]
        //public static IEnumerable<SrvRecord> SrvRecords(this IEnumerable<DnsResourceRecord> records)
        //{
        //    return records.OfType<SrvRecord>();
        //}

        //
        // Summary:
        //     Filters the elements of an System.Collections.Generic.IEnumerable`1 to return
        //     DnsClient.Protocol.UriRecords only.
        //
        // Parameters:
        //   records:
        //     The records.
        //
        // Returns:
        //     The list of DnsClient.Protocol.UriRecord.
        //public static IEnumerable<UriRecord> UriRecords(this IEnumerable<DnsResourceRecord> records)
        //{
        //    return records.OfType<UriRecord>();
        //}

        //
        // Summary:
        //     Filters the elements of an System.Collections.Generic.IEnumerable`1 to return
        //     DnsClient.Protocol.CaaRecords only.
        //
        // Parameters:
        //   records:
        //     The records.
        //
        // Returns:
        //     The list of DnsClient.Protocol.CaaRecord.
        //public static IEnumerable<CaaRecord> CaaRecords(this IEnumerable<DnsResourceRecord> records)
        //{
        //    return records.OfType<CaaRecord>();
        //}

        //
        // Summary:
        //     Filters the elements of an System.Collections.Generic.IEnumerable`1 to return
        //     DnsClient.Protocol.DnsResourceRecords which have the type.
        //
        // Parameters:
        //   records:
        //     The records.
        //
        //   type:
        //     The DnsClient.Protocol.ResourceRecordType to filter for.
        //
        // Returns:
        //     The list of DnsClient.Protocol.ARecord.
        //public static IEnumerable<DnsResourceRecord> OfRecordType(this IEnumerable<DnsResourceRecord> records, ResourceRecordType type)
        //{
        //    return records.Where((DnsResourceRecord p) => p.RecordType == type);
        //}
    }
}