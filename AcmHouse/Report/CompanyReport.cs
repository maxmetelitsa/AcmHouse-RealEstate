using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AcmHouse.Report
{
    public class CompanyReport
    {
        public string Date { get; set; }
        public int OrderID { get; set; }
        public double TotalPrice { get; set; }
        public double EmployeeProfit { get; set; }
        public double CompanyProfit { get; set; }
        public int UserID { get; set; }
        public string EmployeeName { get; set; } 
    }

}
