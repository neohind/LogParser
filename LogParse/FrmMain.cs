using DevExpress.Data.Filtering;
using DevExpress.XtraGrid;
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
            m_docManager = new DocManager();
            m_docManager.OnCompleted += docManager_OnCompleted;
            
            gridViewAll.OptionsFind.HighlightFindResults = true;
            gridViewAll.OptionsFind.FindMode = DevExpress.XtraEditors.FindMode.Always;

            ctrlLoadAndAppendLogs1.SetupDocManager(m_docManager);
            ctrlLoadAndAppendLogs1.OnDetachRequest += ctrlLoadAndAppendLogs1_OnDetachRequest;
            ctrlLoadAndAppendLogs1.OnLogRemoveRequest += ctrlLoadAndAppendLogs1_OnLogRemoveRequest;

            ctrlSearchResults1.SetupDocManager(m_docManager);
            ctrlSearchResults1.OnSearchRequest += ctrlSearchResults1_OnSearchRequest;

            BuildupDataGrid(gridViewAll);
            BuildupDataGrid(gridViewFiltered);

            gridControlAll.DataSource = m_docManager.DataSource;
            m_aryGridCtrls.Add(gridControlAll);
        }

        private void ctrlSearchResults1_OnSearchRequest(string [] arySearchWords)
        {
            StringBuilder sbFindQuery = new StringBuilder();
            foreach (string sWord in arySearchWords)
                sbFindQuery.AppendFormat("{0} ", sWord);

            gridViewAll.OptionsFind.Behavior = DevExpress.XtraEditors.FindPanelBehavior.Search;
            gridViewAll.OptionsFind.HighlightFindResults = true;
            gridViewAll.OptionsFind.FindFilterColumns = "content";
            
            ColumnView view = gridViewAll.Columns["content"].View;
            view.HideFindPanel();
            view.OptionsFind.Behavior = DevExpress.XtraEditors.FindPanelBehavior.Search;            
            view.OptionsFind.HighlightFindResults = true;
            view.ApplyFindFilter(sbFindQuery.ToString());

            gridViewFiltered.OptionsFind.Behavior = DevExpress.XtraEditors.FindPanelBehavior.Search;
            gridViewFiltered.OptionsFind.FindFilterColumns = "content";

            ColumnView viewFiltered = gridViewFiltered.Columns["content"].View;
            viewFiltered.OptionsFind.Behavior = DevExpress.XtraEditors.FindPanelBehavior.Search;
            viewFiltered.ApplyFindFilter(sbFindQuery.ToString());
            gridViewFiltered.MoveFirst();

            int nHandle = 1;
            while (nHandle > 0)
            {
                nHandle = gridViewFiltered.FocusedRowHandle;

                DataRow row = gridViewFiltered.GetDataRow(nHandle);
                if (row != null)
                {
                    log.DebugFormat("Handle:{0} - {1}", nHandle, row["content"]);
                    gridViewFiltered.MoveNext();
                }
            }

        }

        private void ctrlLoadAndAppendLogs1_OnLogRemoveRequest(SourceInfo info)
        {
            DevExpress.XtraBars.Docking.DockPanel panel = m_aryDocPanels.Find(m => string.Equals(m.Text, info.Name));
            if (panel != null)
                panel.Close();
        }

        private void DockPanel_ClosingPanel(object sender, DevExpress.XtraBars.Docking.DockPanelCancelEventArgs e)
        {
            foreach (DevExpress.XtraGrid.GridControl ctrl in m_aryGridCtrls)
                ctrl.DataSource = null;            

            //DevExpress.XtraBars.Docking.ControlContainer container = e.Panel.Container as DevExpress.XtraBars.Docking.ControlContainer;
            GridControl gridControl = e.Panel.Controls[0] as GridControl;
            if (m_aryGridCtrls.Contains(gridControl))
            {
                m_aryGridCtrls.Remove(gridControl);                
            }

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
