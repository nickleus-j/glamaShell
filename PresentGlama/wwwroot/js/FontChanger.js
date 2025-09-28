var FontFacer= {
    FontFaces: ["Tahoma",
        "Arial, sans-serif",
        "Verdana, sans-serif",
        "Georgia, serif",
        "Times New Roman, serif",
        "Courier New, monospace",
        "Lucida Console, monospace",
        "Impact, sans-serif", "cursive"
        ,"Roboto, sans-serif"
    ],
    CreateFontChangeEvent: function(selectorOfFonts,elementWithTexts) {
        let fontSelector = document.querySelector(selectorOfFonts), contentToChange = document.querySelector(elementWithTexts);
        this.FontFaces.forEach(font => {
            let option = document.createElement('option');
            option.value = font;
            option.textContent = font.split(',')[0].trim(); // Display only the primary font name
            fontSelector.appendChild(option);
        });
        fontSelector.addEventListener('change', (event) => {
            const selectedFont = event.target.value;
            contentToChange.style.fontFamily = selectedFont;
        });
    }
}