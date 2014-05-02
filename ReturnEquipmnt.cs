using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;


namespace IT_Equipment_Loan_System
{
    public partial class WebForm10 : System.Web.UI.Page
    {
        SqlConnection conRt = new SqlConnection();

        protected void Page_Load(object sender, EventArgs e)
        {
           
        }


        protected void AlreadybtnReturn_Click(object sender, EventArgs e)
        {

         
        }

        protected void btnReturn_Click(object sender, EventArgs e)
        {

            conRt.ConnectionString = ConfigurationManager.ConnectionStrings["ITES"].ConnectionString;
            conRt.Open();
            string cmdStr = "Select count(*) from Devices where DeviceSN =  '" + lbReturn.SelectedItem.Text + "' and DeviceStatus= 'unavailable' ";

            string cmdStr2 = "Select count(*) from Devices where DeviceSN =  '" + lbReturn.SelectedItem.Text + "' and DeviceStatus= 'available' ";
            SqlCommand DeviceReturn2 = new SqlCommand(cmdStr2, conRt);
            int tempReturn2 = Convert.ToInt32(DeviceReturn2.ExecuteScalar().ToString());

            // conn.Close();

            if (tempReturn2 == 1)
            {
                lblReturnMade.Text = "already available"; //
                lblReturnMade.Visible = true;
            }


            else
            {
                cmdStr = "Select count(*) from Devices where DeviceSN =  '" + lbReturn.SelectedItem.Text + "' and DeviceStatus= 'unavailable' ";
                SqlCommand DeviceReturn = new SqlCommand(cmdStr, conRt);
                int tempReturn = Convert.ToInt32(DeviceReturn.ExecuteScalar().ToString());



                if (tempReturn == 1)
                {

                    string insCmd = "Insert into [dbo].[DeviceReturns] ([DeviceSN],[ReturnDateTime] ) values (@DeviceSN, @ReturnDateTime) ";

                    SqlCommand insertReturn = new SqlCommand(insCmd, conRt);

                    insertReturn.Parameters.AddWithValue("@DeviceSN", lbReturn.SelectedItem.Text);
                    insertReturn.Parameters.AddWithValue("@ReturnDateTime", DateTime.Now);

                    // int res =   insertReturn.ExecuteNonQuery();




                    try
                    {
                        int res = insertReturn.ExecuteNonQuery();
                        if (res == 1)
                        {

                            lblReturnMade.Text = "Return made";
                            lblReturnMade.Visible = true;

                            //once the device is returned it status must change to available


                            string updCmd = "Update [dbo].[Devices] set [DeviceStatus] = @Status where DeviceSN= @DeviceSN ";
                            SqlCommand updateRetStatus = new SqlCommand(updCmd, conRt);
                            updateRetStatus.Parameters.AddWithValue("@Status", "available");
                            updateRetStatus.Parameters.AddWithValue("@DeviceSN", lbReturn.SelectedItem.Text);
                            updateRetStatus.ExecuteNonQuery();

                            //the student must be marked as allowed to book and be loaned again
                            string updStudent = "Update [dbo].[Students] set [Note] = @NOTE where [StudentUN] = (select top(1) [StudentUN] from [dbo].[Loans] where  [DeviceSN]=@DeviceSN  order by LoansNo desc)";
                            SqlCommand updateStudent = new SqlCommand(updStudent, conRt);
                            updateStudent.Parameters.AddWithValue("@Note", "allowed");
                            updateStudent.Parameters.AddWithValue("@DeviceSN", lbReturn.SelectedItem.Text);
                            updateStudent.ExecuteNonQuery();
                            
                            //now delete the booking
                            //string delBookingsRt = "delete from [dbo].[Bookings]  where [StudentUN] = (select [StudentUN] from [dbo].[Loans] where [DeviceSN]=@DeviceSN order by LoansNo desc) ";
                            //SqlCommand deleteStudentRt = new SqlCommand(delBookingsRt, conRt);
                            //deleteStudentRt.Parameters.AddWithValue("@DeviceSN", lbReturn.SelectedItem.Text);
                            //deleteStudentRt.ExecuteNonQuery();

                            lblReturnMade.Text = "";
                            lblReturnMade.Text = "Your return is recorded";

                        } //end inner if

                        else
                        {
                            lblReturnMade.Text = "";
                            lblReturnMade.Text = "There is a problem with the Return. The device might not exist";
                            lblReturnMade.Visible = true;
                        } //end else

                    }//end try

                    catch (SqlException)
                    {
                        lblReturnMade.Text = "";
                        lblReturnMade.Visible = true;
                        lblReturnMade.Text = "catch There is a problem with the Return. The device might not exist";

                    } //end catch

                    finally
                    {
                        conRt.Close();//
                    } //end finally close


                } //end if



            }//end outer else

            //conRt.Close();   

        } //end btnReturn

    }
}



