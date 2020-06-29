using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LogParse
{
    class ConfigManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ConfigManager Current
        {
            get;
            private set;
        }

        static ConfigManager()
        {
            if (Current == null)
                Current = new ConfigManager();        
        }

        public string TableName
        {
            get;
            set;
        }

        public ParserInfo[] AllParserInfo
        {
            get
            {
                return m_aryParserInfos.ToArray();
            }
        }

        public StructInfo [] AllStructures
        {
            get
            {
                return m_aryStructInfos.ToArray();
            }
        }


        private List<StructInfo> m_aryStructInfos = new List<StructInfo>();
        private List<ParserInfo> m_aryParserInfos = new List<ParserInfo>();


        private ConfigManager()
        {
            Load();
        }

                                                                                                    
        private void Load()
        {
            m_aryParserInfos.Clear();

            if (!File.Exists("ParseInfos.xml"))
                return;

            using (StreamReader reader = new StreamReader("ParseInfos.xml"))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);

                XmlNodeList parseInfoNodes = doc.SelectNodes("//ParseInfos/ParseInfo");
                foreach(XmlNode node in parseInfoNodes)
                {
                    ParserInfo info = new ParserInfo();
                    if(info.Load(node))
                        m_aryParserInfos.Add(info);
                }

                XmlNodeList structInfoNodes = doc.SelectNodes("//Structure/Colomn");
                foreach(XmlNode node in structInfoNodes)
                {
                    StructInfo info = new StructInfo();
                    if (info.Load(node))
                    {                        
                        m_aryStructInfos.Add(info);
                    }
                }
                m_aryStructInfos.Sort((m, n) => m.DisplayOrder - n.DisplayOrder);

                StructInfo foundInfo;
                foundInfo = m_aryStructInfos.Find(m => "source".Equals(m.Name));
                if (foundInfo == null)
                {
                    StructInfo info = new StructInfo();
                    info.Caption = "Source";
                    info.Name = "source";
                    info.GridWidth = 50;
                    m_aryStructInfos.Insert(0, info);
                }

                foundInfo = m_aryStructInfos.Find(m => "line".Equals(m.Name));
                if (foundInfo == null)
                {
                    StructInfo info = new StructInfo();
                    info.Caption = "Line";
                    info.Name = "line";
                    info.GridWidth = 35;
                    m_aryStructInfos.Insert(1, info);
                }

                foundInfo = m_aryStructInfos.Find(m => "content".Equals(m.Name));
                if(foundInfo == null)
                {
                    StructInfo info = new StructInfo();
                    info.Caption = "Content";
                    info.Name = "content";                    
                    info.IsReadOnly = false;
                    info.IsMultiEditor = true;
                    info.IsBestFit = true;
                    m_aryStructInfos.Add(info);
                }

                for (int i = 0; i < m_aryStructInfos.Count; i++)
                    m_aryStructInfos[i].DisplayOrder = i;

                this.TableName = doc.SelectSingleNode("//Structure").Attributes["TableName"].Value;
            }
        }
    }
}
