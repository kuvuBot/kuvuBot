﻿$(".language-select").on('change', function () {
    document.cookie = "lang=" + this.value; 
    location.reload(); 
});