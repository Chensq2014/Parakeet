//多Tab标签方案的实现

$(function () {
    window.menu = (function () {
        var footerHeight = $(".main-footer").outerHeight();
        var mainHeaderHeight = $(".main-header").height();
        var tabsHeight = $(".J_menuTabs").outerHeight();
        var paddingHeight = 40;
        this.tabTag = (function () {
            var obj = {};
            obj.createTab = function (id, desc, src, dbKey, lock) {
                //判断当前是否有没有锁住的Tab
                $("#pageTabsContent>a>")
                //创建li标签
                $("#pageTabsContent a").removeClass("active");
                var lockClass = lock === true ? "lock" : "unlock";
                var html = '<a href="javascript:;" class="J_menuTab active tab-' +
                    lockClass +
                    '" data-id="' +
                    id +
                    '" data-dbKey="' +
                    dbKey +
                    '"> <des>' +
                    desc +
                    '</des> <span onclick="toggleLock(this)" class="fa fa-' +
                    lockClass +
                    ' fa-lock-widget"></span> <i class="fa fa-times-circle"></i></a>';
                $("#pageTabsContent").append(html);
                //创建iframe标签
                $("#content-main").children().removeClass("active");
                $("#content-main").children().css("display", "none");
                html = '<iframe class="J_iframe" name="J_iframe" width="100%" height="100%" src="' +
                    src +
                    '" frameborder="0" data-id="' +
                    id +
                    '" id="' +
                    id +
                    '" seamless="" style="display: inline;"></iframe>';
                $("#content-main").append(html);
                //动态插入js等资源文件[由于devExtreme  all.js实在是太大，需要进行动态插入]
            };
            obj.setActiveTab = function (id) {
                $("#pageTabsContent").children().removeClass("active");
                $("#content-main").children().css("display", "none");
                $("#content-main").children().removeClass("active");

                $("#pageTabsContent").find("a[data-id=" + id + "]").addClass("active");//激活当前点击的选项卡
                $("#content-main iframe[data-id=" + id + "]").addClass("active");//激活当前选项卡的iframe
                $("#content-main iframe[data-id=" + id + "]").css("display", "block");//显示当前选项卡的iframe

            };
            
            return obj;
        })();

        this.createNewTab = function (obj, lock) {
            var tag = $(obj);
            if (tag.parent().find("ul").length > 0)
                return;
            var key = tag.attr("data-key");
            var dbKey = tag.attr("data-dbKey");
            while (key.indexOf(".") > -1) {
                key = key.replace('.', '1');
            }
            //判断当前是否存在
            if ($("#pageTabsContent").find("a[data-id=" + key + "]").length > 0) {
                //设置当前状态
                tabTag.setActiveTab(key);
                return;
            }
            var url = tag.attr("href");
            var desc = tag.find("span").html();
            tabTag.createTab(key, desc, url, dbKey, lock);
            //设置当前标签背景
            setNavListBackground(key);
        };

        this.openInFrame = function (obj) {
            var tag = $(obj);
            if (tag.parent().find("ul").length > 0)
                return;
            var key = tag.attr("data-key");
            var dbKey = tag.attr("data-dbKey");
            var url = tag.attr("href");
            while (key.indexOf(".") > -1) {
                key = key.replace('.', '1');
            }

            //设置当前标签背景
            setNavListBackground(key);

            //判断当前是否存在
            if ($("#pageTabsContent").find("a[data-id=" + key + "]").length > 0) {
                //设置当前状态
                tabTag.setActiveTab(key);
                return;
            }
            

            var frames = $("#content-main iframe");

            if (url.indexOf("maxmuim") >= 0) {
                window.open(url);
            }
            else if (frames.length == 0 || $("#pageTabsContent>a.tab-unlock").length == 0)
                createNewTab(obj);
            var url = tag.attr("href");
            var desc = tag.find("span").html();
            // 如果当前iframe为未锁定状态，则需要在当前页面中显示，否则需要展示在第一个iframe上面，同时将active状态转移到其上面
            var atags = $("#pageTabsContent>a");
            if (atags.filter(".tab-unlock").length) {
                if (atags.filter(".tab-unlock").filter(".active").length == 1) {
                    // 在当前iframe里面展开
                    var atag = atags.filter(".tab-unlock").filter(".active").eq(0);
                    var frame = frames.filter("[id=" + atag.attr("data-id") + "]");
                    if (frame.length == 0)
                        frame = null;
                } else {
                    var atag = atags.filter(".tab-unlock").eq(0);
                    var frame = frames.filter("[id=" + atag.attr("data-id") + "]");
                    if (frame.length == 0)
                        frame = null;
                }
            }

            if (frame) {
                if (frame.attr("data-id") == key)
                    return;
                var startTime = new Date().getTime();
                var timeHandler = setInterval(function () {
                    if (typeof frame[0].contentWindow._changeFrameContent === null) {
                        clearInterval(timeHandler);
                    }
                    if (typeof frame[0].contentWindow._changeFrameContent === "function") {
                        var atag = $("#pageTabsContent").find("a[data-id=" + frame.attr("id") + "]").eq(0);
                        atag.attr("data-id", key);
                        atag.attr("data-dbKey", dbKey);
                        atag.find("des").eq(0).html(desc);

                        frame.attr("data-id", key);
                        frame.attr("id", key);
                        url += "&needLayout=false";
                        frame[0].contentWindow._changeFrameContent(url);
                        tabTag.setActiveTab(key);
                        clearInterval(timeHandler);
                    } else {
                        var now = new Date().getTime();
                        if (now - startTime > 6000) {
                            clearInterval(timeHandler);
                        }
                    }
                },
                    50);
            }
        }

        function bindMenuEvent() {
            $("#systemMenu")
                .find("a[href!=#]")
                .unbind("click")//a 标签点击一次请求两次，因为上级也是a标签，click事件邦了两次当前a标签,移除再绑
                .click(
                //.on('click',
                function (e) {
                    //检查是否有空余Tab
                    e.preventDefault();
                    openInFrame(this);
                    //createNewTab(this);
                    //e.stopPropagation();//使用这个居然无效 点击父级不会展开子级
                });
            $(window)
                .bind("resize",
                function () {
                    rechangeFrameSize();
                });
        }

        function getActiveKey() {
            var rootTag = $("#tabLiContent");
            var activeTag = rootTag.find("li.active").eq(0).children().attr("href");
            return activeTag.substring(1, activeTag.length);
        }

        function rechangeFrameSize() {
            var screenHeight = $(".content-wrapper").height();
            var navHeight = $(".content-tabs").outerHeight();
            var resultHeight = screenHeight - footerHeight - tabsHeight - 5;
            window.console.log("resize");
            //重新设置所有iframe高度
            $("#content-main").height(screenHeight - navHeight - 5);
        }

        this.deleteTab = function (obj) {
            var tab = $(obj).parent();
            var key = tab.attr("data-key");
            tabTag.deleteTab(key);
        };

        bindMenuEvent();
        rechangeFrameSize();
        return this;
    })();
    setLeaveTabs();
    $(".sidebar-menu").click(function () {
        //setTimeout(() => {
        //    reCalculateHeight();
        //}, 1000);

    });
    //$("#aside-sidebar").dxScrollView({
    //    scrollByContent: true,
    //    scrollByThumb: true,
    //    showScrollbar: "onScroll",
    //    onReachBottom: updateBottomContent,
    //    reachBottomText: "Updating..."
    //}).dxScrollView("instance");
});

function updateBottomContent(e) {
}

function setLeaveTabs() {
    try {
        var string = localStorage.getItem("tabs");
        if (string == null)
            return;
        localStorage.removeItem("tabs");
        var tabs = JSON.parse(string);
        var atags = $("#systemMenu").find("a[data-key]");

        for (var i = 0; i < tabs.length; i++) {
            var target = atags.filter("[data-key=" + tabs[i].id + "]");
            if (target.length > 0) {
                window.menu.createNewTab(target[0], tabs[i].lock);
            }
        }
    } catch (e) {

    }
}
function toggleLock(obj) {
    var tag = $(obj);
    var activeTag = _getLeftLastActiveTag();
    if (tag.hasClass("fa-lock")) {
        tag.addClass("fa-unlock").removeClass("fa-lock").parent().removeClass("tab-lock").addClass("tab-unlock");
        //需要讲锁定的添加到未锁定的最前排
    }
    else {
        tag.addClass("fa-lock").removeClass("fa-unlock").parent().removeClass("tab-unlock").addClass("tab-lock");
    }
    tag.parent().insertAfter(activeTag);
}
//获取左边最靠后的一个锁定index索引
function _getLeftLastActiveTag() {
    var tags = $("#pageTabsContent>a");
    if (tags.length == 1)
        return tags.eq(0);
    for (var i = 1; i < tags.length; i++) {
        if (tags.eq(i).hasClass("tab-unlock")) {
            i--;
            break;
        }
    }
    //如果是最后一个的话，那就去最后一个tab标签
    if (i == tags.length)
        i--;
    return tags.eq(i);
}



//点击导航菜单后，重新计算高度
function reCalculateHeight() {
    window.console.log("click sidebar-menu!");
    var sidebarMenu_height = $(".sidebar-menu").outerHeight();
    var userPanel_height = $(".user-panel").outerHeight();
    var maincontentWrapper_height = $("#main-content-wrapper").outerHeight();
    var totalHeight = sidebarMenu_height + userPanel_height;
    var window_height = $(window).height();//窗口高度
    var neg = $('.main-header').outerHeight() + $('.main-footer').outerHeight() + 3;

    //如果导航菜单高度大于窗口高度，将更新所有iframe的高度
    if (totalHeight > window_height) {
        $(".sidebar").height(totalHeight);
        $("#main-content-wrapper").height(totalHeight);
        $(".content-wrapper, .right-side").height(totalHeight);
    }
    //如果导航菜单高度小于等于窗口高度，将更新所有iframe的高度为窗口高度
    else {
        $(".content-wrapper, .right-side").css('min-height', window_height - neg);
        $(".main-sidebar>.sidebar").css('height', window_height - neg);
        $("#main-content-wrapper").css('height', window_height - neg);
    }

}




//设置导航菜单背景
function setNavListBackground(moduleId) {
    $('.sidebar-menu li.clicked').removeClass("clicked");

    var menu = $(".sidebar-menu").find("a[key=" + moduleId + "]");
    var parentLi = menu.eq(0).parent();
    if (parentLi) {
        parentLi.addClass("clicked");
    }



}