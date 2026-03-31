const token = localStorage.getItem("token");
const API = "http://localhost:5287";

// 🔐 Redirect if not logged in
if (!token) {
    window.location = "login.html";
}

// ================= LOAD STATS =================
async function loadStats() {
    try {
        const res = await fetch(`${API}/api/dashboard/stats`, {
            headers: { "Authorization": "Bearer " + token }
        });

        if (!res.ok) throw new Error("Stats failed");

        const data = await res.json();

        console.log("DATA:", data); // ✅ DEBUG

        if (!data) {
            console.error("No data received");
            return;
        }

        createCharts(data);

    } catch (err) {
        console.error("Stats error:", err);
    }
}

// ================= CREATE CHARTS =================
function createCharts(data) {

    const c = data.totalCustomers || 0;
    const ct = data.totalContacts || 0;
    const n = data.totalNotes || 0;

    // ❗ Destroy old charts (prevents overlap)
    document.getElementById("pieChart").innerHTML = "";
    document.getElementById("barChart").innerHTML = "";
    document.getElementById("lineChart").innerHTML = "";
    document.getElementById("doughnutChart").innerHTML = "";

    // PIE
    new Chart(document.getElementById("pieChart"), {
        type: "pie",
        data: {
            labels: ["Customers", "Contacts", "Notes"],
            datasets: [{
                data: [c, ct, n],
                backgroundColor: ["#6366f1", "#10b981", "#f59e0b"]
            }]
        }
    });

    // BAR
    new Chart(document.getElementById("barChart"), {
        type: "bar",
        data: {
            labels: ["Customers", "Contacts", "Notes"],
            datasets: [{
                label: "Count",
                data: [c, ct, n],
                backgroundColor: ["#6366f1", "#10b981", "#f59e0b"]
            }]
        }
    });

    // LINE
    new Chart(document.getElementById("lineChart"), {
        type: "line",
        data: {
            labels: ["Customers", "Contacts", "Notes"],
            datasets: [{
                label: "Trend",
                data: [c, ct, n],
                borderColor: "#6366f1",
                fill: true
            }]
        }
    });

    // DOUGHNUT
    new Chart(document.getElementById("doughnutChart"), {
        type: "doughnut",
        data: {
            labels: ["Customers", "Contacts", "Notes"],
            datasets: [{
                data: [c, ct, n],
                backgroundColor: ["#6366f1", "#10b981", "#f59e0b"]
            }]
        }
    });
}

// ================= NOTIFICATIONS =================
async function loadNotifications() {
    try {
        const res = await fetch(`${API}/api/notifications`, {
            headers: { "Authorization": "Bearer " + token }
        });

        if (!res.ok) throw new Error("Notification failed");

        const data = await res.json();

        console.log("Notifications:", data); // ✅ DEBUG

        let html = "";

        if (!data || data.length === 0) {
            html = `<div>No notifications</div>`;
        } else {
            data.forEach(n => {
                html += `<div style="padding:8px;border-bottom:1px solid #eee;">${n.message}</div>`;
            });
        }

        document.getElementById("notifDropdown").innerHTML = html;
        document.getElementById("notifCount").innerText = data.length || 0;

    } catch (err) {
        console.error("Notification error:", err);
    }
}

// ================= TOGGLE =================
function toggleNotifications() {
    const box = document.getElementById("notifDropdown");
    box.style.display = box.style.display === "block" ? "none" : "block";
}

// ================= INIT =================
loadStats();
loadNotifications();
setInterval(loadNotifications, 5000);
