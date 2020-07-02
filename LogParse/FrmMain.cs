using DevExpress.Data.Filtering;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using LogParse.DockCtrls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace LogParse
{
    public partial class FrmMain : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        List<DevExpress.XtraBars.Docking2010.Views.Tabbed.Document> m_aryDocPages = new List<DevExpress.XtraBars.Docking2010.Views.Tabbed.Document>();
        List<DevExpress.XtraBars.Docking.DockPanel> m_aryDocPanels = new List<DevExpress.XtraBars.Docking.DockPanel>();
        List<DevExpress.XtraGrid.GridControl> m_aryGridCtrls = new List<DevExpress.XtraGrid.GridControl>();
        private DocManager m_docManager = null;

        public FrmMain()
        {
            InitializeComponent();

            // Log 파일을 읽고 관리하기 위한 관리자를 생성
            m_docManager = new DocManager();
            m_docManager.OnCompleted += docManager_OnCompleted;
            
            // Load 및 Append 처리를 하기 위한 Docking Control의 설정을 한다.
            ctrlLoadAndAppendLogs1.SetupDocManager(m_docManager);
            ctrlLoadAndAppendLogs1.OnDetachRequest += ctrlLoadAndAppendLogs1_OnDetachRequest;
            ctrlLoadAndAppendLogs1.OnLogRemoveRequest += ctrlLoadAndAppendLogs1_OnLogRemoveRequest;
            ctrlLoadAndAppendLogs1.OnLogRenameRequest += ctrlLoadAndAppendLogs1_OnLogRenameRequest;

            // 검색용 Docking Control의 설정을 한다.
            ctrlSearchResults1.SetupDocManager(m_docManager);
            ctrlSearchResults1.OnSearchRequest += ctrlSearchResults1_OnSearchRequest;
            ctrlSearchResults1.OnRefocusRowRequest += ctrlSearchResults1_OnRefocusRowRequest;

            // GridView의 Column을 설정한다.
            BuildupDataGrid(gridViewAll);

            // Grid Control의 인스턴스를 관리하기 위한 Array에 추가한다.
            m_aryGridCtrls.Add(gridControlAll);
        }

        

        /// <summary>
        /// 검색용 Docking Control의 이벤트 중 특정 Row로 Focus를 옮겨달라는 요청을 처리하기 위한 이벤트 핸들러
        /// </summary>
        /// <param name="nDataSourceIndex">Focus 처리를 해야 할 Row의 DataTable 기준 Index</param>
        private void ctrlSearchResults1_OnRefocusRowRequest(int nDataSourceIndex)
        {
            int nRowHandler = gridViewAll.GetRowHandle(nDataSourceIndex);
            if (nRowHandler != GridControl.InvalidRowHandle)
            {
                gridViewAll.FocusedRowHandle = nRowHandler;
            }
        }

        /// <summary>
        /// 검색용 Docking Control의 이벤트 중 특정 검색어에 대하여 검색 요청을 처리하기 위한 이벤트 핸들러
        /// </summary>
        /// <param name="arySearchWords">검색어 목록</param>
        private void ctrlSearchResults1_OnSearchRequest(string[] arySearchWords)
        {
            // 그리드의 content Column과 Column의 View를 변수로 만듬
            GridColumn column = gridViewAll.Columns["content"];
            ColumnView view = column.View;

            // 검색어를 단순한 문자열로 만드는 작업 
            // "AAA", "BBB", "CCC" 라는 검색어가 있으면 "AAA BBB CCC"로 변환
            StringBuilder sbFindQuery = new StringBuilder();
            foreach (string sWord in arySearchWords)
                sbFindQuery.AppendFormat("{0} ", sWord);
                        
           
            // 그리드 내의 문자열 중 Search 된 문자열을 Highlight 처리를 한다.
            gridViewAll.OptionsFind.Behavior = DevExpress.XtraEditors.FindPanelBehavior.Search;
            gridViewAll.OptionsFind.HighlightFindResults = true;
            gridViewAll.OptionsFind.FindFilterColumns = "content";
            view.HideFindPanel();
            view.OptionsFind.Behavior = DevExpress.XtraEditors.FindPanelBehavior.Search;
            view.OptionsFind.HighlightFindResults = true;
            view.ApplyFindFilter(sbFindQuery.ToString());
           
            // 최초 문장으로 넘어간다.
            gridViewAll.MoveFirst();

            // 시간이 많이 걸리는 작업이므로 별도 Thread로 분리한다.
            Task.Run(() =>
            {
                // 검색어를 Regular Express 문장으로 변경한다.
                StringBuilder sbRegex = new StringBuilder();
                bool bIsFirst = true;
                sbRegex.Append("(");
                foreach (string sWord in arySearchWords)
                {
                    string sWordReplaced = Regex.Replace(sWord, @"([\?\(\)\[\]\*\$\.\-\!\'\""])", @"\$1");
                    if (bIsFirst)
                        bIsFirst = false;
                    else
                        sbRegex.Append("|");

                    sbRegex.AppendFormat("{0}", sWordReplaced);
                }
                sbRegex.Append(")");

                // Regular Express 내용을 기반으로 Regex 개체를 생성한다.
                Regex regex = new Regex(sbRegex.ToString(), RegexOptions.IgnoreCase);

                // 그리드 내용을 검색하기 위한 Index 값 처리
                // 현재 가르키는 (gridViewAll.MoveFirst()) 항목의 RowHandle 값을 가져온다.
                int rowHandle = gridViewAll.FocusedRowHandle;
                // RowHandle 값을 기준으로 화면에 표시되는 Row의 Index를 가져온다.
                int visibleIndex = gridViewAll.GetVisibleIndex(rowHandle);

                // 검색 결과를 저장하기 위한 Dictionary. 
                // Key는 DataSource 기준 Idnex이고, Value는 검색 결과 문자열
                Dictionary<int, string> dicResult = new Dictionary<int, string>();                

                // rowHanle 값이 범위 외의 값인지 확인
                while (rowHandle != GridControl.InvalidRowHandle)
                {
                    // rowHandle 값이 가르키는 Row 값에서 column에 해당하는 값을 추출한다.
                    string sValue = view.GetRowCellValue(rowHandle, column) as string;

                    // Regex에 해당하는 값인지 체크
                    if (regex.IsMatch(sValue))
                    {
                        // Regex에 해당하는 값이면, Dictionary에 더한다.
                        int nDataSourceIndex = view.GetDataSourceRowIndex(rowHandle);
                        if (dicResult.ContainsKey(nDataSourceIndex) == false)
                            dicResult.Add(nDataSourceIndex, sValue);
                    }

                    // 다른 Row 중 화면에 표시되는 Row의 Index를 가져온다.
                    visibleIndex = gridViewAll.GetNextVisibleRow(visibleIndex);
                    // 표시되는 Row의 Index를 기준으로 RowHandle을 가져온다.
                    rowHandle = gridViewAll.GetVisibleRowHandle(visibleIndex);
                }

                // 검색 결과가 담긴 Dictionary 를 검색용 Docking Control에 전달한다.
                ctrlSearchResults1.SetSearchResults(dicResult);
            });
        }

        /// <summary>
        /// Load 혹은 Append 처리를 하기 위한 Docking Control 이번트 중, 
        /// Load/Append 된 Log 항목을 삭제 요청에 대한 이벤트를 처리하기 위한 이벤트 핸들러
        /// </summary>
        /// <param name="info">삭제한 Log 정보 개체</param>
        private void ctrlLoadAndAppendLogs1_OnLogRemoveRequest(SourceInfo info)
        {
            //삭제한 Log의 Docking Control을 가지고 있는 Panel을 찾아 삭제한다.
            DevExpress.XtraBars.Docking.DockPanel panel = m_aryDocPanels.Find(m => string.Equals(m.Text, info.Name));
            if (panel != null)
                panel.Close();
        }

        /// <summary>
        /// Docking Control 중 기존 이름을 기준으로 새롭게 바뀐 이름으로 변경하기 위한 이벤트 핸들러
        /// </summary>
        /// <param name="sOldName">기존 이름</param>
        /// <param name="info">새롭게 개선된 Source 정보에 대한 개체</param>
        private void ctrlLoadAndAppendLogs1_OnLogRenameRequest(string sOldName, SourceInfo info)
        {
            DevExpress.XtraBars.Docking.DockPanel panel = m_aryDocPanels.Find(m => string.Equals(m.Text, sOldName));
            if (panel != null)
            {
                panel.Text = info.Name;

                GridControl gridControl = panel.Controls[0].Controls[0] as GridControl;
                CriteriaOperator criteria = new BinaryOperator(new OperandProperty("source"), new OperandValue(info.Name), BinaryOperatorType.Equal);
                DevExpress.XtraGrid.Views.Grid.GridView view = gridControl.MainView as DevExpress.XtraGrid.Views.Grid.GridView;
                view.ActiveFilterCriteria = criteria;
                
                foreach (GridControl ctrl in m_aryGridCtrls)
                    ctrl.DataSource = null;
                m_docManager.RenameSourceAsync(sOldName, info);

                foreach (GridControl ctrl in m_aryGridCtrls)
                    ctrl.DataSource = m_docManager.DataSource;

            }
        }

        /// <summary>
        /// Load 혹은 Append에 대한 Docking Control을 가지고 있는 Panel이 닫힐 때 발생하는 이벤트 핸들러
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DockPanel_ClosingPanel(object sender, DevExpress.XtraBars.Docking.DockPanelCancelEventArgs e)
        {
            // 모든 Grid Control들의 DataSource를 모두 null 처리해서 Data 처리가 가능하도록 한다.
            foreach (DevExpress.XtraGrid.GridControl ctrl in m_aryGridCtrls)
                ctrl.DataSource = null;            

            // 닫히고 있는 Docking Panel 안의 Grid Control을 추출한다.
            GridControl gridControl = e.Panel.Controls[0] as GridControl;
            if (m_aryGridCtrls.Contains(gridControl))
            {
                // 추출한 Grid Control을 모든 Grid Control을 담고 있는 배열에서 제거한다.
                m_aryGridCtrls.Remove(gridControl);                
            }

            // 
            string sSourceName = e.Panel.Text;
            m_docManager.Remove(sSourceName);

            foreach (DevExpress.XtraGrid.GridControl ctrl in m_aryGridCtrls)
                ctrl.DataSource = m_docManager.DataSource;

            ctrlLoadAndAppendLogs1.RemoveSource(sSourceName);
        }

        private void ctrlLoadAndAppendLogs1_OnDetachRequest()
        {
            foreach (DevExpress.XtraGrid.GridControl ctrl in m_aryGridCtrls)
                ctrl.DataSource = null;
        }

        private void docManager_OnCompleted(bool bIsSuccess, string sName)
        {
            if (bIsSuccess)
            {
                if (this.InvokeRequired)
                    this.Invoke(new DocManager.OnCompletedHandler(docManager_OnCompleted), new object[] { bIsSuccess, sName });
                else
                {
                    AddPage(sName);
                    Application.DoEvents();
                    foreach (DevExpress.XtraGrid.GridControl ctrl in m_aryGridCtrls)
                        ctrl.DataSource = m_docManager.DataSource;
                }
            }
        }

        private void AddPage(string sSourceName)
        {
            DevExpress.XtraBars.Docking.DockPanel dockPanel = new DevExpress.XtraBars.Docking.DockPanel();
            DevExpress.XtraBars.Docking.ControlContainer dockPanel_Container = new DevExpress.XtraBars.Docking.ControlContainer();            
            DevExpress.XtraBars.Docking2010.Views.Tabbed.Document page = new DevExpress.XtraBars.Docking2010.Views.Tabbed.Document(this.components);
            DevExpress.XtraGrid.GridControl gridControl = new DevExpress.XtraGrid.GridControl();
            DevExpress.XtraGrid.Views.Grid.GridView gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            
            m_aryDocPanels.Add(dockPanel);
            m_aryDocPages.Add(page);
            m_aryGridCtrls.Add(gridControl);

            string sCtrlIndex = string.Format("{0:00}", m_aryDocPages.Count);

            this.documentMainGroup.Items.Add(page);
            this.dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Float, dockPanel);

            // 
            // dockPanel
            // 
            dockPanel.Controls.Add(dockPanel_Container);
            dockPanel.DockedAsTabbedDocument = true;
            dockPanel.FloatLocation = new System.Drawing.Point(1920, 0);
            dockPanel.ID = Guid.NewGuid();
            dockPanel.Name = string.Format("dockPanel{0}", sCtrlIndex); 
            dockPanel.OriginalSize = new System.Drawing.Size(200, 200);
            dockPanel.Text = sSourceName;
            dockPanel.ClosingPanel += DockPanel_ClosingPanel;
            // 
            // dockPanel_Container
            // 
            dockPanel_Container.Controls.Add(gridControl);
            dockPanel_Container.Location = new System.Drawing.Point(0, 0);
            dockPanel_Container.Name = string.Format("dockPanel{0}_Container", sCtrlIndex);
            dockPanel_Container.Size = new System.Drawing.Size(1029, 476);
            dockPanel_Container.TabIndex = 0;
            // 
            // gridControl
            // 
            gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            gridControl.Location = new System.Drawing.Point(0, 0);
            gridControl.MainView = gridView;
            gridControl.Name = string.Format("gridControl{0}", sCtrlIndex);
            gridControl.Size = new System.Drawing.Size(1037, 359);
            gridControl.TabIndex = 0;
            gridControl.ViewCollection.Add(gridView);
            gridControl.Resize += new System.EventHandler(this.gridControl_Resize);
            // 
            // gridView
            // 
            gridView.Appearance.Row.Font = new System.Drawing.Font("Consolas", 9F);
            gridView.Appearance.Row.Options.UseFont = true;
            gridView.ColumnPanelRowHeight = 0;
            gridView.FooterPanelHeight = 0;
            gridView.GridControl = gridControl;
            gridView.GroupRowHeight = 0;
            gridView.Name = string.Format("gridView{0}", sCtrlIndex);
            gridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            gridView.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
            gridView.OptionsCustomization.AllowColumnMoving = false;
            gridView.OptionsCustomization.AllowGroup = false;
            gridView.OptionsCustomization.AllowRowSizing = true;
            gridView.OptionsLayout.Columns.AddNewColumns = false;
            gridView.OptionsLayout.Columns.RemoveOldColumns = false;
            gridView.OptionsView.ColumnAutoWidth = false;
            gridView.OptionsView.RowAutoHeight = true;
            gridView.OptionsView.ShowGroupExpandCollapseButtons = false;
            gridView.OptionsView.ShowGroupPanel = false;
            gridView.OptionsView.ShowIndicator = false;
            gridView.RowHeight = 0;
            gridView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.LiveHorzScroll;
            gridView.ViewCaptionHeight = 0;
            CriteriaOperator criteria = new BinaryOperator(new OperandProperty("source"), new OperandValue(sSourceName), BinaryOperatorType.Equal);
            gridView.ActiveFilterCriteria = criteria;
            // 
            // page
            // 
            page.Caption = sSourceName;
            page.ControlName = dockPanel.Name;
            page.FloatLocation = new System.Drawing.Point(this.Location.X / 2, this.Location.Y /2);
            page.FloatSize = new System.Drawing.Size(400, 400);
            page.Properties.AllowClose = DevExpress.Utils.DefaultBoolean.True;
            page.Properties.AllowFloat = DevExpress.Utils.DefaultBoolean.True;
            page.Properties.AllowFloatOnDoubleClick = DevExpress.Utils.DefaultBoolean.True;

            Application.DoEvents();
            BuildupDataGrid(gridView);
            
        }
       
        private void BuildupDataGrid(DevExpress.XtraGrid.Views.Grid.GridView view)
        {
            view.Columns.Clear();

            List<DevExpress.XtraGrid.Columns.GridColumn> aryAllColumns = new List<DevExpress.XtraGrid.Columns.GridColumn>();
            int nAllColumnCount = ConfigManager.Current.AllStructures.Length;
            for (int i = 0; i < nAllColumnCount; i++)
                aryAllColumns.Add(new DevExpress.XtraGrid.Columns.GridColumn());
            view.Columns.AddRange(aryAllColumns.ToArray());

            int nIndex = 0;
            // 설정값을 기준으로 Grid Column을 생성
            foreach (StructInfo info in ConfigManager.Current.AllStructures)
            {
                DevExpress.XtraGrid.Columns.GridColumn column = aryAllColumns[nIndex];
                column.FieldName = info.Name;
                column.Caption = info.Caption;
                column.Name = info.Name;
                column.OptionsColumn.AllowEdit = false;
                column.OptionsColumn.ReadOnly = info.IsReadOnly;
                if (info.IsMultiEditor)
                {
                    DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit memoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
                    column.ColumnEdit = memoEdit1;
                    memoEdit1.WordWrap = true;
                    column.OptionsColumn.AllowEdit = true;
                }

                if (info.GridWidth > 0)
                {
                    column.Width = info.GridWidth;                    
                }

                aryAllColumns[nIndex].VisibleIndex = info.DisplayOrder;
                nIndex++;
            }

            ResizeLastColumn(view);
        }

        private void ResizeLastColumn(DevExpress.XtraGrid.Views.Grid.GridView view)
        {
            DevExpress.XtraEditors.VScrollBar vScrollBar = view.GridControl.Controls.OfType<DevExpress.XtraEditors.VScrollBar>().FirstOrDefault();
            int nWidth = view.GridControl.ClientSize.Width - view.FixedLineWidth * view.Columns.Count - vScrollBar.Width;
            
            foreach (DevExpress.XtraGrid.Columns.GridColumn column in view.Columns)
            {
                if (view.Columns.Count - 1 > view.Columns.IndexOf(column))
                    nWidth = nWidth - column.Width;
                else
                    column.Width = (int)(nWidth);
            }
        }

        private void gridControl_Resize(object sender, EventArgs e)
        {
            if(sender is GridControl)
            {
                GridControl curControl = sender as GridControl;
                ResizeLastColumn(curControl.MainView as DevExpress.XtraGrid.Views.Grid.GridView);
            }
        }
    }
}
