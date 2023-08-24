$(function () {
    function f(l) {
        var k = 0; $(l).each(function () {
            k += $(this).outerWidth(true)
        });
        return k;
    }

    function g(n) {
        var o = f($(n).prevAll()), q = f($(n).nextAll()); var l = f($(".content-tabs").children().not(".J_menuTabs")); var k = $(".content-tabs").outerWidth(true) - l; var p = 0; if ($(".page-tabs-content").outerWidth() < k) {
            p = 0;
        }
        else {
            if (q <= (k - $(n).outerWidth(true) - $(n).next().outerWidth(true))) {
                if ((k - $(n).next().outerWidth(true)) > q) {
                    p = o; var m = n; while ((p - $(m).outerWidth()) > ($(".page-tabs-content").outerWidth() - k)) {
                        p -= $(m).prev().outerWidth(); m = $(m).prev()
                    }
                }
            }

            else {
                if (o > (k - $(n).outerWidth(true) - $(n).prev().outerWidth(true))) {
                    p = o - $(n).prev().outerWidth(true)
                }
            }
        }
        $(".page-tabs-content")
            .animate({
                marginLeft: 0 - p + "px"
            },
            "fast");
    }

    function a() {
        var o = Math.abs(parseInt($(".page-tabs-content").css("margin-left"))); var l = f($(".content-tabs").children().not(".J_menuTabs")); var k = $(".content-tabs").outerWidth(true) - l; var p = 0; if ($(".page-tabs-content").width() < k) {
            return false;
        }

        else {
            var m = $(".J_menuTab:first"); var n = 0; while ((n + $(m).outerWidth(true)) <= o) {
                n += $(m).outerWidth(true); m = $(m).next()
            }
            n = 0; if (f($(m).prevAll()) > k) {
                while ((n + $(m).outerWidth(true)) < (k) && m.length > 0) {
                    n += $(m).outerWidth(true); m = $(m).prev()
                }

                p = f($(m).prevAll())
            }
        } $(".page-tabs-content").animate({
            marginLeft: 0 - p + "px"
        }, "fast")
    }

    function b() {
        var o = Math.abs(parseInt($(".page-tabs-content").css("margin-left"))); var l = f($(".content-tabs").children().not(".J_menuTabs")); var k = $(".content-tabs").outerWidth(true) - l; var p = 0; if ($(".page-tabs-content").width() < k) {
            return false
        }

        else {
            var m = $(".J_menuTab:first"); var n = 0; while ((n + $(m).outerWidth(true)) <= o) {
                n += $(m).outerWidth(true); m = $(m).next()
            }

            n = 0; while ((n + $(m).outerWidth(true)) < (k) && m.length > 0) {
                n += $(m).outerWidth(true); m = $(m).next()
            }

            p = f($(m).prevAll()); if (p > 0) {
                $(".page-tabs-content").animate({
                    marginLeft: 0 - p + "px"
                }, "fast")
            }
        }
    }
    $(".J_menuItem").each(function (k) {
        if (!$(this).attr("data-index")) {
            $(this).attr("data-index", k)
        }
    });

    function c() {
        var o = $(this).attr("href"), m = $(this).data("index"), l = $.trim($(this).text()), k = true; if (o == undefined || $.trim(o).length == 0) {
            return false
        } $(".J_menuTab").each(function () {
            if ($(this).data("id") == o) {
                if (!$(this).hasClass("active")) {
                    $(this).addClass("active").siblings(".J_menuTab").removeClass("active"); g(this); $(".J_mainContent .J_iframe").each(function () {
                        if ($(this).data("id") == o) {
                            $(this).show().siblings(".J_iframe").hide(); return false
                        }
                    })
                }

                k = false; return false
            }
        });

        if (k) {
            var p = '<a href="javascript:;" class="active J_menuTab" data-id="' + o + '">' + l + ' <i class="fa fa-times-circle"></i></a>';
            $(".J_menuTab").removeClass("active");
            var n = '<iframe class="J_iframe" name="iframe' + m + '" width="100%" height="100%" src="' + o + '" frameborder="0" data-id="' + o + '" seamless></iframe>';
            $(".J_mainContent").find("iframe.J_iframe").hide().parents(".J_mainContent").append(n); $(".J_menuTabs .page-tabs-content").append(p); g($(".J_menuTab.active"))
        }
        return false
    }
    $(".J_menuItem").on("click", c);
    function h() {
        var m = $(this).parents(".J_menuTab").data("id");
        var l = $(this).parents(".J_menuTab").width();
        if ($(this).parents(".J_menuTab").hasClass("active")) {
            if ($(this).parents(".J_menuTab").next(".J_menuTab").size()) {
                var k = $(this).parents(".J_menuTab").next(".J_menuTab:eq(0)").data("id"); $(this).parents(".J_menuTab").next(".J_menuTab:eq(0)").addClass("active"); $(".J_mainContent .J_iframe").each(function () {
                    if ($(this).data("id") == k) {
                        $(this).show().siblings(".J_iframe").hide(); return false
                    }
                });
                var n = parseInt($(".page-tabs-content").css("margin-left"));
                if (n < 0) {
                    $(".page-tabs-content").animate({
                        marginLeft: (n + l) + "px"
                    }, "fast")
                }
                $(this).parents(".J_menuTab").remove();
                $(".J_mainContent .J_iframe")
                    .each(function () {
                        if ($(this).data("id") == m) {
                            $(this).remove();
                            return false;
                        }
                    });
            }

            if ($(this).parents(".J_menuTab").prev(".J_menuTab").size()) {
                var k = $(this).parents(".J_menuTab").prev(".J_menuTab:last").data("id"); $(this).parents(".J_menuTab").prev(".J_menuTab:last").addClass("active"); $(".J_mainContent .J_iframe").each(function () {
                    if ($(this).data("id") == k) {
                        $(this).show().siblings(".J_iframe").hide(); return false
                    }
                }); $(this).parents(".J_menuTab").remove(); $(".J_mainContent .J_iframe").each(function () {
                    if ($(this).data("id") == m) {
                        $(this).remove(); return false
                    }
                })
            }
        }
        else {
            $(this).parents(".J_menuTab").remove();
            $(".J_mainContent .J_iframe").each(function () {
                if ($(this).data("id") == m) {
                    $(this).remove();
                    return false
                }
            });

            g($(".J_menuTab.active"))
        }

        return false
    }
    $(".J_menuTabs").on("click", ".J_menuTab i", h);
    // closeOtherTabs
    function i() {
        $(".page-tabs-content").children("[data-id]").not(":first").not(".active").each(function () {
            if ($(this).find(">span.fa-lock").length == 0) {
                $('.J_iframe[data-id="' + $(this).data("id") + '"]').remove();
                $(this).remove()
            }
        });
        $(".page-tabs-content").css("margin-left", "0")
    }
    $(".J_tabCloseOther").on("click", i);
    //关闭其他选项卡
    window._closeOtherTabs = i;
    //定位当前选项卡
    function j() {
        g($(".J_menuTab.active"))
    }
    $(".J_tabShowActive").on("click", j);

    //单击选项卡事件
    function e() {
        if (!$(this.parentElement).hasClass("active")) {
            
            var k = $(this.parentElement).attr("data-dbkey");
            $(".J_mainContent .J_iframe").each(function () {
                if ($(this).attr("id") == k) {
                    $(this).show().siblings(".J_iframe").hide();
                    return false
                }
            });
            $(this.parentElement).addClass("active").siblings(".J_menuTab").removeClass("active");
            g(this.parentElement);
            window.console.log("click the tab : " + k);
            setNavListBackground(k);
        }
    }
    $(".J_menuTabs").on("click", "des", e);

    function d() {
        var l = $('.J_iframe[data-id="' + $(this).data("id") + '"]'); var k = l.attr("src")
    } $(".J_menuTabs").on("dblclick", ".J_menuTab", d);
    $(".J_tabLeft").on("click", a);
    $(".J_tabRight").on("click", b);
    //关闭所有选项卡
    window._closeAllTabs = function () {
        $(".page-tabs-content").children("[data-id]").not(":first").each(function () {
            if ($(this).find(">span.fa-lock").length == 0) {
                $('.J_iframe[data-id="' + $(this).data("id") + '"]').remove();
                $(this).remove();
            }
        });
        $(".page-tabs-content").children("[data-id]:first").each(function () {
            $('.J_iframe[data-id="' + $(this).data("id") + '"]').show(); $(this).addClass("active")
        });
        $(".page-tabs-content").css("margin-left", "0")
    }
    $(".J_tabCloseAll").on("click",
        function() {
            $(".page-tabs-content").children("[data-id]").not(":first").each(function() {
                if ($(this).find(">span.fa-lock").length == 0) {
                    $('.J_iframe[data-id="' + $(this).data("id") + '"]').remove();
                    $(this).remove();
                }

            });
            $(".page-tabs-content").children("[data-id]:first").each(function() {
                $('.J_iframe[data-id="' + $(this).data("id") + '"]').show();
                $(this).addClass("active");
            });
            $(".page-tabs-content").css("margin-left", "0")
        });

    window.bindContextMenu();
});

function bindContextMenu() {
    //绑定tab标签右击菜单
    $.contextMenu({
        selector: '.page-tabs-content .J_menuTab.tab-lock',
        callback: function (key, options) {
            if (key == "unlock") {
                toggleLock($(this).find(">span").eq(0));
            } else if (key == "closeOthers") {
                $(this).click();
                window._closeOtherTabs();
            } else if (key == "closeAlls") {
                window._closeAllTabs();
            }
        },
        items: {
            "unlock": { name: "解锁", icon: "fa-unlock", height: 50 },
            "sep1": "---------",
            "closeOthers": { name: "关闭其他选项卡", icon: "fa-close", height: 50 },
            "closeAlls": { name: "关闭所有选项卡", icon: "fa-window-close", height: 50 },
        }
    });

    $.contextMenu({
        selector: '.page-tabs-content .J_menuTab.tab-unlock',
        callback: function (key, options) {
            if (key == "lock") {
                toggleLock($(this).find(">span").eq(0));
            } else if (key == "closeOthers") {
                $(this).click();
                window._closeOtherTabs();
            } else if (key == "closeAlls") {
                window._closeAllTabs();
            }
        },
        items: {
            "lock": { name: "加锁", icon: "fa-lock", height: 50 },
            "closeOthers": { name: "关闭其他选项卡", icon: "fa-close", height: 50 },
            "closeAlls": { name: "关闭所有选项卡", icon: "fa-window-close", height: 50 }
        }
    });

    //绑定导航菜单右击菜单
    $.contextMenu({
        selector: '#systemMenu li a[data-dbkey]',
        callback: function (key, options) {
            if (key == "openInNew") {
                window.console.log("open in new tab");
                window.menu.createNewTab(this);
            } else if (key == "openInUnLock") {
                window.console.log("open in old tab");
                var tag = _getLeftLastActiveTag().next();
                //在此标签当中设置iframe的值
                window.menu.openInFrame(this);
            }
        },
        items: {
            "openInNew": { name: "在新标签页中打开", icon: "fa-book", height: 50 },
            "openInUnLock": { name: "在未锁定标签中打开", icon: "fa-book", height: 50 },
        }
    });

    $("#systemMenu li a[key]").on("click", function (e) {
        e.preventDefault();
        if ($(this).attr("data-dbkey")) {
            window.menu.openInFrame(this);
        }
    })



}

function refreshSelfFrame(frame) {
    var key = $(frame).attr("id");
    var tag = $("#systemMenu li a[data-key=" + key + "]");
    if (tag.length == 0)
        return;
    var src = tag.eq(0).attr("href");
    $(frame).attr("src", src);
}

function reOpenTab(tag) {
    tag = $(tag);
    var key = tag.attr("data-key");
    $("#content-main").find("iframe[data-id=" + key + "]")
        .remove();
    $("#pageTabsContent>a[data-id=" + key + "]").remove();
    setTimeout(function() {
            parent.window.menu.createNewTab(tag);
        },
        200);
}