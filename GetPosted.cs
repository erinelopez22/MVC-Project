using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Snapshot_API.Models
{
    public class GetPosted
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public String whcode { get; set; }
        
    }
    public class GetPostedHdr
    {
        public int year { get; set; }
        public string whscode { get; set; }
    }
    public class GetPostedDet
    {
        public int DocNum { get; set; }

    }
}