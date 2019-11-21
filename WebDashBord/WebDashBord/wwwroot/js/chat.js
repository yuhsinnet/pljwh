"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("https://whw.cjee.tw/ChatHub").build();

//var jsonArray;

connection.on("JsonGet", (json) => {
    //jsonArray = json;
    
    //document.getElementById("test").innerHTML = jsonArray;
    //appendTable(jsonArray);


    var obj = JSON.parse(json);

    //document.getElementById("test").innerHTML = obj[0].Value;



/// test W3Scool
var txt="";
txt += "<table>"

txt += "<tr>"+
            "<th>區域</th>"+
            "<th>感測器類型</th>"+
            "<th>讀數</th>"+
            //"<th>unit</th>"+
            "</tr>";


for (var i = 0; i < obj.length; i++  ) {
  txt += "<tr>"+ 

            "<td>" + obj[i].AreaName + "</td>"+
            "<td>" + obj[i].Name + "</td>"+
            "<td>" + obj[i].Value + obj[i].unit + "</td>" +
            //"<td>" + obj[i].unit + "</td>"+  
            //"<td>test</tb>"
         "</tr>";
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
    document.getElementById("demo").innerHTML = err;


});










function testsub()
{
    document.getElementById("jjj").value = "WWWW";
}

