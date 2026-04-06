async function login() {

    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;

    const errorBox = document.getElementById("error");

    // ✅ SAFETY CHECK (THIS FIXES YOUR ERROR)
    if (!errorBox) {
        console.error("Error box not found in HTML");
        return;
    }

    errorBox.style.display = "none";
    errorBox.innerText = "";

    try {
        const response = await fetch("http://localhost:5287/api/auth/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email, password })
        });

        if (!response.ok) {
            errorBox.style.display = "block";
            errorBox.innerText = "❌ Invalid email or password";
            return;
        }

        const data = await response.json();

        localStorage.setItem("token", data.token);
        localStorage.setItem("role", data.role);

        window.location = "customers.html";

    } catch (err) {
        console.error(err);

        errorBox.style.display = "block";
        errorBox.innerText = "⚠ Server error. Check API.";
    }
}


function logout() {
    try {
        localStorage.removeItem("token");
        localStorage.removeItem("role");
    } catch (e) {
        console.warn("Error clearing storage during logout", e);
    }
    // back to login page 
    window.location = "login.html";
}