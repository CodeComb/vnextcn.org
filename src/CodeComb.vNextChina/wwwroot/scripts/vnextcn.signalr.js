var hub = $.connection.vNextChinaHub;
hub.client.onStatusChanged = function (id) {
    $.get('/render/status/' + id, {}, function (data) {
        if ($('[data-id="status-' + id + '"]').length == 0)
            $('.lst-statuses').prepend(data);
        else
            $('[data-id="status-' + id + '"]').html($(data).html());
    });
};

hub.client.onStatusDetailChanged = function (id) {
    $.get('/render/statusdetail/' + id, {}, function (data) {
        $('.status-detail').html(data);
    });
}

hub.client.onStatusCasesChanged = function (id) {
    $.get('/render/statuscases/' + id, {}, function (data) {
        $('.test-cases').html($(data));
    });
}

hub.client.onStatusOutputed = function (os, text) {
    if (os == CurrentOS)
    {
        $('pre').append(text);
        $('.pre-outer').scrollTop($('pre').height());
    }
}

$.connection.hub.start(null, function () {
    if ($('.lst-statuses').length > 0) {
        hub.server.joinGroup("StatusList");
    }
    if ($('.status-detail').length > 0) {
        hub.server.joinGroup("Status" + id);
    }
});