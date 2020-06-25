using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogParse
{
    public partial class FrmMain : Form
    {
        private Color[] m_arySourceColors = new Color[] { Color.LightPink, Color.Yellow, Color.LightBlue, Color.Honeydew, Color.GreenYellow, Color.LightSkyBlue };
        private List<Color> m_aryUsableColors = new List<Color>();
        private Dictionary<string, Color> m_dicSourceColors = new Dictionary<string, Color>();
        private List<string> m_arySearchKeyword = new List<string>();

        private int m_nSelectedFoundedRowIndex = 0;
        private List<DataGridViewRow> m_aryFoundedRows = new List<DataGridViewRow>();         
        private object m_montior = new object();
        private ParserEngine m_engine = null;


        string m_sOldFilename = string.Empty;

        public FrmMain()
        {
            InitializeComponent();            
            //dataGridView1.VirtualMode = true;
            cmbLogTypes.DataSource = ConfigManager.Current.AllParserInfo;
            m_engine = new ParserEngine();
            m_engine.OnCompleted += engine_OnCompleted;
            m_engine.OnProcessUpdated += engine_OnProcessUpdated;

        }

        private void engine_OnProcessUpdated(int nPercent)
        {
            Debug.WriteLine(string.Format("{0}% ...", nPercent));
            this.Invoke(new Action(() =>
            {
                progressBar1.Value = nPercent;                
            }));
        }

        private void engine_OnCompleted(DataTable tbResult)
        {
            if (this.InvokeRequired)
                this.Invoke(new ParserEngine.OnCompletedHandler(engine_OnCompleted), tbResult);
            else
            {                
                tbResult.DefaultView.Sort = "logdate ASC, logtime ASC";
                this.gridControl1.DataSource = tbResult;
                //dataGridView1.Update();
                //dataGridView1.Refresh();

                progressBar1.Value = 100;                
            }
        }


        private void btnLoad_Click(object sender, EventArgs e)
        {
            int nIndex = cmbLogTypes.SelectedIndex;
            if (nIndex > -1)
            {
                ParserInfo info = cmbLogTypes.Items[nIndex] as ParserInfo;

                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Multiselect = false;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    m_sOldFilename = dlg.FileName;

                    //if (dataGridView1.DataSource is DataTable)
                    //{
                    //    DataTable tbOldData = (DataTable)dataGridView1.DataSource;
                    //    tbOldData.Dispose();
                    //}
                    //dataGridView1.DataSource = null;


                    if (gridControl1.DataSource is DataTable)
                    {
                        DataTable tbOldData = (DataTable)gridControl1.DataSource;
                        tbOldData.Dispose();
                    }
                    gridControl1.DataSource = null;


                    BuildupDataGrid();
                    m_engine.DoParse(m_sOldFilename, info.RegexForDivider, info.RegexForContents, null, info);
                }
            }
        }

        private void btnAppend_Click(object sender, EventArgs e)
        {
            int nIndex = cmbLogTypes.SelectedIndex;
            if (nIndex > -1)
            {
                ParserInfo info = cmbLogTypes.Items[nIndex] as ParserInfo;
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Multiselect = false;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    m_sOldFilename = dlg.FileName;
                    //DataTable tbOldData = null;
                    //if (dataGridView1.DataSource != null && dataGridView1.DataSource is DataTable)
                    //{
                    //    tbOldData = (DataTable)dataGridView1.DataSource;
                    //}
                    //dataGridView1.DataSource = null;
                    //BuildupDataGrid();

                    DataTable tbOldData = null;
                    if (gridControl1.DataSource != null && gridControl1.DataSource is DataTable)
                    {
                        tbOldData = (DataTable)gridControl1.DataSource;
                    }
                    gridControl1.DataSource = null;
                    //BuildupDataGrid();


                    m_engine.DoParse(m_sOldFilename, info.RegexForDivider, info.RegexForContents, tbOldData, info);
                }
            }
        }



        private void BuildupDataGrid()
        {
            this.gridView1.Columns.Clear();
            DevExpress.XtraGrid.Columns.GridColumn contentColumn;

            // Source Grid Column은 Fix 형태로 제공
            contentColumn = new DevExpress.XtraGrid.Columns.GridColumn();            
            contentColumn.FieldName = "source";
            contentColumn.Caption = "Source";
            contentColumn.Name = "source";
            contentColumn.OptionsColumn.AllowEdit = false;
            contentColumn.OptionsColumn.ReadOnly = true;
            contentColumn.VisibleIndex = 0;
            this.gridView1.Columns.Add(contentColumn);

            // Line Grid Column은 Fix 형태로 제공
            contentColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            contentColumn.FieldName = "line";
            contentColumn.Caption = "Line";
            contentColumn.Name = "line";
            contentColumn.OptionsColumn.AllowEdit = false;
            contentColumn.OptionsColumn.ReadOnly = true;
            contentColumn.VisibleIndex = 1;
            this.gridView1.Columns.Add(contentColumn);
                      

            // 설정값을 기준으로 Grid Column을 생성
            foreach (StructInfo info in ConfigManager.Current.AllStructures)
            {
                DevExpress.XtraGrid.Columns.GridColumn column = new DevExpress.XtraGrid.Columns.GridColumn();
                this.gridView1.Columns.Add(column);
                column.FieldName = info.Name;
                column.Caption = info.Caption;
                column.Name = info.Name;
                column.OptionsColumn.AllowEdit = false;
                column.OptionsColumn.ReadOnly = true;
                if (info.GridWidth > 0)
                    column.Width = info.GridWidth;
                column.VisibleIndex = info.DisplayOrder + 2;
            }

            // Contents Grid Column은 Fix 형태로 제공       
            // Line Grid Column은 Fix 형태로 제공
            contentColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            contentColumn.FieldName = "content";
            contentColumn.Caption = "Content";
            contentColumn.Name = "content";
            contentColumn.OptionsColumn.AllowEdit = false;
            contentColumn.VisibleIndex = (ConfigManager.Current.AllStructures.Count() + 2); 
            this.gridView1.Columns.Add(contentColumn);
        }

        private void ReadFile(ParserInfo info, string sFullFilename)
        {
            
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            if (gridControl1.DataSource != null && gridControl1.DataSource is DataTable)
            {
                DataTable tbLog = (DataTable)gridControl1.DataSource;
                if (tbLog != null)
                {
                    if (!string.IsNullOrEmpty(tbLog.DefaultView.Sort) && tbLog.DefaultView.Sort.Contains("ASC"))
                        tbLog.DefaultView.Sort = "logdate DESC, logtime DESC";
                    else
                        tbLog.DefaultView.Sort = "logdate ASC, logtime ASC";
                }
            }
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            lblSearchCounter.Text = "0 / 0";
            m_nSelectedFoundedRowIndex = 0;
            m_aryFoundedRows.Clear();
            string sSearchWords = txtSearchWord.Text;
            if (string.IsNullOrWhiteSpace(sSearchWords))
            {
                m_arySearchKeyword.Clear();
                return;
            }
            m_arySearchKeyword.AddRange(sSearchWords.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

            workerSearching.RunWorkerAsync();
            
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (m_aryFoundedRows.Count == 0)
                return;

            m_nSelectedFoundedRowIndex--;
            if (m_nSelectedFoundedRowIndex < 0)
                m_nSelectedFoundedRowIndex = m_aryFoundedRows.Count - 1;

            
            dataGridView1.CurrentCell = m_aryFoundedRows[m_nSelectedFoundedRowIndex].Cells[0];
            m_aryFoundedRows[m_nSelectedFoundedRowIndex].Selected = true;

            lblSearchCounter.Text = string.Format("{0} / {1}", m_nSelectedFoundedRowIndex, m_aryFoundedRows.Count);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (m_aryFoundedRows.Count == 0)
                return;

            m_nSelectedFoundedRowIndex++;
            if (m_nSelectedFoundedRowIndex > m_aryFoundedRows.Count -1)
                m_nSelectedFoundedRowIndex = 0;

            
            dataGridView1.CurrentCell = m_aryFoundedRows[m_nSelectedFoundedRowIndex].Cells[0];
            m_aryFoundedRows[m_nSelectedFoundedRowIndex].Selected = true;

            lblSearchCounter.Text = string.Format("{0} / {1}", m_nSelectedFoundedRowIndex, m_aryFoundedRows.Count);
        }


        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            foreach(DataGridViewRow row in dataGridView1.SelectedRows)
            {
                dataGridView1.AutoResizeRow(row.Index, DataGridViewAutoSizeRowMode.AllCellsExceptHeader);                
            }
        }

        private void txtSearchWord_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnSearch_Click(sender, e);
        }

        private void workerSearching_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string sContents = row.Cells["contentDataGridViewTextBoxColumn"].Value as string;
                foreach (string sWords in m_arySearchKeyword)
                    if (sContents.Contains(sWords))
                    {
                        m_aryFoundedRows.Add(row);
                        break;
                    }
            }
        }

        private void workerSearching_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (m_aryFoundedRows.Count > 0)
            {
                m_aryFoundedRows[m_nSelectedFoundedRowIndex].Selected = true;
                lblSearchCounter.Text = string.Format("{0} / {1}", m_nSelectedFoundedRowIndex, m_aryFoundedRows.Count);
            }
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex < 0)
                return;

            DataGridViewColumn column = dataGridView1.Columns[e.ColumnIndex];
            switch (column.Name)
            {
                case "sourceDataGridViewTextBoxColumn": // Source Column Index
                    {
                        string sContents = e.Value as string;
                        if (m_dicSourceColors.ContainsKey(sContents))
                            e.CellStyle.BackColor = m_dicSourceColors[sContents];
                    }
                    break;
                case "loglevelDataGridViewTextBoxColumn": // Log Level Column Index
                    {
                        string sLevel = e.Value as string;
                        switch (sLevel)
                        {
                            //case "DEBUG":
                            //    e.CellStyle.BackColor = Color.LightSlateGray;
                            //    break;
                            case "INFO":
                                e.CellStyle.BackColor = Color.Green;
                                break;
                            case "WARN":
                                e.CellStyle.BackColor = Color.Orange;
                                break;
                            case "ERROR":
                                e.CellStyle.BackColor = Color.Red;
                                break;
                        }
                    }
                    break;
                case "contentDataGridViewTextBoxColumn":
                    {
                        string sContents = e.Value as string;
                        foreach (string sWords in m_arySearchKeyword)
                            if (sContents.Contains(sWords))
                            {
                                e.CellStyle.BackColor = Color.Aquamarine;
                                break;
                            }
                    }
                    break;
                default:
                    break;
            }


        }
    }
}
