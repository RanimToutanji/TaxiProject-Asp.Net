using System;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Taxi_Project.Admin
{
    public partial class Calendar : Page
    {
        private string ConnStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\user\OneDrive\Desktop\I3332\asp.net\Taxi_Project\App_Data\Taxi_DB.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False";

        private SqlConnection GetConnection()
        {
            return new SqlConnection(ConnStr);
        } 

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
        }

        protected void calTrips_DayRender(object sender, DayRenderEventArgs e) 
        {
            if (e.Day.IsOtherMonth) return;  

            int count = GetTripCountForDay(e.Day.Date);//bt3tine date klu e obj,day lyom date klu 12/5/2026
            //ana bi hajte la 22dr jib mn db inform
            if (count > 0)
            {
                string dateStr = e.Day.Date.ToString("yyyy-MM-dd");//am hawlu la date string ken obj ta hata eb3tu bl link 

                Label badge = new Label();
                badge.Text = $"<br/><a href='DayTrips.aspx?date={dateStr}' class='trip-count'>{count} trip{(count > 1 ? "s" : "")}</a>"; // redirect la page daytrips 
                e.Cell.Controls.Add(badge);//e,cell mtl <td>
            }
        }//dayrender is event bl calender byntnfz la kl yom

        private int GetTripCountForDay(DateTime date)//3aded ltrip ta ybynu bl calendd bi hed ldate:))))
        {
            using (SqlConnection con = GetConnection())
            {
                con.Open();
                string sql = "SELECT COUNT(*) FROM Trips WHERE CAST(PickupTime AS DATE) = @Date";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Date", date.Date);
                    return (int)cmd.ExecuteScalar();//bt3te nb trip bl day 
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/Bookings.aspx");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("~/Login.aspx");
        }
    }
}