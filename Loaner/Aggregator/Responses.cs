using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Aggregator
{
    [XmlRoot("Responses")]
    public class Responses
    {
        //[XmlIgnore]
        public string Aggregation_ID { get; set; }
        [XmlElement("Response")]
        public List<UniversalResponse> MasterList = new List<UniversalResponse>();
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
