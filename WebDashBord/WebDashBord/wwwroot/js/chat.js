//"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("https://whw.cjee.tw/ChatHub").build();

//var jsonArray;

connection.on("JsonGet", (json) => {
    //jsonArray = json;
    
    //document.getElementById("test").innerHTML = jsonArray;
    //appendTable(jsonArray);


    var obj = JSON.parse(json);

    //document.getElementById("test").innerHTML = obj[0].Value;



/// test W3Scool
var x, txt = "";
txt += "<table border='1'>"

txt += "<tr>"+
            "<th>AreNsme</th>"+
            "<th>SensorName</th>"+
            "<th>Value</th>"+
            "<th>unit</th>";


for (x in obj) {
  txt += "<tr>"+ 

            "<td>" + obj[x].AreaName + "</td>"+
            "<td>" + obj[x].Name + "</td>"+
            "<td>" + obj[x].Value + "</td>"+
            "<td>" + obj[x].unit + "</td>"+  
  
        + "</tr>";
}
txt += "</table>"    
document.getElementById("test").innerHTML = txt;




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

