$(document).ready(function () {
    $('.navbar-menu').click(function () {
        if ($('.navbar-vertical').width() == 0) {
            $('.navbar-vertical').css('width', '210px');
            $('.wrapper').css('margin-left', '210px');
        }
        else {
            $('.navbar-vertical').css('width', '0');
            $('.wrapper').css('margin-left', '0');
        }
    });
    $(window).resize(function () {
        $('.navbar-vertical').attr('style', '');
        $('.wrapper').attr('style', '');
        //if ($(window).width() <= 767) {
        //    $('#mainImage1').css('width', '100vm');
        //    $('#mainImage2').css('width', '100vm');
        //}
        //else {
        //    resizeImages();
        //}
    });
});