using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LogParse
{
    public class StructInfo
    {
        public string Name
        {
            get;
            set;
        }

        public string RegexName
        {
            get;
            set;
        }

        public string Caption
        {
            get;
            set;
        }

        public int GridWidth
        {
            get;
            set;
        }

        public int DisplayOrder
        {
            get;
            set;
        }

        [DefaultValue(true)]
        public bool IsReadOnly
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool IsMultiEditor
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool IsBestFit
        {
            get;
            set;
        }


        public bool Load(XmlNode nodeStructInfo)
        {
            if (string.Equals("Colomn", nodeStructInfo.Name))
            {
                this.Name = nodeStructInfo.Attributes["name"].Value;
                this.RegexName = nodeStructInfo.Attributes["regname"].Value;
                this.Caption = nodeStructInfo.Attributes["caption"].Value;

                int nGridWidth = 0;
                int nDisplyOrder = -1;

                int.TryParse(nodeStructInfo.Attributes["gridwidth"].Value, out nGridWidth);
                int.TryParse(nodeStructInfo.Attributes["disporder"].Value, out nDisplyOrder);

                this.GridWidth = nGridWidth;
                this.DisplayOrder = nDisplyOrder;
            }

            return true;
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", this.Caption, this.Name);
        }



    }
}
