function open_josm(x) { document.getElementById('josm').src='http://127.0.0.1:8111/load_object?objects='+x; }

var map = L.map('map');
var hash = new L.Hash(map);

var i0 = L.divIcon({ 
	className: 'icon icon-level0',
	iconSize: new L.Point(10, 10)
});
var i1 = L.divIcon({ 
	className: 'icon icon-level1',
	iconSize: new L.Point(10, 10)
});
var i2 = L.divIcon({ 
	className: 'icon icon-level2',
	iconSize: new L.Point(10, 10)
});
var i3 = L.divIcon({ 
	className: 'icon icon-level3',
	iconSize: new L.Point(10, 10)
});
var i4 = L.divIcon({ 
	className: 'icon icon-level4',
	iconSize: new L.Point(10, 10)
});
var i5 = L.divIcon({ 
	className: 'icon icon-level5',
	iconSize: new L.Point(10, 10)
});
var i6 = L.divIcon({ 
	className: 'icon icon-level6',
	iconSize: new L.Point(10, 10)
});
var i7 = L.divIcon({ 
	className: 'icon icon-level7',
	iconSize: new L.Point(10, 10)
});
var i8 = L.divIcon({ 
	className: 'icon icon-level8',
	iconSize: new L.Point(10, 10)
});
var i9 = L.divIcon({ 
	className: 'icon icon-level9',
	iconSize: new L.Point(10, 10)
});

var iErr = L.divIcon({ 
	className: 'icon-err',
	iconSize: new L.Point(10, 10)
});
var iWarn = L.divIcon({ 
	className: 'icon-warn',
	iconSize: new L.Point(10, 10)
});

L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
	maxZoom: 18,
	id: 'mapbox.streets'
}).addTo(map);

var markerCluster = new L.MarkerClusterGroup({
  maxClusterRadius: 50,
  disableClusteringAtZoom: 13,
  showCoverageOnHover: false, 
  spiderfyOnMaxZoom: false,
  zoomToBoundsOnClick: false
});

var info = L.control({position: 'bottomright'});
info.onAdd = function (map) {
	var div = L.DomUtil.create('div', 'info');
	div.innerHTML = "<img height=35 width=35 src=../icons/info.png>";
	div.setAttribute("onmouseenter", "showDisclaimer()");
	div.setAttribute("onmouseleave", "hideDisclaimer()");
	div.id = 'info';
	return div;
};
info.addTo(map);

function hideDisclaimer() 
{ 
	var div = document.getElementById("info"); 
	div.innerHTML = "<img height=35 width=35 src=../icons/info.png>"; 
}

markerCluster.on('clusterclick', function(clickdata){
	var arr = clickdata.layer.getAllChildMarkers();
	var result = "";
	for (var i=0; i<arr.length; i++) {
		if (i>0) result += ',';
		result += arr[i].getPopup().getContent().match(/(?:w|n|r|way|node|relation)\d+/)[0];
	}
	clickdata.layer.bindPopup("<a href=\"http://127.0.0.1:8111/load_object?objects="+result+"\" onClick=\"open_josm("+result+");return false;\">Открыть кластер в Josm</a>");
});