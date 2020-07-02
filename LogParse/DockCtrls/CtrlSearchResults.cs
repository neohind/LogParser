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
            // 결과 값은 별도 스레드이므로 UI 스레드와 충돌이 나지 않도록 Invoke 처리
            Invoke(new Action(() => {
                int nCount = 0;

                // 검색 텍스트 박스를 숨기고 ProgressBar를 표시
                progressBar1.Visible = true;
                txtSearchWords.Visible = false;
                
                // 검색 결과 전체 갯수를 화면에 표시
                lblMax.Text = dicResult.Count.ToString();
                
                // 검색 결과 값을 하나씩 ListBox에 추가한다.
                foreach (int nDataSourceIndex in dicResult.Keys)
                {
                    nCount++;
                    // 검색 결과 값을 Info 형 클래스에 담아 ListBox에 추가한다.
                    SearchResultInfo info = new SearchResultInfo(nDataSourceIndex, dicResult[nDataSourceIndex]);
                    listBoxControl1.Items.Add(info);

                    // 전체 처리 진행 상황을 계산해서 Progress Bar에 반영한다.
                    int nProgress = ((int)100 * nCount / dicResult.Count);
                    if (progressBar1.Value != nProgress)
                    {
                        progressBar1.Value = nProgress;
                        Application.DoEvents();
                    }
                }

                // Progress Bar를 숨기고 검새 텍스트 박스를 표시한다.
                progressBar1.Visible = false;
                txtSearchWords.Visible = true;
            }));
        }

        /// <summary>
        /// ListBox의 이벤트 중, Select Index가 변경을 처리하기 위한 이벤트 핸들러
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 아규먼트</param>
        private void listBoxControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ListBox의 선택된 아이템의 Index가 -1 보다 커야 제대로 선택된 것으로 판단한다.
            if(listBoxControl1.SelectedIndex > -1)
            {
                // 현재 선택된 아이템의 Index 값을 표시한다. 다만, 0부터 시작되므로 +1 한다.
                lblCur.Text = (listBoxControl1.SelectedIndex + 1).ToString();

                // ListBox 안의 Info 객체로 꺼내서 DataSourceIndex 값을 가져온다.
                SearchResultInfo info = (SearchResultInfo)listBoxControl1.Items[listBoxControl1.SelectedIndex];

                // 메인 Form의 Grid의 선택을 해주기 위해 DataSourceIndex 값을 담아 이벤트를 발생시킨다.
                OnRefocusRowRequest?.Invoke(info.DataSourceIndex);
            }
        }


        /// <summary>
        /// 검색 결과 중 현재 기준 이전 검색 내용으로 이동한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrev_Click(object sender, EventArgs e)
        {
            int nSelectedIndex = listBoxControl1.SelectedIndex - 1;
            if (nSelectedIndex < 0)
                nSelectedIndex = 0;
            listBoxControl1.SelectedIndex = nSelectedIndex;
        }

        /// <summary>
        /// 검색 결과 중 현재 기준 다음 검색 내용으로 이동한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNext_Click(object sender, EventArgs e)
        {
            int nSelectedIndex = listBoxControl1.SelectedIndex + 1;
            if (nSelectedIndex == listBoxControl1.Items.Count)
                nSelectedIndex = listBoxControl1.Items.Count - 1;
            listBoxControl1.SelectedIndex = nSelectedIndex;
        }

    }
}
