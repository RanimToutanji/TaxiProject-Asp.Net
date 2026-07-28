using System;
using System.Data;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace Taxi_Project
{
    public partial class Register : Page
    {
        private string cs = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\user\OneDrive\Desktop\I3332\asp.net\Taxi_Project\App_Data\Taxi_DB.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False";
        private SqlConnection GetConnection()
        {
            return new SqlConnection(cs);
        }

        protected void Page_Load(object sender, EventArgs e) { }

        protected void btnSignInTab_Click(object sender, EventArgs e)
        {
            Response.Redirect("login.aspx");
        }

        protected void btnRegisterTab_Click(object sender, EventArgs e) { }

        protected void btnRegister_Click(object sender, EventArgs e)
        {

            using (SqlConnection conn = GetConnection())
            {

                try
                {
                    if (string.IsNullOrWhiteSpace(txtFullName.Text))
                    {

                        Response.Write("<script>alert('name should be not emtpy ')</script>");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txtEmail.Text) || //email have specific condition
                    !txtEmail.Text.Contains("@") ||
                    !txtEmail.Text.Contains(".") ||
                    txtEmail.Text.IndexOf("@") > txtEmail.Text.LastIndexOf("."))
                    {
                        Response.Write("<script>alert('Please enter a valid email address containing @ and .')</script>");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        Response.Write("<script>alert('pass should be not emtpy ')</script>");
                        return;
                    }
                    string password = txtPassword.Text;

                    if (password.Length < 8 ||
                    !Regex.IsMatch(password, @"[A-Z]") ||
                    !Regex.IsMatch(password, @"[0-9]") ||
                    !Regex.IsMatch(password, @"[!@#$%^&*]"))
                    {
                        Response.Write("<script>alert('Password must be at least 8 characters, contain one uppercase, one number, and one special character')</script>");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txtConfirmPassword.Text) || txtConfirmPassword.Text != password)
                    {
                        Response.Write("<script>alert('Passwords do not match')</script>");
                        return;
                    }

                    conn.Open();
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT INTO Users (FullName, Email, PasswordHash, Role) VALUES (@FullName, @Email, @PasswordHash, @Role)"; // insert sql<<<<3

                    cmd.Parameters.AddWithValue("@FullName", txtFullName.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@PasswordHash", txtPassword.Text);
                    cmd.Parameters.AddWithValue("@Role", "Client");
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    SqlCommand cmdGetUser = conn.CreateCommand();
                    cmdGetUser.CommandType = CommandType.Text;
                    cmdGetUser.CommandText = "SELECT UserID, FullName FROM Users WHERE Email = @Email";
                    cmdGetUser.Parameters.AddWithValue("@Email", txtEmail.Text);

                    SqlDataReader reader = cmdGetUser.ExecuteReader();
                    if (reader.Read())
                    {
                        Session["UserID"] = reader["UserID"].ToString();
                        Session["FullName"] = reader["FullName"].ToString();
                    }
                    reader.Close();
                    cmdGetUser.Dispose();
                    conn.Close();
                    txtFullName.Text = "";
                    Response.Write("<script>alert('Successfully Input')</script>");
                    Response.Redirect("Home.aspx");


                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('An error occurred: " + ex.Message + "')</script>");
                }
            }

        }

        protected void lnkLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("login.aspx");
        }
    }
}