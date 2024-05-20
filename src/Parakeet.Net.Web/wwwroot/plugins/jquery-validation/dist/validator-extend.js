

zhValidExtention();//默认使用中文验证消息及风格

function zhValidExtention() {
    $.extend($.validator.messages, {
        required: "这是必填字段",
        remote: "请修正此字段",
        email: "请输入有效的电子邮件地址",
        url: "请输入有效的网址",
        date: "请输入有效的日期",
        dateISO: "请输入有效的日期 (YYYY/MM/DD)",
        number: "请输入有效的数字",
        digits: "只能输入数字",
        creditcard: "请输入有效的信用卡号码",
        equalTo: "你的输入不相同",
        extension: "请输入有效的后缀",
        maxlength: $.validator.format( "最多可以输入 {0} 个字符" ),
        minlength: $.validator.format( "最少要输入 {0} 个字符" ),
        rangelength: $.validator.format( "请输入长度在 {0} 到 {1} 之间的字符串" ),
        range: $.validator.format( "请输入范围在 {0} 到 {1} 之间的数值" ),
        step: $.validator.format( "请输入 {0} 的整数倍值" ),
        max: $.validator.format( "请输入不大于 {0} 的数值" ),
        min: $.validator.format( "请输入不小于 {0} 的数值" )
    });
    //电话号码验证
    $.validator.addMethod("isTel", function (value, element) {
        var phone = /^(0|86|17951)?(13[0-9]|15[012356789]|17[678]|18[0-9]|14[57])[0-9]{8}$/;
        var tel = /^(\d{3}-?)?[1-9]\d{7}$/;
        var telphone = /^(\d{4}-?)?[1-9]\d{6}$/;
        return tel.test(value) || telphone.test(value) || phone.test(value);
    }, $.validator.format("请输入有效的电话号码！"));
    //QQ号码验证
    $.validator.addMethod("isQQ", function (value, element) {
        var tel = /^[1-9][0-9]{4,9}$/;
        return value == null ? true : tel.test(value);
    }, $.validator.format("请输入有效的QQ号码！"));
}

function enValidExtention() {
    //电话号码验证
    $.validator.addMethod("isTel", function (value, element) {
        var phone = /^(0|86|17951)?(13[0-9]|15[012356789]|17[678]|18[0-9]|14[57])[0-9]{8}$/;
        var tel = /^(\d{3}-?)?[1-9]\d{7}$/;
        var telphone = /^(\d{4}-?)?[1-9]\d{6}$/;
        return tel.test(value) || telphone.test(value) || phone.test(value);
    }, $.validator.format("Please enter a valid phone number!"));
}


