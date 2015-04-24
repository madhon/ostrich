function spacify(str) {
    return str.replace(/_/g, ' ');
}

function descendingDisplay(datapoint) {
    if ((typeof datapoint === 'string') || (typeof datapoint === 'number')) {
        return datapoint;
    } else if (typeof datapoint === 'object') {
        var stats = new Array;

        for (k in datapoint) {
            stats.push(spacify(k) + ': ' + datapoint[k]);
        }

        return stats.join(", ");
    }
}

function loaded() {
    $.getJSON("/stats.json", function (data) {
        for (key in data) {
            $("#contents").append('<table id="' + key + '"><tr><th>' + key + '</th></tr></table>');
            for (datapoint in data[key]) {
                $("#" + key).append('<tr><td width="30%">' + spacify(datapoint) + '</td><td width="70%">' + descendingDisplay(data[key][datapoint]) + '</td></tr>');
            }
        }
    });

    $("#gendate").append(new Date().toString())
}

$(document).ready(function () {
    loaded();
});