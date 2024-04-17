
//获取当前窗口的建议高度和宽度值

function getAdvisableWidthAndHeight(obj) {
    var width = $('body').width(),
        height = $('body').height()-40;

    if (width > 1360) {
        width = width * 0.6;
    } else if (width > 800) {
        width = width * 0.8;
    } else {
        width = width - 40;
    }


    if (height > 900) {
        height = height * 0.6;
    } else if (height > 600) {
        height = height * 0.8;
    } else {
        height = height - 100;
    }

    if (obj) {
        if (width < obj.width)
            width = obj.width;
        if (height < obj.height)
            height = obj.height;
    }
    return { width: width, height: height };
}

//js数组去重原型扩展
Array.prototype.distinct = function () {
    var self = this;
    var _a = this.concat().sort();
    _a.sort(function (a, b) {
        if (a === b) {
            var n = self.indexOf(a);
            self.splice(n, 1);
        }
    });
    return self;
};

//金额格式化
function fmoney(s, n) {
    s = parseFloat((s + '').replace(/[^\d\.-]/g, '')).toFixed(n) + '';
    if (isNaN(s) || ((s + '').replace(/\s/g, '')) == '') {
        return '';
    }
    n = n > 0 && n <= 20 ? n : 2;
    var l = s.split('.')[0].split('').reverse(),
        r = s.split('.')[1];
    t = '';
    for (i = 0; i < l.length; i++) {
        //if (s.indexOf('-') >= 0) {
        //    t += l[i] + ((i + 1) % 3 == 0 && (i + 1) != (l.length - 1) ? ',' : '');
        //} else {
        t += l[i] + ((i + 1) % 3 == 0 && (i + 1) != l.length ? ',' : '');
        //}
    }
    if (r === undefined) {
        r = '00';
    }

    return t.split('').reverse().join('') + '.' + r;
}

// 金额格式化还原函数
function rmoney(s) {
    var ret = (s + '').replace(/[^\d\.-]/g, '');
    return parseFloat(ret);
}

function toggleHeader(e) {
    if ($(e).find('i').hasClass('fa-arrow-down')) {
        $(e).parents('.panel-group').find('i.fa-arrow-up').removeClass('fa-arrow-up').addClass('fa-arrow-down');
        $(e).find('i').removeClass('fa-arrow-down').addClass('fa-arrow-up');
    } else if ($(e).find('i').hasClass('fa-arrow-up')) {
        $(e).parents('.panel-group').find('i.fa-arrow-up').removeClass('fa-arrow-up').addClass('fa-arrow-down');
        $(e).find('i').removeClass('fa-arrow-up').addClass('fa-arrow-down');
    }
}


function printCurrentPage(name) {
    window.open(window.location.href + '&isPrintPage=true&printPageName=' + name);
}

function getAllOverflowHiddenStyle(id) {
    var tags = $('#' + id + ' *');
    for (var i = 0; i < tags.length; i++) {
        var overType = tags.eq(i).css('overflow-x');
        if (overType === 'hidden') {
            tags.eq(i).css('overflow-x', 'inherit');
        }
    }
}


function getEnumTypeObj(name) {
    var names = syncGet('/api/parakeet/enum/enumTypeItemKeyNameDescriptions?name=' + name);
    var namesObj = {};
    for (var i = 0; i < names.length; i++) {
        namesObj[names[i].Value] = names[i].Text;
    }
    return namesObj;
}

function uuid() {
    function S4() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }
    function guid() {
        return (S4() + S4() + '-' + S4() + '-' + S4() + '-' + S4() + '-' + S4() + S4() + S4());
    }
    return guid();
}

function setPopover(target, content, containerId) {
    if (containerId == null) containerId = 'MyPopover';
    $(target).on('mouseover',
        function() {
            E(containerId).dxPopover({
                width: 350,
                target: target,
                contentTemplate: function(container) {
                    $(content).appendTo(container);
                }
            }).dxPopover('instance').show();
        });
}
