﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBank
{
    [Serializable]
    public class UniversalResponse
    {
        public string ssn { get; set; }
        public decimal interestrate { get; set; }
    }
}