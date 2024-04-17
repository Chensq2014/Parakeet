(function (w, doc) {
    function generate(tag, className, id) {
        tag = doc.createElement(tag); className && tag.setAttribute('class', className);
        id && tag.setAttribute('id', id);
        return tag;
    }
    w.dragUpLoads = function (obj) { };
    w.dragUpLoads.prototype = {
        getBlob: function (file, callback) {

            var thisObj = this, img = doc.createElement('img'), reader = new FileReader;
            reader.onload = function (d) {
                img.setAttribute('src', this.result);
                callback({
                    img: img,
                    src: this.result,
                    blob: thisObj.dataURItoBlob(this.result, file.name),
                    name: file.name
                });//里面的对象为dropCallback需要的data对象，这样就好做批量导入文件的扩展了
            };
            reader.readAsDataURL(file);
        },
        getDragImage: function (a) {
            var e = this, c = doc.getElementById(a.id); c.addEventListener('drop', function (b) {
                b.cancelBubble = !0;
                b.preventDefault();
                'image/jpeg' == b.dataTransfer.files[0].type || 'image/gif' == b.dataTransfer.files[0].type || 'image/png' == b.dataTransfer.files[0].type ? (1 < b.dataTransfer.files.length && alert('\u672c\u6b21\u4e0a\u4f20\u53ea\u80fd\u4e0a\u4f20\u4e00\u5f20(\u7b2c\u4e00\u5f20)...'), e.getBlob(b.dataTransfer.files[0], a.dropCallback)) : alert('\u8bf7\u4e0a\u4f20\u6b63\u786e\u7684\u56fe\u7247(png/jpg/gif)...')
            }, !1);
            c.addEventListener('dragenter', function (b) { b.preventDefault() }, !1);
            c.addEventListener('dragleave', function (b) { b.preventDefault() }, !1);
            c.addEventListener('dragover', function (b) { b.preventDefault() }, !1);
        },
        getDragCropImage: function (a) {
            a.dropCallback = function (a) {
                var c = doc.body || doc.documentElement,
                    b = generate('div', 'crop-box-bg', 'crop-box-bg'),
                    d = generate('div', 'crop-box', 'crop-box'),
                    e = generate('canvas', 'crop-box-canvas', 'crop-box-canvas');
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
        getCropImage: function (files) {
            _this.getBlob(files[0],
                function (a) {
                    var b = doc.body || doc.documentElement,
                        c = generate('div', 'crop-box-bg', 'crop-box-bg'),
                        d = generate('div', 'crop-box-cont', 'crop-box-cont'),
                        e = generate('div', 'crop-box', 'crop-box'),
                        f = generate('canvas', 'crop-box-canvas', 'crop-box-canvas');
                    b.appendChild(c);
                    c.appendChild(d);
                    d.appendChild(e);
                    e.appendChild(f);
                    e.appendChild(a);
                    b.style.overflow = 'hidden';
                    c.style.top = b.scrollTop + 'px';
                    f.style.display = 'none';
                    e.style.marginLeft = -e.offsetWidth / 2 + 'px';
                });
        }
    };

})(window, document);




function previewMedia(elType, file) {
    var type = file.type;
    var mediaNode = $('#' + elType);
    var canPlay = mediaNode.get(0).canPlayType(type);
    if (canPlay === '') {
        //DevExpress.ui.notify('不支持的媒体类型！');
        errNotify('上传成功');
        return;
    }
    var fileURL = URL.createObjectURL(file);
    mediaNode.show();
    mediaNode.attr({ 'type': type, 'src': fileURL });
}

function previewImages(file) {
    var reader = new FileReader();
    // Closure to capture the file information.
    reader.onload = (function (theFile) {
        return function (e) {
            // Render thumbnail.
            var span = $('<span>');
            span.html([
                '<img class="thumb" src="', e.target.result,
                '" title="', escape(theFile.name), '"/>'
            ].join(''));
            $('#list').append(span);
        };
    })(file);
    reader.readAsDataURL(file);
}


(function (w, d) {
    //获取从后端返回的attachment图片附件的 数据源
    w.getAttachmentImages = function (attachments) {
        var dataSource = [];
        //后端attachments 转成前端file 格式数据
        $.each(attachments,
            function (fIndex, file) {
                // 判断文件是否是图片 图片数据库id或者图片文件服务key
                if (file.isImage) {
                    var selector = file.id;//$.isEmptyObject(file.attachment.key) ? file.id : file.attachment.key;
                    dataSource.push({
                        Id: selector,
                        FileIndex: dataSource.length, //FileIndex等于dataSource数组下标
                        FileName: file.attachment.name,
                        FilePath: file.attachment.path,//URL.createObjectURL(file),
                        VirtualPath: file.attachment.virtualPath,
                        Size: file.attachment.size,
                        FileObj: file.attachment
                    });
                }
            });
        return dataSource;
    };
    //从后端返回的attachment图片附件在$target对象上初始化画廊控件
    w.initAttachmentGallery = function ($target, dataSource) {
        showLoading(function () {
            var config = dxConfig.gallery({
                dataSource: dataSource,
                height: 100,
                //width: '100%',
                loop: true,
                slideshowDelay: 5000,
                //elementAttr: {
                //    id: 'elementId',
                //    class: 'class-name'
                //},
                itemTemplate: function (item, index, itemElement) {
                    $('<img id="' + item.Id + '">').attr('src', item.VirtualPath).addClass('dx-gallery-item-image')
                        .attr('style', 'margin:5px;')
                        .dxContextMenu({
                            dataSource: [{ text: '删除', icon: 'remove' }],
                            width: 100,
                            target: '#' + item.Id,
                            fileIndex: item.FileIndex,
                            filePath: item.FilePath,
                            virtualPath: item.VirtualPath,
                            fileObj: item,//item.FileObj 缓存当前item对象给context实例，便于从数据源中移除
                            onItemClick: function (f) {
                                //添加时js数据源删除功能： 删除缓存的js图片，重新加载Gallery数据源即可
                                //dataSource = dataSource.filter(m =>  m !== f.component._options.fileObj);//注意不支持对象比较
                                dataSource = dataSource.filter(m => m.FileIndex !== f.component._options.fileIndex);//能支持对象上的简单字符串/数值比较
                                $target.dxGallery('instance').option('dataSource', dataSource);
                            }
                        }).appendTo(itemElement);
                    //$('<div>').addClass('item-file-name').text(item.FileName).appendTo(itemElement);
                    return itemElement;
                },
                onItemClick: function (e) {
                    //弹窗查看大图
                    showImagePopup(e);
                }
            });
            $target.dxGallery(config);
            hideLoading();
        });
    };
    //此套路使用FileReader 测试成功 
    w.getImagesDataSource = function (files) {
        var dataSource = [];
        $.each(files,
            function (fIndex, file) {
                var fileReader = new FileReader();
                fileReader.readAsDataURL(file);
                fileReader.onload = function (e) {
                    var imageData = this.result;
                    var selector = file.name.replace(/[\'\"\\\/\b\f\n\r\t\s]/g, '')
                        .replace(/[\#\$\%\^\&\*\{\}\(\)\.\:\"\L\<\>\?]/g, '').trim(); // 去掉转义/特殊字符
                    dataSource.push({
                        Id: selector,
                        FileIndex: dataSource.length, //FileIndex等于dataSource数组下标
                        FileName: file.name,
                        FilePath: imageData,
                        FileObj: file
                    });
                };
                fileReader.onloadend = function () {
                    //上传完毕
                };
            });
        return dataSource;
    };
    //此套路获取图片或者文件url地址bolb对象也测试成功
    w.getImages = function (files) {
        var dataSource = [];
        $.each(files,
            function (fIndex, file) {
                // 判断文件是否是图片
                if (file.type.startsWith('image')) {
                    var selector = file.name.replace(/[\'\"\\\/\b\f\n\r\t\s]/g, '')
                        .replace(/[\#\$\%\^\&\*\{\}\(\)\.\:\"\L\<\>\?]/g, '').trim(); // 去掉转义/特殊字符
                    dataSource.push({
                        Id: selector,
                        FileIndex: dataSource.length, //FileIndex等于dataSource数组下标
                        FileName: file.name,
                        FilePath: URL.createObjectURL(file),
                        FileObj: file
                    });
                } else {
                    //URL.createObjectURL(file)//可以是视频文件的播放地址
                    errNotify('请拖动支持的文件类型');
                }
            });
        return dataSource;
    };
    //传入jquery对象 并且在此对象上初始化画廊控件
    w.initGallery = function ($target, dataSource) {
        showLoading(function () {
            var config = dxConfig.gallery({
                dataSource: dataSource,
                height: '100%',
                //height: 300,
                //width: '100%',
                loop: true,
                slideshowDelay: 5000,
                //elementAttr: {
                //    id: 'elementId',
                //    class: 'class-name'
                //},
                itemTemplate: function (item, index, itemElement) {
                    $('<img id="' + item.Id + '">').attr('src', item.VirtualPath ? item.VirtualPath : item.FilePath)
                        .addClass('dx-gallery-item-image')
                        .attr('style', 'margin:5px;')
                        .dxContextMenu({
                            dataSource: [{ text: '删除' }],
                            width: 200,
                            target: '#' + item.Id,
                            fileIndex: item.FileIndex,
                            filePath: item.FilePath,
                            virtualPath: item.VirtualPath,
                            fileObj: item,//item.FileObj 缓存当前item对象给context实例，便于从数据源中移除
                            onItemClick: function (f) {
                                //添加时js数据源删除功能： 删除缓存的js图片，重新加载Gallery数据源即可
                                //dataSource = dataSource.filter(m =>  m !== f.component._options.fileObj);//注意不支持对象比较
                                dataSource = dataSource.filter(m => m.FileIndex !== f.component._options.fileIndex);//能支持对象上的简单字符串/数值比较
                                $target.dxGallery('instance').option('dataSource', dataSource);//g($target, dataSource);

                                ////如果是编辑：删除按钮就需要删除真实的后端图片存放的url地址 待续
                                //syncGet('/Need/RemoveAttachment?path=' + f.component._options.filePath,
                                //    function (r) { },
                                //    function (res, d) {
                                //        $target.dxGallery('instance').option('dataSource', dataSource);//g($target, dataSource);
                                //    });
                            }
                        }).appendTo(itemElement);
                    //$('<div>').addClass('item-file-name').text(item.FileName).appendTo(itemElement);
                    return itemElement;
                },
                onItemClick: function (e) {
                    //弹窗查看大图
                    showImagePopup(e);
                }
            });
            //debugger;
            //if ($target.find('#gallery-container').length) $target.find('#gallery-container').remove();
            //var gallery = E('gallery-container').dxGallery(config);
            //$('<div class="gallery-container">')
            //.css('style', { position: 'absolute', top: '-100px', 'z-index': '-999999' })
            //.append($('<div class="gallery-view">').dxGallery(config));

            $target.dxGallery(config);
            hideLoading();
        });
    };
    //调用：默认gallery展示 隐藏域
    //initGallery($('.gallery-view'), $('#uploadFiles').length?$('#uploadFiles').val():[]);
    w.initTileView = function ($target, dataSource) {
        showLoading(function () {
            var config = dxConfig.tileView({
                dataSource: dataSource,
                //items:dataSource
                height: '100%',
                //height: 400,
                width: '100%',
                baseItemHeight: 180,
                baseItemWidth: 200,
                itemMargin: 10,
                direction: 'horizontal',//"horizontal",// "vertical",
                noDataText: '请拖动图片到此区域',
                itemTemplate: function (item, index, itemElement) {
                    //itemElement.append("<div class=\"image\" style=\"background-image: url("+ item.FilePath + ")\"></div>");
                    $('<img id="' + item.Id + '">').attr('src', item.VirtualPath ? item.VirtualPath:item.FilePath).addClass('image')
                        //.attr('style', 'margin:10px;')
                        //.css('background-image','url("'+ item.FilePath + '")')
                        .dxContextMenu({
                            dataSource: [{ text: '删除' }],
                            width: 100,
                            target: '#' + item.Id,
                            fileIndex: item.FileIndex,
                            filePath: item.FilePath,
                            virtualPath: item.VirtualPath,
                            fileObj: item,//item.FileObj 缓存当前item对象给context实例，便于从数据源中移除
                            onItemClick: function (f) {
                                //添加时js数据源删除功能： 删除缓存的js图片，重新加载Gallery数据源即可
                                //dataSource = dataSource.filter(m =>  m !== f.component._options.fileObj);//注意不支持对象比较
                                dataSource = dataSource.filter(m => m.FileIndex !== f.component._options.fileIndex);//能支持对象上的简单字符串/数值比较
                                $target.dxTileView('instance').option('dataSource', dataSource);//g($target, dataSource);

                                ////如果是编辑：删除按钮就需要删除真实的后端图片存放的url地址 待续
                                //syncGet('/Need/RemoveAttachment?path=' + f.component._options.filePath,
                                //    function (r) { },
                                //    function (res, d) {
                                //        $target.dxTileView('instance').option('dataSource', dataSource);//g($target, dataSource);
                                //    });
                            }
                        }).appendTo(itemElement);

                    return itemElement;
                },
                onItemClick: function (e) {
                    //弹窗查看大图
                    showImagePopup(e);
                }
            });

            $target.dxTileView(config);
            hideLoading();
        });
    };
    //调用：initTileView($('.tile-view'), $('#uploadFiles').length?$('#uploadFiles').val():[]);

    //全屏预览
    w.showImagePopup = function (e) {
        var item = e.itemData;
        var dataSource = e.component._options.dataSource;
        var itemPopupconfig = {
            dataSource: dataSource,
            //parentComponent:e.component,
            //width: "100%",
            //height: "100%",
            //slideshowDelay: 3000,
            isGallery: true,
            loop: true,
            //fullScreen: true,
            showNavButtons: true,
            showIndicator: false,
            gridId: 'detail-gallery',
            popupId: 'detail-gallery-popup',
            title: '全屏预览',
            //btnOkText : '删除全部',//自定义
            contentTemplate: function (contentElement) {
                var enlargeDiv = $('<div>').css('text-align', 'center');
                showEnlargeImageGallery(enlargeDiv, dataSource,e.component);
                contentElement.html(enlargeDiv).css('text-align', 'center');
            }
        };
        showPopup(itemPopupconfig);
    };

    //全屏gallery浏览
    w.showEnlargeImageGallery = function ($target, dataSource,component) {
        showLoading(function() {
            var config = dxConfig.gallery({
                dataSource: dataSource,
                height: '100%',
                //height: 800,
                //width: '100%',
                loop: true,
                slideshowDelay: 5000,
                //elementAttr: {
                //    id: 'elementId',
                //    class: 'class-name'
                //},
                itemTemplate: function (item, index, itemElement) {
                    $('<img id="enlarge-' + item.Id + '">').attr('src', item.VirtualPath ? item.VirtualPath : item.FilePath)
                        .addClass('dx-gallery-item-image')
                        .attr('style', 'margin:5px;')
                        .dxContextMenu({
                            dataSource: [{ text: '删除' }],
                            width: 200,
                            target: '#enlarge-' + item.Id,
                            fileIndex: item.FileIndex,
                            filePath: item.FilePath,
                            virtualPath: item.VirtualPath,
                            fileObj: item,//item.FileObj 缓存当前item对象给context实例，便于从数据源中移除
                            onItemClick: function (f) {
                                //添加时js数据源删除功能： 删除缓存的js图片，重新加载Gallery数据源即可
                                //dataSource = dataSource.filter(m =>  m !== f.component._options.fileObj);//注意不支持对象比较
                                dataSource = dataSource.filter(m => m.FileIndex !== f.component._options.fileIndex);//能支持对象上的简单字符串/数值比较
                                $target.dxGallery('instance').option('dataSource', dataSource);//g($target, dataSource);
                                component.option('dataSource', dataSource);
                                ////如果是编辑：删除按钮就需要删除真实的后端图片存放的url地址 待续
                                //syncGet('/Need/RemoveAttachment?path=' + f.component._options.filePath,
                                //    function (r) { },
                                //    function (res, d) {
                                //        $target.dxGallery('instance').option('dataSource', dataSource);//g($target, dataSource);
                                //    });
                            }
                        }).appendTo(itemElement);
                    //$('<div>').addClass('item-file-name').text(item.FileName).appendTo(itemElement);
                    return itemElement.css('text-align', 'center');
                },
                onItemClick: function(e) {
                    //弹窗查看大图
                    var item = e.itemData;
                    var itemPopupconfig = {
                        //dataSource: window.imageList[index]["images"],
                        //width: "100%",
                        height: "100%",
                        //slideshowDelay: 3000,
                        isGallery: true,
                        loop: true,
                        fullScreen: true,
                        showNavButtons: true,
                        showIndicator: false,
                        gridId: 'detail-gallery',
                        popupId: 'enlarge-detail-gallery-popup',
                        title: '查看大图',
                        //btnOkText : '删除全部',//自定义
                        contentTemplate: function(contentElement) {
                            contentElement.html(
                                $('<img id="enlarge-' + item.Id + '">').attr('src', item.VirtualPath ? item.VirtualPath : item.FilePath)
                                .addClass('dx-gallery-item-image')
                                .dxContextMenu({
                                    dataSource: [{ text: '删除', icon: 'remove' }],
                                    width: 200,
                                    target: '#enlarge-' + item.Id,
                                    fileIndex: item.FileIndex,
                                    filePath: item.FilePath,
                                    virtualPath: item.VirtualPath,
                                    fileObj: item,//item.FileObj 缓存当前item对象给context实例，便于从数据源中移除
                                    onItemClick: function (f) {
                                        dataSource = dataSource.filter(m => m.FileIndex !== f.component._options.fileIndex);//能支持对象上的简单字符串/数值比较
                                        e.component.option('dataSource', dataSource);//g($target, dataSource);
                                        component.option('dataSource', dataSource);
                                        hidePopup('enlarge-detail-gallery-popup');//关闭popup
                                    }
                                })
                            ).css('text-align', 'center');
                        }
                    };
                    showPopup(itemPopupconfig);
                }
            });
            $target.dxGallery(config);
            hideLoading();
        });
    };

})(window, document);


