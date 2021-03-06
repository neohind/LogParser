﻿namespace LogParse
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            DevExpress.XtraBars.Docking2010.Views.Tabbed.DockingContainer dockingContainer1 = new DevExpress.XtraBars.Docking2010.Views.Tabbed.DockingContainer();
            this.documentMainGroup = new DevExpress.XtraBars.Docking2010.Views.Tabbed.DocumentGroup(this.components);
            this.document1 = new DevExpress.XtraBars.Docking2010.Views.Tabbed.Document(this.components);
            this.gridControlAll = new DevExpress.XtraGrid.GridControl();
            this.gridViewAll = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.dockLoader = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel2_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.ctrlLoadAndAppendLogs1 = new LogParse.DockCtrls.CtrlLoadAndAppendLogs();
            this.dockSearch = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel3_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.ctrlSearchResults1 = new LogParse.DockCtrls.CtrlSearchResults();
            this.dockAllData = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.MainMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.documentManager1 = new DevExpress.XtraBars.Docking2010.DocumentManager(this.components);
            this.tabbedView1 = new DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedView(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.documentMainGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.document1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlAll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewAll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
            this.dockLoader.SuspendLayout();
            this.dockPanel2_Container.SuspendLayout();
            this.dockSearch.SuspendLayout();
            this.dockPanel3_Container.SuspendLayout();
            this.dockAllData.SuspendLayout();
            this.dockPanel1_Container.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.documentManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabbedView1)).BeginInit();
            this.SuspendLayout();
            // 
            // documentMainGroup
            // 
            this.documentMainGroup.Items.AddRange(new DevExpress.XtraBars.Docking2010.Views.Tabbed.Document[] {
            this.document1});
            // 
            // document1
            // 
            this.document1.Caption = "All Logs";
            this.document1.ControlName = "dockAllData";
            this.document1.FloatLocation = new System.Drawing.Point(2876, 184);
            this.document1.FloatSize = new System.Drawing.Size(200, 200);
            this.document1.Properties.AllowClose = DevExpress.Utils.DefaultBoolean.False;
            this.document1.Properties.AllowFloat = DevExpress.Utils.DefaultBoolean.True;
            this.document1.Properties.AllowFloatOnDoubleClick = DevExpress.Utils.DefaultBoolean.True;
            // 
            // gridControlAll
            // 
            this.gridControlAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlAll.Location = new System.Drawing.Point(0, 0);
            this.gridControlAll.MainView = this.gridViewAll;
            this.gridControlAll.Name = "gridControlAll";
            this.gridControlAll.Size = new System.Drawing.Size(1028, 476);
            this.gridControlAll.TabIndex = 0;
            this.gridControlAll.UseDirectXPaint = DevExpress.Utils.DefaultBoolean.True;
            this.gridControlAll.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewAll});
            this.gridControlAll.Resize += new System.EventHandler(this.gridControl_Resize);
            // 
            // gridViewAll
            // 
            this.gridViewAll.Appearance.Row.Font = new System.Drawing.Font("Consolas", 9F);
            this.gridViewAll.Appearance.Row.Options.UseFont = true;
            this.gridViewAll.GridControl = this.gridControlAll;
            this.gridViewAll.Name = "gridViewAll";
            this.gridViewAll.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridViewAll.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridViewAll.OptionsBehavior.AllowIncrementalSearch = true;
            this.gridViewAll.OptionsCustomization.AllowColumnMoving = false;
            this.gridViewAll.OptionsCustomization.AllowGroup = false;
            this.gridViewAll.OptionsCustomization.AllowRowSizing = true;
            this.gridViewAll.OptionsLayout.Columns.AddNewColumns = false;
            this.gridViewAll.OptionsLayout.Columns.RemoveOldColumns = false;
            this.gridViewAll.OptionsView.ColumnAutoWidth = false;
            this.gridViewAll.OptionsView.RowAutoHeight = true;
            this.gridViewAll.OptionsView.ShowGroupExpandCollapseButtons = false;
            this.gridViewAll.OptionsView.ShowGroupPanel = false;
            this.gridViewAll.OptionsView.ShowIndicator = false;
            this.gridViewAll.RowHeight = 0;
            this.gridViewAll.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.LiveHorzScroll;
            this.gridViewAll.ViewCaptionHeight = 0;
            // 
            // dockManager1
            // 
            this.dockManager1.Form = this;
            this.dockManager1.RootPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] {
            this.dockLoader,
            this.dockSearch,
            this.dockAllData});
            this.dockManager1.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "DevExpress.XtraBars.StandaloneBarDockControl",
            "System.Windows.Forms.StatusBar",
            "System.Windows.Forms.MenuStrip",
            "System.Windows.Forms.StatusStrip",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl",
            "DevExpress.XtraBars.Navigation.OfficeNavigationBar",
            "DevExpress.XtraBars.Navigation.TileNavPane",
            "DevExpress.XtraBars.TabFormControl",
            "DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormControl"});
            // 
            // dockLoader
            // 
            this.dockLoader.Controls.Add(this.dockPanel2_Container);
            this.dockLoader.Dock = DevExpress.XtraBars.Docking.DockingStyle.Left;
            this.dockLoader.FloatSize = new System.Drawing.Size(230, 505);
            this.dockLoader.ID = new System.Guid("e6aa4c4e-a8dc-49f7-8dcf-40a8e53b018e");
            this.dockLoader.Location = new System.Drawing.Point(0, 24);
            this.dockLoader.Name = "dockLoader";
            this.dockLoader.Options.ShowCloseButton = false;
            this.dockLoader.OriginalSize = new System.Drawing.Size(230, 109);
            this.dockLoader.Size = new System.Drawing.Size(230, 705);
            this.dockLoader.Text = "Load and Append Log file";
            // 
            // dockPanel2_Container
            // 
            this.dockPanel2_Container.Controls.Add(this.ctrlLoadAndAppendLogs1);
            this.dockPanel2_Container.Location = new System.Drawing.Point(4, 23);
            this.dockPanel2_Container.Name = "dockPanel2_Container";
            this.dockPanel2_Container.Size = new System.Drawing.Size(221, 678);
            this.dockPanel2_Container.TabIndex = 0;
            // 
            // ctrlLoadAndAppendLogs1
            // 
            this.ctrlLoadAndAppendLogs1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlLoadAndAppendLogs1.Location = new System.Drawing.Point(0, 0);
            this.ctrlLoadAndAppendLogs1.Margin = new System.Windows.Forms.Padding(41, 21, 41, 21);
            this.ctrlLoadAndAppendLogs1.MinimumSize = new System.Drawing.Size(230, 500);
            this.ctrlLoadAndAppendLogs1.Name = "ctrlLoadAndAppendLogs1";
            this.ctrlLoadAndAppendLogs1.Size = new System.Drawing.Size(230, 678);
            this.ctrlLoadAndAppendLogs1.TabIndex = 0;
            // 
            // dockSearch
            // 
            this.dockSearch.Controls.Add(this.dockPanel3_Container);
            this.dockSearch.Dock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
            this.dockSearch.ID = new System.Guid("57162f99-74b5-4007-b2cb-400391168486");
            this.dockSearch.Location = new System.Drawing.Point(230, 529);
            this.dockSearch.Name = "dockSearch";
            this.dockSearch.Options.ShowCloseButton = false;
            this.dockSearch.OriginalSize = new System.Drawing.Size(200, 200);
            this.dockSearch.Size = new System.Drawing.Size(1034, 200);
            this.dockSearch.Text = "Search";
            // 
            // dockPanel3_Container
            // 
            this.dockPanel3_Container.Controls.Add(this.ctrlSearchResults1);
            this.dockPanel3_Container.Location = new System.Drawing.Point(4, 24);
            this.dockPanel3_Container.Name = "dockPanel3_Container";
            this.dockPanel3_Container.Size = new System.Drawing.Size(1026, 172);
            this.dockPanel3_Container.TabIndex = 0;
            // 
            // ctrlSearchResults1
            // 
            this.ctrlSearchResults1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlSearchResults1.Location = new System.Drawing.Point(0, 0);
            this.ctrlSearchResults1.Margin = new System.Windows.Forms.Padding(6);
            this.ctrlSearchResults1.Name = "ctrlSearchResults1";
            this.ctrlSearchResults1.Size = new System.Drawing.Size(1026, 172);
            this.ctrlSearchResults1.TabIndex = 0;
            // 
            // dockAllData
            // 
            this.dockAllData.Controls.Add(this.dockPanel1_Container);
            this.dockAllData.Dock = DevExpress.XtraBars.Docking.DockingStyle.Float;
            this.dockAllData.DockedAsTabbedDocument = true;
            this.dockAllData.FloatLocation = new System.Drawing.Point(2876, 184);
            this.dockAllData.FloatVertical = true;
            this.dockAllData.ID = new System.Guid("66aa9621-352a-4e3c-b57b-8ce19cc52e6d");
            this.dockAllData.Location = new System.Drawing.Point(0, 0);
            this.dockAllData.Name = "dockAllData";
            this.dockAllData.Options.ShowCloseButton = false;
            this.dockAllData.OriginalSize = new System.Drawing.Size(200, 200);
            this.dockAllData.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Top;
            this.dockAllData.SavedIndex = 2;
            this.dockAllData.Size = new System.Drawing.Size(1028, 476);
            this.dockAllData.Text = "All Logs";
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Controls.Add(this.gridControlAll);
            this.dockPanel1_Container.Location = new System.Drawing.Point(0, 0);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(1028, 476);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MainMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(3, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(1264, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // MainMenu
            // 
            this.MainMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Size = new System.Drawing.Size(37, 22);
            this.MainMenu.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // documentManager1
            // 
            this.documentManager1.ContainerControl = this;
            this.documentManager1.View = this.tabbedView1;
            this.documentManager1.ViewCollection.AddRange(new DevExpress.XtraBars.Docking2010.Views.BaseView[] {
            this.tabbedView1});
            // 
            // tabbedView1
            // 
            this.tabbedView1.DocumentGroups.AddRange(new DevExpress.XtraBars.Docking2010.Views.Tabbed.DocumentGroup[] {
            this.documentMainGroup});
            this.tabbedView1.Documents.AddRange(new DevExpress.XtraBars.Docking2010.Views.BaseDocument[] {
            this.document1});
            dockingContainer1.Element = this.documentMainGroup;
            this.tabbedView1.RootContainer.Nodes.AddRange(new DevExpress.XtraBars.Docking2010.Views.Tabbed.DockingContainer[] {
            dockingContainer1});
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1264, 729);
            this.Controls.Add(this.dockSearch);
            this.Controls.Add(this.dockLoader);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Malgun Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "FrmMain";
            this.Text = "FrmMain2";
            ((System.ComponentModel.ISupportInitialize)(this.documentMainGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.document1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlAll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewAll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
            this.dockLoader.ResumeLayout(false);
            this.dockPanel2_Container.ResumeLayout(false);
            this.dockSearch.ResumeLayout(false);
            this.dockPanel3_Container.ResumeLayout(false);
            this.dockAllData.ResumeLayout(false);
            this.dockPanel1_Container.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.documentManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabbedView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraBars.Docking.DockPanel dockSearch;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel3_Container;
        private DevExpress.XtraBars.Docking.DockPanel dockLoader;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel2_Container;
        private DockCtrls.CtrlLoadAndAppendLogs ctrlLoadAndAppendLogs1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem MainMenu;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private DevExpress.XtraBars.Docking.DockPanel dockAllData;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;        
        private DevExpress.XtraGrid.GridControl gridControlAll;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewAll;
        private DevExpress.XtraBars.Docking2010.DocumentManager documentManager1;
        private DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedView tabbedView1;
        private DevExpress.XtraBars.Docking2010.Views.Tabbed.DocumentGroup documentMainGroup;
        private DevExpress.XtraBars.Docking2010.Views.Tabbed.Document document1;
        private DockCtrls.CtrlSearchResults ctrlSearchResults1;
    }
}