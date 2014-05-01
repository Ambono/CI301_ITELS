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
    public partial class WebForm22 : System.Web.UI.Page
    {
        SqlConnection conn = new SqlConnection();



        protected void Page_Load(object sender, EventArgs e)
        {
            //ensureLogin();
            makeConnection();
            generatePasscode();
          //  passcodeGeneratorMethod();
        }



        /**Make connection*/
        protected void makeConnection()
        {

            conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ITES"].ConnectionString;
            conn.Open();
        }

        /**Ensure Login*/

        public void ensureLogin()
        {


            if (Session["myNewsession"] != null)
            {
                LblSessionGP.Text = "";
                LblSessionGP.Text = "Welcome" + " " + Session["myNewsession"].ToString();
                LblTestGP.Text = DisplayLocalHostName();
            }
            else
            {
                Response.Redirect("StudentLogin.aspx");
            }
        }//end method ensureLogin


        /**get host name*/
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

        /**Get user name*/

        public static string getUserName()
        {
            string Name = Environment.GetEnvironmentVariable("USERNAME");
            Console.WriteLine("User name is: " + Name);
            return Name;
        }//end method getUserName


        /**generate passcode if there is a loan*/

        public void generatePasscode()
        {
            // the statement to make the actual loan
          
            string deviceNm = DisplayLocalHostName();
            string userName = getUserName();

            ///////////////////////////

            //string deviceNm = DisplayLocalHostName();
            //string userName = getUserName();
            ////First of all check if there is a device
            //string cmdStr = "Select count(*) from Devices where DeviceSN = '" + deviceNm + "' and DeviceStatus= 'available' ";

            ////string cmdStr = "Select count(*) from Devices where DeviceSN =  'SN1' and DeviceStatus= 'available' ";
            //SqlCommand DeviceExist = new SqlCommand(cmdStr, conn);
            //int tempLoan = Convert.ToInt32(DeviceExist.ExecuteScalar().ToString());
            ////lblCountDevices.Text = "";
            //LblTest.Text = "Quantity of devices of this kind: " + tempLoan.ToString();
            ///////////////////

            //check if loan is available before generating passcode
            string cmdStrGP = "select count(*) from [dbo].[Loans]  where DeviceSN = '" + deviceNm + "' and StudentUN = '" + userName + "'";
            SqlCommand LoanExist = new SqlCommand(cmdStrGP, conn);
            int LoanCount = Convert.ToInt32(LoanExist.ExecuteScalar().ToString());
            //LblTestGP.Text = LoanCount.ToString();

            try
            {

                //Random random = new Random();
                //int randomNumber = random.Next(100, 1000);
                //int pc = randomNumber * 5;
                //int PC2 = pc * 3;

                //LblRandNum.Text = PC2.ToString();

                if (LoanCount >= 1)
                {
                    string cmdStrDev = "Select count(*) from Devices where DeviceSN = '" + deviceNm + "' and DeviceStatus= 'unavailable' ";

                    //string cmdStr = "Select count(*) from Devices where DeviceSN =  'SN1' and DeviceStatus= 'available' ";
                    SqlCommand DeviceExist = new SqlCommand(cmdStrDev, conn);
                    int DevEx = Convert.ToInt32(DeviceExist.ExecuteScalar().ToString());

                    //if device exist is available I would like to know if the student has currently made a booking or has a laptop in his/her possession
                    if (DevEx == 1)
                    {
                        LblRandNum.Text = DevEx.ToString();
                        passcodeGeneratorMethod();//passcode generation fucntion call

                    }//Endif
                }//end outer if
            }//end try
            catch (Exception e) { }



            finally
            {

                conn.Close();//
            } //end finally close
            

        }//end method GeneratePasscode

        /**Function to generate passcode ir will generate around 3000 passcodes in 2 year enough to serve all students*/
        public void passcodeGeneratorMethod()
        {
            int passNumber = 0;
            Random random1 = new Random();
            int randomNumber = random1.Next(10, 1010);

            Random random2 = new Random();
            int randomNumber2 = random2.Next(0, 2);

            switch (randomNumber2)
            {
                case 0:
                    passNumber = 17 * randomNumber;
                    break;
                case 1:
                    passNumber = 29 * randomNumber;
                    break;
                case 2:
                    passNumber = 51 * randomNumber;
                    break;
                default:
                    passNumber = 8437;
                    break;
            }//end first switch


            int passModulus = 0;
            string passcode = "";
            try
            {
                for (int i = 16; i < 52; i++)
                {
                    passModulus = passNumber % i;

                    if (passModulus == 0)
                    {

                        switch (i)
                        {
                            case 17:
                                if (passNumber < 10000)
                                {
                                    passcode = "ap" + passNumber;
                                }//end if 17
                                else
                                {
                                    passcode = "tu" + passNumber;
                                }//end else 17
                                break;

                            case 29:

                                if (passNumber < 15000)
                                {
                                    passcode = "Ek" + passNumber;
                                }//end if 29
                                else
                                {
                                    passcode = "rF" + passNumber;
                                }//end else 29
                                break;

                            case 51:

                                if (passNumber < 25000)
                                {
                                    passcode = "vL" + passNumber;
                                }//end if 29
                                else
                                {
                                    passcode = "wM" + passNumber;
                                }//end else 29
                                break;
                        }//end switch
                    }//end if passModulus
                    else { LblPasscode.Text = "Fail to get a passcode!"; }
                }//end for loop                                
                LblPasscode.Text = passcode;
                //string Pssc = passcode;

                //Lbl1Substring.Text = passcode.Substring(2);

            }//end try

            catch(Exception  e){}//end catch

        }//end method passcode generator

    }//end class
}//end mane space
