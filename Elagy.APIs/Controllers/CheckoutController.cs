using Elagy.Core.DTOs.Booking;
using Elagy.Core.Enums;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;


namespace Elagy.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : BaseApiController
    {

        IBookingService _bookingService;
        private readonly SessionService _sessionService;

        public CheckoutController(IBookingService bookingService, SessionService sessionService)
        {
            _bookingService = bookingService;
            _sessionService = sessionService;
        }

        [HttpPost("CreateBooking")]
        public async Task<IActionResult> CreateBookingAsync([FromBody] CreateBookingRequest request)
        {
            if (request == null)
            {
                return BadRequest("Booking request cannot be null.");
            }
            
            try
            {
                var booking = await _bookingService.CreatePendingBookingAsync(GetCurrentUserId(), request);
 
                // Step 3: Prepare Line Items from the retrieved booking details
                var lineItems = new List<SessionLineItemOptions>();

                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)(booking.SpecialtyAppoinment.price * 100), // Convert decimal to cents
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "SpecialtyAppoinemtn",
                            Description = "this is a specialty reservation for ",
                        },
                    },
                    Quantity = 1,
                });

                if(booking.RoomAppointment != null)
                {
                    lineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            UnitAmount = (long)(booking.RoomAppointment.price* 100), // Convert decimal to cents
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Room Appoinemtn",
                                Description = "this is a room reservation for ",
                            },
                        },
                        Quantity = 1,
                    });
                }

                if(booking.CarAppointment != null)
                {
                    lineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            UnitAmount = (long)(booking.CarAppointment.price* 100), // Convert decimal to cents
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Car Appoinemtn",
                                Description = "this is a car reservation for ",
                            },
                        },
                        Quantity = 1,
                    });
                }

                // Step 4: Define Metadata for the Checkout Session
                var metadata = new Dictionary<string, string>
                {
                    { "booking_id", booking.Id.ToString() },
                    { "customer_id", booking.PatientId },
                    // Add any other relevant data from your 'booking' object
                };

                // Step 5: Create SessionCreateOptions
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = lineItems,
                    Mode = "payment",
                     SuccessUrl = "http://localhost:4200/payment-success={CHECKOUT_SESSION_ID}&bookingId=123",
                     CancelUrl = "https://yourdomain.com/payment-cancelled?bookingId=123",

                    //$"{_successUrl}?session_id={{CHECKOUT_SESSION_ID}}&bookingId={booking.Id}",
                    //CancelUrl = $"{_cancelUrl}?bookingId={booking.Id}",
                    Metadata = metadata,
                    CustomerEmail = GetCurrentUserEmial(), // Optional: Set the customer's email for the session
                };

                // Step 6: Create the Checkout Session
                var session = await _sessionService.CreateAsync(options);

                // Step 7: Optionally, update your booking record with the Stripe Session ID
                // This links your internal booking to the Stripe session for easier lookup
                //await _bookingService.UpdateBookingStatusAsync(booking.Id, BookingStatus.PendingPayment, session.Id);

                // Step 8: Return the Checkout Session URL to the frontend
                return Ok(new CreateCheckoutSessionResponse { CheckoutSessionUrl = session.Url });

                

            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the booking: " + ex.Message);
            }
        }


    }
}
