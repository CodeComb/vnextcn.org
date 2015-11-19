var render = { };
render.Post = function (id, callback) {
    $.get('/Render/Post/' + id, {}, function (data) {
        if ($('[data-id="' + id + '"]').length == 0)
            $('.lst-posts').append(data);
        else
            $('[data-id="' + id + '"]').html($(data).html());
        callback();
    });
};