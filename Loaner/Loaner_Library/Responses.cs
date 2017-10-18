using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Loaner_Library
{
    [XmlRoot("Responses")]
    public class Responses
    {
        //[XmlIgnore]
        public string Aggregation_ID { get; set; }
        [XmlElement("Response")]
        public List<UniversalResponseFinal> MasterList = new List<UniversalResponseFinal>();
        //[XmlIgnore]
        public int ExpectedResponses { get; set; }





        public bool CheckIfFull()
        {
            if (MasterList.Count() == ExpectedResponses)
            {
                return true;
            }
            else return false;
        }
    }


    
}
