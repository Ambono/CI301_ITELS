using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace IT_Equipment_Loan_System
{
    public partial class WebForm16 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BtnLogin_Click(object sender, EventArgs e)
        {
           // Label1.Visible = true;
           // Label1.Text = "Done!!";
            //try
            //{


            //    SqlConnection con = new SqlConnection();


            //    con.ConnectionString = ConfigurationManager.ConnectionStrings["ITEquipment"].ConnectionString;

            //    con.Open();



            //    SqlCommand cmd = new SqlCommand();
            //    cmd.Connection = con;
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.CommandText = "SystemLoginStudents";
            //    SqlParameter p1 = new SqlParameter("UID", TxtUN.Text);
            //    SqlParameter p2 = new SqlParameter("PW", TxtPW.Text);
            //    cmd.Parameters.Add(p1);
            //    cmd.Parameters.Add(p2);
            //    SqlDataReader rd = cmd.ExecuteReader();

            //    if (rd.HasRows)
            //    {
            //        string u_name = TxtUN.Text;

            //        if (u_name == "StaffUN1")    //'UN1', 'StaffUN1'
            //        {

            //            Response.Redirect("Admin.aspx");

            //        } //end if inner
            //        else
            //        {
            //            Session["myNewsession"] = TxtUN.Text;
            //            Response.Redirect("LoginDestination.aspx");

            //        } //end else inner

            //    } //end if outer

            //    else
            //    {
            //        lblAlert.Visible = true;
            //        lblAlert.Text = "Wrong user name or password with temp !=1";

            //    } //end else outer

            //    con.Close();

            //}  //end try

            //catch (Exception)
            //{

            //} //end catch






            try
            {

               // SqlConnection con = new SqlConnection("metadata=res://*/ITEquipment.csdl|res://*/ITEquipment.ssdl|res://*/ITEquipment.msl;provider=System.Data.SqlClient;provider connection string='data source=(LocalDB)\v11.0;attachdbfilename=&quot;C:\ProjectCI301\IT Equipment Loan System\Database\DB\ITEquipment.mdf&quot;;integrated security=True;connect timeout=30;MultipleActiveResultSets=True;App=EntityFramework'" providerName="System.Data.EntityClient);

        SqlConnection con = new SqlConnection();
                ///http://stackoverflow.com/questions/20198527/cant-add-stored-procedure-in-visual-studio-2012


              //  Data Source=(LocalDB)\v11.0;AttachDbFilename="C:\ProjectCI301\IT Equipment Loan System\IT Equipment Loan System\App_Data\ITEquipment.mdf";Integrated Security=True;MultipleActiveResultSets=True;Connect Timeout=30;Application Name=EntityFramework

                con.ConnectionString = ConfigurationManager.ConnectionStrings["ITES"].ConnectionString;

             con.Open();


              //  string s1 = "select * from Students WHERE StudentUN ='UN1' ";
                string s1 = "select count(*) from Students WHERE StudentUN ='" + TxtUN.Text + "' ";

               

                SqlCommand cmd = new SqlCommand(s1, con);
                int i = Convert.ToInt16(cmd.ExecuteScalar().ToString());


                if (i >= 1)
                {
                    string s2 = "select StudentPW from Students WHERE  StudentPW= '" + TxtPW.Text + "'";
                    SqlCommand pass = new SqlCommand(s2, con);
                    string password = pass.ExecuteScalar().ToString();
                    //con.Close;

                    if (password == TxtPW.Text)
                    {
                        Session["myNewsession"] = TxtUN.Text;
                        Response.Redirect("StudentLoginDestination.aspx");
                        con.Close();
                    } //and inner if
                    else
                    {

                        lblAlert.Text = "Invalid password";
                    }//end inner else
                }//end outer if
                else
                {

                    lblAlert.Text = "Invalid user name";
                }//end else

            }
            catch (Exception)
            {

            } //end catch
                 
 
        }
    }
}
