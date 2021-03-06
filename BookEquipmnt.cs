using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;

namespace IT_Equipment_Loan_System
{
    public partial class WebForm9 : System.Web.UI.Page
    {
        SqlConnection conn = new SqlConnection();

        protected void Page_Load(object sender, EventArgs e)
        {


        }

        protected void bntBook_Click(object sender, EventArgs e)
        {

            conn.ConnectionString = ConfigurationManager.ConnectionStrings["ITES"].ConnectionString;
            conn.Open();
            //check if there is any available device
            string cmdStr = "Select count(*)from Devices where DeviceStatus='available'";
            SqlCommand DeviceAvail = new SqlCommand(cmdStr, conn);
            int temp = Convert.ToInt32(DeviceAvail.ExecuteScalar().ToString());

            //if device is available I would like to know if the student has currently made a booking or has a laptop in his/her possession
            if (temp > 0)
            {
                string cmdCheck = "Select count(*) from [dbo].[Students] where [StudentUN] = '" + TxtStudentID.Text + "'  And [Note] like '%Booking%' OR [Note] like '%possession%'";

                //string cmdCheck = "Select count(*) from [dbo].[Students] where [StudentUN] = '" + TxtStudentID.Text + "'";
                SqlCommand BookedCount = new SqlCommand(cmdCheck, conn);
                int tempStud = Convert.ToInt32(BookedCount.ExecuteScalar().ToString());
                if (tempStud == 1)
                {
                    lblBooking2.Text = "Sorry this student has already a booking";
                }//end  inner if

   //if the student is clear he/she may procede with booking
                else
                {

                    try
                    {

                        string insCmd = "Insert into [dbo].[EqBookings] ([StudentUN],[DateBooked],[BookedLoanDuration],[TimeBooked]) values (@StudentUN, @DateBooked, @BookedLoanDuration, @TimeBooked) ";

                        SqlCommand staffinsertBooking = new SqlCommand(insCmd, conn);
                        staffinsertBooking.Parameters.AddWithValue("@StudentUN", TxtStudentID.Text);
                        staffinsertBooking.Parameters.AddWithValue("@DateBooked", DateTime.Now);
                        staffinsertBooking.Parameters.AddWithValue("@BookedLoanDuration", "2");
                        staffinsertBooking.Parameters.AddWithValue("@TimeBooked", DateTime.Now);


                        int result = staffinsertBooking.ExecuteNonQuery();
                        //once the booking is accepted student must be marked for booking a device
                        if (result == 1)
                        {


                            string updStudent2 = "Update [dbo].[Students] set [Note] = @NOTE where [StudentUN]= @StudentUN";
                            SqlCommand updateStudent2 = new SqlCommand(updStudent2, conn);
                            updateStudent2.Parameters.AddWithValue("@Note", "Booking made ");
                            updateStudent2.Parameters.AddWithValue("@StudentUN", TxtStudentID.Text);
                            updateStudent2.ExecuteNonQuery();
                            // Response.Redirect("SuccessfulBooking.aspx");
                            lblBooking2.Text = "";
                            lblBooking2.Text = "Booking made";

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

            }//end IF
            else
            {
                Response.Redirect("UnsuccessBookings.aspx");
                lblBooking2.Text = "We found no device available at the moment";
            }//end else


        }//end book



        //Cancel booking
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["ITES"].ConnectionString;
            conn.Open();

            string cmdStr = "Select count(*)from EqBookings where StudentUN= '" + TxtStudentID.Text + "'";
            SqlCommand cancelAvail = new SqlCommand(cmdStr, conn);
            int temp = Convert.ToInt32(cancelAvail.ExecuteScalar().ToString());

            //if device is available I would like to know if the student has currently made a booking or has a laptop in his/her possession
            if (temp > 0)
            {

                string delCmd = "delete from [dbo].[EqBookings] where  StudentUN = '" + TxtStudentID.Text + "' and [BookingNo] =  (select top(1) BookingNo from EqBookings order by BookingNo desc) ";

                SqlCommand delBooking = new SqlCommand(delCmd, conn);

                try
                {
                    int result = delBooking.ExecuteNonQuery();


                    string updStudent3 = "Update [dbo].[Students] set [Note] = @NOTE where [StudentUN]= @StudentUN";
                    SqlCommand updateStudent3 = new SqlCommand(updStudent3, conn);
                    updateStudent3.Parameters.AddWithValue("@Note", "allowed");
                    updateStudent3.Parameters.AddWithValue("@StudentUN", TxtStudentID.Text);
                    updateStudent3.ExecuteNonQuery();
                    lblBooking2.Text = "";
                    lblBooking2.Text = "Cancellation made";
                    lblBooking2.Visible = true;
                }
                catch (SqlException)
                {
                    lblBooking2.Text = "";
                    lblBooking2.Text = "Exception No booking was found";
                    lblBooking2.Visible = true;
                }

            }//end if

            else
            {
                lblBooking2.Text = "";
                lblBooking2.Text = "No booking was found";
                lblBooking2.Visible = true;

            }//end else
        }//cancelBooking
    }//end button cancel booking


}





