"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("https://whw.cjee.tw/ChatHub").build();


connection.on("JsonGet", (json) => {

    document.getElementById("test").innerHTML = json;
});

connection.start().then(function () {
    document.getElementById("jjj").value = "startconnectV#";
    
    //document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    document.getElementById("test").innerHTML = err;


});

function testsub()
{
    document.getElementById("jjj").value = "WWWW";
}

