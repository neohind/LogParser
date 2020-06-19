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
        private DataTable m_tbLog = null;        
        private object m_montior = new object();
        private System.Windows.Forms.DataGridViewTextBoxColumn sourceDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lineDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn logdateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn logtimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn loglevelDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ssidDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn thidDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn loggerDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn contentDataGridViewTextBoxColumn;

        string m_sOldFilename = string.Empty;

        public FrmMain()
        {
            InitializeComponent();
            CleanupGrid();
            dataGridView1.VirtualMode = true;
            cmbLogTypes.DataSource = ConfigManager.Current.AllParserInfo; 


        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                m_sOldFilename = dlg.FileName;
                CleanupGrid();
                BuildupDataGrid();
                //BuildupLogTable();
                LoadFile(m_sOldFilename);            
            }            
        }


        private void LoadFile(string sFullFilename)
        {
            int nIndex = cmbLogTypes.SelectedIndex;
            if (nIndex > -1)
            {
                ParserInfo info = cmbLogTypes.Items[nIndex] as ParserInfo;

                ReadFile(info, sFullFilename);

                
                this.dataGridView1.DataSource = m_tbLog;
                dataGridView1.Update();
                dataGridView1.Refresh();
            }
        }

        private void CleanupGrid()
        {
            m_dicSourceColors.Clear();
            m_aryUsableColors.Clear();
            m_aryUsableColors.AddRange(m_arySourceColors);
            dataGridView1.DataSource = null;            
        }

        private void BuildupDataGrid()
        {
            this.dataGridView1.Columns.Clear();

            foreach(StructInfo info in ConfigManager.Current.AllStructures)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                this.dataGridView1.Columns.Add(column);
                column.DataPropertyName = info.Name;
                column.HeaderText = info.Caption;
                column.Name = info.Name;
                column.ReadOnly = true;
                if(info.GridWidth > 0)
                    column.Width = info.GridWidth;
                column.DisplayIndex = info.DisplayOrder;
            }

            DataGridViewTextBoxColumn contentColumn = new DataGridViewTextBoxColumn();
            this.dataGridView1.Columns.Add(contentColumn);
            contentColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            contentColumn.DataPropertyName = "content";
            contentColumn.HeaderText = "Content";
            contentColumn.Name = "content";
            contentColumn.ReadOnly = false;
            contentColumn.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            contentColumn.DisplayIndex = this.dataGridView1.Columns.Count;
        }

        private void ReadFile(ParserInfo info, string sFullFilename)
        {
            lblSearchCounter.Text = "0 / 0";
            m_aryFoundedRows.Clear();
            FileInfo fileInfo = new FileInfo(sFullFilename);
            string sSource = string.Format("{0}/{1}", fileInfo.Directory.Name, fileInfo.Name);
            int nLineIndex = 0;

            if(!m_dicSourceColors.ContainsKey(sSource))
            {
                Color colorSource = m_aryUsableColors[0];
                m_aryUsableColors.Remove(colorSource);
                m_dicSourceColors.Add(sSource, colorSource);
            }

            Task.Run(() =>
            {
                List<Task> aryTasks = new List<Task>();
                using (StreamReader reader = new StreamReader(sFullFilename))
                {
                    StringBuilder sbBuffer = new StringBuilder();
                    while (!reader.EndOfStream)
                    {
                        nLineIndex++;
                        string sLine = reader.ReadLine();
                        if (info.RegexForDivider.IsMatch(sLine) && sbBuffer.Length > 0)
                        {
                            string sContents = sbBuffer.ToString();
                            Task task = Task.Factory.StartNew((object obj) =>
                            {
                                string sContent = obj as string;
                                Match match = info.RegexForContents.Match(sContent);
                                if (match.Success)
                                {
                                    lock (m_montior)
                                    {
                                        DataRow newRow = m_tbLog.NewRow();
                                        newRow["source"] = sSource;
                                        newRow["line"] = nLineIndex;
                                        newRow["logdate"] = match.Groups["date"].Value.Trim();
                                        newRow["logtime"] = match.Groups["time"].Value.Trim();
                                        newRow["loglevel"] = match.Groups["level"].Value.Trim();
                                        newRow["ssid"] = match.Groups["session"].Value.Trim();
                                        newRow["thid"] = match.Groups["thread"].Value.Trim();
                                        newRow["logger"] = match.Groups["logger"].Value.Trim();
                                        newRow["content"] = sContent.Replace(match.Value, "").Trim();
                                        m_tbLog.Rows.Add(newRow);
                                    }
                                }
                            }, sContents);

                            aryTasks.Add(task);

                            sbBuffer = new StringBuilder();
                            sbBuffer.AppendLine(sLine);
                            continue;
                        }
                        sbBuffer.AppendLine(sLine);
                    }
                }
                Task.WaitAll(aryTasks.ToArray());                
            }).Wait();
            Debug.WriteLine("Complete");
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            if (m_tbLog != null)
            {
                if(!string.IsNullOrEmpty(m_tbLog.DefaultView.Sort) && m_tbLog.DefaultView.Sort.Contains("ASC"))
                    m_tbLog.DefaultView.Sort = "logdate DESC, logtime DESC";
                else 
                    m_tbLog.DefaultView.Sort = "logdate ASC, logtime ASC";
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            if(m_tbLog != null)
            {
                m_tbLog.Dispose();
                m_tbLog = null;
            }
            CleanupGrid();
        }

        private void btnAppend_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                m_sOldFilename = dlg.FileName;                
                CleanupGrid();
                BuildupDataGrid();

                LoadFile(m_sOldFilename);
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


        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            foreach(DataGridViewRow row in dataGridView1.SelectedRows)
            {
                dataGridView1.AutoResizeRow(row.Index, DataGridViewAutoSizeRowMode.AllCellsExceptHeader);                
            }
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex < 0)
                return;
            

            DataGridViewColumn column = dataGridView1.Columns[e.ColumnIndex];

            switch(column.Name)
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (StreamWriter writer = new StreamWriter("temp.dat"))
            {
                m_tbLog.WriteXml(writer);
            }
        }

        private void btnLoad_Click_1(object sender, EventArgs e)
        {
            this.dataGridView1.DataSource = null;
            CleanupGrid();
            BuildupDataGrid();
            // BuildupLogTable();
            using (StreamReader reader = new StreamReader("temp.dat"))
            {
                m_tbLog.ReadXml(reader);
            }

            DataView view = new DataView(m_tbLog);
            DataTable tbSources = view.ToTable(true, "source");

            foreach(DataRow row in tbSources.Rows)
            {
                string sSourceName = Convert.ToString(row["source"]);
                Color colorSource = m_aryUsableColors[0];
                m_aryUsableColors.Remove(colorSource);
                m_dicSourceColors.Add(sSourceName, colorSource);                
            }

            this.dataGridView1.DataSource = m_tbLog;
            dataGridView1.Update();
            dataGridView1.Refresh();
        }
    }
}
