// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const useDarkCheckbox = document.getElementById('useDark');

useDarkCheckbox.addEventListener('change', function () {
    let bodyElem = document.querySelector("body");
    if (this.checked) {
        bodyElem.classList.add("dark");
    } else {
        bodyElem.classList.remove("dark");
    }
});