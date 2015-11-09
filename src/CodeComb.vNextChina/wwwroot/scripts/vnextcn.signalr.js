var hub = $.connection.vNextChinaHub;
hub.client.onStatusChanged = function (id) {
    $.get('/render/status/' + id, {}, function (data) {
        if ($('[data-id="status-' + id + '"]').length == 0)
            $('.lst-statuses').append(data);
        else
            $('[data-id="status-' + id + '"]').html($(data).html());
    });
};

$.connection.hub.start(null, function () {
    if ($('.lst-statuses').length > 0) {
        hub.server.joinGroup("StatusList");
    }
});