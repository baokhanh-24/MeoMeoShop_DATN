// wwwroot/js/site.js
window.searchPopup = () => {
    const $ = window.jQuery;

    $('#header-nav').on('click', '.search-button', function () {
        $('.search-popup').toggleClass('is-visible');
    });

    $('#header-nav').on('click', '.btn-close-search', function () {
        $('.search-popup').toggleClass('is-visible');
    });

    $('.search-popup-trigger').on('click', function (b) {
        b.preventDefault();
        $('.search-popup').addClass('is-visible');
        setTimeout(function () {
            $('.search-popup').find('#search-popup').focus();
        }, 350);
    });

    $('.search-popup').on('click', function (b) {
        if (
            $(b.target).is('.search-popup-close') ||
            $(b.target).is('.search-popup-close svg') ||
            $(b.target).is('.search-popup-close path') ||
            $(b.target).is('.search-popup')
        ) {
            b.preventDefault();
            $(this).removeClass('is-visible');
        }
    });

    $(document).keyup(function (b) {
        if (b.which === 27) {
            $('.search-popup').removeClass('is-visible');
        }
    });
};

window.initProductQty = () => {
    const $ = window.jQuery;
    $('.product-qty').each(function () {
        var $el_product = $(this);
        $el_product.find('.quantity-right-plus').click(function (e) {
            e.preventDefault();
            var quantity = parseInt($el_product.find('#quantity').val());
            $el_product.find('#quantity').val(quantity + 1);
        });

        $el_product.find('.quantity-left-minus').click(function (e) {
            e.preventDefault();
            var quantity = parseInt($el_product.find('#quantity').val());
            if (quantity > 0) {
                $el_product.find('#quantity').val(quantity - 1);
            }
        });
    });
};

window.initSwipers = () => {
    new Swiper('.main-swiper', {
        speed: 500,
        navigation: {
            nextEl: '.swiper-arrow-prev',
            prevEl: '.swiper-arrow-next',
        },
    });

    new Swiper('.product-swiper', {
        slidesPerView: 4,
        spaceBetween: 10,
        pagination: {
            el: '#mobile-products .swiper-pagination',
            clickable: true,
        },
        breakpoints: {
            0: {
                slidesPerView: 2,
                spaceBetween: 20,
            },
            980: {
                slidesPerView: 4,
                spaceBetween: 20,
            },
        },
    });

    new Swiper('.product-watch-swiper', {
        slidesPerView: 4,
        spaceBetween: 10,
        pagination: {
            el: '#smart-watches .swiper-pagination',
            clickable: true,
        },
        breakpoints: {
            0: {
                slidesPerView: 2,
                spaceBetween: 20,
            },
            980: {
                slidesPerView: 4,
                spaceBetween: 20,
            },
        },
    });

    new Swiper('.testimonial-swiper', {
        loop: true,
        navigation: {
            nextEl: '.swiper-arrow-prev',
            prevEl: '.swiper-arrow-next',
        },
    });
};
