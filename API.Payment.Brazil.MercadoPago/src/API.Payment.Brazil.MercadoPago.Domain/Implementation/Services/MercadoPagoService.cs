using API.Payment.Brazil.MercadoPago.Domain.Implementation.Interfaces;
using API.Payment.Brazil.MercadoPago.Models.Input;
using API.Payment.Brazil.MercadoPago.Models.Output;
using MercadoPago.Client.Common;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace API.Payment.Brazil.MercadoPago.Domain.Implementation.Services
{
	public class MercadoPagoService : IMercadoPagoService
	{
		private readonly ILogger<MercadoPagoService> _logger;

		private string _accessTokenMercadoPago { get; set; }

		public MercadoPagoService(
				ILogger<MercadoPagoService> logger,
				IConfiguration configuration
			)
		{
			_logger = logger;

			_accessTokenMercadoPago = configuration["AccessTokenMercadoPago"]!;
		}

		public PixMercadoPago? Generate(MercadoPagoInput mercadoPagoInput)
		{
			MercadoPagoConfig.AccessToken = _accessTokenMercadoPago;

			var Pix = this.Pix(mercadoPagoInput.ValueTransaction, mercadoPagoInput.Description, mercadoPagoInput.FirstName, mercadoPagoInput.LastName, mercadoPagoInput.Email, mercadoPagoInput.DocumentNumber, mercadoPagoInput.UidInvoincing).Result;
			if (Pix.PixCopiaCola != null)
			{
				return Pix;
			}
			else
			{
				_logger.LogInformation($"There was an error sending the PIX creation request to the payment gateway!");

				return null;
			}
		}

		public MercadoPagoCheckingPaymentOutput? CheckingPayment(MercadoPagoCheckingPaymentInput mercadoPagoCheckingPaymentInput)
		{
			MercadoPagoConfig.AccessToken = _accessTokenMercadoPago;

			var resultChecking = GetPix(mercadoPagoCheckingPaymentInput.Id);
			var checkingPaymentOutput = new MercadoPagoCheckingPaymentOutput()
			{
				WasPaidOut = false
			};

			try
			{
				switch (resultChecking)
				{
					case "pending":
						checkingPaymentOutput.Status = resultChecking;
						if (Convert.ToInt32(((int)mercadoPagoCheckingPaymentInput.InitialPaymentPix.Subtract(DateTime.Now).TotalSeconds).ToString().Replace("-", "")) > mercadoPagoCheckingPaymentInput.TimeWaitPayment)
						{
							if (CancelPix(mercadoPagoCheckingPaymentInput.Id))
							{
								checkingPaymentOutput.Status = "cancelled";
								checkingPaymentOutput.Message = $"PIX payment canceled, waiting timeout reached 60 seconds.";
							}
							else
							{
								checkingPaymentOutput.Message = $"There was an error canceling PIX, process completed, try again.";
							}
						}
						else
						{
							checkingPaymentOutput.Message = $"Waiting for payment.";
						}
						break;
					case "error":
						checkingPaymentOutput.Status = resultChecking;
						CancelPix(mercadoPagoCheckingPaymentInput.Id);
						checkingPaymentOutput.Message = $"There was an error trying to validate the connection with the gateway, process waiting for the webhook.";
						break;
					case "success":
						checkingPaymentOutput.WasPaidOut = true;
						checkingPaymentOutput.Status = resultChecking;
						checkingPaymentOutput.Message = $"Customer made payment successfully!";
						break;
					case "cancelled":
						checkingPaymentOutput.Status = resultChecking;
						checkingPaymentOutput.Message = $"PIX was cancelled at another time";
						break;
					default:
						checkingPaymentOutput.Message = $"Item ({resultChecking}) not mapped.";
						break;
				}
			}
			catch (Exception Ex)
			{
				checkingPaymentOutput.Status = "error";
				checkingPaymentOutput.Message = $"Item ({resultChecking}) not mapped. Error - " + Ex.Message;
			}

			return checkingPaymentOutput;
		}

		private async Task<PixMercadoPago> Pix(decimal ValueTransaction, string Description, string FirstName, string LastName, string Email, string DocumentNumber, string UidInvoincing)
		{
			MercadoPagoConfig.AccessToken = _accessTokenMercadoPago;

			var request = new PaymentCreateRequest
			{
				TransactionAmount = ValueTransaction,
				Description = Description,
				PaymentMethodId = "pix",
				BinaryMode = true,
				Payer = new PaymentPayerRequest
				{
					FirstName = FirstName,
					LastName = LastName,
					Email = Email,
					Identification = new IdentificationRequest()
					{
						Type = "CPF",
						Number = DocumentNumber
					}
				},
				AdditionalInfo = new PaymentAdditionalInfoRequest()
				{
					Items = new List<PaymentItemRequest>()
				}
			};

			request.AdditionalInfo.Items.Add(new PaymentItemRequest() { });

			var client = new PaymentClient();
			var payment = await client.CreateAsync(request);

			return new PixMercadoPago()
			{
				Id = (long)payment.Id!,
				Status = payment.Status,
				PixQRCode = payment.PointOfInteraction.TransactionData.QrCodeBase64,
				PixCopiaCola = payment.PointOfInteraction.TransactionData.QrCode
			};
		}

		private string GetPix(long Id)
		{
			var client = new PaymentClient();
			return client.Get(Id).Status;
		}

		private bool CancelPix(long Id)
		{
			var client = new PaymentClient();
			return client.Cancel(Id).Status.Equals("cancelled");
		}
	}
}