using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Taxi_Project
{
    public partial class History : Page
    {
        private readonly string cs =
            @"Data Source=(LocalDB)\MSSQLLocalDB;" +
            @"AttachDbFilename=C:\Users\user\OneDrive\Desktop\I3332\asp.net\Taxi_Project\App_Data\Taxi_DB.mdf;" +
            @"Integrated Security=True;Connect Timeout=30;Encrypt=False";

        private SqlConnection GetConnection() => new SqlConnection(cs);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadTrips(null);
                SetActiveButton("All");
            }
        }

        // Cancel trip
        private void CancelTrip(int tripId)
        {
            using (SqlConnection con = GetConnection())
            {
                const string q =
                    "UPDATE Trips SET Status='Cancelled' " +
                    "WHERE TripID=@TripID AND Status='Pending'";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@TripID", tripId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Load & bind trips
        private void LoadTrips(string statusFilter)
        {
            int clientID = int.Parse(Session["UserID"].ToString());

            string query = @"
                SELECT
                    TripID,
                    PickupLocation,
                    DropoffLocation,
                    DistanceKm,
                    Price,
                    PickupTime,
                    Status
                FROM Trips
                WHERE ClientID = @ClientID
                  AND Status != 'Cancelled'
                  AND TripID NOT IN (SELECT TripID FROM Feedback)";

            if (!string.IsNullOrEmpty(statusFilter))
                query += " AND Status = @Status";

            query += " ORDER BY CreatedAt DESC";

            DataTable dt = new DataTable();

            using (SqlConnection con = GetConnection())
            {
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@ClientID", clientID);

                if (!string.IsNullOrEmpty(statusFilter))
                    cmd.Parameters.AddWithValue("@Status", statusFilter);

                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            gvTrips.DataSource = dt;
            gvTrips.DataBind();

            lblEmpty.Visible = (dt.Rows.Count == 0);
            gvTrips.Visible = (dt.Rows.Count > 0);
        }

        // Status badge styling
        public string GetBadgeClass(string status)
        {
            switch (status)
            {
                case "Active":
                    return "badge-active";

                case "Completed":
                    return "badge-completed";

                case "Cancelled":
                    return "badge-cancelled";

                default:
                    return "badge-pending";
            }
        }

        // GridView commands
        protected void gvTrips_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int tripId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "CancelTrip")
            {
                CancelTrip(tripId);

                // Reload all trips after cancel
                LoadTrips(null);
                SetActiveButton("All");
            }
            else if (e.CommandName == "LeaveFeedback")
            {
                Response.Redirect("Feedback.aspx?TripID=" + tripId);
            }
        }

        // Filter buttons
        protected void btnAll_Click(object sender, EventArgs e)
        {
            LoadTrips(null);
            SetActiveButton("All");
        }

        protected void btnPending_Click(object sender, EventArgs e)
        {
            LoadTrips("Pending");
            SetActiveButton("Pending");
        }

        protected void btnActive_Click(object sender, EventArgs e)
        {
            LoadTrips("Active");
            SetActiveButton("Active");
        }

        protected void btnCompleted_Click(object sender, EventArgs e)
        {
            LoadTrips("Completed");
            SetActiveButton("Completed");
        }

        // Highlight active filter button
        private void SetActiveButton(string which)
        {
            btnAll.CssClass =
                "filter-btn" + (which == "All" ? " active" : "");

            btnPending.CssClass =
                "filter-btn" + (which == "Pending" ? " active" : "");

            btnActive.CssClass =
                "filter-btn" + (which == "Active" ? " active" : "");

            btnCompleted.CssClass =
                "filter-btn" + (which == "Completed" ? " active" : "");
        }

        protected void lnkReserve_Click(object sender, EventArgs e)
        {
            Response.Redirect("Reserve.aspx");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("Login.aspx");
        }
    }
}