﻿<%@ Page Language="C#" Async="true" AutoEventWireup="true"
CodeBehind="AIInsights.aspx.cs"
Inherits="Taxi_Project.Admin.AIInsights" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>AI Decision Support — Admin</title>
    <link href="https://fonts.googleapis.com/css2?family=Space+Mono:wght@400;700&family=DM+Sans:wght@400;500;600;700&display=swap" rel="stylesheet">
    <style>
        :root {
            --bg-base:       #000;
            --bg-card:       #0d0d0d;
            --bg-card-hover: #131313;
            --bg-header:     #080808;
            --border:        #1f1f1f;
            --border-bright: #2a2a2a;
            --text-primary:  #f0f0f0;
            --text-secondary:#888;
            --text-muted:    #555;
            --accent-blue:   #3b82f6;
            --accent-violet: #8b5cf6;
            --accent-red:     #ef4444;
            --accent-green:   #22c55e;
            --accent-amber:   #f59e0b;
        }

        * { box-sizing: border-box; margin: 0; padding: 0; }
        body { font-family: 'DM Sans', sans-serif; background: var(--bg-base); color: var(--text-primary); min-height: 100vh; }

        .page-header {
            background: var(--bg-header); border-bottom: 1px solid var(--border);
            padding: 20px 40px; display: flex; align-items: center; gap: 14px;
            position: sticky; top: 0; z-index: 100;
        }
        .header-icon { font-size: 26px; }
        .page-title  { font-family: 'Space Mono', monospace; font-size: 18px; font-weight: 700; }
        .page-subtitle { font-size: 12px; color: var(--text-secondary); margin-top: 2px; }
        .header-badge {
            margin-left: auto; background: rgba(59,130,246,.1); border: 1px solid rgba(59,130,246,.3);
            color: var(--accent-blue); font-size: 10px; font-family: 'Space Mono', monospace;
            font-weight: 700; padding: 4px 10px; border-radius: 4px;
        }

        .toolbar {
            background: var(--bg-card); border-bottom: 1px solid var(--border);
            padding: 16px 40px; display: flex; align-items: center; justify-content: center; gap: 16px;
        }

        .btn-primary, .btn-secondary {
            padding: 12px 32px;
            border-radius: 10px;
            font-family: 'DM Sans', sans-serif;
            font-size: 15px;
            font-weight: 700;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            gap: 10px;
            transition: all .2s;
            border: none;
        }
        .btn-primary {
            background: rgba(59,130,246,.08);
            color: var(--accent-blue);
            border: 1px solid rgba(59,130,246,.5);
        }
        .btn-primary:hover {
            background: rgba(59,130,246,.15);
            transform: translateY(-1px);
        }
        .btn-secondary {
            background: rgba(139,92,246,.08);
            color: var(--accent-violet);
            border: 1px solid rgba(139,92,246,.5);
        }
        .btn-secondary:hover {
            background: rgba(139,92,246,.15);
            transform: translateY(-1px);
        }

        #loadingMsg {
            display: none; background: rgba(59,130,246,.06); border: 1px solid rgba(59,130,246,.25);
            border-radius: 10px; padding: 12px 18px; margin-bottom: 20px;
            font-size: 13px; font-weight: 600; color: var(--accent-blue);
            align-items: center; gap: 10px;
        }
        #loadingMsg .spinner {
            width: 15px; height: 15px; border: 2px solid rgba(59,130,246,.2);
            border-top-color: var(--accent-blue); border-radius: 50%;
            animation: spin .8s linear infinite;
        }
        @keyframes spin { to { transform: rotate(360deg); } }

        .main-content { padding: 28px 40px; max-width: 1400px; margin: 0 auto; }

        .section-title {
            font-family: 'Space Mono', monospace; font-size: 12px; font-weight: 700;
            letter-spacing: 1px; text-transform: uppercase; color: var(--text-secondary);
            margin-bottom: 14px; margin-top: 28px;
        }
        .section-title:first-of-type { margin-top: 0; }

        .kpi-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 16px;
            margin-bottom: 24px;
        }
        .kpi-card {
            background: var(--bg-card);
            border: 1px solid var(--border);
            border-radius: 12px;
            padding: 20px;
            text-align: center;
        }
        .kpi-value {
            font-family: 'Space Mono', monospace;
            font-size: 36px;
            font-weight: 700;
            margin-bottom: 8px;
        }
        .kpi-label {
            font-size: 12px;
            color: var(--text-secondary);
            text-transform: uppercase;
            letter-spacing: 1px;
        }

        .sentiment-grid {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 16px;
            margin-bottom: 24px;
        }
        .sentiment-card {
            background: var(--bg-card);
            border: 1px solid var(--border);
            border-radius: 12px;
            padding: 20px;
            text-align: center;
        }
        .sentiment-positive { border-top: 3px solid var(--accent-green); }
        .sentiment-neutral { border-top: 3px solid var(--accent-amber); }
        .sentiment-negative { border-top: 3px solid var(--accent-red); }
        .sentiment-count {
            font-size: 32px;
            font-weight: 700;
            margin-bottom: 8px;
        }
        .sentiment-percent {
            font-size: 14px;
            color: var(--text-secondary);
        }

        
        
        .feedback-grid {
            width: 100%;
            border-collapse: collapse;
            font-size: 13.5px;
        }
        .feedback-grid th {
            text-align: left;
            padding: 12px 14px;
            font-family: 'Space Mono', monospace;
            font-size: 10px;
            letter-spacing: .5px;
            text-transform: uppercase;
            color: var(--text-muted);
            border-bottom: 2px solid var(--border);
            background: var(--bg-card);
        }
        .feedback-grid td {
            padding: 11px 14px;
            border-bottom: 1px solid var(--border);
            vertical-align: middle;
        }
        .feedback-grid tr:hover td {
            background: var(--bg-card-hover);
        }

        .badge {
            display: inline-flex;
            align-items: center;
            gap: 4px;
            padding: 3px 10px;
            border-radius: 6px;
            font-size: 12px;
            font-weight: 600;
        }
        .badge-positive { background: rgba(34,197,94,.1); border: 1px solid rgba(34,197,94,.3); color: #4ade80; }
        .badge-negative { background: rgba(239,68,68,.1); border: 1px solid rgba(239,68,68,.3); color: #f87171; }
        .badge-neutral  { background: rgba(245,158,11,.1); border: 1px solid rgba(245,158,11,.3); color: #fbbf24; }
        .badge-complaint{ background: rgba(239,68,68,.1); border: 1px solid rgba(239,68,68,.3); color: #f87171; }
        .badge-normal   { background: rgba(34,197,94,.1); border: 1px solid rgba(34,197,94,.3); color: #4ade80; }

        .empty { text-align: center; padding: 60px 20px; color: var(--text-muted); }

        @media (max-width: 768px) {
            .page-header, .toolbar, .main-content { padding-left: 20px; padding-right: 20px; }
            .sentiment-grid { grid-template-columns: 1fr; }
        }
    </style>
</head>

<body>
<form id="form1" runat="server">
    <asp:ScriptManager runat="server" />

    <header class="page-header">
        <div class="header-icon">🚖</div>
        <div>
            <div class="page-title">AI Decision Support Panel</div>
            <div class="page-subtitle">Intelligent feedback analysis powered by Hugging Face</div>
        </div>
        <div class="header-badge">Admin</div>
    </header>

    <div class="toolbar">
        <asp:Button ID="btnRunAnalysis" CssClass="btn-primary" runat="server" 
            Text="🤖 Run AI Analysis" OnClick="btnRunAll_Click" OnClientClick="showLoading()" />
        <asp:Button ID="btnDashboard" CssClass="btn-secondary" runat="server" 
            Text="📊 Back to Dashboard" OnClick="btnDashboard_Click" />
    </div>

    <div class="main-content">
        <div id="loadingMsg">
            <div class="spinner"></div>
            Hugging Face AI is analysing feedback, please wait…
        </div>
        
        <asp:Panel ID="pnlContent" runat="server" Visible="false">
            <!-- KPIs -->
            <div class="kpi-grid">
                <div class="kpi-card">
                    <div class="kpi-value"><asp:Literal ID="litTotalFeedback" runat="server" /></div>
                    <div class="kpi-label">📝 Total Feedback</div>
                </div>
                <div class="kpi-card">
                    <div class="kpi-value"><asp:Literal ID="litAvgRating" runat="server" /></div>
                    <div class="kpi-label">⭐ Average Rating</div>
                </div>
                <div class="kpi-card">
                    <div class="kpi-value"><asp:Literal ID="litComplaints" runat="server" /></div>
                    <div class="kpi-label">🚨 Complaints</div>
                </div>
            </div>

            <!-- Sentiment Analysis -->
            <div class="section-title">📊 Sentiment Analysis</div>
            <div class="sentiment-grid">
                <div class="sentiment-card sentiment-positive">
                    <div class="sentiment-count"><asp:Literal ID="litPositiveCount" runat="server" /></div>
                    <div class="sentiment-percent"><asp:Literal ID="litPositivePercent" runat="server" />% Positive</div>
                </div>
                <div class="sentiment-card sentiment-neutral">
                    <div class="sentiment-count"><asp:Literal ID="litNeutralCount" runat="server" /></div>
                    <div class="sentiment-percent"><asp:Literal ID="litNeutralPercent" runat="server" />% Neutral</div>
                </div>
                <div class="sentiment-card sentiment-negative">
                    <div class="sentiment-count"><asp:Literal ID="litNegativeCount" runat="server" /></div>
                    <div class="sentiment-percent"><asp:Literal ID="litNegativePercent" runat="server" />% Negative</div>
                </div>
            </div>

                    
            <!-- Feedback Details -->
            <div class="section-title">📋 Feedback Details</div>
            <asp:GridView ID="gvFeedback" runat="server" AutoGenerateColumns="false" 
                CssClass="feedback-grid" GridLines="None" 
                PageSize="10" >
                <Columns>
                    <asp:BoundField HeaderText="#" DataField="Id" />
                    <asp:BoundField HeaderText="Comment" DataField="Comment" />
                    <asp:BoundField HeaderText="Rating" DataField="Rating" />
                    <asp:TemplateField HeaderText="Sentiment">
                        <ItemTemplate>
                            <span class='badge <%# GetSentimentClass(Eval("Sentiment")) %>'>
                                <%# GetSentimentIcon(Eval("Sentiment")) %> <%# Eval("Sentiment") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='badge <%# GetFlagClass(Eval("Flag")) %>'>
                                <%# GetFlagIcon(Eval("Flag")) %> <%# Eval("Flag") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <PagerStyle CssClass="kpi-label" />
            </asp:GridView>

            
        </asp:Panel>

        <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
            <div class="empty">
                <span class="empty-icon">📭</span>
                <div class="empty-text">No feedback found.</div>
            </div>
        </asp:Panel>
    </div>

</form>

<script>
    function showLoading() {
        document.getElementById('loadingMsg').style.display = 'flex';
        window.scrollTo({ top: document.querySelector('.toolbar').offsetTop, behavior: 'smooth' });
    }
</script>
</body>
</html>
