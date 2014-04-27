using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Network;

namespace NetStatCS
{
	/// <summary>
	/// DataGridViewで表示するための TcpConnectionInformationEx 継承クラス。
	/// </summary>
    public class ViewTcpConnectionInformationEx : TcpConnectionInformationEx
    {
        /// <summary>
        /// Stores some cached process information.
        /// </summary>
        static CachedFunc<int, Process> _process_cache = new CachedFunc<int, Process>(_pid => Process.GetProcessById(_pid))
        {
            Timeout = TimeSpan.FromSeconds(30),
        };
		/// <summary>
		/// Lazy process information.
		/// </summary>
        Lazy<Process> _process;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewTcpConnectionInformationEx"/> class.
        /// </summary>
        /// <param name="src">An instance of TcpConnectionInformationEx.</param>
        public ViewTcpConnectionInformationEx(TcpConnectionInformationEx src) : base(src) {

            _process = new Lazy<Process>(() => {
                try
                {
                    return _process_cache[ProcessId];
                }
                catch (ArgumentException)
                {
                    // processId パラメーターで指定されたプロセスは実行されていません。 ID の有効期限が切れている可能性があります。 
                    _process_cache[ProcessId] = null;
                    return null;
                }
                catch (InvalidOperationException)
                {
                    // プロセスが、このオブジェクトによって開始されなかった。
                    throw;
                }
            });
        
        }

        /// <summary>
        /// Gets the process.
        /// </summary>
        /// <value>
        /// The process.
        /// </value>
        public Process Process
        {
            get { return _process.Value; }
        }

        /// <summary>
        /// Gets the name of the process.
        /// </summary>
        /// <value>
        /// The name of the process.
        /// </value>
        public string ProcessName
        {
            get
            {
                if (Process == null) return String.Empty;
                else return Process.ProcessName;
            }
        }

        #region Operators / Overrides
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="a">1st value</param>
        /// <param name="b">2nd value</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(ViewTcpConnectionInformationEx a, ViewTcpConnectionInformationEx b)
        {
            return
                a.RemoteEndPoint.ToString() == b.RemoteEndPoint.ToString() &&
                a.LocalEndPoint.ToString() == b.LocalEndPoint.ToString() &&
                a.State == b.State &&
                a.ProcessId == b.ProcessId;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="a">1st value</param>
        /// <param name="b">2nd value</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(ViewTcpConnectionInformationEx a, ViewTcpConnectionInformationEx b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is ViewTcpConnectionInformationEx)
                return (obj as ViewTcpConnectionInformationEx) == this;
            else
                return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format(
                "LocalEndPoint: {1}{0}" +
                "RemoteEndPoint: {2}{0}" +
                "State: {3}{0}" +
                "Pid:{4}{0}" +
                "Process: {5}",
                Environment.NewLine,
                LocalEndPoint.ToString(),
                RemoteEndPoint.ToString(),
                State,
                ProcessId,
                Process.ProcessName);
        }
        #endregion
    }
}
