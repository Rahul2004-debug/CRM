const token = localStorage.getItem("token");
const API = "http://localhost:5287";

// ================= INIT =================
document.addEventListener("DOMContentLoaded", () => {
    console.log("✅ Contacts page loaded");
    if (!token) {
        alert("Please login first");
        window.location = "login.html";
        return;
    }
    loadContacts();
});

// ================= LOAD =================
async function loadContacts() {
    try {
        const res = await fetch(`${API}/api/contacts`, {
            headers: {
                "Authorization": "Bearer " + token
            }
        });

        if (!res.ok) {
            const text = await res.text();
            alert(text);
            return;
        }

        const data = await res.json();
        renderContacts(data);

    } catch (err) {
        console.error("Load error:", err);
    }
}

// ================= RENDER =================
function renderContacts(contacts) {

    const table = document.getElementById("contactTable");

    if (!table) {
        console.error("❌ contactTable not found");
        return;
    }

    let rows = "";

    if (!contacts || contacts.length === 0) {
        rows = `<tr><td colspan="7">No contacts found</td></tr>`;
    } else {
        contacts.forEach(c => {
            rows += `
            <tr>
                <td>${c.id}</td>
                <td>${c.customerId}</td>
                <td>${c.name}</td>
                <td>${c.email}</td>
                <td>${c.phone}</td>
                <td>${c.position || "-"}</td>
                <td>
                    <button onclick="deleteContact(${c.id})">Delete</button>
                </td>
            </tr>`;
        });
    }

    table.innerHTML = rows;
}

// ================= ADD =================
async function addContact() {

    // ✅ SAFE FETCH (NO NULL ERROR EVER)
    const customerInput = document.getElementById("customerId");
    const nameInput = document.getElementById("name");
    const positionInput = document.getElementById("position");
    const emailInput = document.getElementById("email");
    const phoneInput = document.getElementById("phone");
    const messageBox = document.getElementById("formMessage");

    // 🔥 HARD PROTECTION
    if (!customerInput) {
        alert("❌ Customer ID input not found (wrong HTML loaded)");
        return;
    }

    const customerId = customerInput.value.trim();
    const name = nameInput.value.trim();
    const position = positionInput.value.trim() || "N/A";
    const email = emailInput.value.trim();
    const phone = phoneInput.value.trim();

    messageBox.style.display = "block";
    messageBox.style.color = "red";
    messageBox.innerText = "";

    if (!customerId || isNaN(customerId)) {
        messageBox.innerText = "❌ Enter valid Customer ID";
        return;
    }

    if (!name || !email) {
        messageBox.innerText = "⚠ Please fill required fields";
        return;
    }

    try {
        const res = await fetch(`${API}/api/contacts`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + token
            },
            body: JSON.stringify({
                customerId: parseInt(customerId),
                name,
                position,
                email,
                phone
            })
        });

        const text = await res.text();

        if (!res.ok) {
            messageBox.innerText = text;
            return;
        }

        messageBox.style.color = "green";
        messageBox.innerText = "✅ Contact added successfully";

        // CLEAR FORM
        customerInput.value = "";
        nameInput.value = "";
        positionInput.value = "";
        emailInput.value = "";
        phoneInput.value = "";

        loadContacts();

    } catch (err) {
        console.error(err);
        messageBox.innerText = "❌ Server error";
    }
}

// ================= DELETE =================
async function deleteContact(id) {
    await fetch(`${API}/api/contacts/${id}`, {
        method: "DELETE",
        headers: {
            "Authorization": "Bearer " + token
        }
    });

    loadContacts();
}