using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;

namespace SIPSorcery
{
    public static class Extensions
    { 
        public static bool TryParse<T>(string str, out T val) where T : struct
        {
#if NET20
            return EnumEx.TryParse<T>(str, out val);
#else
            return Enum.TryParse<T>(str, out val);
#endif
        }

        public static bool TryParse<T>(string str, bool ignoreCase, out T val) where T : struct
        {
#if NET20
            return EnumEx.TryParse<T>(str, out val);
#else
            return Enum.TryParse<T>(str, ignoreCase, out val);
#endif
        }

#if NET20
        public static IPAddress MapToIPv6(this IPAddress addr)
        {
            if (addr.AddressFamily == AddressFamily.InterNetworkV6)
            {
                return addr;
            }

            var newAddr = new ushort[8]
            {
                0,
                0,
                0,
                0,
                0,
                65535,
                (ushort)(((addr.Address & 0xFF00) >> 8) | ((addr.Address & 0xFF) << 8)),
                (ushort)(((addr.Address & 4278190080u) >> 24) | ((addr.Address & 0xFF0000) >> 8))
            };
            byte[] target = new byte[newAddr.Length * 2];
            Buffer.BlockCopy(newAddr, 0, target, 0, newAddr.Length * 2);

            return new IPAddress(target, 0u);
        }
#endif

        public static bool DualMode(this Socket socket)
        {
#if NET20
            if (Socket.OSSupportsIPv6)
            {
                var val = socket.GetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27);
                return (int)val == 0;
            }
            return false;

#else
            return socket.DualMode;
#endif
        }

        public static void DualMode(this Socket socket, bool dualMode)
        {
#if NET20
            if (Socket.OSSupportsIPv6)
            {
                socket.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, dualMode ? 0 : -1);
            }
#else
            socket.DualMode = dualMode;
#endif
        }

        public static bool IsNullOrWhiteSpace(string str)
        {
#if NET20
            return string.IsNullOrEmpty(str?.Trim());
#else
            return string.IsNullOrWhiteSpace(str);
#endif
        }

        public static bool OSSupportsIPv4
        {
            get
            {
#if NET20
                return true;
#else
                return Socket.OSSupportsIPv4;
#endif
            }
        }
        public static Socket CreateDualModeSocket(SocketType socketType, ProtocolType protocolType)
        {
#if NET20
            var rtpSocket = new Socket(Socket.OSSupportsIPv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork, socketType, protocolType);
            if (Socket.OSSupportsIPv6)
            {
                rtpSocket.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, 0);
            }
#else
            var rtpSocket = new Socket(SocketType.Dgram, ProtocolType.Udp);
#endif
            return rtpSocket;
        }
        public static bool IsIPv4MappedToIPv6(this IPAddress address)
        {
#if NET20
            if (address.AddressFamily != AddressFamily.InterNetworkV6)
            {
                return false;
            }
            var m_Numbers = address.IPAddressNumbers();
            for (int i = 0; i < 5; i++)
            {
                if (m_Numbers[i] != 0)
                {
                    return false;
                }
            }
            return (m_Numbers[5] == 0xFFFF);
#else
            return address.IsIPv4MappedToIPv6;
#endif
        }

#if NET20
        public static byte[] ToArray(this ArraySegment<byte> arr)
        {
            return arr.Array;
        }
#endif
    }

#if NET20
    public static class RuntimeInformation
    {
        public static string OSDescription
        {
            get
            {
                return "Microsoft Windows";
            }
        }
    }
#endif

    public class CertPair
    {
        public Org.BouncyCastle.Crypto.Tls.Certificate Certificate { get; set; }
        public AsymmetricKeyParameter PrivateKey { get; set; }
    }
}
