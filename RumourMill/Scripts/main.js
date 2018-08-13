$('.reason').change(function () {

    var thisQID = $(this).attr("class");

    var str = thisQID.substring(15, thisQID.length);

    var edited = thisQID.substring(7, 8);

    console.log("Class ID: " + str +". Edited: " +edited);


    if ($(this).val() == "Other" || edited==0) {
        $('.edittextarea' + str).attr('required');

        $('.edittextarea' + str).css("display", "block");
    } else {
        $('.edittextarea' + str).removeAttr('required');

        $('.edittextarea' + str).css("display", "none");
    }
});