using API.Payment.Brazil.MercadoPago.Models.Input;
using API.Payment.Brazil.MercadoPago.Models.Output;

namespace API.Payment.Brazil.MercadoPago.Domain.Implementation.Interfaces
{
	public interface IMercadoPagoService
	{
		PixMercadoPago? Generate(MercadoPagoInput mercadoPagoInput);

		MercadoPagoCheckingPaymentOutput? CheckingPayment(MercadoPagoCheckingPaymentInput mercadoPagoCheckingPaymentInput);
	}
}