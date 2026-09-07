using System;

namespace SMS.Data.DTO
{
    [Serializable]
    public class TransactionSims
	{
		public int Id { get; set; }

		public string Number { get; set; }

        public int Amount { get; set; }

        public int Type { get; set; }

		public string Description { get; set; }

        public string Note { get; set; }

        public DateTime Time { get; set; }

        public string ClientIP { get; set; }


	}
}
