using System;

namespace CMS.Data.DTO
{
    [Serializable]
    public class Transactions
	{
		public int Id { get; set; }

		public string Username { get; set; }

        public int Amount { get; set; }
        public int Balance { get; set; }
        public int BalanceHold { get; set; }
        public int Type { get; set; }

        public int ActionType { get; set; }

        public string Description { get; set; }

        public string Note { get; set; }

        public DateTime Time { get; set; }

        public string ClientIP { get; set; }


	}
}
