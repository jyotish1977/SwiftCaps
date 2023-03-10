var JsFunctions = window.JsFunctions || {};

JsFunctions.carousel = {
    scrollInToView: function (id) {
        const elem = document.getElementById(id);
        elem.scrollIntoView();
    }
}