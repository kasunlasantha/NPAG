using NPAG.Entities;
using NPAG.Helpers;
using System;
using System.IO;

namespace NPAG.Services
{
    public interface ILogService
    {
        void WriteLogRejectPayment(string MethodName, Exception ex, string OrderNummber, string CompanyCode, string BranchCode, string PaymentAmount, string PaymentDateTime, string UID, string SecuCode, string ReceiptNo, string Ip);
        void WriteSucessLog(string MethodName, string returned_values, string OrderNummber, string CompanyCode, string BranchCode, string PaymentAmount, string PaymentDateTime, string UID, string SecuCode, string ReceiptNo, string Ip);

    }

    public class LogService: ILogService
    {
        private DataContext _context;

        public LogService(DataContext context)
        {
            _context = context;

        }

        public void WriteLogRejectPayment(string MethodName, Exception exception, string OrderNummber, string CompanyCode, string BranchCode, string PaymentAmount, string PaymentDateTime, string UID, string SecuCode, string ReceiptNo, string Ip)
        {

            //-------------------------------------
            //StreamWriter oWrite = new StreamWriter("");
        // Dim FILE_NAME As String = "ErrorLog.txt"
        String ErrorPayPath = AppDomain.CurrentDomain.BaseDirectory + "Reject Payment Details";
            String ErrorPayPathMonthVise = ErrorPayPath + "\\" + DateTime.Now.ToString("MM - yy");

            try{

                if(!Directory.Exists(ErrorPayPath))  Directory.CreateDirectory(ErrorPayPath);

                if (!Directory.Exists(ErrorPayPathMonthVise)) Directory.CreateDirectory(ErrorPayPathMonthVise);

                String SavePath = Path.Combine(ErrorPayPathMonthVise, DateTime.Now.ToString("dd-MM-yy"));

                if (File.Exists(SavePath))
                {

                    // Append error to existing file.
                    using (StreamWriter writer = new StreamWriter(SavePath, true))
                    {
                        //writer.WriteLine("----------------------------------------------------------------------------------");
                        writer.WriteLine("");
                        writer.WriteLine(" ");
                        writer.WriteLine(DateTime.Now);
                        writer.WriteLine("");
                        writer.WriteLine("Method Name: "+ MethodName);
                        writer.WriteLine("");
                        writer.WriteLine("Return Message " + exception);
                        writer.WriteLine("");
                        writer.WriteLine("Posted Values");
                        //AccountNumbe, CompanyCode, BranchCode, PaymentAmount, PaymentDateTime, UID, SecuCode, ReceiptNo
                        writer.WriteLine("OrderNummber ~ CompanyCode ~ BranchCode ~ PaymentAmount ~ PaymentDateTime ~ UID ~ SecuCode ~ ReceiptNo ~ IP Address");
                        writer.WriteLine("");
                        writer.WriteLine(OrderNummber + " ~ " + CompanyCode + " ~ " + BranchCode + " ~ " + PaymentAmount + " ~ " + PaymentDateTime + " ~ " + UID + " ~ " + SecuCode + " ~ " + ReceiptNo + " ~ " + Ip);
                        writer.WriteLine("----------------------------------------------------------------------------------");

                        writer.Close();
                    }
                }
                else
                {
                    // Create file Write error.
                    using (StreamWriter oWrite = new StreamWriter(SavePath))
                    {

                        oWrite.WriteLine("----------------------------------------------------------------------------------");
                        oWrite.WriteLine("         N-P-A-G Error log Info File.(c)Copyright 2021 EPIC Lanka Technologies   ");
                        oWrite.WriteLine("         This File was Generated for Developers Purposes, Please DO NOT DELETE!   ");
                        oWrite.WriteLine("         Report Generated On : " + DateTime.Now);
                        oWrite.WriteLine("----------------------------------------------------------------------------------");
                        oWrite.WriteLine("");
                        oWrite.WriteLine(" ");
                        oWrite.WriteLine(DateTime.Now);
                        oWrite.WriteLine("");
                        oWrite.WriteLine("Method Name: " + MethodName);
                        oWrite.WriteLine("");
                        oWrite.WriteLine("Return Message " + exception);
                        oWrite.WriteLine("");
                        oWrite.WriteLine("Posted Values");
                        //AccountNumbe, CompanyCode, BranchCode, PaymentAmount, PaymentDateTime, UID, SecuCode, ReceiptNo
                        oWrite.WriteLine("OrderNummber ~ CompanyCode ~ BranchCode ~ PaymentAmount ~ PaymentDateTime ~ UID ~ SecuCode ~ ReceiptNo ~ IP Address");
                        oWrite.WriteLine("");
                        oWrite.WriteLine(OrderNummber + " ~ " + CompanyCode + " ~ " + BranchCode + " ~ " + PaymentAmount + " ~ " + PaymentDateTime + " ~ " + UID + " ~ " + SecuCode + " ~ " + ReceiptNo + " ~ " + Ip);
                        oWrite.WriteLine("----------------------------------------------------------------------------------");
                        oWrite.Close();

                    }

                }
                //-------------------------------------


                //INSERT DATA IN TO NPAG_

                //CompanyCode, BranchCode, PaymentAmount, PaymentDateTime, UID, SecuCode, ReceiptNo

                NPAG_ERROR_PAYMENT error_payment = new NPAG_ERROR_PAYMENT();
                error_payment.ORDER_NUMBER = OrderNummber;
                error_payment.COMPANYCODE = CompanyCode;
                error_payment.BRANCHCODE = BranchCode;
                error_payment.AMOUNT = PaymentAmount;
                error_payment.PAYMENTDATE = PaymentDateTime;
                error_payment.USERID = UID;
                error_payment.SECURECODE = SecuCode;
                error_payment.RECEIPTNUMBER = ReceiptNo;
                error_payment.IP_ADDRESS = Ip;
                error_payment.ERRORMESSAGE = "(FROM METHOD: "+MethodName+") "+ exception.Message;

                var error_payment_saved = _context.NPAG_ERROR_PAYMENTs.Add(error_payment);
                _context.SaveChanges();

            }
            catch(Exception er)
            {
                throw;
            }

            //return "";
        }


        public void WriteSucessLog(string MethodName, string returned_values, string OrderNummber, string CompanyCode, string BranchCode, string PaymentAmount, string PaymentDateTime, string UID, string SecuCode, string ReceiptNo, string Ip)
        {
            String SuceessPayPath = AppDomain.CurrentDomain.BaseDirectory + "Sucess Payment Log";
            String SucessPayPathMonthVise = SuceessPayPath + "\\" + DateTime.Now.ToString("MM - yy");

            try
            {

                if (!Directory.Exists(SuceessPayPath)) Directory.CreateDirectory(SuceessPayPath);

                if (!Directory.Exists(SucessPayPathMonthVise)) Directory.CreateDirectory(SucessPayPathMonthVise);

                String SavePath = Path.Combine(SucessPayPathMonthVise, DateTime.Now.ToString("dd-MM-yy"));

                if (File.Exists(SavePath))
                {

                    // Append error to existing file.
                    using (StreamWriter writer = new StreamWriter(SavePath, true))
                    {
                        //writer.WriteLine("----------------------------------------------------------------------------------");
                        writer.WriteLine("");
                        writer.WriteLine(" ");
                        writer.WriteLine(DateTime.Now);
                        writer.WriteLine("");
                        writer.WriteLine("Method Name: " + MethodName);
                        writer.WriteLine("");
                        writer.WriteLine("This is a Successful Payment");
                        writer.WriteLine("");
                        writer.WriteLine("Posted Values");
                        //AccountNumbe, CompanyCode, BranchCode, PaymentAmount, PaymentDateTime, UID, SecuCode, ReceiptNo
                        writer.WriteLine("OrderNummber ~ CompanyCode ~ BranchCode ~ PaymentAmount ~ PaymentDateTime ~ UID ~ SecuCode ~ ReceiptNo ~ IP Address");
                        writer.WriteLine("");
                        writer.WriteLine(OrderNummber + " ~ " + CompanyCode + " ~ " + BranchCode + " ~ " + PaymentAmount + " ~ " + PaymentDateTime + " ~ " + UID + " ~ " + SecuCode + " ~ " + ReceiptNo + " ~ " + Ip);
                        writer.WriteLine("");
                        writer.WriteLine("Returned Values: " + returned_values);
                        writer.WriteLine("----------------------------------------------------------------------------------");

                        writer.Close();
                    }
                }
                else
                {
                    // Create file Write error.
                    using (StreamWriter oWrite = new StreamWriter(SavePath))
                    {

                        oWrite.WriteLine("----------------------------------------------------------------------------------");
                        oWrite.WriteLine("         N-P-A-G Error log Info File.(c)Copyright 2021 EPIC Lanka Technologies   ");
                        oWrite.WriteLine("         This File was Generated for Developers Purposes, Please DO NOT DELETE!   ");
                        oWrite.WriteLine("         Report Generated On : " + DateTime.Now);
                        oWrite.WriteLine("----------------------------------------------------------------------------------");
                        oWrite.WriteLine("");
                        oWrite.WriteLine(" ");
                        oWrite.WriteLine(DateTime.Now);
                        oWrite.WriteLine("");
                        oWrite.WriteLine("Method Name: " + MethodName);
                        oWrite.WriteLine("");
                        oWrite.WriteLine("Posted Values:");
                        //AccountNumbe, CompanyCode, BranchCode, PaymentAmount, PaymentDateTime, UID, SecuCode, ReceiptNo
                        oWrite.WriteLine("OrderNummber ~ CompanyCode ~ BranchCode ~ PaymentAmount ~ PaymentDateTime ~ UID ~ SecuCode ~ ReceiptNo ~ IP Address");
                        oWrite.WriteLine("");
                        oWrite.WriteLine(OrderNummber + " ~ " + CompanyCode + " ~ " + BranchCode + " ~ " + PaymentAmount + " ~ " + PaymentDateTime + " ~ " + UID + " ~ " + SecuCode + " ~ " + ReceiptNo + " ~ " + Ip);
                        oWrite.WriteLine("");
                        oWrite.WriteLine("Returned Values: "+ returned_values);
                        oWrite.WriteLine("----------------------------------------------------------------------------------");
                        oWrite.Close();

                    }

                }

            }
            catch (Exception er)
            {
                throw;
            }

        }
    }
}
