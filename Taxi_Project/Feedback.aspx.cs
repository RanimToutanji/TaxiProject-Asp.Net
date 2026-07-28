using System;
using System.Data.SqlClient;
using System.Web.UI;

namespace Taxi_Project
{
    public partial class Feedback : Page
    {
        // Connection string
        private readonly string cs =
            @"Data Source=(LocalDB)\MSSQLLocalDB;" +
            @"AttachDbFilename=C:\Users\user\OneDrive\Desktop\I3332\asp.net\Taxi_Project\App_Data\Taxi_DB.mdf;" +
            @"Integrated Security=True;Connect Timeout=30;Encrypt=False";

        private SqlConnection GetConnection() => new SqlConnection(cs);

        
        protected void Page_Load(object sender, EventArgs e)
        {
            // Auth guard
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // TripID must be in the query string
            string tripIdParam = Request.QueryString["TripID"]; // part of link after ? in URL
            if (string.IsNullOrEmpty(tripIdParam))
            {
                Response.Redirect("History.aspx");
                return;
            }

            int tripId;
            if (!int.TryParse(tripIdParam, out tripId))
            {
                Response.Redirect("History.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // Store TripID in hidden field so postback can read it
                hdnTripID.Value = tripId.ToString();

                // Validate the trip belongs to this user and is Completed with no feedback yet
                if (!ValidTrip(tripId))
                {
                    lblError.Text = "This trip is not available for feedback.";
                    lblError.Visible = true;
                    btnSubmit.Enabled = false;
                    return;
                }

               
            }
        }

        // Validate trip ownership, status, and no duplicate feedback
        private bool ValidTrip(int tripId)
        {
            int clientID = int.Parse(Session["UserID"].ToString());

            const string q = @"
                SELECT COUNT(1)  
                FROM   Trips
                WHERE  TripID    = @TripID
                  AND  ClientID  = @ClientID
                  AND  Status    = 'Completed'
                  AND  TripID NOT IN (SELECT TripID FROM Feedback)";

            using (SqlConnection con = GetConnection())
            {
                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@TripID", tripId);
                cmd.Parameters.AddWithValue("@ClientID", clientID);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        // Validate rating manually
        private bool ValidateRating(out int rating)
        {
            rating = 0;

            // Check if empty
            if (string.IsNullOrWhiteSpace(txtRating.Text))
            {
                lblRatingError.Text = "Rating is required.";
                return false;
            }

            // Check if it's a valid number
            if (!int.TryParse(txtRating.Text.Trim(), out rating))
            {
                lblRatingError.Text = "Rating must be a number between 1 and 5.";
                return false;
            }

            // Check range
            if (rating < 1 || rating > 5)
            {
                lblRatingError.Text = "Rating must be between 1 and 5.";
                return false;
            }

            // Clear any previous error
            lblRatingError.Text = "";
            return true;
        }

        // Submit feedback
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Manual validation
            int rating;
            if (!ValidateRating(out rating))
            {
                return;
            }

            int tripId = int.Parse(hdnTripID.Value);
            int clientID = int.Parse(Session["UserID"].ToString());
            string comment = txtComment.Text.Trim();

            // Double-check the trip is still valid (race condition / browser back)
            if (!ValidTrip(tripId))
            {
                lblError.Text = "Feedback has already been submitted for this trip.";
                lblError.Visible = true;
                return;
            }

            try
            {
                using (SqlConnection con = GetConnection())
                {
                    const string q = @"
                        INSERT INTO Feedback (TripID, ClientID, Rating, Comment)
                        VALUES (@TripID, @ClientID, @Rating, @Comment)";

                    SqlCommand cmd = new SqlCommand(q, con);
                    cmd.Parameters.AddWithValue("@TripID", tripId);
                    cmd.Parameters.AddWithValue("@ClientID", clientID);
                    cmd.Parameters.AddWithValue("@Rating", rating);
                    cmd.Parameters.AddWithValue("@Comment", comment);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                // Success — redirect back to history
                Response.Redirect("History.aspx");
            }
            catch (Exception ex)
            {
                lblError.Text = "An error occurred while saving your feedback. Please try again.";
                lblError.Visible = true;
                // Log ex.Message to your logging system if available
            }
        }

        // Logout
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("Login.aspx");
        }
    }
}
