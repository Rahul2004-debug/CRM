async function login() {
 
const email=document.getElementById("email").value
const password=document.getElementById("password").value
 
const response=await fetch("https://localhost:5287/api/auth/login",{
 
method:"POST",
 
headers:{
"Content-Type":"application/json"
},
 
body:JSON.stringify({
email:email,
password:password
})
 
})
 
if(!response.ok){
 
alert("Login failed")
return
 
}
 
const data=await response.json()
 
console.log("Login response",data)
 
localStorage.setItem("token",data.token)
localStorage.setItem("role",data.role)
 
window.location="customers.html"
 
}