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

                //StringBuilder sbRegex = new StringBuilder();
                //sbRegex.Append("(");
                //bool bIsFirst = true;
                //foreach (string sToken in aryTokens)
                //{
                //    if (bIsFirst)
                //        bIsFirst = false;
                //    else
                //        sbRegex.Append("|");

                //    sbRegex.Append(sToken);
                //}
                //sbRegex.Append(")");

                //Regex regex = new Regex(sbRegex.ToString(), RegexOptions.IgnoreCase);

                //foreach (DataRow row in m_docManager.DataSource.Rows)
                //{
                //    string sContent = row["content"] as string;
                //    if (regex.IsMatch(sContent))
                //    {
                //        SearchResultInfo info = new SearchResultInfo(m_docManager.DataSource.Rows.IndexOf(row), row);
                //        listBoxControl1.Items.Add(info);
                //    }
                //}
            }
        }

        internal void AddSearchResult(int nDataSourceIndex)
        {
            
        }


        internal void RemoveSearchResult(DataRowView row)
        {
                   

        }

        private void txtSearchWords_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnSearch_Click(btnSearch, e);
        }
    }
}
