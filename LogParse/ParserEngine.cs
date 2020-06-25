using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LogParse
{
    class ParserEngine
    {
        public delegate void OnCompletedHandler(DataTable tbResult);
        public event OnCompletedHandler OnCompleted;

        public delegate void OnProcessUpdatedHandler(int nPercent);
        public event OnProcessUpdatedHandler OnProcessUpdated;

        public DataTable Results
        {
            get
            {
                return m_tbLog;
            }
        }


        private int m_nProgress = 0;
        private Regex m_regexDivider = null;
        private Regex m_regexContents = null;
        private DataTable m_tbLog = null;
        private string m_sFullFilename = string.Empty;
        private string m_sSourceItem = string.Empty;
        private object m_montior = new object();

        public ParserEngine()
        {
        }

        public void DoParse(string sFullFilename, Regex regexDivider, Regex regexContents, DataTable tbLog, ParserInfo info)
        {
            FileInfo fileInfo = new FileInfo(sFullFilename);
            m_sSourceItem = string.Format("{0}/{1}", fileInfo.Directory.Name, fileInfo.Name);
            m_sFullFilename = sFullFilename;
            m_regexDivider = regexDivider;
            m_regexContents = regexContents;

            m_tbLog = tbLog;
            if (m_tbLog == null)
                BuildupLogTable();
            else
                m_tbLog = tbLog.Copy();

            Thread thread = new Thread(new ParameterizedThreadStart(WorkerMakeTable));
            thread.Start(new object[] { m_sFullFilename, m_sSourceItem, m_tbLog, m_regexDivider, m_regexContents });        
        }


        private void WorkerMakeTable(object param)
        {
            object[] aryParams = (object[])param;

            string sFullFilename = aryParams[0] as string;
            string sSource = aryParams[1] as string;
            DataTable tbLog = (DataTable)aryParams[2];
            Regex regexDivider = (Regex)aryParams[3];
            Regex regexContents = (Regex)aryParams[4];

            int nPercent = 0;
            int nLineIndex = 0;            
            List<Task> aryTasks = new List<Task>();

            using (StreamReader reader = new StreamReader(sFullFilename))
            {
                long lnSize = reader.BaseStream.Length;

                StringBuilder sbBuffer = new StringBuilder();
                while (!reader.EndOfStream)
                {
                    nLineIndex++;
                    long lnPosition = reader.BaseStream.Position;

                    int nCurPercent = (int)(lnPosition * 100 / lnSize);
                    if (nCurPercent != nPercent)
                    {                        
                        nPercent = nCurPercent;
                        OnProcessUpdated?.Invoke(nPercent);
                    }

                    string sLine = reader.ReadLine();
                    if (regexDivider.IsMatch(sLine) && sbBuffer.Length > 0)
                    {
                        string sContents = sbBuffer.ToString();
                        Task task = Task.Factory.StartNew((object obj) =>
                        {
                            try
                            {
                                string sContent = obj as string;
                                Match match = regexContents.Match(sContent);
                                if (match.Success)
                                {
                                    lock (m_montior)
                                    {
                                        DataRow newRow = m_tbLog.NewRow();
                                        newRow["source"] = sSource;
                                        newRow["line"] = nLineIndex;

                                        foreach (StructInfo structInfo in ConfigManager.Current.AllStructures)
                                        {
                                            newRow[structInfo.Name] = match.Groups[structInfo.RegexName].Value.Trim();
                                        }
                                        newRow["content"] = sContent.Replace(match.Value, "").Trim();
                                        m_tbLog.Rows.Add(newRow);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
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
            OnCompleted?.Invoke(tbLog);
        }




        private void BuildupLogTable()
        {
            m_tbLog = new DataTable();
            System.Data.DataColumn column;

            column = new DataColumn();
            this.m_tbLog.Columns.Add(column);
            column.Caption = "Source";
            column.ColumnName = "source";

            column = new DataColumn();
            this.m_tbLog.Columns.Add(column);
            column.Caption = "Line";
            column.ColumnName = "line";
            column.DataType = typeof(int);

            foreach (StructInfo info in ConfigManager.Current.AllStructures)
            {
                column = new DataColumn();
                column.Caption = info.Caption;
                column.ColumnName = info.Name;
                this.m_tbLog.Columns.Add(column);
            }

            column = new DataColumn();
            this.m_tbLog.Columns.Add(column);
            column.Caption = "Content";
            column.ColumnName = "content";

            this.m_tbLog.TableName = "tbLog";
        }
    }
}
