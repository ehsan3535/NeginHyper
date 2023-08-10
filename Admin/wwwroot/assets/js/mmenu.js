
if($('.nav__block').length) {
    // desktop -- submenu stays open
    $(".section__mega_a[data-index]").hover(function (e) {
        $(".section__mega_a[data-index]").removeClass("active-hover");
        $(this).addClass("active-hover");
        $(".section__mega_sub--container[data-index-div]").removeClass(
            "active"
        );
        $('[data-index-div="'.concat($(this).data("index"), '"]')).addClass(
            "active"
        );
    });
    // desktop -- change img src based on data-img
    var cacheSrc;
    $('.section__mega_img--container .lds-ring').css({'display':'none'})
    $(".a--sub[data-img]").hover(
    // function () {
    //     cacheSrc = $(".section__mega_img").attr("src");
    //     $('loadinggg').css({'display':'block'})
    //     $(".section__mega_img").on('load', function() {
    //         $('loadinggg').css({'display':'none'})
    //       $(this).attr("src", $(this).data("img"));  
    //     })
    // },
    function () {
        cacheSrc = $(".section__mega_img").attr("src");
        $(".section__mega_img").attr("src", $(this).data("img"));
        $('.section__mega_img--container .lds-ring').css({'display':'block'})
        $(".section__mega_img").on('load', function() {
            $('.section__mega_img--container .lds-ring').css({'display':'none'})
        })

    },
    function () {
        $(".section__mega_img").attr("src", '');
        $('.section__mega_img--container .lds-ring').css({'display':'none'})
    }
    );
    // desktop -- mega menu styles
    $(".div_mega--container").hover(
        function () {
            $("header").css({ boxShadow: "none" });
            $(".section__mega_a:first-of-type").addClass("active-hover");
            var defaultCategory = $(".section__mega_a:first-of-type").data('index')
            $(`.section__mega_sub--container[data-index-div="${defaultCategory}"]`).addClass("active");
        },
        function () {
            $(".section__mega_a").removeClass("active-hover");
            $(".section__mega_sub--container").removeClass("active");
            $("header").css({
                boxShadow: "0 7px 8px 0 rgb(0 0 0 / 4%)",
            });
        }
    );
    // mobile -- menu
     document.addEventListener("DOMContentLoaded", () => {
         var mobileMenu = new Mmenu(
             "#mega-menu",
             {
                 extensions: [
                     "pagedim-black",
                     "position-right",
                     "border-full",
                     "fx-menu-slide",
                     // "fullscreen"
                 ],
             },
             {
                 language: "fa",
             }
         );
         // reset to main menu
         var api = mobileMenu.API;
         $(".menu-icon_img").on("click", function () {
             if ($('.mm-menu[aria-hidden="true"]').hasClass("mm-menu_opened")) {
                 api.open();
             }
             else {
                 api.close();
                 setTimeout(function () {
                     api.closeAllPanels();
                 }, 300);
             }
         });
     });
    // margin based on banner height
    $('#mega-menu').css({ marginTop: parseInt($('.fixed-top').height()) + 'px'});
    // search box
    $('.searchbox__input').on('focus', function () {
        $(this).addClass('focused')
        $('.nav__search-result').css({ width: ($(this).parents('.nav__searchbox').width() - '14' + 'px'), overflowY: 'auto' }).fadeIn('fast');
    })
    $('.mc_desktop_header .searchbox__input').on('blur', function () {
        $(this).removeClass('focused')
        $('.nav__search-result').fadeOut('fast');
    })
    $('.fuse__input').on('keyup paste', function (e) {
        // START fuse search box
        $.ajax({
            url: 'site/search',
            async: true,
            dataType: "json",
            phrase: $('.fuse__input').val(),
            success: function (data) {
                var list = data;
                if ($(window).outerWidth() > 767) {
                    var searchEle = document.querySelector(".searchbox__input")
                } else {
                    var searchEle = document.querySelector(".searchbox__modal_input")
                }
                var homeUrl = $('head base').attr('href')
                function doSearch() {
                    var resultJSON = fuse.search(searchEle.value.trim());
                    // result.innerHTML = resultJSON.map(item =>item.item.name), null, 3;
                    $(".search__results").html('')
                    if (searchEle.value.trim().length > 2) {
                        if (resultJSON == false) {
                            $(".search__results").append(
                                "<div class='search__result'>نتیجه‌ای برای جستجو یافت نشد.</div>"
                            );
                        } else {
                            resultJSON.forEach((item) => {
                                $(".search__results").append(
                                    "<a class='search__result' href='" + item.item.link + "'><img class='search__result_icon' src='" + homeUrl + "/images/template/search-icon-green.svg' alt='لینک'/><img class='search__result_product' alt='' src='" + item.item.image + "'/><p class='ellipsis__line search__result_name'>" + item.item.name + "</p></a>"
                                );
                            })
                        }
                    }
                }
                var options = {
                    shouldSort: false,
                    matchAllTokens: true,
                    findAllMatches: true,
                    threshold: 0.1,
                    location: 0,
                    distance: 100,
                    maxPatternLength: 32,
                    minMatchCharLength: 3,
                    keys: ["name"]
                };
                var fuse = new Fuse(list, options);

                searchEle.addEventListener("input", doSearch);

                doSearch();
            }
        });
    })

    // //search box mobile
    $('#searchbox__modal--mobile').on('shown.bs.modal', function (e) {
        $('.searchbox__modal_input').focus()
    })
}