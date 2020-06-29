using DevExpress.CodeParser.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public delegate void OnCompletedHandler(bool bIsSuccess, string sName, DataTable tbResult);
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

        public List<SourceInfo> AllSources
        {
            get;
            private set;
        }


        private Regex m_regexDivider = null;
        private Regex m_regexContents = null;
        private DataTable m_tbLog = null;
        private string m_sFullFilename = string.Empty;
        private string m_sSourceItem = string.Empty;
        private object m_montior = new object();

        public ParserEngine()
        {
            AllSources = new List<SourceInfo>();
        }


        public void DoParse(SourceInfo sourceInfo, ParserInfo parseInfo, DataTable tbLog)
        {
            m_regexDivider = parseInfo.RegexForDivider;
            m_regexContents = parseInfo.RegexForContents;

            Thread thread = new Thread(new ParameterizedThreadStart(WorkerMakeTable));
            thread.Start(new object[] { sourceInfo, tbLog, m_regexDivider, m_regexContents });
        }

        public void DoParse_Old(string sFullFilename, ParserInfo info, DataTable tbLog)
        {
            m_regexDivider = info.RegexForDivider;
            m_regexContents = info.RegexForContents;

            m_tbLog = tbLog;
            if (m_tbLog == null)
            {
                ClearSources();
                BuildupLogTable();
            }
            else
                m_tbLog = tbLog.Copy();

            SourceInfo sourceInfo = new SourceInfo(sFullFilename);
            AllSources.Add(sourceInfo);

            Thread thread = new Thread(new ParameterizedThreadStart(WorkerMakeTable));
            thread.Start(new object[] { sourceInfo, m_tbLog, m_regexDivider, m_regexContents });        
        }

        public void ClearSources()
        {
            AllSources.Clear();
        }

        private void WorkerMakeTable(object param)
        {
            object[] aryParams = (object[])param;

            SourceInfo sourceInfo = aryParams[0] as SourceInfo;            
            DataTable tbLog = (DataTable)aryParams[1];
            Regex regexDivider = (Regex)aryParams[2];
            Regex regexContents = (Regex)aryParams[3];

            bool bIsSucess = false;
            int nPercent = 0;
            int nLineIndex = 0;            
            List<Task> aryTasks = new List<Task>();

            try
            {
                using (StreamReader reader = new StreamReader(sourceInfo.FullFilename))
                {
                    long lnSize = reader.BaseStream.Length;

                    StringBuilder sbBuffer = new StringBuilder();
                    while (!reader.EndOfStream)
                    {
                        nLineIndex++;
                        long lnPosition = reader.BaseStream.Position;

                        int nCurPercent = (int)(lnPosition * 100 / lnSize * 0.9) ;
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
                                            DataRow newRow = tbLog.NewRow();
                                            newRow["source"] = sourceInfo.Name;
                                            newRow["line"] = nLineIndex;
                                            string sLogContents = sContent.Replace(match.Value, "").Trim();

                                            if (!string.IsNullOrEmpty(sLogContents))
                                            {
                                                newRow["content"] = sLogContents;
                                                foreach (StructInfo structInfo in ConfigManager.Current.AllStructures)
                                                {
                                                    if (!string.IsNullOrEmpty(structInfo.RegexName))
                                                        newRow[structInfo.Name] = match.Groups[structInfo.RegexName].Value.Trim();
                                                }
                                                tbLog.Rows.Add(newRow);
                                            }
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
                bIsSucess = true;
            }
            catch(Exception ex)
            {
                
            }
            OnCompleted?.Invoke(bIsSucess, sourceInfo.Name, tbLog);
        }

        internal bool IsSourceContained(string sLogFilename)
        {
            SourceInfo foundInfo = AllSources.Find(m => string.Equals(sLogFilename, m.FullFilename));
            return foundInfo != null;
        }

        private void BuildupLogTable()
        {
            m_tbLog = new DataTable();
            System.Data.DataColumn column;

            foreach (StructInfo info in ConfigManager.Current.AllStructures)
            {
                column = new DataColumn();
                column.Caption = info.Caption;
                column.ColumnName = info.Name;
                this.m_tbLog.Columns.Add(column);
            }

            this.m_tbLog.TableName = "tbLog";
        }
    }
}
