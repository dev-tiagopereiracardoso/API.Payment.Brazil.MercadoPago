using API.Payment.Brazil.MercadoPago.Domain.Implementation.Interfaces;
using API.Payment.Brazil.MercadoPago.Models.Input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Payment.Brazil.MercadoPago.Service.Controllers
{
	[Route("v1/payment/brazil/mercado-pago")]
	[ApiController]
	[ApiExplorerSettings(GroupName = "brazil")]
	[Authorize]
	public class MercadoPagoController : ControllerBase
	{
		private readonly ILogger<MercadoPagoController> _logger;

		private readonly IMercadoPagoService _mercadoPagoService;

		public MercadoPagoController(
				ILogger<MercadoPagoController> logger,
				IMercadoPagoService mercadoPagoService
			)
		{
			_logger = logger;
			_mercadoPagoService = mercadoPagoService;
		}

		[HttpPost("pix/generate")]
		public IActionResult Generate(MercadoPagoInput mercadoPagoInput)
		{
			var Data = _mercadoPagoService.Generate(mercadoPagoInput);

			return Ok(Data);
		}

		[HttpPost("pix/checking/status/payment")]
		public IActionResult CheckingPayment(MercadoPagoCheckingPaymentInput mercadoPagoCheckingPaymentInput)
		{
			var Data = _mercadoPagoService.CheckingPayment(mercadoPagoCheckingPaymentInput);

			return Ok(Data);
		}
	}
}
