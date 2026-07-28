using System;
using System.Web.UI;

namespace Taxi_Project
{
    public partial class Home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CheckUserLoginStatus();
            }
        }

        private void CheckUserLoginStatus()
        {
            if (Session["UserID"] != null)
            {
                phLoggedIn.Visible = true; // for logged in users
                phLoggedOut.Visible = false; // for guests
                lblUserName.Text = Session["FullName"]?.ToString();
            }
            else
            {
                phLoggedIn.Visible = false;
                phLoggedOut.Visible = true;
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Home.aspx");
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            Response.Redirect("Register.aspx");
        }

        protected void btnStandard_Click(object sender, EventArgs e)
        {
            if (Session["UserID"] != null)
            {
                Session["SelectedCarType"] = "Standard"; // send to reserve page
                Response.Redirect("Reserve.aspx");
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }

        protected void btnBusiness_Click(object sender, EventArgs e)
        {
            if (Session["UserID"] != null)
            {
                Session["SelectedCarType"] = "Business";
                Response.Redirect("Reserve.aspx");
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }

        protected void btnVan_Click(object sender, EventArgs e)
        {
            if (Session["UserID"] != null)
            {
                Session["SelectedCarType"] = "Van";
                Response.Redirect("Reserve.aspx");
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }

        protected void btnMyTrips_Click(object sender, EventArgs e)
        {
            if (Session["UserID"] != null)
            {
                Response.Redirect("History.aspx");
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
    }
}