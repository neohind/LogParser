using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace LogParse
{
    public class ParserInfo
    {
        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }


        public string RegexStrDivider
        {
            get;
            set;
        }

        public string RegexStrContents
        {
            get;
            set;
        }

        public Regex RegexForDivider
        {
            get;
            private set;
        }

        public Regex RegexForContents
        {
            get;
            private set;
        }
           
        public bool Load(XmlNode nodeParseInfo)
        {
            if(string.Equals("ParseInfo", nodeParseInfo.Name))
            {
                this.Name = nodeParseInfo.Attributes["Name"].Value;
                this.Description = nodeParseInfo.Attributes["Description"].Value;
                this.RegexStrDivider = nodeParseInfo.SelectSingleNode("Divider").InnerText;
                this.RegexStrContents = nodeParseInfo.SelectSingleNode("Contents").InnerText;

                try
                {
                    RegexForDivider = new Regex(this.RegexStrDivider);
                    RegexForContents = new Regex(this.RegexStrContents);
                }
                catch(Exception ex)
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", this.Description, this.Name);
        }
    }
}
