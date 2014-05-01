using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace IT_Equipment_Loan_System
{
    public partial class WebForm20 : System.Web.UI.Page
    {
         SqlConnection conn = new SqlConnection();
        protected void makeConnection()
        {

            conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ITES"].ConnectionString;
            conn.Open();
        }


        protected void Page_Load(object sender, EventArgs e)
        {
              if (Session["myNewsession"] != null)
            {
                lblSessionLM.Text = "";
                lblSessionLM.Text = "Welcome" + " " + Session["myNewsession"].ToString();
              LblTest.Text = DisplayLocalHostName();
            }
            else
            {
                Response.Redirect("StudentLogin.aspx");
            }

              sendEmail();


        }//end page load




        static string DisplayLocalHostName()
        {
            String hostName = "";
            try
            {
                // Get the local computer host name.
                hostName = Dns.GetHostName();
                

            }
            catch (Exception e)
            {
                Console.WriteLine("SocketException caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }
           
            return hostName;

        } //end method display host name

        public static string getUserName()
        {
            string Name = Environment.GetEnvironmentVariable("USERNAME");
            Console.WriteLine("User name is: " + Name);
            return Name;
        }//end method getUserName



      
        public static void sendEmail()
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("modpleh66@googlemail.com");
                mail.To.Add("modpleh66@googlemail.com");
                mail.Subject = "Computer login alert Mail";
                mail.Body = getUserName() + " is currently using " + DisplayLocalHostName();
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("modpleh66@googlemail.com", "london2008");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);

            }//end try
            catch (Exception)
            {
                // MessageBox.Show(ex.ToString());
            }//end catch

        }//end method send email


        protected void btnValidateLoan_Click(object sender, EventArgs e)
        {
            
       
            makeConnection();

            string deviceNm = DisplayLocalHostName();
            //First of all check if there is a device
            string cmdStr = "Select count(*) from Devices where DeviceSN = '" +deviceNm+ "' and DeviceStatus= 'available' ";
            
            //string cmdStr = "Select count(*) from Devices where DeviceSN =  'SN1' and DeviceStatus= 'available' ";
            SqlCommand DeviceExist = new SqlCommand(cmdStr, conn);
            int tempLoan = Convert.ToInt32(DeviceExist.ExecuteScalar().ToString());
            //lblCountDevices.Text = "";
            LblTest.Text = "Quantity of devices of this kind: " + tempLoan.ToString();



            //if device exist is available I would like to know if the student has currently made a booking or has a laptop in his/her possession
            if (tempLoan == 1)
            {
               // string studentName = getUserName();
                string cmdCheckLoan = "Select count(*) from [dbo].[Students] where [StudentUN] = 'getUserName()'  And [Note]  like '%possession%'";
                SqlCommand LoanCount = new SqlCommand(cmdCheckLoan, conn);
                int tempStud = Convert.ToInt32(LoanCount.ExecuteScalar().ToString());

                if (tempStud == 1)
                {
                    lblLoanMade.Text = "";
                    lblLoanMade.Text = "You may already have a device in his/her possession. Please return it first!";
                    lblLoanMade.Visible = true;
                }//end  inner if

                else
                {
                    //if the student is clear he/she may procede with the loan


                    // the statement to make the actual loan
                    string insCmd = "Insert into [dbo].[Loans] ([DeviceSN],[StudentUN],[LoanStartDate]) values (@DeviceSN, @StudentUN,@LoanStartDate ) ";

                    SqlCommand insertLoan = new SqlCommand(insCmd, conn);

                    insertLoan.Parameters.AddWithValue("@DeviceSN", DisplayLocalHostName());
                    insertLoan.Parameters.AddWithValue("@StudentUN", getUserName());
                    insertLoan.Parameters.AddWithValue("@LoanStartDate", DateTime.Now);

                    try
                    {
                        int res = insertLoan.ExecuteNonQuery();
                        if (res == 1)
                        {
                            // lblLoanMade.Text = "";
                            lblLoanMade.Text = "Loan made";
                            lblLoanMade.Visible = true;

                            //If I make a loan I need to specify that the device I made the loan on is unavailable


                            string updCmd = "Update [dbo].[Devices] set [DeviceStatus] = @Status where DeviceSN= @DeviceSN ";
                            SqlCommand updateStatus = new SqlCommand(updCmd, conn);
                            updateStatus.Parameters.AddWithValue("@DeviceSN", DisplayLocalHostName());
                            updateStatus.Parameters.AddWithValue("@Status", "unavailable");
                            updateStatus.ExecuteNonQuery();


                            //I also need to say that the student who is benefiting from the loan cannot book again until the device is returned

                            string updStudent = "Update [dbo].[Students] set [Note] = @NOTE where [StudentUN]= @StudentUN";
                            SqlCommand updateStudent = new SqlCommand(updStudent, conn);
                            updateStudent.Parameters.AddWithValue("@Note", "Laptop in possession");
                            updateStudent.Parameters.AddWithValue("@StudentUN", getUserName());
                            updateStudent.ExecuteNonQuery();

                            Response.Redirect("GeneratePasscode.aspx");


                        } //end inner if

                        else
                        {
                            lblLoanMade.Text = "There is a problem with the loan. The student or the device might not be available/exist. ";
                            lblLoanMade.Visible = true;
                        } //end else



                    }//end try

                    catch (SqlException)
                    {
                        lblLoanMade.Visible = true;
                        lblLoanMade.Text = "There is a problem with the loan. The student or the device might not be available/exist. ";

                    } //end catch

                    finally
                    {

                        conn.Close();//
                    } //end finally close

                }//end else
            }//end if

            else
            {
                lblLoanMade.Visible = true;
                lblLoanMade.Text = "There is a problem with the loan. The student or the device might not be available/exist. ";
            } //end outer else


        }//end validate loan
    }
}
