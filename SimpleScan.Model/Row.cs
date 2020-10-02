using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleScan.Model
{
    public class Row : SimpleScan.Core.IBaseEntity
    {
        public int Id { get; set; }
        public string Pallet { get; set; }
        public string Carton { get; set; }
        public string SsCc { get; set; }
        public DateTime Date { get; set; }      
    }
}
