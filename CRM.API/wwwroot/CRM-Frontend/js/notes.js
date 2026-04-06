async function login() {

    const email = document.getElementById("email").value

    const password = document.getElementById("password").value

    const response = await fetch("https://localhost:5287/api/auth/login", {

        method: "POST",

        headers: {

            "Content-Type": "application/json"

        },

        body: JSON.stringify({

            email: email,

            password: password

        })

    })

    if (!response.ok) {

        alert("Login failed")

        return

    }

    const data = await response.json()

    console.log("Login response", data)

    localStorage.setItem("token", data.token)

    localStorage.setItem("role", data.role)

    window.location = "customers.html"

}

const token = localStorage.getItem("token");
const userRole = localStorage.getItem("role");
const canDelete = userRole !== "SalesRep";
document.addEventListener("DOMContentLoaded", () => {
    //const role = localStorage.getItem("role");
    

    if (!canDelete) { 
        const actionHeader = document.getElementById("actionHeader");
        actionHeader?.remove();
    }


    loadNotes();
});

async function loadNotes() {



    const response = await fetch("/api/notes", {

        headers: {

            "Authorization": "Bearer " + token

        }

    });

    const notes = await response.json();

    let rows = "";

    notes.forEach(n => {

        rows += `
<tr>
<td>${n.id}</td>
<td>${n.customerId}</td>
<td>${n.content}</td>
<td>
    ${
       userRole !== "SalesRep"
                ? `<button onclick="deleteNote(${n.id})">Delete</button>`
                : ``
     }
</button>
</td>
</tr>

`;

    });

    document.getElementById("noteTable").innerHTML = rows;

}

async function addNote() {

    const customerId = document.getElementById("customerId").value;

    const content = document.getElementById("content").value;

    await fetch("/api/notes", {

        method: "POST",

        headers: {

            "Content-Type": "application/json",

            "Authorization": "Bearer " + token

        },

        body: JSON.stringify({

            customerId, content

        })

    });

    loadNotes();

}

async function deleteNote(id) {

    await fetch("/api/notes/" + id, {

        method: "DELETE",

        headers: {

            "Authorization": "Bearer " + token

        }

    });

    loadNotes();

}

loadNotes();
