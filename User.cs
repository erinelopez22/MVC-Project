using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Snapshot_API.Models
{
    public class User
    {
        public string EmpID { get; set; }
        public string EmpName { get; set; }
        public string WhsCode { get; set; }
        public string Dept { get; set; }
        public string UType { get; set; }
        public int DeptCode { get; set; }
        public int SecCode { get; set; }
        public string FName { get; set; }
        public string Role { get; set; }
    }
}