var imageList = [];
var galleryDirectory = "images/gallery/";
var isImage1 = true;
var timer;

function loadImages(extension) {
    for (var i = 1; i <= 5; ++i) {
        imageList[i-1] = extension + "IMG_0" + i + ".jpg";
    }
    clearInterval(timer);
    timer = setInterval(moveNext, 5000);
}

function refreshGallery(image) {
    if (isImage1) {
        $("#mainImage2").attr("src", image);
    }
    else {
        $("#mainImage1").attr("src", image);
    }  
    resizeImages();
}

function moveNext() {
    if (isImage1) {
        var firstItem = imageList.shift();
        imageList.push(firstItem);
        refreshGallery(imageList[2]);
        $("#mainImage1").stop().fadeOut(3000);
        $("#mainImage2").stop().fadeIn(3000);
        isImage1 = false;
    }
    else {
        var firstItem = imageList.shift();
        imageList.push(firstItem);
        refreshGallery(imageList[2]);
        $("#mainImage2").stop().fadeOut(3000);
        $("#mainImage1").stop().fadeIn(3000);
        isImage1 = true;
    } 
}

function resizeImages() {
    $("#mainImage1").css({
        "height": $(".gallery").height() + "px",
        "width": $(".gallery").width() + "px"

    });
    $("#mainImage2").css({
        "height": $(".gallery").height() + "px",
        "width": $(".gallery").width() + "px"
    })
}



