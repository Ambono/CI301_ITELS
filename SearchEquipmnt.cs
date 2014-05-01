using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
//using System.Data.DataSet;
using System.Data;

namespace IT_Equipment_Loan_System
{
    public partial class WebForm11 : System.Web.UI.Page
    {

        SqlConnection conSear = new SqlConnection();


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

        }//end Page_Load

        DataTable GetData()
        {
            DataTable dt = new DataTable();
            conSear.ConnectionString = ConfigurationManager.ConnectionStrings["ITES"].ConnectionString;
            conSear.Open();

            string searchCount = "Select count(*) from [dbo].[Devices] where [DeviceName] like '" + TxtSearch.Text + "%' And [DeviceStatus] = 'available'";

            SqlCommand cmd2 = new SqlCommand(searchCount, conSear);
            int temp = Convert.ToInt32(cmd2.ExecuteScalar().ToString());
            lblSearchCount.Text = "";
            lblSearchCount.Visible = true;
            lblSearchCount.Text = "Available number of this device is: " + temp.ToString();

            try
            {
                string searchCmd = "Select[DeviceName],[DeviceDescr],[DeviceStatus],[DevQtyAvailable],[DevicePicture] from [dbo].[Devices] where [DeviceName] like '" + TxtSearch.Text + "%' Order by [DeviceName]";

                SqlCommand cmd = new SqlCommand(searchCmd, conSear);
                SqlDataAdapter adpt = new SqlDataAdapter(cmd);
                adpt.Fill(dt);

            }


            catch (SqlException)
            {
                lblMessage.Text = "Sorry no such device found";

            }

            finally
            {

                conSear.Close();//

            }



            return dt;
        }//end getData



        protected void btnSearch_Click(object sender, EventArgs e)
        {

            GridView1.DataSource = GetData();
            GridView1.DataBind();
            btnBook.Visible = true;
        }//end btnSearch

       

        protected void btnBook_Click(object sender, EventArgs e)
        {
           
                conSear.ConnectionString = ConfigurationManager.ConnectionStrings["ITES"].ConnectionString;
                conSear.Open();
            

            //check if there is any available device
            string cmdStr = "Select count(*)from Devices where DeviceStatus='available'";
            SqlCommand DeviceAvail = new SqlCommand(cmdStr, conSear);
            int temp = Convert.ToInt32(DeviceAvail.ExecuteScalar().ToString());

            //if device is available I would like to know if the student has currently made a booking or has a laptop in his/her possession
            if (temp > 0)
            {
                string cmdCheck = "Select count(*) from [dbo].[Students] where [StudentUN] = '" + Session["myNewsession"].ToString() + "'  And [Note] like '%Booking%' OR [Note] like '%possession%'";
                SqlCommand BookedCount = new SqlCommand(cmdCheck, conSear);
                int tempStud = Convert.ToInt32(BookedCount.ExecuteScalar().ToString());
                if (tempStud==1)
                {
                    lblMessage.Text = "Sorry you have already booked";
                }//end  inner if

   //if the student is clear he/she may procede with booking
                else
                {
                    string insCmd = "Insert into [dbo].[Bookings] ([StudentUN],[DateBooked], [TimeBooked] ) values (@StudentUN, @DateBooked, @TimeBooked) ";

                    SqlCommand insertBooking = new SqlCommand(insCmd, conSear);
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
                            SqlCommand updateStudent2 = new SqlCommand(updStudent2, conSear);
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

                        conSear.Close();//
                    }

                }//end outer else

            }//end IF
            else
            {
                Response.Redirect("UnsuccessBookings.aspx");
                lblMessage.Text = "We found no device available at the moment";
            }//end else

        }//end btnBook
    }//end class
    }//end namespace
