
$(document).ready(function () {
    var unmodFilter = function () {
        $(".approved").fadeOut();
        $(".rejected").fadeOut();
        $(".unmoderated").fadeIn();
    };
    var appFilter = function () {
        $(".unmoderated").fadeOut();
        $(".rejected").fadeOut();
        $(".approved").fadeIn();
    };
    var rejFilter = function () {
        $(".unmoderated").fadeOut();
        $(".approved").fadeOut();
        $(".rejected").fadeIn();
    };

    $(".unmod.filterButton").click(function () {
        $(".approved").fadeOut();
        $(".rejected").fadeOut();
        $(".unmoderated").fadeIn();
        $(".filterButton").removeClass("active");
        $(this).addClass("active");
        //$.when(unmodFilter()).done(function () {
        //    allHidden();
        //});
    });

    $(".app.filterButton").click(function () {
        $(".unmoderated").fadeOut();
        $(".rejected").fadeOut();
        $(".approved").fadeIn();
        $(".filterButton").removeClass("active");
        $(this).addClass("active");
        //$.when(appFilter()).done(function () {
        //    allHidden();
        //});
    });

    $(".rej.filterButton").click(function () {
        $(".unmoderated").fadeOut();
        $(".approved").fadeOut();
        $(".rejected").fadeIn();
        $(".filterButton").removeClass("active");
        $(this).addClass("active");
        //$.when(rejFilter()).done(function () {
        //    allHidden();
        //});
    });



    $(".all.filterButton").click(function () {
        $(".answered").fadeIn();
        $(".unanswered").fadeIn();
        $(".filterButton").removeClass("active");
        $(this).addClass("active");
        //$(".answered").promise().done(function () {
        //    allHidden();
        //});
    });

    $(".ans.filterButton").click(function () {
        $(".answered").fadeIn();
        $(".unanswered").fadeOut();
        $(".filterButton").removeClass("active");
        $(this).addClass("active");
        //$(".unanswered").promise().done(function () {
        //    allHidden();
        //});
    });

    $(".unans.filterButton").click(function () {
        $(".answered").fadeOut();
        $(".unanswered").fadeIn();
        $(".filterButton").removeClass("active");
        $(this).addClass("active");

        //$(".answered").promise().done(function () {
        //    allHidden();
        //});
    });

});

function allHidden() {
    console.log("Hidden: "+$('.mainparent').children(':hidden').length);
    if ($('.mainparent').children(':hidden').length == 0) {
        $(".noquestions").fadeIn();
        console.log("no items");

    } else {
        $(".noquestions").fadeOut();
        console.log("items");
    }
}