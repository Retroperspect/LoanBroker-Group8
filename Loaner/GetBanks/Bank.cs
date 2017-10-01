using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GetBanks
{
    
    public class Bank
    {
        [XmlElement("format")]
        public string format { get; set; }
        [XmlElement("Input")]
        public string Input { get; set; }
        [XmlElement("Output")]
        public string Output { get; set; }
    }
}
