using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;


namespace IT_Equipment_Loan_System
{
    public partial class WebForm7 : System.Web.UI.Page
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


        }

        protected void btnCancelBooking_Click(object sender, EventArgs e)
        {
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["ITES"].ConnectionString;
            conn.Open();

            string cmdStr = "Select count(*)from Bookings where StudentUN= '" + TxtCancel.Text + "'";
            SqlCommand cancelAvail = new SqlCommand(cmdStr, conn);
            int temp = Convert.ToInt32(cancelAvail.ExecuteScalar().ToString());

            //if device is available I would like to know if the student has currently made a booking or has a laptop in his/her possession
            if (temp >0)
            {

                string delCmd = "delete from [dbo].[Bookings] where  StudentUN = '" + TxtCancel.Text + "' and [BookingNo] =  (select top(1) BookingNo from Bookings order by BookingNo desc) ";

                SqlCommand delBooking = new SqlCommand(delCmd, conn);

                try
                {
                    int result = delBooking.ExecuteNonQuery();


                    string updStudent3 = "Update [dbo].[Students] set [Note] = @NOTE where [StudentUN]= @StudentUN";
                    SqlCommand updateStudent3 = new SqlCommand(updStudent3, conn);
                    updateStudent3.Parameters.AddWithValue("@Note", "allowed");
                    updateStudent3.Parameters.AddWithValue("@StudentUN", TxtCancel.Text);
                    updateStudent3.ExecuteNonQuery();
                    lblMessage.Text = "";
                    lblMessage.Text = "Cancellation made";
                    lblMessage.Visible = true;
                }
                catch (SqlException) {
                    lblMessage.Text = "";
                    lblMessage.Text = "Exception No booking was found";
                    lblMessage.Visible = true;
                }

            }//end if

            else
            {
                lblMessage.Text = "";
                lblMessage.Text = "No booking was found";
                lblMessage.Visible = true;

            }//end else
        }//end cancelBooking
    }
}
