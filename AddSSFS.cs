using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Snapshot_API.Models
{
    public class AddSSFSH
    {
        public Int32 DocNum { get; set; }
        public string WhsCode { get; set; }
        public Int32 Yr { get; set; }
        public Int32 Pd { get; set; }
        public Decimal Total { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }

        public List<AddSSFS1> Details { get; set; }
    }
    public class AddSSFS1
    {
        public Int32 DocNum { get; set; }
        public Int32 DocEntry { get; set; }
        public string TBType { get; set; }
        public long Amount { get; set; }

    }
}