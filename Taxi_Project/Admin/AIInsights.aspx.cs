using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Taxi_Project.Admin
{
    public class FeedbackItem 
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public string Sentiment { get; set; } = "NEUTRAL"; // or "POSITIVE" or "NEGATIVE" 
        public string Flag { get; set; } = "Normal"; // or "Complaint"
    }

    public partial class AIInsights : System.Web.UI.Page
    {
        private static readonly string ApiKey =
    Environment.GetEnvironmentVariable("HUGGINGFACE_API_KEY");
        private const string SentimentUrl = "https://router.huggingface.co/hf-inference/models/cardiffnlp/twitter-roberta-base-sentiment"; // ai model for sentiment analysis 
        

        private string cs = @"Data Source=(LocalDB)\MSSQLLocalDB;
            AttachDbFilename=C:\Users\user\OneDrive\Desktop\I3332\asp.net\Taxi_Project\App_Data\Taxi_DB.mdf;
            Integrated Security=True";

        private static string[] ComplaintWords =
        {
            "late","delay","delayed","rude","bad","worst","terrible","horrible",
            "awful","poor","unacceptable","slow","angry","upset","problem","issue",
            "complaint","never","disappointed","unprofessional","dangerous","accident",
            "crash","dirty","broken","wrong","cheated","overcharged","cancel","cancelled"
        };

        private List<FeedbackItem> _feedbackItems; // store processed feedback for display 

        // DATA ACCESS 

        private List<FeedbackItem> LoadFeedback()
        {
            var list = new List<FeedbackItem>();
            using (var con = new SqlConnection(cs))
            {
                var cmd = new SqlCommand("SELECT FeedbackID, Rating, Comment FROM Feedback", con);
                con.Open();
                var r = cmd.ExecuteReader();
                while (r.Read())
                    list.Add(new FeedbackItem
                    {
                        Id = Convert.ToInt32(r["FeedbackID"]),
                        Rating = Convert.ToInt32(r["Rating"]),
                        Comment = r["Comment"]?.ToString() ?? ""
                    });
            }
            return list;
        }

        // AI PROCESSING

        private async Task<List<FeedbackItem>> ProcessFeedbackAsync()
        {
            var items = LoadFeedback();
            foreach (var fb in items)
            {
                fb.Sentiment = await GetSentiment(fb.Comment);  
                fb.Flag = DetectComplaint(fb);
            }
            return items;
        }

        private async Task<string> GetSentiment(string text) 
        {
            if (string.IsNullOrWhiteSpace(text)) return "NEUTRAL";
            try
            {
                using (var client = new HttpClient()) // http request to Hugging Face API for sentiment analysis
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
                    var content = new StringContent(JsonConvert.SerializeObject(new { inputs = text }), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(SentimentUrl, content);
                    var result = await response.Content.ReadAsStringAsync();

                    if (result.Contains("estimated_time") || result.Contains("loading"))
                    {
                        await Task.Delay(20000);
                        result = await (await client.PostAsync(SentimentUrl, content)).Content.ReadAsStringAsync();  
                    }

                    return result.Contains("error") ? "NEUTRAL" : ParseSentimentLabel(result);
                }
            }
            catch { return "NEUTRAL"; } 
        }

        private static string ParseSentimentLabel(string json) // Ai output -> sentiment label (POSITIVE, NEGATIVE, NEUTRAL)
        {
            try
            {
                var inner = (JArray)JArray.Parse(json)[0];
                string best = "LABEL_1";
                double top = -1;

                foreach (var item in inner)
                {
                    double s;

                    if (item["score"] != null)
                    {
                        s = item["score"].ToObject<double>();
                    }
                    else
                    {
                        s = 0;
                    }

                    if (s > top)
                    {
                        top = s;

                        if (item["label"] != null)
                        {
                            best = item["label"].ToString();
                        }
                        else
                        {
                            best = "LABEL_1";
                        }
                    }
                }

                if (best == "LABEL_0")
                {
                    return "NEGATIVE";
                }
                else if (best == "LABEL_2")
                {
                    return "POSITIVE";
                }
                else
                {
                    return "NEUTRAL";
                }
            }
            catch
            {
                return "NEUTRAL";
            }
        }

        // RULE-BASED

        private static string DetectComplaint(FeedbackItem fb)
        {
            if (fb.Sentiment == "NEGATIVE" && fb.Rating <= 2) return "Complaint";
            var lower = fb.Comment.ToLower();
            return ComplaintWords.Any(w => lower.Contains(w)) ? "Complaint" : "Normal";
        }

        // just for styling and icons based on sentiment and flag

        protected string GetSentimentClass(object sentiment)
        {
            string s = sentiment.ToString();
            return s == "POSITIVE" ? "badge-positive" : (s == "NEGATIVE" ? "badge-negative" : "badge-neutral");
        }

        protected string GetSentimentIcon(object sentiment)
        {
            string s = sentiment.ToString();
            return s == "POSITIVE" ? "😊" : (s == "NEGATIVE" ? "😞" : "😐");
        }

        protected string GetFlagClass(object flag)
        {
            return flag.ToString() == "Complaint" ? "badge-complaint" : "badge-normal";
        }

        protected string GetFlagIcon(object flag)
        {
            return flag.ToString() == "Complaint" ? "🚨" : "✅";
        }

        // MAIN HANDLER 

        protected async void btnRunAll_Click(object sender, EventArgs e)
        {
            try
            {
                pnlContent.Visible = false;
                pnlEmpty.Visible = false;

                _feedbackItems = await ProcessFeedbackAsync();

                if (_feedbackItems.Count == 0)
                {
                    pnlEmpty.Visible = true;
                    return;
                }

                // Calculate stats
                int total = _feedbackItems.Count;
                int positive = _feedbackItems.Count(x => x.Sentiment == "POSITIVE");
                int negative = _feedbackItems.Count(x => x.Sentiment == "NEGATIVE");
                int neutral = _feedbackItems.Count(x => x.Sentiment == "NEUTRAL");
                int complaints = _feedbackItems.Count(x => x.Flag == "Complaint");
                double avgRating = _feedbackItems.Average(x => x.Rating);
               
                // Bind KPIs
                litTotalFeedback.Text = total.ToString();
                litAvgRating.Text = $"{avgRating:F1}/5";
                litComplaints.Text = complaints.ToString();

                // Bind Sentiment
                litPositiveCount.Text = positive.ToString();
                litPositivePercent.Text = ((positive / (double)total) * 100).ToString("F1");
                litNeutralCount.Text = neutral.ToString();
                litNeutralPercent.Text = ((neutral / (double)total) * 100).ToString("F1");
                litNegativeCount.Text = negative.ToString();
                litNegativePercent.Text = ((negative / (double)total) * 100).ToString("F1");

                // Bind GridView
                var dataTable = new System.Data.DataTable();
                dataTable.Columns.Add("Id");
                dataTable.Columns.Add("Comment");
                dataTable.Columns.Add("Rating");
                dataTable.Columns.Add("Sentiment");
                dataTable.Columns.Add("Flag");

                foreach (var fb in _feedbackItems.OrderByDescending(x => x.Flag == "Complaint").ThenByDescending(x => x.Id))
                {
                    string comment = fb.Comment.Length > 80 ? fb.Comment.Substring(0, 80) + "..." : fb.Comment;
                    dataTable.Rows.Add(fb.Id, comment, $"{new string('⭐', fb.Rating)} ({fb.Rating})", fb.Sentiment, fb.Flag);
                }

                gvFeedback.DataSource = dataTable;
                gvFeedback.DataBind();

                pnlContent.Visible = true;
            }
            catch (Exception ex)
            {
                pnlContent.Visible = true;
                
            }
        }

        protected void btnDashboard_Click(object sender, EventArgs e)
        {
            Response.Redirect("Dashboard.aspx");
        }
    }
}
