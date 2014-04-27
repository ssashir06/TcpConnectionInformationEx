using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Network;

namespace NetStatCS
{
    public partial class NetStatForm : Form
    {
        public NetStatForm()
        {
            InitializeComponent();
            EndPointPortNumericUpDown.Text = String.Empty;
        }

        private void NetStatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var query =
                from tcp_conn_view in
                    (
                    from tcp_conn in TcpTableApi.GetActiveTcpConnectionEx()
                    select new ViewTcpConnectionInformationEx(tcp_conn)
                    )
                where String.IsNullOrEmpty(EndPointTextBox.Text) || tcp_conn_view.RemoteEndPoint.Address.ToString() == EndPointTextBox.Text
                where String.IsNullOrEmpty(EndPointPortNumericUpDown.Text) || tcp_conn_view.RemoteEndPoint.Port == EndPointPortNumericUpDown.Value
                where String.IsNullOrEmpty(ProcessNameTextBox.Text) || tcp_conn_view.ProcessName == ProcessNameTextBox.Text
                select tcp_conn_view;

            Replace(query.ToList());
        }

        void Replace(List<ViewTcpConnectionInformationEx> list_new)
        {
            SuspendLayout();
            for (int i = 0; i < viewTcpConnectionInformationExBindingSource.Count - list_new.Count(); i++)
            {
                viewTcpConnectionInformationExBindingSource.RemoveAt(list_new.Count());
            }
            for (int i = 0; i < list_new.Count(); i++)
            {
                if (i < viewTcpConnectionInformationExBindingSource.Count)
                {
                    if ((ViewTcpConnectionInformationEx)viewTcpConnectionInformationExBindingSource[i] != list_new[i])
                        viewTcpConnectionInformationExBindingSource[i] = list_new[i];
                }
                else
                {
                    viewTcpConnectionInformationExBindingSource.Add(list_new[i]);
                }
            }
            ResumeLayout();
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
			//TODO
        }
    }
}
