using NPAG.Entities;
using System;
using System.Linq;
using NPAG.Helpers;
using Microsoft.Extensions.Options;
using NPAG.Models;
using System.Security.Authentication;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Globalization;
using System.Security.Cryptography;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace NPAG.Services
{
    public interface IUserService
    {
        VerifyResponse Authenticate(VerifyRequest model, string ipAddress);
        string Payment(PayRequest model, string ipAddress);
        //IEnumerable<Users> GetAll();
        CRM_PAYMENT PostData(CRM_PAYMENT paymment);

    }
    public class UserService : IUserService
    {
        private DataContext _context;
        private readonly AppSettings _appSettings;
        //private LogService _logService;

        public UserService(
            DataContext context,
            IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value; 
        }

        //public IEnumerable<Users> GetAll()
        //{
        //    return _context.Users;
        //}


        public VerifyResponse Authenticate(VerifyRequest model, string ipAddress)
        {
            var jwtToken = "";

            try
            {
                //check if SecureCode is empty
                if (model.SecurityCode == null || model.SecurityCode == "") throw new AuthenticationException("Error-6010: Invalid Security Code");

                //check if Company Code is empty
                if (model.CompanyCode == null || model.CompanyCode.ToString() == "") throw new AuthenticationException("Error-6003: Invalid Company Code");

                //check if Branch Code is empty
                if (model.BranchCode == null || model.BranchCode.ToString() == "") throw new AuthenticationException("Error-6004: Invalid Branch Code");

                //check if Order Number is empty
                if (model.OrderNumber == null || model.OrderNumber == "") throw new AuthenticationException("Error-6002: Invalid Order Number");

                //decrypt Secure code recived
                var decryptedSec = Decrypt(model.SecurityCode.ToString());
                  var secCodefromReq = decryptedSec;

                //get matching securecode from DB
                var compUser = _context.NPAG_COMPANYDATAs.Where(x => x.BRANCHCODE == model.BranchCode && x.COMPCODE == model.CompanyCode).FirstOrDefault();

                //return null if two secure codes dont match or no matching result in COMPANYDATA
                if (compUser == null || ( compUser.SECUCODE.ToString() != secCodefromReq)) throw new AuthenticationException("Error-6012: User Authorization Failed"); ;

                //confirm order nummber from DB
                var orderData = _context.CRM_INVOICESs.Where(x => x.ORDER_NO == model.OrderNumber && x.ORDER_STATUS == "VERIFIED").FirstOrDefault();

                if (orderData == null) throw new AuthenticationException("Error-6001: Invalid Order Number");

                if (orderData.BILLING_ACC_TYPE != "POSTPAID") throw new AuthenticationException("Error-6015: Unable to proceed payments for CRM Pre-Paid Invoices");

                if (orderData.ISPAID == "1")
                {
                    //get payment details from CRM_PAYMENT
                    var payDetails = _context.CRM_PAYMENTs.Where(x => x.ORDER_NUMBER == model.OrderNumber).FirstOrDefault();

                    if (payDetails == null)throw new AuthenticationException("No any realated payment found to OrderNo(in PAYMENT tbl)");

                    if (payDetails.BRANCHCODE != null && payDetails.BANKCODE != null)
                    {
                        //get matching COMPANYDATA 
                        var matchingCompNameBname = _context.NPAG_COMPANYDATAs.Where(x => x.BRANCHCODE.ToString() == payDetails.BRANCHCODE && x.COMPCODE.ToString() == payDetails.BANKCODE).Select(x=> new { x.COMPNAME, x.BRANCHNAME }).FirstOrDefault();
                        if (matchingCompNameBname == null) throw new AuthenticationException("No matching companydata found in table related to already paid invoice compcode/branchcode");
                        throw new AuthenticationException("Error-6014: Payment is already Paid by " + matchingCompNameBname.BRANCHNAME.ToString() + "/" + matchingCompNameBname.COMPNAME.ToString() + " ("+ payDetails.CRMPAYMENTID.ToString()+ ")");
                    }
                    if (payDetails.BRANCHCODE == null && payDetails.BANKCODE == null) throw new AuthenticationException("Error-6013: Payment is already Paid by the SLT bill center");

                }

                // authentication successful so generate jwt 
                else jwtToken = generateJwtToken(model, compUser.PAYMENTMODE);

                return new VerifyResponse(orderData, jwtToken);

            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public string Payment(PayRequest model, string ipAddress)
        {

            try
            {
                //confirm order nummber from DB
                var invoiceData = _context.CRM_INVOICESs.Where(x => x.ORDER_NO == model.OrderNumber && x.ORDER_STATUS == "VERIFIED").FirstOrDefault();

                if (invoiceData == null) throw new AuthenticationException("Error-6001: Invalid Order Number");

                //...............

                if (invoiceData.ISPAID == "1")
                {
                    //get payment details from CRM_PAYMENT
                    var payDetails = _context.CRM_PAYMENTs.Where(x => x.ORDER_NUMBER == model.OrderNumber).FirstOrDefault();

                    if (payDetails == null) throw new AuthenticationException("No any realated payment found to OrderNo(in PAYMENT tbl)");

                    if (payDetails.BRANCHCODE != null && payDetails.BANKCODE != null)
                    {
                        //get matching COMPANYDATA 
                        var matchingCompNameBname = _context.NPAG_COMPANYDATAs.Where(x => x.BRANCHCODE.ToString() == payDetails.BRANCHCODE && x.COMPCODE.ToString() == payDetails.BANKCODE).Select(x => new { x.COMPNAME, x.BRANCHNAME }).FirstOrDefault();
                        throw new AuthenticationException("Error-6014: Payment is already Paid by " + matchingCompNameBname.BRANCHNAME.ToString() + "/" + matchingCompNameBname.COMPNAME.ToString());
                    }
                    if (payDetails.BRANCHCODE == null && payDetails.BANKCODE == null) throw new AuthenticationException("Error-6013: Payment is already Paid by the SLT bill center");

                }

                //check decimal points (not more than 2)
                int decimlCount = BitConverter.GetBytes(decimal.GetBits(model.PaymentAmount)[3])[2];

                if (decimlCount > 2) throw new AuthenticationException("Error-6005: Invalid Payment Amount");

                // check if amount is less than zero
                if (Math.Round((model.PaymentAmount), 2, MidpointRounding.ToEven) <= 0) throw new AuthenticationException("Error-6005: Invalid Payment Amount");


                // check if amount is more than 49,999,999.99
                if (decimal.ToDouble(model.PaymentAmount) > 49999999.99) throw new AuthenticationException("Error-6005: Invalid Payment Amount");

                // if payment amount no equal to invoice amount
                if (model.PaymentAmount != decimal.Parse(invoiceData.BALANCE_AMOUNT)) throw new AuthenticationException("Error-6006: Invalid Payment Amount");

                //...............

               
                if (model.PaymentDate == null) throw new AuthenticationException("Error - 6007: Invalid Payment Date Time");

                //String[] payFormats = { "M/dd/yyyy hh:mm:ss tt", "MM/dd/yyyy hh:mm:ss tt", "M/d/yyyy hh:mm:ss tt", "MM/d/yyyy hh:mm:ss tt"};

                //DateTime expectedDT = DateTime.Now;

                //if (DateTime.TryParseExact(model.PaymentDate, payFormats,
                //                      System.Globalization.CultureInfo.InvariantCulture,
                //                      DateTimeStyles.None, out expectedDT)) throw new AuthenticationException("Error - 6008: Invalid Payment Date Time");

                string[] formats = { "M/dd/yyyy hh:mm:ss tt", "MM/dd/yyyy hh:mm:ss tt", "M/d/yyyy hh:mm:ss tt", "MM/d/yyyy hh:mm:ss tt" };
                //string[] formats = { "mm/dd/yyyy hh:mm: ss am/pm" };
                DateTime expectedDate;
                if (!DateTime.TryParseExact(model.PaymentDate, formats, System.Globalization.CultureInfo.InvariantCulture,
                                            DateTimeStyles.None, out expectedDate))
                {
                    throw new AuthenticationException("Error - 6008: Invalid Payment Date Time");
                }

                //check if user ID is empty
                if (model.UserID == "" || model.UserID == null) throw new AuthenticationException("Error-6009: Invalid User ID");

                //check for special characters in user ID
                var regx = new Regex("[^a-zA-Z0-9_.]");
                if (regx.IsMatch(model.UserID)) throw new AuthenticationException("Error-6009: Invalid User ID");


                //check if Receipt Number is invalid
                if (model.ReceiptNumber == "" || model.ReceiptNumber == null) throw new AuthenticationException("Error-6011: Invalid Receipt Number");

                //check if Receipt Number Length is invalid
                if (model.ReceiptNumber.Length >20) throw new AuthenticationException("Error-6011: Invalid Receipt Number");


                //Send payment to CRM_PAYMENT table

                CRM_PAYMENT payemnt = new CRM_PAYMENT();
                payemnt.PAYMENTDATE = DateTime.Parse(model.PaymentDate);
                payemnt.BANKCODE = model.CompanyCode.ToString();
                payemnt.BRANCHCODE = model.BranchCode.ToString();
                payemnt.PAYMENTMODE = model.PaymentMode;
                payemnt.CASHIER = model.UserID;
                payemnt.AMOUNT = model.PaymentAmount * 1000;
                payemnt.PAYMENTREF = "";
                payemnt.CURRENCY = "LKR";
                //payemnt.PAYSEQ_OUT
                //payemnt.ITEMACC_OUT
                //payemnt.ACCPAYSEQ_OUT
                payemnt.RECEIPTNUMBER = model.ReceiptNumber;
                //payemnt.CRMPAYMENTID 
                payemnt.BILLING_ACCOUNTNUMBER = invoiceData.BILLING_ACC_NO;
                //payemnt.CREATEDATE
                payemnt.CENTERCODE = "NPMC"; //previous NPBC
                //payemnt.ERRORCODE
                //payemnt.STATUS
                //payemnt.TRYCOUNT
                //payemnt.CANCELSTATUS
                //payemnt.PAYMENTIDORIGIN
                // payemnt.ACCOUNTNO_OUT
                payemnt.BILLING_TYPE = invoiceData.BILLING_ACC_TYPE;
                payemnt.ORDER_NUMBER = model.OrderNumber;
                payemnt.CUSTOMERREF = invoiceData.CUS_ACC_NO;
                //FDU,CFTS,000,On - Line,CF,CFTS0101C20111700001,CA,,172.21.11.21,1 - 725031916,CRM
                payemnt.PAYMENT_TEXT = "FDU,NPMC,000,On-Line,NP," + model.ReceiptNumber + "," + model.PaymentMode + ",," + ipAddress +","+ model.OrderNumber+ ",CRM";
                //payemnt.PAYMENT_TEXT = "FDU" + "," + model.ReceiptNumber.Substring(0, 4) + "," + "000"+ "," +"On-Line"+ "," + model.ReceiptNumber.Substring(0, 2) + "," + model.ReceiptNumber + "," + model.PaymentMode + ",," + ipAddress + "," + model.OrderNumber + "," + "CRM";
                //"FDU," + crmobj.Recept_Num.Substring(0, 4) + ",000,On-Line," + crmobj.Recept_Num.Substring(0, 2) + "," + crmobj.Recept_Num + "," + crmobj.strP_Mode1 + "," + crmobj.strPayment_No + "," + GlobalVar.LocalIP + "," + crmobj.intOrder_Num+ "," +"CRM";
                payemnt.ORDER_ID = invoiceData.ORDER_ID;
                //payemnt.CRM_RETURNMESSAGE
                //payemnt.CRM_RETURNCODE
                //payemnt.EDT_SEND = 0;

                
                // make ISPAID = 1 in CRM_INVOICES
                invoiceData.ISPAID = "1";
                //_context.SaveChanges();

                //Update invoiceData
                var entry = _context.Entry(invoiceData);
                (entry).CurrentValues.SetValues(invoiceData.ISPAID);

                CRM_PAYMENT result = PostData(payemnt);

                return result.CRMPAYMENTID.ToString();
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        //Helper Methods
        public CRM_PAYMENT PostData(CRM_PAYMENT paymment)
        {
            try
            {
                //confirm order nummber from DB
                var invoiceData = _context.CRM_PAYMENTs.Where(x => x.ORDER_NUMBER == paymment.ORDER_NUMBER).AsNoTracking().FirstOrDefault();

                if(invoiceData == null)
                {
                    var invoiceSaved = _context.CRM_PAYMENTs.Add(paymment);
                    _context.SaveChanges();
                    return invoiceSaved.Entity;

                }
                else
                {
                    ResetContextState();
                    throw new AuthenticationException("Error-6013: Payment is already Exist for the Order Number");
                }


            }
            catch (Exception er)
            {
                //string errorMessage = er.InnerException.Message;

                //if (errorMessage.Contains("ORA-00001") && errorMessage.Contains("UNIQUE_RECEIPT"))
                //{
                //    ResetContextState();
                //    throw new AuthenticationException("Error-6016: Receipt Number must be unique");
                //}
                //else
                //{
                //    throw er;
                //}
                throw er;
            }
        }

        //Below function is used to reset context state 
        private void ResetContextState() => _context.ChangeTracker.Entries()
        .Where(e => e.Entity != null).ToList()
        .ForEach(e => e.State = EntityState.Detached);

        //private bool validateUserRequest(string OrderNumber, decimal CompanyCode, decimal BranchCode, decimal SecureCode )
        //{
        //    try
        //    {
        //        //decrypt Secure code recived
        //        var secCodefromReq = SecureCode;

        //        //get matching securecode from DB
        //        var compUser = _context.COMPANYDATA.Where(x => x.BRANCHCODE == BranchCode && x.COMPCODE == CompanyCode).FirstOrDefault();

        //        //return null if two secure codes dont match or no matching result in COMPANYDATA
        //        if (compUser == null || (compUser.SECUCODE != secCodefromReq)) throw new AuthenticationException("Authentication details incorrect"); ;

        //        //confirm order nummber from DB
        //        var orderData = _context.CRM_INVOICES.Where(x => x.ORDER_NO == OrderNumber).FirstOrDefault();

        //        if (orderData == null) throw new AuthenticationException("Order Nummber not found");

        //        if (orderData.ISPAID == "1") throw new AuthenticationException("Payment already received");


        //        return true;
        //    }
        //    catch (Exception) {
        //        throw;
        //    }
        //}

        private string generateJwtToken(VerifyRequest compUser, string paymentMode)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            //var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var decryptdSecrt = Decrypt(_appSettings.Secret);
            var key = Encoding.ASCII.GetBytes(decryptdSecrt);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, (compUser.CompanyCode + compUser.BranchCode).ToString()),
                    new Claim("OrderNumber", compUser.OrderNumber.ToString()),
                    new Claim("CompanyCode", compUser.CompanyCode.ToString()),
                    new Claim("BranchCode", compUser.BranchCode.ToString()),
                    new Claim("SecurityCode", compUser.SecurityCode.ToString()),
                    new Claim("PaymentMode", paymentMode)
                }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public string Decrypt(String cipherText)
        {
            Byte[] initVectorBytes;
            Byte[] saltValueBytes;
            Byte[] cipherTextBytes;
            PasswordDeriveBytes password;
            Byte[] keyBytes;
            RijndaelManaged key;
            ICryptoTransform decryptor;
            MemoryStream memoryStream;
            CryptoStream cryptoStream;
            Byte[] plainTextBytes = null;
            int decryptedByteCount;

            String passPhrase = "Status#1";
            String saltValue = "Ep1kS@lt";
            String hashAlgorithm  = "SHA512"; //SHA256       //'You can use either MD5 or SHA1           
            int passwordIterations = 2;
            String initVector = "@1G2c7A4r5T6H7J4"; //'@1B2c3D4e5F6g7H8
            int keySize = 256;

            try
            {

                initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
                cipherTextBytes = Convert.FromBase64String(cipherText);


                password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);
                keyBytes = password.GetBytes(keySize / 8);
                key = new RijndaelManaged();
                key.Mode = CipherMode.CBC;
                decryptor = key.CreateDecryptor(keyBytes, initVectorBytes);


                memoryStream = new MemoryStream(cipherTextBytes);
                cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                //plainTextBytes.(cipherTextBytes.Length);
                // ReDim plainTextBytes(cipherTextBytes.Length)
                //plainTextBytes.Length = cipherTextBytes.Length;
                Array.Resize(ref plainTextBytes, cipherTextBytes.Length);
                decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);


                memoryStream.Close();
                cryptoStream.Close();
                string planText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

                return planText;

            }
            catch (Exception)
            {
                throw new AuthenticationException("Error-6010: Invalid Security Code");
            }
            
        }

    }
}
