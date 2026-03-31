const token = localStorage.getItem("token");
const API = "http://localhost:5287";

// ✅ NEW: track edit mode
let editCustomerId = null;

// ================= LOAD =================
async function loadCustomers() {
    try {
        const res = await fetch(`${API}/api/customers`, {
            headers: {
                "Authorization": "Bearer " + token
            }
        });

        const data = await res.json();
        renderCustomers(data);

    } catch (err) {
        console.error(err);
    }
}

// ================= RENDER =================
function renderCustomers(customers) {

    const table = document.getElementById("customerTable");
    if (!table) {
        console.error("customerTable not found");
        return;
    }

    let rows = "";

    if (!customers || customers.length === 0) {
        rows = `<tr><td colspan="5">No customers found</td></tr>`;
    } else {
        customers.forEach(c => {
            rows += `
<tr>
<td>${c.id}</td>
<td>${c.name}</td>
<td>${c.email}</td>
<td>${c.company || "-"}</td>
<td>
<button onclick='editCustomer(${JSON.stringify(c)})'>Edit</button>
<button onclick="deleteCustomer(${c.id})">Delete</button>
</td>
</tr>`;
        });
    }

    table.innerHTML = rows;
}

// ================= EDIT =================
function editCustomer(c) {
    document.getElementById("name").value = c.name;
    document.getElementById("email").value = c.email;
    document.getElementById("phone").value = c.phone;
    document.getElementById("company").value = c.company;
    document.getElementById("address").value = c.address;
    document.getElementById("status").value = c.status;

    editCustomerId = c.id;

    // change button text
    document.querySelector("button").innerText = "Update Customer";
}

// ================= ADD / UPDATE =================
async function addCustomer() {

    const name = document.getElementById("name").value;
    const email = document.getElementById("email").value;
    const phone = document.getElementById("phone").value;
    const company = document.getElementById("company").value;
    const address = document.getElementById("address").value;
    const status = document.getElementById("status").value;

    const messageBox = document.getElementById("formMessage");

    messageBox.style.display = "block";
    messageBox.style.color = "red";

    if (!name || !email) {
        messageBox.innerText = "Fill required fields";
        return;
    }

    try {

        // ✅ decide ADD or UPDATE
        const method = editCustomerId ? "PUT" : "POST";
        const url = editCustomerId
            ? `${API}/api/customers/${editCustomerId}`
            : `${API}/api/customers`;

        const res = await fetch(url, {
            method: method,
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + token
            },
            body: JSON.stringify({
                name,
                email,
                phone,
                company,
                address,
                status
            })
        });

        const text = await res.text();

        if (!res.ok) {
            messageBox.innerText = text;
            return;
        }

        messageBox.style.color = "green";
        messageBox.innerText = editCustomerId
            ? "Customer updated"
            : "Customer added";

        // clear
        document.getElementById("name").value = "";
        document.getElementById("email").value = "";
        document.getElementById("phone").value = "";
        document.getElementById("company").value = "";
        document.getElementById("address").value = "";
        document.getElementById("status").value = "";

        // ✅ reset edit mode
        editCustomerId = null;
        document.querySelector("button").innerText = "Add Customer";

        loadCustomers();

    } catch (err) {
        console.error(err);
        messageBox.innerText = "Server error";
    }
}

// ================= DELETE =================
async function deleteCustomer(id) {
    await fetch(`${API}/api/customers/${id}`, {
        method: "DELETE",
        headers: {
            "Authorization": "Bearer " + token
        }
    });

    loadCustomers();
}

// ================= INIT =================
if (!token) {
    alert("Login first");
    window.location = "login.html";
} else {
    loadCustomers();
}