using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace LogParse.DockCtrls
{
    public partial class CtrlSearchResults : DevExpress.XtraEditors.XtraUserControl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public delegate void OnSearchRequestHandler(string sSearchWords);
        public event OnSearchRequestHandler OnSearchRequest;

        public CtrlSearchResults()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            listBoxControl1.Items.Clear();
            if(!string.IsNullOrEmpty(txtSearchWords.Text))
                OnSearchRequest?.Invoke(txtSearchWords.Text);
        }

        internal void AddSearchResult(int nDataSourceIndex, DataRowView row)
        {
            SearchResultInfo info = new SearchResultInfo(nDataSourceIndex, row);
            listBoxControl1.Items.Add(info);
        }
    }
}
