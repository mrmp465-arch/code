using System;

namespace Card.Data.DTO
{
    [Serializable]
    public class Functions
	{
		public int FunctionID { get; set; }

		public string FunctionName { get; set; }

        public string FunctionCode { get; set; }

        public string Url { get; set; }

		public string UrlDisplay { get; set; }

		public bool IsDisplay { get; set; }

        public bool NewStatus { get; set; }

		public bool IsActive { get; set; }

		public DateTime CreatedTime { get; set; }

		public int FatherID { get; set; }
        public string FatherName { get; set; }

		public int Order { get; set; }

		public int SystemID { get; set; }

		public string SystemName { get; set; }
        public string IconId { get; set; }

		public int Counter { get; set; }
	}
}
