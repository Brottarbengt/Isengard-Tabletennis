﻿// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const toggleButton = document.getElementById('menuToggle');
const nav = document.getElementById('mainNav');

toggleButton.addEventListener('click', () => {
    nav.classList.toggle('show');
});