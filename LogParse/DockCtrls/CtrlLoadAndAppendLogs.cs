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
using System.Diagnostics;

namespace LogParse.DockCtrls
{
    public partial class CtrlLoadAndAppendLogs : DevExpress.XtraEditors.XtraUserControl
    {
        public event Action OnDetachRequest;

        public delegate void OnLogRemoveRequestHandler(SourceInfo info);
        public event OnLogRemoveRequestHandler OnLogRemoveRequest;

        private DocManager m_docManager = null;        

        public CtrlLoadAndAppendLogs()
        {
            InitializeComponent();

            if (DesignMode == false)
            {
                cmbLogTypes.DataSource = ConfigManager.Current.AllParserInfo;                
            }
        }

        public void SetupDocManager(DocManager mgr)
        {
            m_docManager = mgr;
            m_docManager.OnCompleted += docManager_OnCompleted;
            m_docManager.OnProcessUpdated += docManager_OnProcessUpdated;
            
        }

        private void docManager_OnProcessUpdated(int nPercent)
        {
            Debug.WriteLine(string.Format("{0}% ...", nPercent));
            this.Invoke(new Action(() =>
            {
                progressBar1.Value = nPercent;
            }));
        }

        private string GetLogFilename()
        {
            string sLogFilename = string.Empty;
            int nIndex = cmbLogTypes.SelectedIndex;
            if (nIndex > -1)
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Multiselect = false;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    sLogFilename = dlg.FileName;
                }
            }
            return sLogFilename;
        }



        private void docManager_OnCompleted(bool bIsSuccess, string sName)
        {
            if (this.InvokeRequired)
                this.Invoke(new DocManager.OnCompletedHandler(docManager_OnCompleted), new object[] { bIsSuccess, sName });
            else
            {
                listSources.Items.Clear();
                foreach (SourceInfo info in m_docManager.LogFileSource)
                    listSources.Items.Add(info);
                
                progressBar1.Value = 100;
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            string sOldFilename = GetLogFilename();
            if (!string.IsNullOrEmpty(sOldFilename))
            {
                if(m_docManager.IsAlreadyLoaded(sOldFilename))
                {
                    MessageBox.Show("The log files is Already loaded!");
                    return;
                }

                OnDetachRequest?.Invoke();
                ParserInfo info = cmbLogTypes.Items[cmbLogTypes.SelectedIndex] as ParserInfo;
                m_docManager.Load(sOldFilename, info);
            }
        }

        internal void RemoveSource(string sSourceName)
        {
            object[] arySources = new object[listSources.Items.Count];
            listSources.Items.CopyTo(arySources, 0);

            foreach (object obj in arySources)
            {
                SourceInfo info = (SourceInfo)obj;
                if(info != null)
                {
                    if (string.Equals(sSourceName, info.Name))
                    {
                        listSources.Items.Remove(obj);
                        return;
                    }
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listSources.SelectedIndex < 0)
                return;

            SourceInfo info = (SourceInfo)listSources.Items[listSources.SelectedIndex];
            if(info != null)
            {
                OnLogRemoveRequest?.Invoke(info);                
            }
        }
    }
}
