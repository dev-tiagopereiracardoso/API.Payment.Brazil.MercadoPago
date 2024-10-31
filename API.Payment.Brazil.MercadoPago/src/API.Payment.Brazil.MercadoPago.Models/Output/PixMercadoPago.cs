namespace API.Payment.Brazil.MercadoPago.Models.Output
{
	public class PixMercadoPago
	{
		public long Id { set; get; }

		public string? Status { set; get; }

		public string? PixQRCode { set; get; }

		public string? PixCopiaCola { set; get; }
	}
}