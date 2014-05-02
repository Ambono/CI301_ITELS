using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

namespace IT_Equipment_Loan_System
{
    public partial class WebForm6 : System.Web.UI.Page
    {

        SqlConnection conn = new SqlConnection();


        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["myNewsession"] != null)
            {
                lblStSession.Text = "";
                lblStSession.Text = "Welcome" + " " + Session["myNewsession"].ToString();
            }
            else
            {
                Response.Redirect("StudentLogin.aspx");
            }

            conn.ConnectionString = ConfigurationManager.ConnectionStrings["ITES"].ConnectionString;
            conn.Open();

            string cmdStr = "Select count(*)from Devices where DeviceStatus='available'";
            SqlCommand DeviceAvail = new SqlCommand(cmdStr, conn);
            int tempCon = Convert.ToInt32(DeviceAvail.ExecuteScalar().ToString());

            lblCount.Text = tempCon.ToString();
            conn.Close();

        }

       /* public int getBookingLimit()
        {
            SqlConnection conn2 = new SqlConnection();
            conn2.ConnectionString = ConfigurationManager.ConnectionStrings["ITES"].ConnectionString;
         
            conn2.Open();




            //check if there is any available device
            string cmdStr = "Select count(*)from Devices";
            SqlCommand AvailNumber = new SqlCommand(cmdStr, conn2);
            int maxBooking = Convert.ToInt32(AvailNumber.ExecuteScalar().ToString());
            conn2.Close();
            return maxBooking;

        }
        * */

        protected void btnBookADevice_Click(object sender, EventArgs e)
        {

            conn.ConnectionString = ConfigurationManager.ConnectionStrings["ITES"].ConnectionString;
            conn.Open();




            //check if there is any available device
            string cmdStr = "Select count(*)from Devices where DeviceStatus='available'";
            SqlCommand DeviceAvail = new SqlCommand(cmdStr, conn);
            int temp = Convert.ToInt32(DeviceAvail.ExecuteScalar().ToString());

            //if device is available I would like to know if the student has currently made a booking or has a laptop in his/her possession
            if (temp > 0 ) //first if
            {
                
                string cmdCheck = "Select count(*) from [dbo].[Students] where [StudentUN] = '" + Session["myNewsession"].ToString() + "'  And [Note] like '%Booking%' OR [Note] like '%possession%'";
                SqlCommand BookedCount = new SqlCommand(cmdCheck, conn);
                int tempStud = Convert.ToInt32(BookedCount.ExecuteScalar().ToString());
                if (tempStud == 1) //second if
                {
                    lblMessage.Text = "Sorry you have already booked";
                }//end  inner if(second if)

   //if the student is clear he/she may procede with booking
                else
                {
                    string insCmd = "Insert into [dbo].[Bookings] ([StudentUN],[DateBooked], [TimeBooked] ) values (@StudentUN, @DateBooked, @TimeBooked) ";

                    SqlCommand insertBooking = new SqlCommand(insCmd, conn);
                    insertBooking.Parameters.AddWithValue("@StudentUN", Session["myNewsession"].ToString());
                    insertBooking.Parameters.AddWithValue("@DateBooked", DateTime.Now);
                    insertBooking.Parameters.AddWithValue("@TimeBooked", DateTime.Now);


                    try
                    {
                        int result = insertBooking.ExecuteNonQuery();
                        //once the booking is accepted student must be marked for booking a device
                        if (result == 1)
                        {

                            string updStudent2 = "Update [dbo].[Students] set [Note] = @NOTE where [StudentUN]= @StudentUN";
                            SqlCommand updateStudent2 = new SqlCommand(updStudent2, conn);
                            updateStudent2.Parameters.AddWithValue("@Note", "Booking made ");
                            updateStudent2.Parameters.AddWithValue("@StudentUN", Session["myNewsession"].ToString());
                            updateStudent2.ExecuteNonQuery();
                            Response.Redirect("SuccessfulBooking.aspx");

                        }
                        else { Response.Redirect("UnsuccessBookings.aspx"); }


                    }//end try


                    catch (SqlException)
                    {
                        Response.Redirect("UnsuccessBookings.aspx");

                    }

                    finally
                    {

                        conn.Close();//
                    }

                }//end outer else

            }//end IF (first if)
            else
            {
                Response.Redirect("UnsuccessBookings.aspx");
                lblMessage.Text = "We found no device available at the moment. Try again later";
            }//end else
            //
        }//end btnBookADevice

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session["myNewsession"] = null;
            Response.Redirect("StudentLogin.aspx");
        }//end  logout

     
    }


}
