"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("https://whw.cjee.tw/ChatHub").build();
//var connection = new signalR.HubConnectionBuilder().withUrl("/ChatHub").build();
//Disable send button until connection is established
//document.getElementById("sendButton").disabled = true;

connection.on("JsonGet", (json) => {
    //const encodedMsg = user + " says " + message;
    //const li = document.createElement("li");
    //li.textContent = encodedMsg;
    //document.getElementById("jjj").value = "data" + json;
    document.getElementById("test").innerHTML = json;
});

connection.start().then(function () {
    document.getElementById("jjj").value = "startconnectV#";
    
    //document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    document.getElementById("test").innerHTML = err;


});

//document.getElementById("sendButton").addEventListener("click", function (event) {
//    var user = document.getElementById("userInput").value;
//    var message = document.getElementById("messageInput").value;
//    connection.invoke("SendMessage", user, message).catch(function (err) {
//        return console.error(err.toString());
//    });
//    event.preventDefault();
//});

function testsub()
{
    document.getElementById("jjj").value = "WWWW";
}

