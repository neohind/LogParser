using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogParse
{
    public class DocManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public delegate void OnCompletedHandler(bool bIsSuccess, string sName);
        public event OnCompletedHandler OnCompleted;

        

        public delegate void OnProcessUpdatedHandler(int nPercent);
        public event OnProcessUpdatedHandler OnProcessUpdated;

        public DataTable DataSource
        {
            get;
            private set;
        }

        public List<SourceInfo> LogFileSource
        {
            get;
            private set;
        }

        private ParserEngine m_parserEngine = null;        
        private SourceInfo m_currentSource = null;

        public DocManager()
        {
            BuildupLogTable();
                        
            m_parserEngine = new ParserEngine();
            m_parserEngine.OnCompleted += parserEngine_OnCompleted; ;
            m_parserEngine.OnProcessUpdated += parserEngine_OnProcessUpdated;

            LogFileSource = new List<SourceInfo>();
        }

        private void parserEngine_OnProcessUpdated(int nPercent)
        {
            OnProcessUpdated?.Invoke(nPercent);
        }

        private void parserEngine_OnCompleted(bool bIsSuccess, string sName, DataTable tbResult)
        {
            if (bIsSuccess && m_currentSource != null)
            {
                LogFileSource.Add(m_currentSource);
                m_currentSource = null;
            }
            OnCompleted?.Invoke(bIsSuccess, sName);
        }

        private void BuildupLogTable()
        {
            DataSource = new DataTable();
            System.Data.DataColumn column;

            foreach (StructInfo info in ConfigManager.Current.AllStructures)
            {
                column = new DataColumn();
                column.Caption = info.Caption;
                column.ColumnName = info.Name;
                this.DataSource.Columns.Add(column);
            }
            this.DataSource.TableName = "tbLog";
        }

        internal void Remove(string sSourceName)
        {
            DataRow [] rows = DataSource.Select(string.Format("source = '{0}'", sSourceName));
            foreach (DataRow row in rows)
                DataSource.Rows.Remove(row);

            SourceInfo foundInfo = LogFileSource.Find(m => string.Equals(sSourceName, m.Name));
            if (foundInfo != null)
                LogFileSource.Remove(foundInfo);
        }

        public bool IsAlreadyLoaded(string sFullFilename)
        {
            SourceInfo foundInfo = LogFileSource.Find(m => string.Equals(sFullFilename, m.FullFilename));
            if (foundInfo != null)
                return true;
            return false;
        }

        public void Load(string sFullFilename, ParserInfo info)
        {
            m_currentSource = new SourceInfo(sFullFilename);
            m_parserEngine.DoParse(m_currentSource, info, DataSource);
        }

        internal bool HasSameName(string name)
        {
            SourceInfo foundInfo = LogFileSource.Find(m => string.Equals(name, m.Name));
            if (foundInfo != null)
                return true;

            return false;
        }

        internal void RenameSourceAsync(string sOldName, SourceInfo info)
        {
            string sQuery = string.Format("source='{0}'", sOldName);
            DataRow[] aryRows = DataSource.Select(sQuery);
            Task.Run(() =>
            {
                    //List<DataRow> rows = new List<DataRow>();
                    //rows.AddRange(aryRows);
                    foreach (DataRow row in aryRows)
                    row["source"] = info.Name;
            }).Wait();
            DataSource.AcceptChanges();
        }
    }
}
