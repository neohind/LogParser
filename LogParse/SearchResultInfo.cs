using DevExpress.CodeParser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogParse
{
    public class SearchResultInfo
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SearchResultInfo(int nDataSourceIndex, DataRowView row)
        {
            this.DataSourceIndex = nDataSourceIndex;
            this.DataSource = row;
        }

        public string SourceName
        {
            get
            {
                try
                {
                    if (DataSource.Row.Table.Columns.Contains("source"))
                        return DataSource["source"] as string;
                    return string.Empty;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                return string.Empty;
            }
        }

        public string SourceLine
        {
            get
            {
                try
                {
                    if (DataSource.Row.Table.Columns.Contains("line"))
                        return DataSource["line"] as string;
                }
                catch(Exception ex)
                {
                    log.Error(ex);
                }
                return string.Empty;
            }
        }


        public int DataSourceIndex
        {
            get;
            set;
        }

        public DataRowView DataSource
        {
            get;
            set;
        }

        public override string ToString()
        {
            try
            {
                if (DataSource != null && DataSource.DataView.Table.Columns.Contains("content"))
                    return DataSource["content"].ToString();
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }
            return string.Empty;
        }
    }
}
