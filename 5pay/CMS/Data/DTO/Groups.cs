using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Data.DTO
{
    [Serializable]
    public class Groups
    {
        public int GroupID { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public bool IsActive { get; set; }
        public int Type { get; set; }
    }
}
