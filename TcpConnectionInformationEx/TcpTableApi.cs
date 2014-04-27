using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace Network
{
    /// <summary>
    /// Create instances of TcpConnectionInformationEx class from Win32Api result.
    /// <seealso cref="http://www.pinvoke.net/default.aspx/iphlpapi/GetExtendedTcpTable.html"/>
    /// </summary>
    public static class TcpTableApi
    {
        #region Win32Api
        /// <summary>
        /// A class calls Win32Api, named GetExtendedTcpTable
        /// </summary>
        internal static class SafeNativeMethods
        {
            /// <summary>
            /// GetExtendedTcpTable, a Win32Api.
            /// </summary>
            /// <seealso cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa365928(v=vs.85).aspx"/>
            /// <returns>0 ... NO_ERROR, 122 ... ERROR_INSUFFICIENT_BUFFER, 87 ... ERROR_INVALID_PARAMETER</returns>
            [DllImport("iphlpapi.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.U4)]
            public static extern UInt32 GetExtendedTcpTable(
                IntPtr pTcpTable, ref UInt32 dwOutBufLen, bool sort, int ipVersion, TCP_TABLE_CLASS tblClass, uint reserved = 0);

            public enum TCP_TABLE_CLASS
            {
                TCP_TABLE_BASIC_LISTENER,
                TCP_TABLE_BASIC_CONNECTIONS,
                TCP_TABLE_BASIC_ALL,
                TCP_TABLE_OWNER_PID_LISTENER,
                TCP_TABLE_OWNER_PID_CONNECTIONS,
                TCP_TABLE_OWNER_PID_ALL,
                TCP_TABLE_OWNER_MODULE_LISTENER,
                TCP_TABLE_OWNER_MODULE_CONNECTIONS,
                TCP_TABLE_OWNER_MODULE_ALL
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MIB_TCPROW_OWNER_PID
        {
            UInt32 _state;
            UInt32 _local_address;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            byte[] _local_port;
            UInt32 _remote_address;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            byte[] _remote_port;
            UInt32 _owning_pid;

            public IPAddress LocalAddress
            {
                get
                {
                    return new IPAddress(_local_address);
                }
            }

            public ushort LocalPort
            {
                get
                {
                    return BitConverter.ToUInt16(
                    new byte[2] { _local_port[1], _local_port[0] }, 0);
                }
            }

            public IPAddress RemoteAddress
            {
                get
                {
                    return new IPAddress(_remote_address);
                }
            }

            public ushort RemotePort
            {
                get
                {
                    return BitConverter.ToUInt16(
                    new byte[2] { _remote_port[1], _remote_port[0] }, 0);
                }
            }

            public TcpState TcpState
            {
                get
                {
                    if (_state > 0 && _state < 13) return (TcpState)_state;
                    else return TcpState.Unknown;
                }
            }

            public int Pid
            {
                get
                {
                    return (int)_owning_pid;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MIB_TCPTABLE_OWNER_PID
        {
            public uint dwNumEntries;
            MIB_TCPROW_OWNER_PID table;
        }

        static MIB_TCPROW_OWNER_PID[] GetAllTcpConnections()
        {
            MIB_TCPROW_OWNER_PID[] rows;
            int AF_INET = 2;    // IP_v4
            uint buff_size = 0;

            uint ret = SafeNativeMethods.GetExtendedTcpTable(IntPtr.Zero, ref buff_size, true, AF_INET, SafeNativeMethods.TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL);
            IntPtr buffTable = Marshal.AllocHGlobal((int)buff_size);

            try
            {
                ret = SafeNativeMethods.GetExtendedTcpTable(buffTable, ref buff_size, true, AF_INET, SafeNativeMethods.TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL);
                if (ret != 0) return null;

                var tab = (MIB_TCPTABLE_OWNER_PID)Marshal.PtrToStructure(buffTable, typeof(MIB_TCPTABLE_OWNER_PID));
                IntPtr row_ptr = (IntPtr)((long)buffTable + Marshal.SizeOf(tab.dwNumEntries));
                rows = new MIB_TCPROW_OWNER_PID[tab.dwNumEntries];

                for (int i = 0; i < tab.dwNumEntries; i++)
                {
                    var row = (MIB_TCPROW_OWNER_PID)Marshal.PtrToStructure(row_ptr, typeof(MIB_TCPROW_OWNER_PID));
                    rows[i] = row;
                    row_ptr = (IntPtr)((long)row_ptr + Marshal.SizeOf(row));
                }

            }
            finally
            {
                Marshal.FreeHGlobal(buffTable);
            }
            return rows;
        }

        #endregion

        /// <summary>
        /// Get tcp connections with pid.
        /// </summary>
        /// <returns>Enumerable of TcpConnectionInformationEx</returns>
        public static IEnumerable<TcpConnectionInformationEx> GetActiveTcpConnectionEx()
        {
            var query =
                from conn in GetAllTcpConnections()
                select new TcpConnectionInformationEx(
                    new IPEndPoint(conn.RemoteAddress, (int)conn.RemotePort),
                    new IPEndPoint(conn.LocalAddress, (int)conn.RemotePort),
                    conn.TcpState,
                    conn.Pid);
            return query;
        }
    }
}
