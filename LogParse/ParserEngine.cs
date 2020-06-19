using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogParse
{
    class ParserEngine
    {
        public event Action OnCompleted;

        public DataTable Results
        {
            get
            {
                return m_tbLog;
            }
        }

        Regex m_regexDivider = null;
        Regex m_regexContents = null;
        DataTable m_tbLog = null;
        string m_sFullFilename = string.Empty;

        public ParserEngine(string sFullFilename,  Regex regexDivider, Regex regexContents)
        {
            m_regexDivider = regexDivider;
            m_regexContents = regexContents;            
        }

        public void DoParse(DataTable tbLog)
        {
            m_tbLog = tbLog;
            if (m_tbLog == null)
                BuildupLogTable();

        }


        private void BuildupLogTable()
        {
            m_tbLog = new DataTable();

            foreach(StructInfo info in ConfigManager.Current.AllStructures)
            {
                System.Data.DataColumn column = new DataColumn();
                column.Caption = info.Caption;
                column.ColumnName = info.Name;
                this.m_tbLog.Columns.Add(column);
            }



            this.m_tbLog.TableName = "tbLog";

        }
    }
}
