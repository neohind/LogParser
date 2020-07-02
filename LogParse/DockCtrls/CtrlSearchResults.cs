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
using DevExpress.XtraBars.Docking2010;
using System.Text.RegularExpressions;

namespace LogParse.DockCtrls
{
    public partial class CtrlSearchResults : DevExpress.XtraEditors.XtraUserControl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public delegate void OnSearchRequestHandler(string [] sSearchWords);
        public event OnSearchRequestHandler OnSearchRequest;

        public delegate void OnRefocusRowRequestHandler(int nDataSourceIndex);
        public event OnRefocusRowRequestHandler OnRefocusRowRequest;

        private Dictionary<DataRow, SearchResultInfo> m_dicSearchResult = new Dictionary<DataRow, SearchResultInfo>();

        private DocManager m_docManager = null;

        public CtrlSearchResults()
        {
            InitializeComponent();
        }

        public void SetupDocManager(DocManager mgr)
        {
            m_docManager = mgr;            
        }
        

        private void btnSearch_Click(object sender, EventArgs e)
        {
            listBoxControl1.Items.Clear();
            m_dicSearchResult.Clear();

            string sSearchWords = txtSearchWords.Text;
            string[] aryTokens = sSearchWords.Split(" ".ToCharArray(), StringSplitOptions.None);

            if (aryTokens.Length > 0)
            {
                OnSearchRequest?.Invoke(aryTokens);
            }
        }


        private void txtSearchWords_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnSearch_Click(btnSearch, e);
        }

        internal void SetSearchResults(Dictionary<int, string> dicResult)
        {
            Invoke(new Action(() => {
                progressBar1.Visible = true;
                txtSearchWords.Visible = false;
                
                lblMax.Text = dicResult.Count.ToString();
                int nCount = 0;
                foreach (int nDataSourceIndex in dicResult.Keys)
                {
                    nCount++;
                    SearchResultInfo info = new SearchResultInfo(nDataSourceIndex, dicResult[nDataSourceIndex]);
                    listBoxControl1.Items.Add(info);

                    int nProgress = ((int)100 * nCount / dicResult.Count);
                    if (progressBar1.Value != nProgress)
                    {
                        progressBar1.Value = nProgress;
                        Application.DoEvents();
                    }
                }
                progressBar1.Visible = false;
                txtSearchWords.Visible = true;
            }));
        }

        private void listBoxControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBoxControl1.SelectedIndex > -1)
            {
                lblCur.Text = (listBoxControl1.SelectedIndex + 1).ToString();
                SearchResultInfo info = (SearchResultInfo)listBoxControl1.Items[listBoxControl1.SelectedIndex];
                OnRefocusRowRequest?.Invoke(info.DataSourceIndex);
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            int nSelectedIndex = listBoxControl1.SelectedIndex - 1;
            if (nSelectedIndex < 0)
                nSelectedIndex = 0;
            listBoxControl1.SelectedIndex = nSelectedIndex;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            int nSelectedIndex = listBoxControl1.SelectedIndex + 1;
            if (nSelectedIndex == listBoxControl1.Items.Count)
                nSelectedIndex = listBoxControl1.Items.Count - 1;
            listBoxControl1.SelectedIndex = nSelectedIndex;
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void txtSearchWords_EditValueChanged(object sender, EventArgs e)
        {

        }
    }
}
