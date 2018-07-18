(function ($) {

    'use strict';

    var $filters = $('.filter [data-filter]'),
        $boxes = $('.cd-timeline__container [data-color]');

    $filters.on('click', function (e) {
        e.preventDefault();
        var $this = $(this);

        $filters.removeClass('active');
        $this.addClass('active');

        var $filterColor = $this.attr('data-filter');


        var $filterColor2 = $this.attr('data-answered');

        if ($filterColor == 'all') {
            $boxes.removeClass('is-animated')
                .fadeOut().promise().done(function () {
                    $boxes.addClass('is-animated').fadeIn();
                });
        } else {
            $boxes.removeClass('is-animated')
                .fadeOut().promise().done(function () {
                    $boxes.filter('[data-color = "' + $filterColor + '"]')
                        .addClass('is-animated').fadeIn();
                    
                });
        }
    });

})(jQuery);