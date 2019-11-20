"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("https://whw.cjee.tw/ChatHub").build();


connection.on("JsonGet", (json) => {

    document.getElementById("test").innerHTML = json;
});


var headArray = [];
    function parseHead(oneRow) {
        debugger  
        for (var i in oneRow) {
            
            headArray[headArray.length] = i;
            console.log(i);

            


        }
    }
    

    function appendTable() {

        parseHead(jsonArray[0]);
        var div = document.getElementById("div1");
        var table = document.createElement("table");
        var thead = document.createElement("tr");


        for (var count = 0; count < headArray.length; count++) {

            var td = document.createElement("th");
            td.innerHTML = headArray[count];
            thead.appendChild(td);
        }
        table.appendChild(thead);




        for (var tableRowNo = 0; tableRowNo < jsonArray.length; tableRowNo++) {

            var tr = document.createElement("tr");
            for (var headCount = 0; headCount < headArray.length; headCount++) {
                var cell = document.createElement("td");
                cell.innerHTML = jsonArray[tableRowNo][headArray[headCount]];
                tr.appendChild(cell);
            }
            table.appendChild(tr);
        }
        div.appendChild(table);
    }
















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

