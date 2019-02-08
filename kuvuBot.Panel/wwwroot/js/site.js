// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

$("#sidebar-toggle").click(function (e) {
    e.preventDefault();
    $(".main").toggleClass("toggled");
});
$(window).resize(() => {
    let w = $(this);
    let main = $(".main");
    if (w.width() <= 980) {
        if (main.hasClass('toggled')) {
            main.removeClass('toggled');
            main.addClass('toggled-off');
        }
    } else if (w.width() > 980) {
        if (main.hasClass('toggled-off')) {
            main.removeClass('toggled-off');
            main.addClass('toggled');
        }
    }
});
