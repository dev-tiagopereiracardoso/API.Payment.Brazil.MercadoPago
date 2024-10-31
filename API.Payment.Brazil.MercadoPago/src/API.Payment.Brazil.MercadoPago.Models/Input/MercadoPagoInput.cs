namespace API.Payment.Brazil.MercadoPago.Models.Input
{
	public class MercadoPagoInput
	{
		public decimal ValueTransaction { set; get; }

		public string Description { set; get; }

		public string FirstName { set; get; }

		public string LastName { set; get; }

		public string Email { set; get; }

		public string DocumentNumber { set; get; }

		public string UidInvoincing { set; get; }
	}
}