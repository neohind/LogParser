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

        public SearchResultInfo(int nDataSourceIndex, DataRow row)
        {
            this.DataSourceIndex = nDataSourceIndex;
            this.DataSource = row;

            if (DataSource != null && DataSource.Table.Columns.Contains("content")
                    && DataSource.Table.Columns.Contains("line"))
                DisplayText = string.Format("<{0,-6}> {1}", DataSource["line"], DataSource["content"]);
        }

        public string DisplayText
        {
            get;
            private set;
        }

        public string SourceName
        {
            get
            {
                try
                {
                    if (DataSource.Table.Columns.Contains("source"))
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
                    if (DataSource.Table.Columns.Contains("line"))
                        return DataSource["line"] as string;
                }
                catch (Exception ex)
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

        public DataRow DataSource
        {
            get;
            set;
        }

        public override string ToString()
        {
            return DisplayText;

        }
    }
}
