using Libs.API;
using Libs.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.Report
{
    public   class PartnerGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<PartnerGroup> GetList()
        {
            return new List<PartnerGroup> { new PartnerGroup { Id = 1, Name = "" } };
        }
    }
}
