
var watchID;
var startPos;
var marker;
var infoWindow;
var lastLat, lastLong;
// keep a tally of how many points we've added to the map
var pointsAdded = 0;

function success(position) {

    var ul = document.getElementById("locationList");
    var li = document.createElement("li");
    li.className = "list-group-item";
    var calDist = calculateDistance(startPos.coords.latitude, startPos.coords.longitude,
                    position.coords.latitude, position.coords.longitude);

    var lidist = document.createElement("span");
    lidist.className = "badge";
    lidist.appendChild(document.createTextNode(calDist + "km"));
    li.appendChild(lidist);

    li.appendChild(document.createTextNode("Lat:" + position.coords.latitude + "°, Long:" + position.coords.longitude + "° "));
    ul.appendChild(li);


    var contentString = MakeInfoWindowText(position.timestamp, position.coords.latitude, position.coords.longitude, position.coords.accuracy, position.coords.altitude, position.coords.heading, position.coords.speed);

    // Display Point an Info Window on Map
    AddMapPoint(position.coords.latitude, position.coords.longitude, contentString, true);

    //var marker = new google.maps.Marker({
    //    position: latlng,
    //    map: map,
    //    title: "You are here! (at least within a " + position.coords.accuracy + " meter radius)"
    //});

    // Save User Data
    PostUserData(position);
}

function error(msg) {
    var s = document.getElementById('status');
    s.innerHTML = typeof msg == 'string' ? msg : "failed";
    s.className = 'fail';

    // console.log(arguments);
}

function startLocationTracking() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            startPos = position;
            // Slide in

            $("#btnStart").attr("disabled", "disabled");
            $("#btnStop").removeAttr("disabled");
            $("#btnStop").css("background-color", "#004b37");
            $("#btnSaveRoute").removeAttr("disabled");
        });
        // Update every 3 seconds
        var options = { frequency: 3000, maximumAge: 3000, timeout: 5000, enableHighAccuracy: true };
        watchID = navigator.geolocation.watchPosition(success, error, options);
    } else {
        error('not supported');
    }

    //checkConnection();
}

function stopLocationTracking() {

    $("#btnStop").attr("disabled", "disabled");
    $("#btnStart").removeAttr("disabled");
    $("#btnStart").css("background-color", "#004b37");

    if (watchID > 0) {
        navigator.geolocation.clearWatch(watchID);
    }
}

function calculateDistance(lat1, lon1, lat2, lon2) {
    var R = 6371; // km
    var dLat = (lat2 - lat1).toRad();
    var dLon = (lon2 - lon1).toRad();
    var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
            Math.cos(lat1.toRad()) * Math.cos(lat2.toRad()) *
            Math.sin(dLon / 2) * Math.sin(dLon / 2);
    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    var d = R * c;
    return d;
}

Number.prototype.toRad = function () {
    return this * Math.PI / 180;
}

function parseTimestamp(timestamp) {
    var d = new Date(timestamp);
    var day = d.getDate();
    var month = d.getMonth() + 1;
    var year = d.getFullYear();
    var hour = d.getHours();
    var mins = d.getMinutes();
    var secs = d.getSeconds();
    var msec = d.getMilliseconds();

    return day + "." + month + "." + year + " " + hour + ":" + mins + ":" + secs + "," + msec;
}

function formatTimestamp(timestamp) {
    var d = new Date(timestamp);
    return d.format("yyyy-MM-dd h:mm:ss");
}

Date.prototype.format = function (format) {
    var o = {
        "M+": this.getMonth() + 1, //month
        "d+": this.getDate(),    //day
        "h+": this.getHours(),   //hour
        "m+": this.getMinutes(), //minute
        "s+": this.getSeconds(), //second
        "q+": Math.floor((this.getMonth() + 3) / 3),  //quarter
        "S": this.getMilliseconds() //millisecond
    }

    if (/(y+)/.test(format)) format = format.replace(RegExp.$1,
      (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o) if (new RegExp("(" + k + ")").test(format))
        format = format.replace(RegExp.$1,
          RegExp.$1.length == 1 ? o[k] :
            ("00" + o[k]).substr(("" + o[k]).length));
    return format;
}

function toggleBounce() {
    if (marker.getAnimation() != null) {
        marker.setAnimation(null);
    } else {
        marker.setAnimation(google.maps.Animation.BOUNCE);
    }
}

// wormhole.js needed
function checkConnection() {
    var networkState = navigator.network.connection.type;

    var states = {};
    states[Connection.UNKNOWN] = 'Unknown connection';
    states[Connection.ETHERNET] = 'Ethernet connection';
    states[Connection.WIFI] = 'WiFi connection';
    states[Connection.CELL_2G] = 'Cell 2G connection';
    states[Connection.CELL_3G] = 'Cell 3G connection';
    states[Connection.CELL_4G] = 'Cell 4G connection';
    states[Connection.NONE] = 'No network connection';

    var networkStateIndicator = document.createElement("span");
    networkStateIndicator.className = "badge";
    networkStateIndicator.appendChild(document.createTextNode(states[networkState] + "km"));

    $("connectionType").append(networkStateIndicator);
}

function PostUserData(position) {

    var data = {
        Long: position.coords.longitude,
        Lat: position.coords.latitude,
        Accuracy: position.coords.accuracy,
        Alt: position.coords.altitude,
        Heading: position.coords.heading,
        Speed: position.coords.speed,
        Timestamp: position.timestamp,
        CreatedDate: formatTimestamp(position.timestamp)
    };
    var userCoord = JSON.stringify(data);

    $.ajax({
        type: "POST",
        url: "SaveUserCoord",
        data: userCoord,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (returnPayload) {
            console && console.log("request succeeded");
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console && console.log("request failed");
        },

        processData: false,
        async: false,
        cache: false,
        traditional: true
    });
}

function saveRoute() {
    $('#modalSaveRoute').modal('toggle')
}

// var arrJsonObject = JSON.parse(mapItems);
//var mapItems='[
//{
//    "Latitude": "22.575897",
//    "Lonngitude": "88.431754",
//    "CustomerCode": "*$$$*",
//    "CustomerName": "*@@@*",
//    "markedDate": "2/20/2014 12:04:41 PM"
//},
//{
//    "Latitude": "22.615067",
//    "Lonngitude": "88.431759",
//    "CustomerCode": "*$$$*",
//    "CustomerName": "*@@@*",
//    "markedDate": "2/20/2014 3:02:19 PM"
//}
//]';

function GetRoutePoints(routeId) {
    var dataToBeSend = {
        id: routeId
    };

    $.ajax({
        type: "POST",
        url: "GetRoutePoints",
        data: JSON.stringify(dataToBeSend),
        dataType: "json",
        contentType: "application/json",
        success: function (returnPayload) {
            //var arrJsonObject = JSON.parse(returnPayload);
            alert(returnPayload);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console && console.log("request failed");
            alert(thrownError);
        }
    });
}

function ShowMapPoints(arrJsonObject) {

    for (var y = 1; y <= arrJsonObject.length; y++) {

        var infoWindowTest = MakeInfoWindowText(arrJsonObject[y - 1].Timestamp, arrJsonObject[y - 1].Latitude, arrJsonObject[y - 1].Lonngitude,
            arrJsonObject[y - 1].Accuracy, arrJsonObject[y - 1].Altitude, arrJsonObject[y - 1].Heading, arrJsonObject[y - 1].Speed);

        AddMapPoint(arrJsonObject[y - 1].Latitude, arrJsonObject[y - 1].Lonngitude, infoWindowTest, true);
    }
}

function AddMapPoint(latitude, longitude, infoWindowText, showWeather) {

    if ((typeof (lastLat) == "undefined" || lastLat != latitude)
        && (typeof (lastLong) == "undefined" || lastLong != longitude)) {

        var mapcanvas = document.createElement('div');
        mapcanvas.id = 'mapcanvas';
        mapcanvas.style.height = '600px';
        mapcanvas.style.width = '700px';

        document.getElementById("map").appendChild(mapcanvas);

        var latlng = new google.maps.LatLng(latitude, longitude);

        var myOptions = {
            zoom: 13,
            center: latlng,
            mapTypeControl: false,
            navigationControlOptions: { style: google.maps.NavigationControlStyle.SMALL },
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            timeout: 60000
        };
        var map = new google.maps.Map(document.getElementById("mapcanvas"), myOptions);

        if (showWeather) {
            var weatherLayer = new google.maps.weather.WeatherLayer({
                temperatureUnits: google.maps.weather.TemperatureUnit.CELSIUS,
                windSpeedUnit: google.maps.weather.WindSpeedUnit.KILOMETERS_PER_HOUR
            });
            weatherLayer.setMap(map);
        }
        // Remove the current marker, if there is one
        //if (typeof (marker) != "undefined") marker.setMap(null);
        marker = new google.maps.Marker({
            position: latlng,
            map: map,
            animation: google.maps.Animation.DROP,
            title: "User location"
        });

        // Remove the current infoWindow, if there is one
        //if (typeof (infoWindow) != "undefined") infoWindow.setMap(null);
        infowindow = new google.maps.InfoWindow({
            content: infoWindowText
        });

        google.maps.event.addListener(marker, 'click', function () {
            infowindow.open(map, marker);
        });

    } // New Position
}

function MakeInfoWindowText(timestamp, latitude, longitude, accuracy, altitude, heading, speed) {
    return "<b>Timestamp:</b> " + parseTimestamp(timestamp)
        + "<br/><b>User location:</b> lat " + latitude + ", long " + longitude
        + "<br><b>Accuracy</b>:" + accuracy + " meters"
        + "<br><b>Altitude</b>:" + altitude + " meters"
        + "<br><b>Heading</b>:" + heading + " °"
        + "<br><b>Speed</b>:" + speed + " m/sec";
}
