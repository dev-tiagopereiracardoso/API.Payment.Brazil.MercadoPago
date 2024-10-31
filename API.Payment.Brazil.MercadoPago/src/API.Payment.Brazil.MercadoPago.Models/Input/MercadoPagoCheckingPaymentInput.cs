namespace API.Payment.Brazil.MercadoPago.Models.Input
{
	public class MercadoPagoCheckingPaymentInput
	{
		public long Id { set; get; }

		public int TimeWaitPayment { set; get; }

		public DateTime InitialPaymentPix { set; get; }

		public string UidInvoincing { set; get; }
	}
}