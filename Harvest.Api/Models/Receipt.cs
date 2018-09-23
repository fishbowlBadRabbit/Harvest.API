using System;
using System.Collections.Generic;
using System.Text;

namespace Harvest.Api
{
    public class Receipt
    {
        public string FileName { get; set; }
        public string Url { get; set; }
        public long? FileSize { get; set; }
        public string ContentType { get; set; }
    }
}
