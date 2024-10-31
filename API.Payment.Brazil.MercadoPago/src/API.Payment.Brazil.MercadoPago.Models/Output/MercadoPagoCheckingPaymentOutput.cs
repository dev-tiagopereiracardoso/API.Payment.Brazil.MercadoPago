namespace API.Payment.Brazil.MercadoPago.Models.Output
{
	public class MercadoPagoCheckingPaymentOutput
	{
		public bool WasPaidOut { set; get; }

		public string Status { set; get; }

		public string Message { set; get; }
	}
}