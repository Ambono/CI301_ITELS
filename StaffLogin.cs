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
    public partial class WebForm2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BtnSfLogin_Click(object sender, EventArgs e)
        {
            try
            {

                // SqlConnection con = new SqlConnection("metadata=res://*/ITEquipment.csdl|res://*/ITEquipment.ssdl|res://*/ITEquipment.msl;provider=System.Data.SqlClient;provider connection string='data source=(LocalDB)\v11.0;attachdbfilename=&quot;C:\ProjectCI301\IT Equipment Loan System\Database\DB\ITEquipment.mdf&quot;;integrated security=True;connect timeout=30;MultipleActiveResultSets=True;App=EntityFramework'" providerName="System.Data.EntityClient);

                SqlConnection con = new SqlConnection();
                ///http://stackoverflow.com/questions/20198527/cant-add-stored-procedure-in-visual-studio-2012


                //  Data Source=(LocalDB)\v11.0;AttachDbFilename="C:\ProjectCI301\IT Equipment Loan System\IT Equipment Loan System\App_Data\ITEquipment.mdf";Integrated Security=True;MultipleActiveResultSets=True;Connect Timeout=30;Application Name=EntityFramework

                con.ConnectionString = ConfigurationManager.ConnectionStrings["ITES"].ConnectionString;

                con.Open();


                //  string s1 = "select * from Students WHERE StudentUN ='UN1' ";
                string s1 = "select count(*) from Staff WHERE StaffUN ='" + TxtSfUN.Text + "' ";



                SqlCommand cmd = new SqlCommand(s1, con);
                int i = Convert.ToInt16(cmd.ExecuteScalar().ToString());


                if (i >= 1)
                {
                    string s2 = "select StaffPW from Staff WHERE  StaffPW= '" + TxtSfPW.Text + "'";
                    SqlCommand pass = new SqlCommand(s2, con);
                    string password = pass.ExecuteScalar().ToString();
                    //con.Close;

                    if (password == TxtSfPW.Text)
                    {
                        Session["myStaffNewSession"] = TxtSfUN.Text;
                        Response.Redirect("StaffLoginDestination.aspx");
                        con.Close();
                    } //and inner if
                    else
                    {

                        lblSfAlert.Text = "Invalid password";
                    }//end inner else
                }//end outer if
                else
                {

                    lblSfAlert.Text = "Invalid user name";
                }//end else

            }
            catch (Exception)
            {

            } //end catch
                 
        }
    }
}
