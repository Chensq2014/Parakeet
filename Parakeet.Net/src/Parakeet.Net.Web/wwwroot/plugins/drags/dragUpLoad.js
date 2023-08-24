(function (k, f) {
    function upload(a) { } function g(a, e, c) {
        a = f.createElement(a); e && a.setAttribute('class', e);
        c && a.setAttribute('id', c);
        return a;
    }
    upload.prototype = {
        getBlob: function (a, e) {
            var c = this, b = f.createElement('img'), d = new FileReader;
            d.onload = function (d) {
                b.setAttribute('src', this.result);
                e({
                    img: b,
                    src: this.result,
                    blob: c.dataURItoBlob(this.result, a.name),
                    name: a.name
                });//e为dropCallback   里面的对象为dropCallback需要的data对象，这样就好做批量导入文件的扩展了
            };
            d.readAsDataURL(a);//dxFileUpload也有这个a对象，FileReader的readAsDataURL将a对象构造出图片的src地址
        },
        getDragImage: function (a) {
            debugger;
            var e = this, c = f.getElementById(a.id); c.addEventListener('drop', function (b) {
                b.cancelBubble = !0;
                b.preventDefault();
                'image/jpeg' == b.dataTransfer.files[0].type || 'image/gif' == b.dataTransfer.files[0].type || 'image/png' == b.dataTransfer.files[0].type ? (1 < b.dataTransfer.files.length && alert('\u672c\u6b21\u4e0a\u4f20\u53ea\u80fd\u4e0a\u4f20\u4e00\u5f20(\u7b2c\u4e00\u5f20)...'), e.getBlob(b.dataTransfer.files[0], a.dropCallback)) : alert('\u8bf7\u4e0a\u4f20\u6b63\u786e\u7684\u56fe\u7247(png/jpg/gif)...')
            }, !1);
            c.addEventListener('dragenter', function (b) { b.preventDefault() }, !1);
            c.addEventListener('dragleave', function (b) { b.preventDefault() },!1);
            c.addEventListener('dragover', function(b) { b.preventDefault() }, !1);
        },
        getDragCropImage: function (a) {
            a.dropCallback = function(a) {
                var c = f.body || f.documentElement,
                    b = g('div', 'crop-box-bg', 'crop-box-bg'),
                    d = g('div', 'crop-box', 'crop-box'),
                    e = g('canvas', 'crop-box-canvas', 'crop-box-canvas');
                c.appendChild(b);
                b.appendChild(d);
                d.appendChild(e);
                d.appendChild(a);
            };
            _this.getDragImage(a);
        },
        dataURItoBlob: function (a, e) {
            var c = atob(a.split(',')[1]); a = a.split(',')[0].split(':')[1].split(';')[0];
            for (var b = new ArrayBuffer(c.length),
                d = new Uint8Array(b), f = 0; f < c.length; f++)d[f] = c.charCodeAt(f);
            return new Blob([b], { type: a, name: e ? e : '' });
        },
        getCropImage: function (a) {
            _this.getBlob(a[0],
                function(a) {
                    var c = f.body || f.documentElement,
                        b = g('div', 'crop-box-bg', 'crop-box-bg'),
                        d = g('div', 'crop-box', 'crop-box'),
                        e = g('div', 'crop-box-cont', 'crop-box-cont'),
                        h = g('canvas', 'crop-box-canvas', 'crop-box-canvas');
                    c.appendChild(b);
                    b.appendChild(e);
                    e.appendChild(d);
                    d.appendChild(h);
                    d.appendChild(a);
                    c.style.overflow = 'hidden';
                    b.style.top = c.scrollTop + 'px';
                    h.style.display = 'none';
                    d.style.marginLeft = -d.offsetWidth / 2 + 'px';
                });
        }
    };
    k.dragUpLoads = upload;
})(window, document);