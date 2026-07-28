﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="History.aspx.cs" Inherits="Taxi_Project.History" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Yalla Taxi – My Trips</title>
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
        .nav-links { display:flex; gap:2rem; align-items:center; }
        .nav-links a { font-size:13px; color:#555; text-decoration:none; font-weight:500; }
        .nav-links a.active { color:#F5C300; }
        .btn-logout {
            background:transparent; border:1px solid rgba(245,195,0,0.3);
            color:#F5C300; border-radius:8px; padding:6px 14px; font-size:12px; cursor:pointer;
        }

        /* ── Page ── */
        .page { margin-top:80px; padding:2rem; max-width:1100px; margin-left:auto; margin-right:auto; }
        .page-title { font-size:28px; font-weight:700; }
        .page-title span { color:#F5C300; }
        .page-sub { font-size:14px; color:#555; margin-bottom:2rem; }

        /* ── Filter tabs ── */
        .filter-tabs { display:flex; gap:8px; margin-bottom:2rem; flex-wrap:wrap; }
        .filter-btn {
            height:36px; padding:0 18px; border-radius:50px;
            border:1px solid rgba(255,255,255,0.07);
            background:#1a1a1a; color:#555;
            font-size:13px; font-weight:500; cursor:pointer;
        }
        .filter-btn.active {
            background:#F5C300 !important; color:#111 !important;
            border-color:#F5C300 !important; font-weight:700 !important;
        }

        /* ── GridView table ── */
        .grid-wrap { overflow-x: auto; }

        table.trip-grid {
            width: 100%;
            border-collapse: collapse;
            background: #1a1a1a;
            border-radius: 18px;
            overflow: hidden;
            border: 1px solid rgba(255,255,255,0.07);
        }
        table.trip-grid th {
            background: #111;
            color: #888;
            font-size: 11px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.08em;
            padding: 14px 16px;
            text-align: left;
            border-bottom: 1px solid rgba(255,255,255,0.07);
        }
        table.trip-grid td {
            padding: 14px 16px;
            font-size: 13px;
            color: #ccc;
            border-bottom: 1px solid rgba(255,255,255,0.05);
            vertical-align: middle;
        }
        table.trip-grid tr:last-child td { border-bottom: none; }
        table.trip-grid tr:hover td { background: rgba(255,255,255,0.02); }

        /* ── Badges ── */
        .badge { padding:4px 12px; border-radius:50px; font-size:11px; font-weight:600; white-space:nowrap; }
        .badge-pending   { color:#F5C300; border:1px solid rgba(245,195,0,0.3); }
        .badge-active    { color:#60a5fa; border:1px solid rgba(59,130,246,0.3); }
        .badge-completed { color:#4ade80; border:1px solid rgba(34,197,94,0.3); }
        .badge-cancelled { color:#f87171; border:1px solid rgba(239,68,68,0.3); }

        /* ── Price ── */
        .trip-price { font-family:'Bebas Neue',sans-serif; font-size:20px; color:#F5C300; }

        /* ── Action buttons ── */
        .btn-feedback {
            background:#F5C300; color:#111; border:none; border-radius:8px;
            padding:6px 14px; font-size:12px; cursor:pointer; font-weight:600;
            text-decoration:none; display:inline-block;
        }
        .btn-cancel {
            background:transparent; border:1px solid rgba(239,68,68,0.4);
            color:#f87171; border-radius:8px; padding:6px 14px;
            font-size:12px; cursor:pointer; text-decoration:none; display:inline-block;
        }

        /* ── Empty state ── */
        .empty-msg { color:#555; text-align:center; margin-top:2rem; font-size:14px; }

        /* ── Success/Error message ── */
        .msg-success {
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
        <div class="nav-links">
            <asp:LinkButton ID="lnkReserve" runat="server" OnClick="lnkReserve_Click" style="font-size:13px;color:#555;font-weight:500;">Reserve</asp:LinkButton>
            <a class="active" href="History.aspx">My Trips</a>
        </div>
        <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn-logout" OnClick="btnLogout_Click" />
    </div>

    <!-- Page body -->
    <div class="page">
        <p class="page-title">My <span>Trips</span></p>
        <p class="page-sub">Track all your reservations</p>

        <!-- Success message (shown after cancel) -->
        <asp:Label ID="lblMessage" runat="server" Visible="false" CssClass="msg-success" />

        <!-- Filter tabs -->
        <div class="filter-tabs">
            <asp:Button ID="btnAll"       runat="server" Text="All"          CssClass="filter-btn" OnClick="btnAll_Click" />
            <asp:Button ID="btnPending"   runat="server" Text="⏳ Pending"   CssClass="filter-btn" OnClick="btnPending_Click" />
            <asp:Button ID="btnActive"    runat="server" Text="🔵 Active"    CssClass="filter-btn" OnClick="btnActive_Click" />
            <asp:Button ID="btnCompleted" runat="server" Text="✅ Completed" CssClass="filter-btn" OnClick="btnCompleted_Click" />
        </div>

        <!-- GridView -->
        <div class="grid-wrap">
            <asp:GridView
                ID="gvTrips"
                runat="server"
                AutoGenerateColumns="false"
                CssClass="trip-grid"
                GridLines="None"
                EmptyDataText=""
                OnRowCommand="gvTrips_RowCommand">

                <Columns>
                    <asp:BoundField DataField="PickupLocation"   HeaderText="Pickup"   />
                    <asp:BoundField DataField="DropoffLocation"  HeaderText="Drop-off" />
                    <asp:BoundField DataField="DistanceKm"       HeaderText="Dist (km)" DataFormatString="{0:F1}" />
                    <asp:BoundField DataField="PickupTime"       HeaderText="Date / Time" DataFormatString="{0:MMM dd, yyyy HH:mm}" />
                    
                    <asp:TemplateField HeaderText="Price">
                        <ItemTemplate>
                            <span class="trip-price">$<%# Eval("Price", "{0:F2}") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class="badge <%# GetBadgeClass(Eval("Status").ToString()) %>">
                                <%# Eval("Status") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Action">
    <ItemTemplate>
        <asp:LinkButton
            runat="server"
            CommandName="CancelTrip"
            CommandArgument='<%# Eval("TripID") %>'    
            CssClass="btn-cancel"
            Visible='<%# Eval("Status").ToString() == "Pending" %>'
            Text="Cancel"
            OnClientClick="return confirm('Cancel this trip?');" />

        <asp:LinkButton
            runat="server"
            CommandName="LeaveFeedback"
            CommandArgument='<%# Eval("TripID") %>'
            CssClass="btn-feedback"
            Visible='<%# Eval("Status").ToString() == "Completed" %>'
            Text="Leave Feedback" />

        <asp:Label
            runat="server"
            Text="—"
            Visible='<%# Eval("Status").ToString() != "Pending" && Eval("Status").ToString() != "Completed" %>' />
    </ItemTemplate>
</asp:TemplateField>
                </Columns>
            </asp:GridView>

            <!-- Empty state shown from code-behind -->
            <asp:Label ID="lblEmpty" runat="server" Visible="false" CssClass="empty-msg" Text="No trips found." />
        </div>
    </div>

</form>
</body>
</html>
