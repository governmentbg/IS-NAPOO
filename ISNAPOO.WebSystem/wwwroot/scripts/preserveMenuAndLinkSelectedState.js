$(document).ready(function () {
    let liMenuElements = document.getElementsByClassName("pcoded-hasmenu");

    if (localStorage.getItem("menu-id") !== null) {
        let elId = localStorage.getItem("menu-id");
        let el = document.getElementById(elId);
        el.classList.add("pcoded-trigger");
    }

    if (liMenuElements.length > 0) {
        for (let i = 0; i < liMenuElements.length; i++) {
            liMenuElements[i].addEventListener("click", function () {
                let elIdValue = liMenuElements[i].id;

                localStorage.setItem("menu-id", elIdValue);
            });
        }
    }
});

$(document).ready(function () {
    
    $('li.active').removeClass('active');
    $('a[href="' + location.pathname + location.search + '"]').parent('li').addClass('active');
});

