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
    if ($('.status-detail').length > 0) {
        $.get('/render/statusdetail/' + id, {}, function (data) {
            $('.status-detail').html(data);
        });
    }
}

hub.client.onStatusCasesChanged = function (id) {
    if ($('.test-cases').length > 0) {
        $.get('/render/statuscases/' + id, {}, function (data) {
            $('.test-cases').html($(data));
        });
    }
}

hub.client.onStatusOutputed = function (os, text) {
    if (os == CurrentOS)
    {
        $('pre').append(text);
        $('.pre-outer').scrollTop($('pre').height());
    }
}

hub.client.onCIResultChanged = function (id) {
    if ($('[data-id="ci-' + id + '"]').length > 0) {
        $.get('/render/ci/' + id, {}, function (data) {
            $('[data-id="ci-' + id + '"]').html(data.html());
        });
    }
}

hub.client.onThreadChanged = function (id) {
    $.get('/render/thread/' + id, {}, function (data) {
        if ($('[data-id="thread-' + id + '"]').length == 0)
            $('.lst-threads').prepend(data);
        else
            $('[data-id="thread-' + id + '"]').html($(data).html());
    });
}

hub.client.onThreadEdited = function (id) {
    $.get('/render/threadcontent/' + id, {}, function (data) {
        $('.thread-content').html(data);
    });
}

hub.client.onPostRemoved = function (id) {
    if ($('[data-id="' + id + '"]').length > 0)
        $('[data-id="' + id + '"]').remove();
}

hub.client.onPostChanged = function (id) {
    $.get('/render/post/' + id, {}, function (data) {
        if ($('[data-id="' + id + '"]').length == 0)
            $('.lst-posts').append(data);
        else
            $('[data-id="' + id + '"]').html($(data).html());
    });
}

$.connection.hub.start(null, function () {
    if ($('.lst-statuses').length > 0) {
        hub.server.joinGroup("StatusList");
    }
    if ($('.status-detail').length > 0 || $('.project-building').length > 0) {
        hub.server.joinGroup("Status" + id);
    }
    if ($('.lst-ci').length > 0) {
        hub.server.joinGroup("CI");
    }
    if ($('.thread-announcements').length > 0) {
        hub.server.joinGroup('Forum-' + id);
    }
    if ($('.thread-content').length > 0) {
        hub.server.joinGroup('Thread-' + id);
    }
});