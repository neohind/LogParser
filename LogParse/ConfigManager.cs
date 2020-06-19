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
                        m_aryStructInfos.Add(info);
                }

                this.TableName = doc.SelectSingleNode("//Structure").Attributes["TableName"].Value;
            }
        }
    }
}
