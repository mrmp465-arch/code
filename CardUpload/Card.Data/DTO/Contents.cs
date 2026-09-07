using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card.Data.DTO
{
    [Serializable]
    public class Contents
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public int IsHot { get; set; }
        public DateTime PublishDate { get; set; }
        public string CreatedUser { get; set; }
    }
}
