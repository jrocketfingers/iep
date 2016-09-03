$(function () {
    var slider = document.getElementById("price-slider");

    _.defaults(window, { halfPrice: 10000, maxPrice: 50000, step: 100 });

    noUiSlider.create(slider, {
        start: [0, window.maxPrice],
        step: window.step,
        connect: true,
        range: {
            "min": [0],
            "10%": [500, 500],
            "50%": [window.halfPrice, 1000],
            "max": [window.maxPrice]
        },
        tooltips: [wNumb({ thousand: '.', mark: ',', decimals: 2 }), wNumb({ mark: ',', thousand: '.', decimals: 2 })]
    })

    slider.noUiSlider.on('update', function (values, handle) {
        var $inputs = [$("#lower-price"), $("#higher-price")];
        var $tooltips = [$("#price-slider .noUi-handle-lower .noUi-tooltip"), $("#price-slider .noUi-handle-upper .noUi-tooltip")];

        $inputs[handle].val(values[handle]);

        if(values[handle] < window.halfPrice)
            $tooltips[handle].removeClass("docked-right").addClass("docked-left");
        else
            $tooltips[handle].removeClass("docked-left").addClass("docked-right");
    });
});
