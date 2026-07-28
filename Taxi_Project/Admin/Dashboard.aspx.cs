using System;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Taxi_Project.Admin
{
    public partial class Dashboard : Page
    {
        private string ConnStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\user\OneDrive\Desktop\I3332\asp.net\Taxi_Project\App_Data\Taxi_DB.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False";

        private SqlConnection GetConnection()
        {
            return new SqlConnection(ConnStr);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")

                Response.Redirect("~/Login.aspx");
        }   

        protected void btnTripsToday_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/Profits.aspx");
        }

        protected void btnNewBookings_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/Bookings.aspx");
        }

        protected void btnTotalCars_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/Cars.aspx");
        }

        protected void btnDriversRegistered_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/Drivers.aspx");
        }


        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("~/Login.aspx");
        }
    }
}
