using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Element.Reveal.Crew.Lib
{
    class ParameterDTO
    {
        public int IntergerValue1 { get; set; }
        public int IntergerValue2 { get; set; }
        public int IntergerValue3 { get; set; }
        public int IntergerValue4 { get; set; }
        public int IntergerValue5 { get; set; }

        public decimal DecimalValue1 { get; set; }
        public decimal DecimalValue2 { get; set; }
        public decimal DecimalVlaue3 { get; set; }

        public string StringValue1 { get; set; }
        public string StringValue2 { get; set; }
        public string StringValue3 { get; set; }

        public Object CustomValue1 { get; set; }
        public Object CustomValue2 { get; set; }

        public DateTime DateValue1 { get; set; }
    }
}
