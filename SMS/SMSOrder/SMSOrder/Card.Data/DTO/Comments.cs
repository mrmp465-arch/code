using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Data.DTO
{
    [Serializable]
    public class Comments
    {
        public int Id { get; set; }
        public string Title { get; set; }
     
        public string Description { get; set; }
        public int Status { get; set; }
        public int NewsId { get; set; }
        public DateTime PublishDate { get; set; }
        public string CreatedUser { get; set; }
    }
}
