using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;

namespace Network
{
	/// <summary>
	/// Tcp information with pid.
	/// </summary>
    public class TcpConnectionInformationEx : TcpConnectionInformation
    {
		/// <summary>
		/// Remote IP address and its port.
		/// </summary>
        IPEndPoint _remote;
		/// <summary>
		/// Local IP address and its port.
		/// </summary>
        IPEndPoint _local;
		/// <summary>
		/// Connection state.
		/// </summary>
        TcpState _state = TcpState.Unknown;
		/// <summary>
		/// Process Id.
		/// </summary>
        int _pid;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpConnectionInformationEx"/> class.
        /// </summary>
        /// <param name="Remote">Remote IP address and port.</param>
        /// <param name="Local">Local IP address and port.</param>
        /// <param name="State">Connection State.</param>
        /// <param name="Pid">Process Id.</param>
        public TcpConnectionInformationEx(IPEndPoint Remote, IPEndPoint Local, TcpState State, int Pid)
        {
            _remote = Remote;
            _local = Local;
            _state = State;
            _pid = Pid;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpConnectionInformationEx" /> class.
        /// </summary>
        /// <param name="src">The source.</param>
        protected TcpConnectionInformationEx(TcpConnectionInformationEx src)
        {
            _remote = src._remote;
            _local = src._local;
            _state = src._state;
            _pid = src._pid;
        }

        #region TcpConnectionInformation
        /// <summary>
        /// 伝送制御プロトコル (TCP) 接続のローカル エンドポイントを取得します。
        /// </summary>
        /// <returns>ローカル コンピューターの IP アドレスとポートを格納している <see cref="T:System.Net.IPEndPoint" /> インスタンス。</returns>
        public override IPEndPoint LocalEndPoint
        {
            get { return _local; }
        }

        /// <summary>
        /// 伝送制御プロトコル (TCP) 接続のリモート エンドポイントを取得します。
        /// </summary>
        /// <returns>リモート コンピューターの IP アドレスとポートを格納している <see cref="T:System.Net.IPEndPoint" /> インスタンス。</returns>
        public override IPEndPoint RemoteEndPoint
        {
            get { return _remote; }
        }

        /// <summary>
        /// この伝送制御プロトコル (TCP) 接続の状態を取得します。
        /// </summary>
        /// <returns>
        ///   <see cref="T:System.Net.NetworkInformation.TcpState" /> 列挙値の 1 つ。</returns>
        public override TcpState State
        {
            get { return _state; }
        }
        #endregion

        /// <summary>
        /// OwnerのProcess Idを返します。
        /// </summary>
        public int ProcessId
        {
            get
            {
                return _pid;
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ _pid;
        }
    }
}
