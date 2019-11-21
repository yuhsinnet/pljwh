"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("https://whw.cjee.tw/ChatHub").build();


connection.on("JsonGet", (json) => {

    var obj = JSON.parse(json);



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
    document.getElementById("SensorTable").innerHTML = txt;




});


connection.start().then(function () {
    document.getElementById("ConnectStat").innerHTML = "連線成功";
    
    //document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    document.getElementById("ConnectStat").innerHTML = err;


});


