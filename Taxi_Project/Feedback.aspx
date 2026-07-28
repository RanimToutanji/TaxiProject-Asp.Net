﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Feedback.aspx.cs" Inherits="Taxi_Project.Feedback" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Yalla Taxi – Leave Feedback</title>
    <link href="https://fonts.googleapis.com/css2?family=Bebas+Neue&family=Inter:wght@300;400;500;600;700&display=swap" rel="stylesheet" />
    <style>
        * { box-sizing: border-box; margin: 0; padding: 0; }
        html, body { height: 100%; }
        body { font-family: 'Inter', sans-serif; background: #0f0f0f; color: #fff; }

        /* ── Navbar ── */
        .navbar {
            position: fixed; top: 0; left: 0; right: 0; height: 60px;
            background: #1a1a1a;
            border-bottom: 1px solid rgba(245,195,0,0.15);
            display: flex; align-items: center; justify-content: space-between;
            padding: 0 2rem; z-index: 1000;
        }
        .logo-badge {
            background: #F5C300; border-radius: 8px; padding: 5px 12px;
            display: flex; align-items: center; gap: 6px;
        }
        .logo-badge span { font-family:'Bebas Neue',sans-serif; font-size:18px; color:#111; letter-spacing:2px; }
        .logo-sep { width:2px; height:18px; background:#111; opacity:0.3; }
        .btn-logout {
            background:transparent; border:1px solid rgba(245,195,0,0.3);
            color:#F5C300; border-radius:8px; padding:6px 14px; font-size:12px; cursor:pointer;
        }

        /* ── Card ── */
        .page {
            margin-top: 100px;
            padding: 2rem;
            display: flex;
            justify-content: center;
        }
        .card {
            background: #1a1a1a;
            border: 1px solid rgba(255,255,255,0.07);
            border-radius: 20px;
            padding: 2.5rem;
            width: 100%;
            max-width: 480px;
        }
        .card-title { font-size: 22px; font-weight: 700; margin-bottom: 0.3rem; }
        .card-title span { color: #F5C300; }
        .card-sub { font-size: 13px; color: #555; margin-bottom: 2rem; }

        /* ── Trip info box ── */
        .trip-info {
            background: #111;
            border: 1px solid rgba(255,255,255,0.06);
            border-radius: 12px;
            padding: 14px 16px;
            margin-bottom: 1.8rem;
            font-size: 13px;
            color: #aaa;
            line-height: 1.7;
        }
        .trip-info strong { color: #fff; }

        /* ── Form fields ── */
        .field { margin-bottom: 1.4rem; }
        .field label {
            display: block;
            font-size: 12px;
            font-weight: 600;
            color: #888;
            text-transform: uppercase;
            letter-spacing: 0.06em;
            margin-bottom: 8px;
        }
        .field input[type="text"],
        .field textarea {
            width: 100%;
            background: #111;
            border: 1px solid rgba(255,255,255,0.08);
            border-radius: 10px;
            padding: 10px 14px;
            color: #fff;
            font-family: 'Inter', sans-serif;
            font-size: 14px;
            transition: border-color 0.2s;
        }
        .field input[type="text"]:focus,
        .field textarea:focus {
            outline: none;
            border-color: #F5C300;
        }
        .field textarea { resize: vertical; min-height: 100px; }

        /* Rating hint */
        .rating-hint { font-size: 11px; color: #555; margin-top: 5px; }

        /* ── Validation error ── */
        .field-error { color: #f87171; font-size: 12px; margin-top: 5px; display: block; }

        /* ── Buttons ── */
        .btn-row { display: flex; gap: 10px; margin-top: 0.5rem; }
        .btn-submit {
            flex: 1; background: #F5C300; color: #111; border: none;
            border-radius: 10px; padding: 11px; font-size: 14px;
            font-weight: 700; cursor: pointer;
        }
        .btn-submit:hover { background: #e0b200; }
        .btn-back {
            flex: 1; background: transparent; color: #555;
            border: 1px solid rgba(255,255,255,0.1); border-radius: 10px;
            padding: 11px; font-size: 14px; cursor: pointer; text-align: center;
            text-decoration: none; display: flex; align-items: center; justify-content: center;
        }
        .btn-back:hover { color: #fff; border-color: rgba(255,255,255,0.3); }

        /* ── Error / success banners ── */
        .banner-error {
            background: rgba(248,113,113,0.1); border: 1px solid rgba(239,68,68,0.3);
            color: #f87171; border-radius: 10px; padding: 12px 16px;
            margin-bottom: 1.5rem; font-size: 13px;
        }
        .banner-success {
            background: rgba(74,222,128,0.1); border: 1px solid rgba(74,222,128,0.3);
            color: #4ade80; border-radius: 10px; padding: 12px 16px;
            margin-bottom: 1.5rem; font-size: 13px;
        }
    </style>
</head>

<body>
<form id="form1" runat="server">

    <!-- Navbar -->
    <div class="navbar">
        <div class="logo-badge">
            <span>YALLA</span>
            <div class="logo-sep"></div>
            <span>TAXI</span>
        </div>
        <asp:Button ID="btnLogout" runat="server" Text="Logout"
                    CssClass="btn-logout" OnClick="btnLogout_Click" />
    </div>

    <!-- Card -->
    <div class="page">
        <div class="card">
            <p class="card-title">Leave <span>Feedback</span></p>
            <p class="card-sub">Rate your experience and help us improve.</p>

            <!-- Error / success banners -->
            <asp:Label ID="lblError"   runat="server" Visible="false" CssClass="banner-error" />
            <asp:Label ID="lblSuccess" runat="server" Visible="false" CssClass="banner-success" />

            <!-- Hidden field: TripID -->
            <asp:HiddenField ID="hdnTripID" runat="server" />

            <!-- Rating -->
            <div class="field">
                <label>Rating (1 – 5)</label>
                <asp:TextBox ID="txtRating" runat="server" MaxLength="1"
                             placeholder="Enter a number from 1 to 5" />
                <asp:Label ID="lblRatingError" runat="server" CssClass="field-error" />
                <span class="rating-hint">⭐ 1 = Poor &nbsp;|&nbsp; 5 = Excellent</span>
            </div>

            <!-- Comment -->
            <div class="field">
                <label>Comment <span style="color:#555;font-size:10px;">(optional)</span></label>
                <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine"
                             placeholder="Share your experience with the driver..." />
            </div>

            <!-- Buttons -->
            <div class="btn-row">
                <asp:Button ID="btnSubmit" runat="server" Text="Submit Feedback"
                            CssClass="btn-submit" OnClick="btnSubmit_Click" />
                <a href="History.aspx" class="btn-back">← Back</a>
            </div>

        </div>
    </div>

</form>
</body>
</html>
