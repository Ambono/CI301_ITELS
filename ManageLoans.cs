using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;


namespace IT_Equipment_Loan_System
{
    public partial class WebForm14 : System.Web.UI.Page
    {
        SqlConnection conn = new SqlConnection();
        protected void makeConnection()
        {

            conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ITES"].ConnectionString;
            conn.Open();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["myStaffNewSession"] != null)
            {
                lblSessionLM.Text = "";
                lblSessionLM.Text = "Welcome" + " " + Session["myStaffNewSession"].ToString();
            }
            else
            {
                Response.Redirect("StaffLogin.aspx");
            }
        } // end if



        protected void btnLoan_Click(object sender, EventArgs e)
        {
            makeConnection();


            //First of all check if there is a device
            string cmdStr = "Select count(*) from Devices where DeviceSN = '" + LBDeviceSN.SelectedItem.Text + "' and DeviceStatus= 'available' ";
            //string cmdStr = "Select count(*) from Devices where DeviceSN =  'SN1' and DeviceStatus= 'available' ";
            SqlCommand DeviceExist = new SqlCommand(cmdStr, conn);
            int tempLoan = Convert.ToInt32(DeviceExist.ExecuteScalar().ToString());
            lblCountDevices.Text = "";
            lblCountDevices.Text = "Quantity of devices of this kind: " + tempLoan.ToString();



            //if device exist is available I would like to know if the student has currently made a booking or has a laptop in his/her possession
            if (tempLoan == 1)
            {
                string cmdCheckLoan = "Select count(*) from [dbo].[Students] where [StudentUN] = '" + LBStudentUN.SelectedItem.Text + "'  And [Note]  like '%possession%'";
                SqlCommand LoanCount = new SqlCommand(cmdCheckLoan, conn);
                int tempStud = Convert.ToInt32(LoanCount.ExecuteScalar().ToString());

                if (tempStud == 1)
                {
                    lblLoanMade.Text = "";
                    lblLoanMade.Text = "The student has already a device in his/her possession";
                    lblLoanMade.Visible = true;
                }//end  inner if

                else
                {



                    //if the student is clear he/she may procede with the loan


                    // the statement to make the actual loan
                    string insCmd = "Insert into [dbo].[Loans] ([DeviceSN],[StudentUN],[LoanStartDate]) values (@DeviceSN, @StudentUN,@LoanStartDate ) ";

                    SqlCommand insertLoan = new SqlCommand(insCmd, conn);

                    insertLoan.Parameters.AddWithValue("@DeviceSN", LBDeviceSN.SelectedItem.Text);
                    insertLoan.Parameters.AddWithValue("@StudentUN", LBStudentUN.SelectedItem.Text);
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
                            updateStatus.Parameters.AddWithValue("@DeviceSN", LBDeviceSN.SelectedItem.Text);
                            updateStatus.Parameters.AddWithValue("@Status", "unavailable");
                            updateStatus.ExecuteNonQuery();


                            //I also need to say that the student who is benefiting from the loan cannot book again until the device is returned

                            string updStudent = "Update [dbo].[Students] set [Note] = @NOTE where [StudentUN]= @StudentUN";
                            SqlCommand updateStudent = new SqlCommand(updStudent, conn);
                            updateStudent.Parameters.AddWithValue("@Note", "Laptop in possession");
                            updateStudent.Parameters.AddWithValue("@StudentUN", LBStudentUN.SelectedItem.Text);
                            updateStudent.ExecuteNonQuery();




                        } //end inner if

                        else
                        {
                            lblLoanMade.Text = "There is a problem with the loan. The student or the device might not be available/exist ";
                            lblLoanMade.Visible = true;
                        } //end else



                    }//end try

                    catch (SqlException)
                    {
                        lblLoanMade.Visible = true;
                        lblLoanMade.Text = "There is a problem with the loan. The student or the device might not be available/exist";

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
                lblLoanMade.Text = "There is a problem with the loan. The student or the device might not be available/exist";
            } //end outer else


        } //end btnLooan

    }
}


