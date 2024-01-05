using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPAG.Models;
using NPAG.Services;
using System;
using System.Linq;


namespace NPAG.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private IUserService _userService;
        private ILogService _logService;

        public PaymentController(IUserService userService, ILogService logService)
        {
            _userService = userService;
            _logService = logService;
        }

        [AllowAnonymous]
        [HttpGet]
        //[RequireHttps]
        public IActionResult Get()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            return Ok("NPAG version: " + version);

        }

        [AllowAnonymous]
        [HttpPost("CRMPAYVERIFY")]
        public IActionResult Authenticate([FromBody] VerifyRequest model)
        {
            try
            {
                var response = _userService.Authenticate(model, ipAddress());

                //if (response == null)
                //    return BadRequest(new { message = "Authentication details incorrect" });

                string logReturnedVal = response.CustomerName + ", " + response.ContactNumber + ", " + response.BalanceAmount;

                _logService.WriteSucessLog("CRMPAYVERIFY", logReturnedVal, model.OrderNumber, model.CompanyCode.ToString(), model.BranchCode.ToString(), "No-Amount", "No-DateTime", "No UID", model.SecurityCode, "No-ReceiptNo", ipAddress());

                return Ok(response);
            }
            catch (Exception ex)
            {
                //string ReturnMsg, string OrderNummber, string CompanyCode, string BranchCode, string PaymentAmount, string PaymentDateTime, string UID, string SecuCode, string ReceiptNo, string Ip
                _logService.WriteLogRejectPayment("CRMPAYVERIFY", ex, model.OrderNumber, model.CompanyCode.ToString(), model.BranchCode.ToString(), "No-Amount", "No-DateTime", "No UID", model.SecurityCode, "No-ReceiptNo", ipAddress());
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("CRMPAYACCEPT")]
        public IActionResult PostPayment([FromBody] PayRequest model)
        {
            try
            {

                string OrderNumber = User.Claims.Where(c => c.Type == "OrderNumber")
                    .Select(x => x.Value).FirstOrDefault();

                string CompanyCode = User.Claims.Where(c => c.Type == "CompanyCode")
                    .Select(x => x.Value).FirstOrDefault();

                string BranchCode = User.Claims.Where(c => c.Type == "BranchCode")
                    .Select(x => x.Value).FirstOrDefault();

                string SecureCode = User.Claims.Where(c => c.Type == "SecurityCode")
                    .Select(x => x.Value).FirstOrDefault();

                string PaymentMode = User.Claims.Where(c => c.Type == "PaymentMode")
                    .Select(x => x.Value).FirstOrDefault();

                model.OrderNumber = OrderNumber;
                model.CompanyCode = CompanyCode;
                model.BranchCode = BranchCode;
                model.SecurityCode = SecureCode;
                model.PaymentMode = PaymentMode;


                var response = _userService.Payment(model, ipAddress());

                _logService.WriteSucessLog("CRMPAYACCEPT", response, model.OrderNumber, model.CompanyCode.ToString(), model.BranchCode.ToString(), model.PaymentAmount.ToString(), model.PaymentDate, model.UserID, model.SecurityCode, model.ReceiptNumber, ipAddress());
                //if (response == null)
                //    return BadRequest(new { message = "Authentication details incorrect" });

                //return Ok(OrderNumber + " " + CompanyCode+" " + BranchCode+" " + SecureCode);
                return Ok(response);
            }
            catch (Exception ex)
            {
                //string ReturnMsg, string OrderNummber, string CompanyCode, string BranchCode, string PaymentAmount, string PaymentDateTime, string UID, string SecuCode, string ReceiptNo, string Ip
                _logService.WriteLogRejectPayment("CRMPAYACCEPT", ex, model.OrderNumber, model.CompanyCode.ToString(), model.BranchCode.ToString(), model.PaymentAmount.ToString(), model.PaymentDate, model.UserID, model.SecurityCode, model.ReceiptNumber, ipAddress());
                return BadRequest(new { message = ex.Message });
            }
        }

        // helper methods

        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
