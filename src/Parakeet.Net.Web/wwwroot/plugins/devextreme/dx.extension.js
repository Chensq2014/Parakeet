
//#region  全局常量定义区域 最大最小值 日期格式等
/* devextreme 扩展 20181201
 * chensq
 * dataGrid treeList selectBox lookup
 * 定义 全局常量
 */
window.__Max = 99999999999999999;
window.__MonthFormat = 'yyyy/MM';
window.__DateFormat = 'yyyy/MM/dd';
window.__DateTimeFormat = 'yyyy/MM/dd hh:mm:ss';
window.__DxWidgetStageSnapShot = {};
// #endregion

//#region  定义dxConfig变量 扩展DevExtreme各种控件默认配置 grid editGrid treeList pivotGrid popup selectBox lookup dxDateBoxRanger ...
//treeView dropDownBox ratioEditor percentEditor setDataSource setStore dxFileUploader dxGallery等默认配置

var dxConfig = (function () {

    // #region dataGrid 默认配置
    /* url为字符串'/host/controller/get'/或数组url=[]*/
    this.grid = function (url) {
        var store = isString(url)
            ? DevExpress.data.AspNet.createStore({
                key: 'id',
                loadUrl: url
                // loadUrl: url + '/Get',
                // insertUrl: url + '/Insert',
                // updateUrl: url + '/Update',
                // deleteUrl: url + '/Delete'
            }) : $.isArray(url) ? url : [];
        var config = {
            dateSerializationFormat: __DateFormat,//日期转换18.2.6以后
            baseUrl: isString(url) ? url : '',
            dataSource: store,
            showBorders: false,
            showRowLines: true,
            allowColumnResizing: true,
            //remoteOperations: true,
            remoteOperations: {
                summary: false,
                grouping: false,
                groupPaging: false,
                paging: true,
                sorting: true,
                filtering: true
            },
            wordWrapEnabled: true,//支持换行
            searchPanel: {
                visible: false,
                highlightCaseSensitive: true,
                searchVisibleColumnsOnly: false,
                //text:'',
                placeholder: '搜索...',
                width: 200
            },
            filterRow: {
                visible: false
            },
            headerFilter: {
                visible: true
                // texts: {//交给多语言控制
                //     cancel: '取消',
                //     ok: '确认',
                //     emptyValue: '空'
                // }
            },
            groupPanel: {
                visible: false
                //emptyPanelText: '拖动列头到此分组'
            },
            grouping: {
                autoExpandAll: true
            },
            export: {
                enabled: false
                //fileName: '',
                //texts: {
                //    //excelFormat:'导出',
                //    exportAll: '导出所有数据',
                //    exportSelectedRows: '导出选中行'
                //}
            },
            pager: {
                showNavigationButtons: true,
                pageSize: 10,
                showPageSizeSelector: true,
                allowedPageSizes: [5, 10, 15, 20, 30],
                showInfo: true,
                visible: true,
                infoText: '第{0}/{1}页(总共{2}条数据)'
            },
            paging: {
                pageSize: 10
            },
            selection: {
                mode: 'single',
                allowSelectAll: false
            }

            //默认事件 扩展区
            //onRowClick: function (info) {
            //    window.onRowClick(info);
            //}

        };
        //if (isString(url)) setDataSource(config);
        config.editing = {
            mode: 'cell',
            allowAdding: false,
            allowDeleting: false,
            allowUpdating: false
            //无需扩展，使用本地语言化解决
            //texts: {
            //    addRow: '添加',
            //    addRowToNode: '添加子节点',
            //    cancelAllChanges: '撤销',
            //    cancelRowChanges: '撤销',
            //    confirmDeleteMessage: '确认删除此数据',
            //    confirmDeleteTitle: '删除警告框',
            //    deleteRow: '删除',
            //    editRow: '修改',
            //    saveAllChanges: '保存所有',
            //    saveRowChanges: '保存',
            //    undeleteRow: '撤销'
            //}
        };
        return config;
    };

    /* grid treeList selectbox等config 以及建立好的store参数
     * 根据store设置datasource
     * 如果无store参数，使用config.baseUrl作为请求建立默认store
     */
    this.setDataSource = function (config, store) {
        config.dataSource = store
            ? store
            : DevExpress.data.AspNet.createStore({
                key: 'id',
                loadUrl: config.baseUrl
            });
    };

    //请求url='/host/controller/get'或url=[]  key：主键字符串(可选，默认为'Id')
    this.setStore = function (url, key) {
        //store devExtreme
        return isString(url)
            ? DevExpress.data.AspNet.createStore({
                key: key ? key : 'id',
                loadUrl: url
                //loadUrl: url + '/Get' + idString,
                //insertUrl: url + '/Insert' + idString,
                //updateUrl: url + '/Update' + idString,
                //deleteUrl: url + '/Delete' + idString
            }) : $.isArray(url) ? url : [];
        //DevExpress的dataSource
        //dataSource: new DevExpress.data.DataSource({ 
        //    store: employeesTasks, //[]
        //    key: "ID", 
        //    group: "Assigned"
        //}),
    };
    // #endregion

    // #region dataGrid可编辑 默认config设置 参数示例：obj='' / [] / {url:'/host/controller',id:1,onLoading:funciton(res){},onLoaded:function(res){}}
    /*
     * obj可以直接是url字符串或者是包含obj={url:'/host/controller',id:1,onLoading:funciton(res){},onLoaded:function(res){}}参数的对象
     */
    this.editGrid = function (obj) {
        var data = {
            url: obj.url ? obj.url : obj,
            id: obj.id,
            onLoading: obj.onLoading,
            onLoaded: obj.onLoaded
        };
        var config = grid(data.url);
        if (isString(config.baseUrl)) {
            var idString = exist(data.id) ? '?id=' + data.id : '';
            //切换为自定义store
            var store = DevExpress.data.AspNet.createStore({
                key: 'id',//对大小写敏感了
                keyType: 'Guid', //{ Id: 'Guid' },
                loadUrl: config.baseUrl + '/Get' + idString,
                insertUrl: config.baseUrl + '/Insert' + idString,
                updateUrl: config.baseUrl + '/Update' + idString,
                deleteUrl: config.baseUrl + '/Delete' + idString,
                onLoading: data.onLoading,
                onLoaded: data.onLoaded
            });

            //var store = new DevExpress.data.ODataStore({
            //    type: 'odata',
            //    key: 'id',
            //    keyType: { Id: 'Guid' },//'Guid', //
            //    url: config.baseUrl + '/Get' + idString
            //});

            //var store = new DevExpress.data.CustomStore({
            //    //key: 'id',
            //    //keyType: { Id: 'Guid' },//'Guid', //
            //    load: function (loadOptions) {
            //        var deferred = $.Deferred(),
            //            args = {};

            //        if (loadOptions.sort) {
            //            args.orderby = loadOptions.sort[0].selector;
            //            if (loadOptions.sort[0].desc)
            //                args.orderby += " desc";
            //        }

            //        args.skip = loadOptions.skip;
            //        args.take = loadOptions.take;

            //        $.ajax({
            //            url: config.baseUrl + '/Get' + idString,
            //            dataType: 'json',
            //            data: args,
            //            success: function (result) {
            //                deferred.resolve(result.data, { totalCount: result.data.length });
            //            },
            //            error: function () {
            //                deferred.reject("Data Loading Error");
            //            },
            //            timeout: 5000
            //        });
            //        return deferred.promise();
            //    }
            //});

            config.dataSource = store;
        }
        //默认编辑设置
        config.editing = {
            mode: 'cell',
            allowAdding: true,
            allowDeleting: true,
            allowUpdating: true,
            //无需扩展，使用本地化解决
            //texts: {
            //    addRow: '添加',
            //    addRowToNode: '添加子节点',
            //    cancelAllChanges: '撤销',
            //    cancelRowChanges: '撤销',
            //    confirmDeleteMessage: '确认删除此数据',
            //    confirmDeleteTitle: '删除警告框',
            //    deleteRow: '删除',
            //    editRow: '修改',
            //    saveAllChanges: '保存所有',
            //    saveRowChanges: '保存',
            //    undeleteRow: '撤销'
            //}
        };
        return config;
    };
    // #endregion

    // #region treeList的默认配置获取 参数示例：obj='' / [] / {url:'/host/controller',id:1,onLoading:funciton(res){},onLoaded:function(res){}}
    /*
     * obj为字符串/带url参数对象/数组
    */
    this.treeList = function (obj) {
        var data = {
            url: obj.url ? obj.url : obj,
            id: obj.id,
            onLoading: obj.onLoading,
            onLoaded: obj.onLoaded
        };
        var idString = exist(data.id) ? '?id=' + data.id : '';
        //构建数据链接
        var store = isString(data.url)
            ? DevExpress.data.AspNet.createStore({
                key: 'id',
                loadUrl: data.url + '/Get' + idString,
                insertUrl: data.url + '/Insert' + idString,
                updateUrl: data.url + '/Update' + idString,
                deleteUrl: data.url + '/Delete' + idString
            }) : $.isArray(data.url) ? data.url : [];

        var config = {
            dateSerializationFormat: __DateFormat,//日期转换18.2.6以后
            baseUrl: isString(data.url) ? data.url : '',
            dataSource: store,
            keyExpr: 'id',
            parentIdExpr: 'parentId',
            autoExpandAll: false,
            showRowLines: true,
            allowColumnResizing: true,
            //height: $('body').height() * 0.8,
            //wordWrapEnabled: true,
            //showEditorAlways: false,
            //expandedRowKeys: [2, 3],
            //selectedRowKeys: [7],
            //columnAutoWidth: true,
            searchPanel: {
                visible: false,
                highlightCaseSensitive: true,
                searchVisibleColumnsOnly: false,
                //text:'',
                placeholder: '搜索...',
                width: 200
            },
            headerFilter: {
                visible: false
            },
            columnChooser: {
                enabled: false
            },
            selection: {
                mode: 'single'
            },
            sorting: {
                mode: 'none'
            },
            editing: {
                mode: 'cell',
                allowDeleting: true,
                allowAdding: true,
                allowUpdating: true,
                //无需扩展，使用本地语言化解决...
                //texts: {
                //    addRow: '添加',
                //    addRowToNode: '添加子节点',
                //    cancelAllChanges: '撤销',
                //    cancelRowChanges: '撤销',
                //    confirmDeleteMessage: '确认删除此数据',
                //    confirmDeleteTitle: '删除警告框',
                //    deleteRow: '删除',
                //    editRow: '修改',
                //    saveAllChanges: '保存所有',
                //    saveRowChanges: '保存',
                //    undeleteRow: '撤销'
                //}
            },
            columnFixing: {
                enable: true
            },
            columns: []
        };

        return config;
    };
    // #endregion

    // #region pivotGrid的默认配置获取 参数示例：obj='' / [] / {url:'/host/controller',id:1,onLoading:funciton(res){},onLoaded:function(res){}}
    /*
     * obj为字符串/带url参数对象/数组
    */
    this.pivotGrid = function (obj) {
        var data = {
            url: obj.url ? obj.url : obj,
            id: obj.id,
            onLoading: obj.onLoading,
            onLoaded: obj.onLoaded
        };
        var idString = exist(data.id) ? '?id=' + data.id : '';
        //构建数据链接 前端可以替换新的store 这里给出默认值
        var store = isString(data.url)
            ? DevExpress.data.AspNet.createStore({
                key: 'id',
                loadUrl: data.url + '/Get' + idString,
                onLoaded: $.isFunction(obj.onLoaded) ? obj.onLoaded : function (res) { }
                //在dx.aspnet.mvc.js里面可以添加onLoaded扩展,格式化日期
                //onLoaded: function (res) {
                //    $.each(res.data, function (i, row) {
                //        if(row.Date)row.Date = T(row.Date, __DateFormat);
                //        if(row.Time)row.Time = T(row.Time, __DateFormat);
                //        if(row.startDate)row.startDate = T(row.startDate, __DateFormat);
                //        if(row.StartTime)row.StartTime = T(row.StartTime, __DateFormat);
                //        if(row.endDate)row.endDate = T(row.endDate, __DateFormat);
                //        if(row.EndTime)row.EndTime = T(row.EndTime, __DateFormat);
                //    });
                //}
            })
            : [];

        var config = {
            allowSortingBySummary: true,
            allowSorting: true,
            allowFiltering: true,
            allowExpandAll: true,
            showBorders: true,
            wordWrapEnabled: false,
            showRowGrandTotals: true,//是否显示全行/列pivotGrid合计 默认显示
            showTotalsPrior: 'columns',//是否优先显示合计行/列 (一般指'columns/rows'的合计位置优先显示在前面)
            baseUrl: isString(data.url) ? data.url : '',
            //dataSource: $.isArray(data.url) ? data.url : isString(data.url) ? store : [],
            dataSource: {
                store: $.isArray(data.url) ? data.url : isString(data.url) ? store : [], //array,//
                //type: 'array',//type: 'xmla',
                //remoteOperations: true,//每次点击再重新请求数据
                fields: [
                    //{
                    //    caption: '分期',//同时排序行/列
                    //    dataField: 'InstallmentName',//dataField: "InstallmentId",
                    //    width: 120,
                    //    allowSorting: true,
                    //    expanded: false,//默认展开行/列
                    //    showTotals: true,//是否显示合计行/列,默认显示
                    //    //wordWrapEnabled: true,
                    //    sortOrder: 'asc',//排序会根据selector里面字符串row.Sort开头  升序排列
                    //    selector: function (row) {
                    //        return row.Sort + '-' + row.InstallmentName;
                    //    },
                    //    customizeText: function (e) {
                    //        var str = e.value.split('-');
                    //        return str[1];//自定义界面显示：row.InstallmentName，达到排序与显示分离
                    //    },
                    //    //使用了selector之后，sortingMethod就自动失效，不用selecor/特殊情况下写sortingMethod自定义排序
                    //    //sortingMethod: function (a, b) {
                    //    //    var index1 = parseInt(a.value.split('-')[0]);
                    //    //    var index2 = parseInt(b.value.split('-')[0]);
                    //    //    return index1 > index2;
                    //    //}
                    //    area: 'row'
                    //},
                    //{
                    //    caption: '业态产品',
                    //    dataField: 'ProductName',
                    //    width: 120,
                    //    expanded: false,
                    //    showTotals: true,
                    //    area: 'row'
                    //},
                    //{
                    //    dataField: 'Date',
                    //    dataType: 'date',
                    //    groupInterval: 'year',//日期 年
                    //    area: 'column'
                    //},
                    //{
                    //    dataField: 'Date',
                    //    dataType: 'date',
                    //    groupInterval: 'month',//日期 月
                    //    area: 'column'
                    //},
                    //{
                    //    caption: '金额',
                    //    dataField: 'Amount',
                    //    dataType: 'number',
                    //    summaryType: 'sum',
                    //    format: { type: 'fixedPoint', precision: 2 },
                    //    area: 'data'
                    //}
                ]
            },
            fieldChooser: {
                enabled: false
            },
            export: {
                enabled: true,
                //text:'导出到Excel',
                fileName: '导出数据' + new Date().toLocaleDateString()
            },
            stateStoring: {
                enabled: true,
                type: 'localStorage',
                storageKey: 'dx-widget-gallery-pivotgrid-storing-' + new Date().toJSON()
            },
            scrolling: {
                mode: 'virtual',
                useNative: false
            }
            //height: $('body').height() * 0.8,
            //width: 'auto',
            //texts: {//留给多语言控制
            //    grandTotal: '合计',
            //    exportToExcel: '导出到Excel',
            //    expandAll: '展开所有',
            //    collapseAll: '关闭所有',
            //    total: '{0}合计',
            //    noData: '空数据'
            //},

            //#region 事件扩展区 默认示例
            //用户自定义扩展 onCellClick 给出默认示例
            //onCellClick: function (e) {
            //    if (e.area === 'data') {
            //        var pivotGridDataSource = e.component.getDataSource(),//获取dataSource方法
            //            rowPathLength = e.cell.rowPath.length,
            //            rowPathName = e.cell.rowPath[rowPathLength - 1],
            //            popupTitle = (rowPathName ? rowPathName : '汇总') + '数据源';
            //        var drillDownDataSource = pivotGridDataSource.createDrillDownDataSource(e.cell);
            //        drillDownDataSource.load().done(function (items) {
            //            showPivotGridSourcePopup(items, popupTitle, e.cellElement, e.component);
            //        });
            //    }
            //},

            //用户自定义扩展 onToolbarPreparing
            //onToolbarPreparing: function (items, config) {
            //    items.push({
            //        widget: 'dxCustomHtml',
            //        options: {
            //            template: function (container) {
            //                var html = $('<div class="dx-field-label">预计毛利率</div>' +
            //                    '<div class="dx-field-value">' +
            //                    '   <div class="expect-ratio"></div>' +
            //                    '</div>');
            //                return html;
            //            }
            //        },
            //        location: 'before'
            //    });
            //    items.push({
            //        widget: 'dxButton',
            //        options: {
            //            text: '刷新',
            //            onClick: function (e) {
            //                e.event.stopPropagation();
            //                config.refreshFunc(config.component);
            //            }
            //        },
            //        location: 'after'
            //    });
            //},
            //onContentReady: function (e) {
            //    //$('.expect-ratio').dxNumberBox(ratioConfig);
            //}
            //#endregion

        };
        return config;
    };
    //#endregion

    // #region popup 弹窗默认配置设置 预留确定与取消回调函数扩展 参数示例：extraConfig={toolbarItems:[]}

    this.popup = function (extraConfig) {
        var config = {
            fullScreen: false,
            hint: false,
            showTitle: true,
            visible: true,
            dragEnabled: true,
            closeOnOutsideClick: true,
            showCloseButton: true, //showTitle=true 才生效
            resizeEnabled: true,
            shading: false,
            showPopupTitle: true,
            //position : 'center',//默认center
            //container:,//可以设定弹窗位置,container可以是jquery对象
            width: 600,
            height: 'auto',
            shadingColor: 'rgba(151,151,151,0.7)',
            //title: extraConfig.title ? extraConfig.title : extraConfig, //默认有title
            //toolbarItems: extraConfig.toolbarItems ? extraConfig.toolbarItems : []

            //titleTemplate : function(container) {
            //    container.append($('<div class="green" />').html('title'));
            //},
            //contentTemplate : function (container) {
            //    container.append($('<div class="" />').html('msg')); //green
            //    return container;
            //},
            //toolbarItems: [
            //    {
            //        toolbar: 'bottom',
            //        widget: 'dxButton',
            //        options: {
            //            text: '确认',
            //            elementAttr: {
            //                //'class': 'green',
            //                'style': 'background-color:#1BBC9B;' //color:white;
            //            },
            //            icon: 'fa fa-save',
            //            hoverStateEnabled: true,
            //            onClick: function (f) {
            //                //if (fn) fn(f);//f.event.stopPropagation();
            //                //if (completeFn) completeFn();
            //                //window.__confirmHandler.hide();
            //            }
            //        },
            //        location: 'after'
            //    },
            //    {
            //        toolbar: 'bottom',
            //        widget: 'dxButton',
            //        options: {
            //            text: '取消',
            //            icon: 'fa fa-undo',
            //            //elementAttr: {
            //            //    'style': 'color:green;'
            //            //},
            //            hoverStateEnabled: true,
            //            onClick: function (f) {
            //                //if (cancelFn) cancelFn(f);//f.event.stopPropagation();
            //                //if (completeFn) completeFn(f);
            //                //window.__confirmHandler.hide();
            //            }
            //        },
            //        location: 'after'
            //    }
            //]
        };

        if (!$.isEmptyObject(extraConfig)) {
            extraConfig = $.extend(true, config, extraConfig); //合并为同一个config,extraConfig也发生合并
        }
        //window.__confirmHandler = E('popup-container').dxPopup(popup).dxPopup('instance');
        //window.__confirmHandler.show();
        return config;
    };

    // #endregion

    // #region selectBox 默认配置设置 url='/host/controller/get'或url=[],value=1

    this.selectBox = function (url, value) {
        // var store = isString(url)
        //     ? DevExpress.data.AspNet.createStore({
        //         key: 'Value',
        //         loadUrl: url,
        //     }) : $.isArray(url) ? url : [];
        var config = selectBoxExpr();
        config.baseUrl = isString(url) ? url : '';
        config.dataSource = setStore(url, 'value');//store;//setDataSource(config, store);
        config.value = value;
        return config;
    };

    this.selectIdNameBox = function (url, value) {
        // var store = isString(url)
        //     ? DevExpress.data.AspNet.createStore({
        //         key: 'Value',
        //         loadUrl: url,
        //     }) : $.isArray(url) ? url : [];
        var config = selectBoxExpr();
        config.displayExpr = 'name';
        config.valueExpr = 'id';
        config.baseUrl = isString(url) ? url : '';
        config.dataSource = setStore(url, 'value');//store;//setDataSource(config, store);
        config.value = value;
        return config;
    };
    this.selectBoxTvExchange = function (url, value) {
        // var store = isString(url)
        //     ? DevExpress.data.AspNet.createStore({
        //         key: 'Value',
        //         loadUrl: url,
        //     }) : $.isArray(url) ? url : [];
        var config = selectBoxExpr();
        config.valueExpr = 'text';
        config.displayExpr = 'value';
        config.baseUrl = isString(url) ? url : '';
        config.dataSource = setStore(url, 'value');//store;//setDataSource(config, store);
        config.value = value;
        return config;
    };

    this.selectEnumBox = function (url, value) {
        // var store = isString(url)
        //     ? DevExpress.data.AspNet.createStore({
        //         key: 'Value',
        //         loadUrl: url,
        //     }) : $.isArray(url) ? url : [];
        var config = selectBoxExpr();
        config.displayExpr = 'itemDescription';
        config.valueExpr = 'itemKey';
        config.baseUrl = isString(url) ? url : '';
        config.dataSource = setStore(url, 'itemKey');//store;//setDataSource(config, store);
        config.value = value;
        return config;
    };

    this.selectBoxExpr = function () {
        return {
            displayExpr: 'text',
            valueExpr: 'value',
            searchPlaceholder: '请选择...',
            closeOnOutsideClick: true
            //displayExpr: 'Id',
            //valueExpr: 'Name',
            //searchEnabled: true,//默认就能搜索
            //searchMode: 'Contains'
        };
    };

    // #endregion

    // #region lookup 默认配置设置 url='/host/controller/get'或url=[],value=1

    this.lookup = function (url, value) {
        var config = selectBox(url, value);
        config.showCancelButton = false;
        config.showPopupTitle = false;
        config.searchEnabled = true;
        config.searchMode = 'Contains';
        return config;
    }

    // #endregion

    // #region dxDateBoxRanger 日期范围设置 参数示例： obj={formId:'dx-form-container-example', startField:'startDate', endField:'endDate'}

    this.dxDateBoxRanger = function (obj) {
        if (!obj.startField) {
            obj.startField = 'startDate';
            obj.endField = 'endDate';
        }
        var dateTimeOption = {
            width: '100%',
            //editorOptions: {
            //    acceptCustomValue: false,
            //    maxZoomLevel: 'year',
            //    max: projectendDate,
            //    displayFormat: function (value) { return T(value, __MonthFormat); }
            //},
            //cellTemplate: function (ele, info) {
            //    info.data.date = T(info.data.Date, __DateFormat);
            //    $(ele).append(info.data.date.slice(0, info.data.date.length - 3));
            //},
            //validationRules: [{ type: 'required', message: '时间必填项' }],
            onValueChanged: function (e) {
                bindFormDateRanger(obj, e.component);
            }
        };
        return { startTime: dateTimeOption, endTime: dateTimeOption };
    };

    // #endregion

    // #region treeView 默认配置 ds='/host/controller/get'或者ds=[]

    this.treeView = function (ds) {
        var store = setStore(ds);
        var config = {
            baseUrl: isString(ds) ? ds : '',
            dataSource: $.isArray(ds) ? ds : store, //syncGet(ds)
            dataStructure: 'plain',
            keyExpr: 'id',
            displayExpr: 'name', //'Name',
            parentIdExpr: 'parentId',
            expandedExpr: 'expanded',
            selectedExpr: 'isSelected', //'IsSelected',
            selectionMode: 'multiRecursive', //multiple ,multiRecursive
            showCheckBoxesMode: 'normal', // normal none selectAll
            scrollDirection: 'vertical',
            //width: 'auto',
            //height: 300, //默认高度300
            showCheckBoxes: true,
            activeStateEnabled: false,
            hoverStateEnabled: false,
            expandAllEnabled: true,
            focusStateEnabled: false,
            selectNodesRecursive: true,
            selectByClick: false
        };
        return config;
    };

    // #endregion

    // #region dropDownBox下拉框扩展 参数示例： ds='/host/controller/get'或者ds=[]
    this.dropDownBox = function (ds) {
        var store = setStore(ds);
        var config = {
            baseUrl: isString(ds) ? ds : '',
            dataSource: $.isArray(ds) ? ds : store, //syncGet(ds)
            valueExpr: 'id',
            displayExpr: 'name',
            placeholder: '请选择数据',
            width: 500,
            showClearButton: true
        };
        return config;
    };
    // #endregion

    //#region dxNumberBox 默认配置(包含百分数) 参数：百分数示例：obj={min:0,max:1,format:'#,###.## %',onValueChanged:function(e){//业务逻辑}}
    this.numberConfig = function (obj) {
        //所有可配置项
        var config = {
            //accessKey: null,
            //activeStateEnabled: false,
            //disabled: false,
            //elementAttr: {},
            //focusStateEnabled: true,
            //height: undefined,
            //hint: undefined,
            //hoverStateEnabled: true,
            //inputAttr: {},
            //invalidValueMessage: 'Value must be a number',
            //isValid: true,
            //mode: 'text',
            //name: '',
            //onChange: null,
            //onContentReady: null,
            //onCopy: null,
            //onCut: null,
            //onDisposing: null,
            //onEnterKey: null,
            //onFocusIn: null,
            //onFocusOut: null,
            //onInitialized: null,
            //onInput: null,
            //onKeyDown: null,
            //onKeyPress: null,
            //onKeyUp: null,
            //onOptionChanged: null,
            //onPaste: null,
            //readOnly: false,
            //rtlEnabled: false,
            //showClearButton: false,
            //showSpinButtons: false,
            //step: 1,
            //stylingMode: 'outlined',
            //tabIndex: 0,
            //useLargeSpinButtons: false,
            //validationError: undefined,
            //validationMessageMode: 'auto',
            //valueChangeEvent: 'change',
            //visible: true,
            //width: undefined,
            min: $.isNumeric(obj.min) ? obj.min : 0,
            max: $.isNumeric(obj.max) ? obj.max : __Max,
            value: $.isNumeric(obj.defaultValue) ? obj.defaultValue : null, //有默认值，需要获取
            format: isString(obj.format) ? obj.format : '#,###.##',//默认数字格式两位小数 
            placeholder: '请输入...',
            onValueChanged: function (e) {
                if ($.isFunction(obj.onValueChanged)) {
                    //e.value = Number((e.value * 1).toFixed(4));//应该写入obj.onValueChanged 函数中
                    //if (config.value !== e.value) {
                    //    config.value = e.value;
                    //    obj.onValueChanged(e);
                    //}
                    obj.onValueChanged(e);
                }
            },
            targetSelector: isString(obj.targetSelector) ? obj.targetSelector : null
        };
        if (config.targetSelector) $(config.targetSelector).dxNumberBox(config);//如果配置了targetSelector，初始化就会直接显示出来
        return config;
    };
    //#endregion

    // #region dxTextBox ratioEditor/percentEditor带百分号的数据  编辑默认配置 defaultRatio 默认值 
    //可以将配置绑定在dxForm的Items 的editorOptions上 默认应该是dxTextBox方式显示的

    this.ratioEditor = function (defaultRatio) {
        //闭包
        var getRatioData = function (value) {
            if (!value) return { text: '', value: '' };
            var number = Number(value);
            if (isNaN(number)) number = 0;
            number = number < 0 ? 0 : number;
            number = Number((number * 1).toFixed(4));
            var text = Number((number * 100).toFixed(2));
            return { text: text + '%', value: number };
        };
        var baseRatioConfig = {
            min: 0,
            max: __Max,
            onFocusOut: function (e) {
                var data = getRatioData(e.component.option('value'));
                e.component.option('value', data.value);
                setTimeout(function () {
                    e.element.find('.dx-texteditor-container>input').val(data.text);
                    //e.element.find('.dx-texteditor-container>input').append('%');
                }, 50);
            },
            onFocusIn: function (e) {
                var value = e.component.option('value');
                e.element.find('.dx-texteditor-container>input').val(value);
            },
            onInitialized: function (e) {
                setTimeout(function () {
                    e.element.find('.dx-texteditor-container>input').css('text-align', 'right');
                    // 初始化数据
                    var data = getRatioData(e.component.option('value'));
                    e.element.find('.dx-texteditor-container>input').val(data.text);
                    //e.element.find('.dx-texteditor-container>input').append('%');
                }, 50);
            }
        };
        if (!defaultRatio) baseRatioConfig.value = defaultRatio;
        return baseRatioConfig;
    };
    //百分数(0-1) textbox 编辑默认配置 defaultRatio 默认值
    this.percentEditor = function (defaultRatio) {
        var getPercentData = function (value) {
            if (!value) return { text: '', value: '' };
            var number = parseFloat(value);
            if (isNaN(number)) number = 0;
            number = number < 0 ? 0 : (number > 1 ? 1 : number);
            number = Number((number * 1).toFixed(4));
            var text = (number * 100).toFixed(2);
            return { text: text + '%', value: number };
        };
        var baseConfig = {
            min: 0,
            max: 1,
            onFocusOut: function (e) {
                var data = getPercentData(e.component.option('value'));
                e.component.option('value', data.value);
                setTimeout(function () {
                    e.element.find('.dx-texteditor-container>input').val(data.text);
                    //e.element.find('.dx-texteditor-container>input').append('%');
                }, 50);
            },
            onFocusIn: function (e) {
                var value = e.component.option('value');
                e.element.find('.dx-texteditor-container>input').val(value);
            },
            onInitialized: function (e) {
                e.element.addClass('dx-percent-editor');
                setTimeout(function () {
                    // 初始化数据
                    var data = getPercentData(e.component.option('value'));
                    e.element.find('.dx-texteditor-container>input').val(data.text);
                    //e.element.find('.dx-texteditor-container>input').append('%');
                }, 50);
            }
        };
        if (!defaultRatio) baseConfig.value = defaultRatio;
        return baseConfig;
    };
    // #endregion

    //#region dxFileUploader  默认文件上传
    this.fileUploader = function (uploaderConfig) {
        var config = {
            //activeStateEnabled: false,
            //allowCanceling: true,
            //disabled: false,
            //focusStateEnabled: true,
            //hoverStateEnabled: false,
            //isValid: true,
            //readOnly: false,
            //rtlEnabled: false,
            //progress: 0,
            //tabIndex: 0,
            //visible: true,//false,//
            //accessKey: null,
            //chunkSize: 0,
            //validationError: undefined,
            //height: undefined,
            //hint: undefined,
            //width: undefined,
            //chunkSize: 6291456,

            //allowedFileExtensions: ['.jpg', '.jpeg', '.gif', '.png'],
            //accept: 'image/*',
            name: 'uploadFile',
            uploadMethod: 'POST',
            uploadUrl: 'uploadUrl',
            uploadMode: 'instantly', //'useForm',//'useButtons',//
            icon: 'upload',
            value: [],
            showFileList: false,//true, 
            multiple: false,//默认单文件上传
            maxFileSize: 6291456,//6兆字节(MB)
            minFileSize: 0,
            elementAttr: {},
            uploadHeaders: {},
            //交给本地化多语言控制....
            //uploadButtonText: '上传',
            labelText: '或拖拽文件到此区域',
            //selectButtonText: '上传文件...',
            //uploadFailedMessage: '上传失败！',
            //uploadedMessage: '上传成功！',
            //invalidFileExtensionMessage: '不支持的文件类型',
            //invalidMaxFileSizeMessage: '文件太大',
            //invalidMinFileSizeMessage: '文件太小',
            //readyToUploadMessage: '准备上传',
            //onContentReady: null,
            //onDisposing: null,
            //onInitialized: null,
            //onOptionChanged: null,
            //onUploaded: null,
            //onUploadStarted: null,
            onProgress: function (e) {
                showLoading();
            },
            onUploadError: function (e) {
                errNotify('上传失败，注意文件大小，请压缩小于4MB后上传！');
                hideLoading();
            },
            onUploadAborted: function (e) {
                hideLoading();
            }
            //onValueChanged: function(e) {
            //    //已选择的文件名称
            //    //e.value[0].name = 'project11.jpg';//改文件名
            //    //e.component.option('selectButtonText', e.value[0].name);
            //},
            //,onUploaded: function(e) {
            //    sucNotify('上传成功');
            //    hideLoading();
            //}
        };
        if (!$.isEmptyObject(uploaderConfig)) {
            uploaderConfig = $.extend(true, config, uploaderConfig); //合并为同一个config,uploaderConfig也发生合并
        }
        return config;
    };
    //#endregion

    //#region dxGallery 默认画廊
    this.gallery = function (galleryConfig) {
        var config = {
            dataSource: galleryConfig.dataSource ? galleryConfig.dataSource : setStore(galleryConfig),
            swipeEnabled: true,
            rtlEnabled: true,
            wrapAround: true,
            stretchImages: true,
            showNavButtons: true,
            showIndicator: false,
            noDataText: '请拖动图片到此区域',
            height: 1200,
            width: '100%',
            loop: true,
            slideshowDelay: 5000
            //elementAttr: {
            //    id: 'elementId',
            //    class: 'class-name'
            //},
            //itemTemplate: function (item, index, itemElement) {
            //    $('<img id="' + item.Order + '">').attr('src', item.VirtualPath).attr('filePath', item.FilePath)
            //        .attr('width', '98%')
            //        //.attr('width', 'auto').attr('height', 'auto')
            //        //.attr('max-width', '100%').attr('max-height', '100%')
            //        .dxContextMenu({
            //            dataSource: [{ text: '删除' }],
            //            width: 200,
            //            target: '#' +item.Order,//item.Order,
            //            filePath: item.FilePath,
            //            onItemClick: function (f) {
            //                //f.component._options.filePath// f.element.attr('filePath')
            //                syncGet('/HKLAND.Project/Project/RemoveAttachment?path=' + f.component._options.filePath,
            //                    function (r) { },
            //                    function (res, d) {
            //                        //逻辑;
            //                    });
            //            }
            //        })
            //        .appendTo(itemElement);
            //    $('<div>').addClass('item-file-name').text(item.FileName).appendTo(itemElement);
            //    return itemElement;
            //},
            //itemTemplate: $("#item-template"),
            //onItemClick: function (e) {
            //    //showGalleryPopup('查看大图', itemPopupconfig);
            //    debugger;
            //},
            //onItemContextMenu: function (e) {
            //    //删除功能 '/HKLAND.Project/Project/RemoveAttachment?path='
            //}
        };
        if (!$.isEmptyObject(galleryConfig)) {
            galleryConfig = $.extend(true, config, galleryConfig);
        }
        return config;
    };
    //#endregion

    //#region dxTileView 视图
    this.tileView = function (tileViewConfig) {
        var config = {
            dataSource: tileViewConfig.dataSource ? tileViewConfig.dataSource : setStore(tileViewConfig),
            //items: null,//使用dataSource代替items
            accessKey: null,
            activeStateEnabled: true,
            baseItemHeight: 100,
            baseItemWidth: 100,
            direction: 'horizontal',//vertical
            disabled: false,
            elementAttr: {},
            focusStateEnabled: true,
            height: 300,
            hint: undefined,
            hoverStateEnabled: true,
            itemHoldTimeout: 750,
            itemMargin: 10,
            itemTemplate: 'item',
            noDataText: '请添加视图文件',
            onContentReady: null,
            onDisposing: null,
            onInitialized: null,
            onItemClick: null,
            onItemContextMenu: null,
            onItemHold: null,
            onItemRendered: null,
            onOptionChanged: null,
            rtlEnabled: false,
            showScrollbar: false,
            tabIndex: 0,
            visible: true,
            width: undefined
        };
        if (!$.isEmptyObject(tileViewConfig)) {
            tileViewConfig = $.extend(true, config, tileViewConfig);
        }
        return config;
    };
    //#endregion

    //#region  dxScrollView
    this.scrollViewConfig = function (scrollViewConfig) {
        var config = {
            scrollByContent: true,
            //scrollByThumb: true,
            useNative: false
            //showScrollbar: 'always'
        };
        if (!$.isEmptyObject(scrollViewConfig)) {
            scrollViewConfig = $.extend(true, config, scrollViewConfig);
        }
        return config;
    };
    //#endregion

    //#region  dxChart
    this.chartConfig = function (chartConfig) {
        var config = {
            dataSource: [{
                day: 'Monday',
                oranges: 3
            }, {
                day: 'Tuesday',
                oranges: 2
            }],
            series: {
                argumentField: 'day',
                valueField: 'oranges',
                name: 'My oranges',
                type: 'bar',
                color: '#ffaa66'
            }
        };
        if (!$.isEmptyObject(chartConfig)) {
            chartConfig = $.extend(true, config, chartConfig);
        }
        return config;
    };
    //#endregion

    //#region  dxButtonGroup 
    this.buttonGroupConfig = function (btnGroupConfig) {
        var config = {
            items: [
                //{
                //    //icon: 'alignleft',
                //    text:'所有项目',
                //    alignment: 'left',
                //    onClick: function(e) {}
                //},
                //{
                //    //icon: 'aligncenter',
                //    text:'重庆',
                //    alignment: 'center',
                //    onClick: function(e) {}
                //},
                //{
                //    //icon: 'alignright',
                //    text:'上海',
                //    alignment: 'right',
                //    onClick: function(e) {}
                //},
                //{
                //    //icon: 'alignjustify',
                //    text:'成都',
                //    alignment: 'justify',
                //    onClick: function(e) {}
                //}
            ],
            keyExpr: 'alignment',
            stylingMode: 'outlined'
        };
        if (!$.isEmptyObject(btnGroupConfig)) {
            btnGroupConfig = $.extend(true, config, btnGroupConfig);
        }
        return config;
    };
    //#endregion
    return this;
})();

// #endregion

//#region 获取dxDataGrid与dxTreeList实例及扩展的windows全局函数

(function () {

    // #region 根据selector 获取dxDataGrid instance/component对象 selector:jquerySelector 

    window.getDataGrid = function (selector) {
        var grid = getJqueryObj(selector);
        if (grid.length) {
            grid = grid.dxDataGrid('instance');
            grid._options.component = grid;
        }
        return grid;
    };

    window.getDataGridInstance = function (selector) {
        return getDataGrid(selector);
    };
    // #endregion

    //#region 根据selector 获取dxTreeList 获取控件 selector:jquerySelector 
    window.getTreeList = function (selector) {
        var tree = getJqueryObj(selector);
        if (tree.length) {
            tree = grid.dxTreeList('instance');
            tree._options.component = tree;
        }
        return tree;
    };
    //#endregion

    //#region 根据instance与index 获取dxDataGrid/dxTreeList row对象 instance为dx实例对象，rowIndex:行下标数字， 此方法仍有待调试
    window.getDataGridRowByInstance = function (instance, rowIndex) {
        var key = instance.getKeyByRowIndex(rowIndex);
        //instance.selectRowsByIndexes([rowIndex]);
        //var data = getSelectedRowsData();//getSelectedRowKeys() //getRowElement(rowIndex)
        var data = instance.getDataByKeys([key]);//这个方法在新版dx官网没发现了
        return data;
    };
    // #endregion

    //#region  判断dxDataGrid是否有选择行 选中行数据等 selector:jquerySelector 
    window.isDataGridHasSelectRows = function (selector) {
        var grid = getDataGrid(selector);
        return grid && grid.getSelectedRowsData() && grid.getSelectedRowsData().length;
    };

    //根据selector获取dxDataGrid 当前选中所有行数组
    window.getDataGridSelectedDatas = function (selector) {
        var grid = getDataGrid(selector);
        return $.isEmptyObject(grid) ? grid : grid.getSelectedRowsData();
    };
    //根据selector获取dxDataGrid 当前选中行
    window.getDataGridSelectedData = function (selector) {
        var datas = getDataGridSelectedDatas(selector);
        return $.isEmptyObject(datas) ? datas : datas[0];
    };
    //根据selector获取dxDataGrid 当前选中行Key值
    window.getDataGridSelectedDataId = function (selector) {
        var data = getDataGridSelectedData(selector);
        return $.isEmptyObject(data) ? data : data.Id;
    };
    //根据selector 与index获取控件指定行数据
    window.getDataGridDataByIndex = function (selector, index) {
        var grid = getDataGrid(selector);
        if (!$.isEmptyObject(grid)) {
            var key = grid.getKeyByRowIndex(index);
            var items = grid.getDataSource()._items;
            for (var i = 0; i < items.length; i++) {
                if (items[i].Id === key) {
                    return items[i];
                }
            }
        }
        return grid;
    };
    // #endregion

    //#region 根据selector获取component/dxDataGrid的所有editData{ inserts: [],updates: [], deletes: []}

    window.getDataGridEditData = function (selector) {
        var instance = getDataGridInstance(selector);
        return $.isEmptyObject(instance) ? { inserts: [], updates: [], deletes: [] } : getDataGridEditDataByInstance(instance);
    };
    //根据instance获取编辑的数据返回含(添加，编辑，删除)集合的自定义对象 
    window.getDataGridEditDataByInstance = function (instance) {
        var data = instance._controllers.editing._editData;
        var updateData = [],
            deleteData = [],
            addData = [];
        $.each(data, function (rIndex, row) {
            switch (row.type) {
                case 'insert': addData.push(row); break;
                case 'update': updateData.push(row); break;
                case 'remove': deleteData.push(row); break;
            }
        });
        return { updates: updateData, deletes: deleteData, inserts: addData };
    };
    //根据selector获取component某个dataField页面编辑的 insert update remove 的keyValue集合 组装成后端数据集
    window.getDataGridEditKeyValueData = function (selector, dataField) {
        var grid = getDataGrid(selector);
        return getDataGridEditKeyValueDataByInstance(grid, dataField);
    };
    //获取component 某个dataField编辑的 insert update remove 的keyValues集合 组装成后端数据集
    window.getDataGridEditKeyValueDataByInstance = function (instance, dataField) {
        var data = instance._controllers.editing._editData;
        var typeEnum = { 'insert': 0, 'update': 10, 'remove': 20 };//后端枚举类型定义倍率为10
        var keyValueData = [];
        $.each(data, function (rIndex, row) {
            if (row.type) {
                var temp = {};
                if (row.oldData) {//update,remove
                    temp.key = row.oldData.Id;
                    temp.value = row.oldData[dataField];
                    if (row.data && (row.data[dataField] || row.data[dataField] === 0)) {
                        temp.value = row.data[dataField];
                    }
                } else {//insert 没有key(0)
                    if (row.data) {
                        temp.value = row.data[dataField];
                    }
                }
                temp.Type = typeEnum[row.type];
                keyValueData.push(temp);
            }
        });
        return keyValueData;
    };

    // #endregion

    //#region 获取datagrid inserts/updates/deletes
    window.getDataGridPostData = function (grid) {
        var editData = getDataGridEditDataByInstance(grid);
        var data = {
            inserts: [],
            updates: [],
            deletes: []
        };
        if (editData.inserts.length > 0) {
            $.each(editData.inserts,
                function (addIndex, row) {
                    data.inserts.push({
                        Key: row.key,
                        Value: JSON.stringify(row.data)
                    });
                });
        }
        if (editData.updates.length > 0) {
            $.each(editData.updates,
                function (updateIndex, row) {
                    data.updates.push({
                        Key: row.key,
                        Value: JSON.stringify(row.data)
                    });
                });
        }
        if (editData.deletes.length > 0) {
            $.each(editData.deletes,
                function (deleteIndex, row) {
                    data.deletes.push({
                        Key: row.key,
                        Value: JSON.stringify(row.oldData)
                    });
                });
        }
        return data;
    };

    //#endregion

    //#region 可用于dxGrid/dxTreeList 的日期范围、小数位数、验证重复等 gridSugarHelper类
    var gridSugarHelper = (function () {
        // 设置开始时间和结束时间的范围选择插件
        this.setDateTimeRange = function (config, startField, endField) {
            //缓存用户自定义onEditorPreparing逻辑 待扩展逻辑完毕后再执行
            var customFunc = config.onEditorPreparing;
            customFunc = customFunc ? customFunc : function (e) { };
            if (typeof customFunc == 'function') {
                config.onEditorPreparing = function (e) {
                    if (e.row && e.row.isEditing) {
                        e.editorOptions = e.editorOptions ? {} : e.editorOptions;
                        if (e.dataField === startField || e.dataField === endField) {
                            var otherFiledName = e.dataField === startField ? endField : startField;
                            var minOrMaxDate = e.row.data[otherFiledName] ? new Date(e.row.data[otherFiledName]) : null;
                            switch (e.dataField) {
                                case startField:
                                    e.editorOptions.max = minOrMaxDate ? minOrMaxDate : new Date('2200-01-01');
                                    break;
                                case endField:
                                    e.editorOptions.min = minOrMaxDate ? minOrMaxDate : new Date('1900-01-01');
                                    break;
                            }
                        }
                    }
                    customFunc(e);
                }
            }
        };
        // 数字类型字段，统一设置小数位数
        this.calculateNumberFields = function (columns, numberFields, numberics) {
            $.each(columns, function (cIndex, c) {
                if (c.columns) calculateNumberFields(c.columns, numberFields, numberics);
            });
            if (columns.length) {
                $.each(columns, function (cIndex, column) {
                    if (column.dataType && column.dataType === 'number') {
                        if (column.format && column.format.type) {
                            if ($.inArray(column.format.type, numberics) >= 0) {
                                var precision = 0;
                                if (column.format.precision) {
                                    precision = column.format.precision;
                                }
                                if (column.format.type === 'percent') {
                                    precision = precision + 2;
                                }
                                numberFields.push({ dataField: column.dataField, precision: precision });
                            }
                        }else if (column.valueFormat && column.valueFormat.type) {
                            if ($.inArray(column.valueFormat.type, numberics) >= 0) {
                                var precision = 0;
                                if (column.valueFormat.precision) {
                                    precision = column.valueFormat.precision;
                                }
                                if (column.valueFormat.type === 'percent') {
                                    precision = precision + 2;
                                }
                                numberFields.push({ dataField: column.dataField, precision: precision });
                            }
                        }
                    }
                });
            }
        };

        //formatNumberAndPercent函数 扩展dxGrid onRowValidating 加入小数位数扩展：
        //percent 4位小数,其余类型number 2位小数,注意Int类型数据不能保存两位小数
        this.formatNumberValidating = function (config) {
            var numberFields = [];
            var numberics = [
                'currency', 'fixedPoint', 'percent', 'decimal', 'exponential', 'largeNumber', 'thousands', 'millions',
                'billions', 'trillions'
            ];
            calculateNumberFields(config.columns, numberFields, numberics);
            var onRowValidating = config.onRowValidating;
            onRowValidating = onRowValidating ? function () { } : onRowValidating;
            config.onRowValidating = function (e) {
                if (e.newData) {
                    $.each(numberFields,
                        function (pIndex, p) {
                            if (e.newData.hasOwnProperty(p.dataField)) {
                                if (typeof (e.newData[p.dataField]) == 'number') {
                                    e.newData[p.dataField] = Number(e.newData[p.dataField].toFixed(p.precision));
                                }
                            }
                        });
                }
                if ($.isFunction(onRowValidating)) onRowValidating(e);
            };
            return config;
        };

        //根据控件selector(目前支持dataGrid与treeList) 与fieldValues 检查是否重复编辑/插入 暂未使用
        this.validateWidgetData = function (selector, fieldValues) {
            var tag = getJqueryObj(selector);
            if (!tag.length) return '';
            var instance = null;
            var child = tag.find('>div');
            if (child.hasClass('dx-datagrid')) {
                instance = getDataGrid(selector);
            } else if (child.hasClass('dx-treelist')) {
                instance = getTreeList(selector);
            }
            //检查重复编辑keys
            function getEditDataRepeatIds(fields, source) {
                var repeatIds = [];
                for (var i = 0; i < source.length; i++) {
                    var valid = true;
                    for (var j = 0; j < fields.length; j++) {
                        if (source[i][fields[j].field] !== fields[j].value) valid = false;
                    }
                    if (valid) repeatIds.push(source[i].Id);
                }
                return repeatIds;
            }

            var editData = getDataGridEditDataByInstance(instance);
            // 与 update-insert-数据进行对比
            if (editData.inserts && editData.inserts.length) {
                var insertRepeatIds = getEditDataRepeatIds(fieldValues, editData.inserts);
                if (insertRepeatIds && insertRepeatIds.length) return { type: 'insert_repeat' };
            }
            if (editData.update && editData.update.length) {
                var updateRepeatIds = getEditDataRepeatIds(fieldValues, editData.update);
                if (updateRepeatIds && updateRepeatIds.length) return { type: 'update_repeat' };
            }
            return { type: 'check_db_repeat' };
        };
        return this;
    })();
    //#endregion

    //#region dataGrid/treeList动态列时间轴 折叠展开公共函数expandAll collapsAll toggleYearCommon
    /*
     *展开dataGrid动态列时间轴
     *@grid  dataGrid实例对象
     *设计：single_与band_的name前缀 为独立年份列和包含月份的年分列
     * 展开函数 默认将所有独立年份列隐藏，包含月份的年份列(包含月份)展开
     */
    window.expandAll = function (grid) {
        var columns = grid._options.columns;
        if (columns && columns.length) {
            $.each(columns,
                function (cIndex, column) {
                    if (column.name) {
                        if (column.name.indexOf('band_') >= 0 && column.columns) {
                            if (column.columns.length) {
                                grid.columnOption('single_' + column.dataField, 'visible', false);
                                grid.columnOption('band_' + column.dataField, 'visible', true);
                            }
                        }
                        //if (column.name.indexOf('month_') >= 0) {
                        //    grid.columnOption('month_' + column.dataField, 'visible', true);
                        //}
                    }
                });
        }
    };
    /*
     *折叠dataGrid动态列时间轴
     *@grid dataGrid实例对象
     *设计：single_与band_的name前缀 为独立年份列和包含月份的年分列
     *折叠函数 默认将所有独立年份列展开，包含月份的年份列(包含月份)隐藏
     */
    window.collapsAll = function (grid) {
        var columns = grid._options.columns; //window.columns;
        if (columns && columns.length) {
            $.each(columns,
                function (cIndex, column) {
                    if (column.name) {
                        if (column.name.indexOf('band_') >= 0 && column.columns) {
                            if (column.columns.length) {
                                grid.columnOption('single_' + column.dataField, 'visible', true);
                                grid.columnOption('band_' + column.dataField, 'visible', false);
                            }
                        }
                    }
                });
        }
    };

    /*
     *折叠dataGrid动态列时间轴
     * @config  grid的config配置
     * @info 传入行数据对象 至少有dataField属性,hasChildColumn可有可无
     *
     */
    window.toggleYearCommon = function (config, info) {
        var year = info.column.dataField;
        if (info.column.hasChildColumn) {
            config.component.columnOption('single_' + year, 'visible', true);
            config.component.columnOption('band_' + year, 'visible', false);
        } else {
            config.component.columnOption('single_' + year, 'visible', false);
            config.component.columnOption('band_' + year, 'visible', true);
        }
    };

    //#endregion

    //#region grid头部显示提示消息 addGridMessage addPivotGridMessage removeGridMessage
    window.addPivotGridMessage = function (selector, message, type) {
        var tag = getJqueryObj(selector + '>div >div.alert');
        if (tag.length === 0) {
            if (!type) type = 'danger';
            var html = '<div class="alert alert-' +
                type +
                ' alert-dismissable" style="margin-bottom:5px;padding:8px 30px 8px 8px;">';
            html +=
                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true" style="color:white; opacity:1;">×</button>';
            html += '<p>' + message + '</p>';
            html += '</div>';
            var table = getJqueryObj(selector + '>div >table.dx-word-wrap');
            $(html).insertBefore(table);
        } else {
            tag.find('>p').text(message);
        }
    };

    window.addGridMessage = function (selector, message, type) {
        var tag = getJqueryObj(selector + '>div >div.alert');
        var wrap = getJqueryObj(selector + ' div.dx-datagrid >div.dx-datagrid-headers.dx-datagrid-nowrap');
        if (tag.length === 0) {
            if (!type) type = 'danger';
            var html = '<div class="alert alert-' +
                type +
                ' alert-dismissable" style="margin-bottom:5px;padding:8px 30px 8px 8px;">';
            html +=
                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true" style="color:white; opacity:1;" >×</button>';
            html += '<p>' + message + '</p>';
            html += '</div>';
            $(html).insertBefore(wrap);
        } else {
            tag.find('>p').text(message);
        }
    };

    window.removeGridMessage = function (id) {
        var target = getJqueryObj(id + '>div >div.alert');
        target.remove();
    };
    //#endregion

    //#region  getColumnByDataField通过dataField返回对应Column  通用：dataGrid/treelist/others
    window.getColumnByDataField = function (columns, dataField) {
        var currentColumn = [];
        $.each(columns, function (cIndex, column) {
            if (column.dataField === dataField) {
                currentColumn = column;
                return false;
            } else {
                return true;
            }
        });
        return currentColumn;
    };
    //#endregion

    //#region 自定义 DataGrid/TreeList 通用默认扩展 config为dx控件config

    window.gridSugar = function (config) {
        // #region 检测columns 中ValidationRules 默认Id列，开始结束日期初始化， 移除错误消息
        if (config.columns && config.columns.length > 0) {
            var idColumnsIndex = -1, startIndex = -1, endIndex = -1;// 检测Id及开始时间和结束时间索引
            var startDateArray = ['StartTime', 'startDate', 'startDateTime', 'BeginDate'];
            var endDateArray = ['EndTime', 'endDate', 'endDateTime', 'DeadLine'];
            $.each(config.columns, function (cIndex, column) {
                if ($.inArray(column.dataField, startDateArray) >= 0 || column.startRange) startIndex = cIndex;
                if ($.inArray(column.dataField, endDateArray) >= 0 || column.endRange) endIndex = cIndex;
                if (column.dataField === 'id') idColumnsIndex = cIndex;
                //监测ValidationRules
                if (column.validationRules && column.validationRules.length > 0) {
                    //遍历Rules ，并查找出定制Rules 使用j控制rule下标
                    $.each(column.validationRules, function (rIndex, rule) {
                        if (rule.type === 'fieldUnique') {
                            var url = exist(rule.checkUrl)
                                ? rule.checkUrl
                                : config.baseUrl + '/CheckField';
                            if (!url) throw 'fieldUnique 必须指定checkUrl'; //throw会怎样显示？
                            rule.type = 'custom';
                            rule.validationCallback = function (e) {
                                var group = e.validator._options.validationGroup;
                                var pass = true;//true表示验证通过，如果有重复就应该为false pass=!repeat;
                                if (group.type === 'insert' || group.type === 'update') {
                                    //var field = column.dataField;
                                    //var id = group.type !== 'insert' ? group.key : null;
                                    //var connector = url.indexOf('?') > 0 ? '&' : '?';
                                    //var repeat = syncGet(url +
                                    //    connector +
                                    //    'field=' +
                                    //    field +
                                    //    '&value=' +
                                    //    e.value +
                                    //    '&id=' +
                                    //    (id > 0 ? id : 0));

                                    //改用post请求
                                    var repeat = syncPost(url,
                                        {
                                            field: {
                                                Field: column.dataField,
                                                Value: e.value,
                                                Id: group.type === 'insert'
                                                    ? '00000000-0000-0000-0000-000000000000'
                                                    : group.key
                                            }
                                        });
                                    pass = !repeat;
                                }
                                return pass;
                            };
                        }
                        if (rule.type === 'fieldsUnique') {
                            var fields = rule.fields;
                            if ($.isEmptyObject(fields) || !fields.length) {
                                throw 'fieldsUnique 必须配置多字段';
                            }
                            var checkUrl = exist(rule.checkUrl)
                                ? rule.checkUrl
                                : config.baseUrl + '/CheckFields';
                            if (!checkUrl) throw 'fieldUnique 必须指定checkUrl ';
                            rule.type = 'custom';
                            rule.validationCallback = function (e) {
                                var pass = true;//true表示验证通过，如果有重复就应该为false pass=!repeat;
                                var group = e.validator._options.validationGroup;
                                if (group.type === 'insert' || group.type === 'update') {
                                    //判断是否有本字段配置
                                    if (rule.fields.indexOf(column.dataField) === -1) {
                                        rule.fields.push(column.dataField);
                                    }
                                    var fields = [];
                                    $.each(rule.fields, function (fIndex, field) {
                                        field = field.replace(field[0], field[0].toLowerCase());
                                        var value = $.isEmptyObject(group.data[field]) && exist(group.oldData)
                                            ? group.oldData[field]
                                            : group.data[field];
                                        fields.push({
                                            Field: field,
                                            Value: value,
                                            //Id: group.key//key
                                            Id: group.type === 'insert'
                                                ? '00000000-0000-0000-0000-000000000000'
                                                : group.key
                                        });
                                    });
                                    //var widgetId = options.validator._$element.parent().parent().attr('outer-widget-id');
                                    var repeat = syncPost(checkUrl, { fields: fields });
                                    pass = !repeat;
                                }
                                return pass;
                            };
                        }
                    });
                }
            });
            // 排序按照Id倒叙来(默认最后被插入数据优先显示) columns中没有Id列才添加默认Id列
            if (idColumnsIndex === -1) {
                config.columns.push({
                    dataField: 'Id',
                    visible: false,
                    sortOrder: 'desc',
                    sortIndex: 0
                });
            }

            // 调整时间范围选择 column上配置日期的allowSetTimeRange属性
            if (startIndex !== -1 && endIndex !== -1) {
                if (config.columns[startIndex].allowSetTimeRange &&
                    config.columns[endIndex].allowSetTimeRange) {
                    gridSugarHelper.setDateTimeRange(config,
                        config.columns[startIndex].dataField,
                        config.columns[endIndex].dataField);
                }
            }

            //onContentReady 中移除自定义错误消息
            var onContentReady = config.onContentReady ? config.onContentReady : function (e) { };
            config.onContentReady = function (e) {
                removeGridMessage(e.component._options.gridSelector);//(e.element.attr('id'));//
                onContentReady(e);
            };
        }
        //#endregion

        //#region 利用onToolbarPreparing事件 如果没有刷新按钮，在全局里添加默认更新按钮 批量编辑扩展
        var onToolbarPreparing = config.onToolbarPreparing ? config.onToolbarPreparing : function (e) { };
        if (typeof onToolbarPreparing == 'function') {
            config.onToolbarPreparing = function (e) {
                //判断是否存在Refresh button
                var hasRefreshButton = false, saveButtonIndex = -1;
                $.each(e.toolbarOptions.items,
                    function (tIndex, item) {
                        if (item.name === 'searchPanel') {
                            item.location = 'before';//searchPanel 展示到最前面
                        }
                        if (item.widget === 'dxButton' &&
                            (item.options.icon === 'refresh' || item.name === 'refresh')) {
                            hasRefreshButton = true;
                        }
                        if (item.name === 'saveButton') {
                            saveButtonIndex = tIndex;
                        }
                    });
                if (!hasRefreshButton) {
                    e.toolbarOptions.items.push({
                        name: 'refresh', //添加刷新按钮名称，方便grid按钮扩展chensq
                        widget: 'dxButton',
                        options: {
                            icon: 'refresh',
                            onClick: function (f) {
                                f.event.stopPropagation();
                                //扩展：绑定在控件上的自定义联动刷新函数//e.component_options.dataSource.reload();
                                if ($.isFunction(e.component._options.refreshFunc)) {
                                    e.component._options.refreshFunc(e.component);
                                } else {
                                    e.component.refresh();
                                }
                                if (e.component._options.parentCmp) e.component._options.parentCmp.refresh();//刷新有关系的父级控件
                            }
                        },
                        location: 'after'
                    });
                }
                if (config.editing && config.editing.allowMultipleApi && saveButtonIndex > -1) {
                    // 对saveButton自定义保存操作
                    e.toolbarOptions.items[saveButtonIndex].options.onClick = function (f) {
                        f.event.stopPropagation();
                        var editData = getDataGridEditDataByInstance(e.component);
                        showLoading(function () {
                            if (typeof config.onSaving === 'function') {
                                if (config.editing.allowMultipleNextTickApiAction === true) {
                                    config.onSaving(e.component, editData, multiplaApiNextTickAction);
                                } else {
                                    if (config.onSaving(e.component, editData))
                                        multiplaApiNextTickAction();
                                }
                            } else {
                                multiplaApiNextTickAction();
                            }
                        });

                        function multiplaApiNextTickAction() {
                            var data = {};
                            if (editData.inserts.length) {
                                data.inserts = [];
                                $.each(editData.inserts,
                                    function (i, insert) {
                                        data.inserts.push({
                                            Key: insert.key ? insert.key : 0,
                                            Value: JSON.stringify(insert.data)
                                        });
                                    });
                            }
                            if (editData.updates.length) {
                                data.updates = [];
                                $.each(editData.updates,
                                    function (i, update) {
                                        data.updates.push({
                                            Key: update.key,
                                            Value: JSON.stringify(update.data)
                                        });
                                    });
                            }
                            if (editData.deletes.length) {
                                data.deletes = [];
                                $.each(editData.deletes,
                                    function (i, del) {
                                        data.updates.push({
                                            Key: del.key,
                                            Value: JSON.stringify(del.oldData)
                                        });
                                    });
                            }
                            var editingConfig = e.component.option('editing');
                            var multipleApiUrl = editingConfig.mutipleApiUrl;
                            if ($.isEmptyObject(multipleApiUrl)) {
                                throw '请配置  editing.mutipleApiUrl  才能进行批量操作';
                            }
                            syncPost(multipleApiUrl,
                                { data: data },
                                function (err) { },
                                function (res, result) {
                                    result = res.responseJSON; //发生错误时的responseJSON含有Msg信息
                                    if (result.Status) {
                                        e.component.option('onSavedTime', null);
                                        e.component.cancelEditData();
                                        var onSave = e.component.option('onSaved');
                                        if (typeof onSave === 'function') {
                                            // 添加数据更新时间
                                            onSave(e.component, result);
                                        }
                                        e.component.refresh();
                                    } else {
                                        e.component.option('onSavedTime', new Date());
                                        //result.Msg = result.Msg ? result.Msg : result.Message;
                                        var widgetId = e.component._$element.attr('id');
                                        if (!widgetId) widgetId = e.component._options.gridId;
                                        addGridMessage(widgetId, result.Msg);//msg
                                    }
                                });
                        }
                    }
                }
                onToolbarPreparing(e);
            };
        }
        //#endregion

        //#region 对Command 列进行处理 交给本地化多语言处理
        //var onCellPrepared = config.onCellPrepared ? config.onCellPrepared : function () { };
        //config.onCellPrepared = function(e) {
        //    if (e.cellElement.hasClass('dx-command-edit')) {
        //        var $links = e.cellElement.find('.dx-link');
        //        $links.filter('.dx-link-save').addClass('dx-icon-save').attr('title', '保存').text('');
        //        $links.filter('.dx-link-edit').addClass('dx-icon-edit').attr('title', '编辑').text('');
        //        $links.filter('.dx-link-delete').addClass('dx-icon-close').attr('title', '删除').text('');
        //        $links.filter('.dx-link-add').addClass('dx-icon-add').attr('title', '添加').text('');
        //        $links.filter('.dx-link-undelete').addClass('dx-icon-revert').attr('title', '撤销').text('');
        //        $links.filter('.dx-link-cancel').addClass('dx-icon-revert').attr('title', '撤销').text('');
        //    }
        //    onCellPrepared(e);
        //};
        //#endregion

        //#region 扩展onEditorPrepared 对所有的editor 都添加row-index属性
        var onEditorPrepared = config.onEditorPrepared ? config.onEditorPrepared : function (e) { };
        config.onEditorPrepared = function (e) {
            if (e.row && e.row.isEditing) {
                $(e.editorElement).attr('row-index', e.row.rowIndex).attr('outer-widget-id', e.element.attr('id'));
            }
            onEditorPrepared(e);
        };
        //#endregion

        //#region 在每一行的tr标签上面都添加一个row-index 属性
        var onRowPrepared = config.onRowPrepared ? config.onRowPrepared : function (e) { };
        config.onRowPrepared = function (e) {
            if (e.rowType === 'data') {
                setTimeout(function () {
                    var tag = $(e.rowElement).parent();
                    var children = tag.find('>tr.dx-data-row');
                    for (var i = 0; i < children.length; i++) {
                        children.eq(i).attr('row-index', i);
                    }
                    if (!tag.attr('outer-widget-id')) {
                        tag.attr('outer-widget-id', e.element.attr('id'));
                    }
                }, 50);
            }
            onRowPrepared(e);
        };
        //#endregion

        //#region 常用crud事件扩展 添加beginCustomLoading.. 与endCustomLoading

        //#region onRowUpdating
        var onRowUpdating = config.onRowUpdating ? config.onRowUpdating : function (e) { };
        config.onRowUpdating = function (e) {
            e.component.beginCustomLoading();//'Loading...'
            onRowUpdating(e);
        };
        //#endregion

        //#region onRowUpdated
        var onRowUpdated = config.onRowUpdated ? config.onRowUpdated : function (e) { };
        config.onRowUpdated = function (e) {
            onRowUpdated(e);
            e.component.endCustomLoading();
        };
        //#endregion

        //#region onDataErrorOccurred
        var onDataErrorOccurred = config.onDataErrorOccurred ? config.onDataErrorOccurred : function (e) { };
        config.onDataErrorOccurred = function (e) {
            onDataErrorOccurred(e);
            e.component.endCustomLoading();
        };
        //#endregion

        //#region onRowInserting
        var onRowInserting = config.onRowInserting ? config.onRowInserting : function (e) { };
        config.onRowInserting = function (e) {
            e.component.beginCustomLoading(); //'Loading...'
            onRowInserting(e);
        };
        //#endregion

        //#region onRowInserted
        var onRowInserted = config.onRowInserted ? config.onRowInserted : function (e) { };
        config.onRowInserted = function (e) {
            onRowInserted(e);
            e.component.endCustomLoading();
        };
        //#endregion

        //#region onRowRemoving
        var onRowRemoving = config.onRowRemoving ? config.onRowRemoving : function (e) { };
        config.onRowRemoving = function (e) {
            e.component.beginCustomLoading(); //'Loading...'
            onRowRemoving(e);
        };
        //#endregion

        //#region onRowRemoved
        var onRowRemoved = config.onRowRemoved ? config.onRowRemoved : function (e) { };
        config.onRowRemoved = function (e) {
            e.component.endCustomLoading();
            onRowRemoved(e);
        };
        //#endregion

        //#endregion

        //#region 执行number字段小数位数控制
        config = gridSugarHelper.formatNumberValidating(config);
        //#endregion
    };

    // #endregion

})();

// #endregion

//#region 获取dxPivotGrid dxForm dxSelectBox dxLookup dxTagBox dxDateBox dxTreeView dxPopup等控件实例， 全局函数loading，确认框提示消息等默认扩展 

(function () {

    //#region 根据selector 获取dxPivotGrid instance/component对象 selector:jquerySelector 
    window.getPivotGridInstance = function (selector) {
        var instance = getJqueryObj(selector);
        if (instance.length) {
            instance = instance.dxPivotGrid('instance');
            instance._options.component = instance;
        }
        return instance;
    };
    // #endregion

    //#region dxPivotGrid 编辑与默认扩展

    //#region pivotGridSugar对dxPivotGrid进行默认扩展 (含编辑) config:pivotGrid配置

    window.pivotGridSugar = function (config) {
        var defaultConfig = {
            editing: {
                enabled: false,
                columns: [],
                rows: [],
                editData: [],
                mode: 'cell',
                editorMinValue: 0,
                checkEditable: function (e) { return true; }
            },
            export: config.export ? config.export : { enabled: true }
        };
        config = $.extend(true, defaultConfig, config);
        config.editing.editData = [];//缓存编辑数据 默认为空数组 扩展pivotGrid编辑
        // 对数据源的配置进行搜索
        var dataSource = config.dataSource;
        if (!$.isFunction(dataSource.fields)) {
            $.each(dataSource.fields, function (fIndex, field) {
                if (field.area === 'data')
                    config.editing.dataColumn = field;
                else if (field.area === 'column')
                    config.editing.columns.push(field);
                else if (field.area === 'row')
                    config.editing.rows.push(field);
            });
        }
        if (config.editing.dataColumn && !config.editing.dataColumn.editorOptions)
            config.editing.dataColumn.editorOptions = {};

        if (config.editing.enabled) {
            //设置哪些cell为元数据cell
            (function () {
                //缓存onCellPrepared
                var onCellPrepared = config.onCellPrepared ? config.onCellPrepared : function (e) { };
                config.onCellPrepared = function (e) {
                    if (e.area === 'data') {
                        if ($(e.cellElement).addClass('dx-meta-data').hasClass('dx-total')) return;
                        if (config.editing.mode === 'cell') return;
                        var editData = getPivotEditData(e.component, e.cell);
                        if (!config.editing.checkEditable(editData)) return;//目前checkEditable是个function直接返回的true
                        if (editData) {
                            var data = getPivotMetaData(e.component, e.cell);
                            if (!data) return;

                            //throw '数据源里面不存在此数据或部分配置不规范';
                            // 初始化组件，并配置OnBlur事件，如果没有进行修改，是不展示为修改状态
                            // 如果有修改，便讲数据添加到eiditingData里面去
                            // 如果某cell是已经修改后的，在后面进行初始化的时候还需要对状态进行还原

                            var instance = e.component;
                            if (e.cellElement.hasClass('dx-focused')) return;
                            if ($.isFunction(instance._options.onEditorPreparing)) {
                                var cancelConfig = {
                                    data: data.data,
                                    cancel: false
                                }
                                instance._options.onEditorPreparing(cancelConfig);
                                if (cancelConfig.cancel) {
                                    return;
                                }
                            }
                            var oldHtml = e.cellElement.html();
                            var option = instance._options.editing.dataColumn.editorOptions;

                            var dataFieldString = instance._options.editing.dataColumn.dataField;
                            option.oldData = data.data[dataFieldString];
                            var cellEditData = getPivotEditData(e.component, e.cell);
                            if (cellEditData)
                                option.value = cellEditData[dataFieldString];
                            else
                                option.value = option.oldData;

                            option.min = config.editorMinValue;

                            //var cellOnFocusOut = option.onFocusOut ? option.onFocusOut : function(e) {};
                            (function () {
                                option.onFocusOut = function (arg) {
                                    var editData = instance._options.editing.editData;
                                    var fieldsValue = getCellFieldsValue(instance, e.cell);
                                    var cellEditData = getDxPivotGridDataSourceData(editData, fieldsValue);

                                    var inputValue = arg.element.find('input.dx-texteditor-input').eq(0).val();
                                    inputValue = parseFloat(inputValue);
                                    i = editData.indexOf(cellEditData);
                                    if (inputValue == data.data[dataFieldString]) {
                                        //开始清除cell的元素和数据
                                        if (editData.length > 0 && cellEditData) {
                                            instance._options.editing.editData.splice(i, 1);
                                        }
                                        e.cellElement.html(oldHtml).removeClass('dx-editor-cell')
                                            .removeClass('dx-focused').data('data', null);
                                    } else {

                                        if (editData.length > 0 && i > -1) {
                                            instance._options.editing.editData[i][dataFieldString] = inputValue;
                                        } else {
                                            var cloneCellData = cloneData(data.data);
                                            cloneCellData[dataFieldString] = inputValue;
                                            instance._options.editing.editData.push(cloneCellData);
                                        }
                                    }
                                }
                            }());
                            var textBoxId = dataFieldString + '_' + data.Id;
                            var textBox = $('<div id="' + textBoxId + '" > ').dxNumberBox(option);
                            var outline = $('<div class="dx-highlight-outline dx-pointer-events-target">')
                                .html(textBox);
                            e.cellElement.addClass('dx-editor-cell').addClass('dx-focused').html(outline);

                            //选中组件
                            // $('#' + textBoxId).dxNumberBox('instance').focus();
                            //重现组件
                            //var dataFieldString = e.component._options.editing.dataColumn.dataField;
                            //var option = e.component._options.editing.dataColumn.editorOptions;
                            //option.value = editData[dataFieldString];
                            //var textBoxId = dataFieldString + '_' + editData.Id;
                            //var textBox = $('<div id="' + textBoxId + '" > ').dxNumberBox(option);
                            //var outline = $('<div class="dx-highlight-outline dx-pointer-events-target">')
                            //    .html(textBox);
                            //e.cellElement.addClass('dx-editor-cell').addClass('dx-focused').html(outline);
                            //e.component.updateDimensions();
                        }
                    }
                    onCellPrepared(e);
                };
            })();

            // 当每个cell 开始点击的时候，还是需要对其进行组件初始化
            (function () {
                var onCellClick = config.onCellClick ? config.onCellClick : function (e) { };
                config.onCellClick = function (e) {
                    if (e.cellElement.hasClass('dx-meta-data')) {
                        if (e.cellElement.hasClass('dx-total')) return;
                        var data = getPivotMetaData(e.component, e.cell);
                        if (!config.editing.checkEditable(data)) return;
                        if (!data) return;

                        //throw '数据源里面不存在此数据或部分配置不规范';

                        // 初始化组件，并配置OnBlur事件，如果没有进行修改，是不展示为修改状态
                        // 如果有修改，便将数据添加到eiditingData里面去
                        // 如果某cell是已经修改后的，在后面进行初始化的时候还需要对状态进行还原
                        var instance = e.component;
                        if ($.isFunction(config.onEditorPreparing)) {
                            var cancelConfig = {
                                data: data.data,
                                cancel: false
                            }
                            config.onEditorPreparing(cancelConfig);
                            if (cancelConfig.cancel) {
                                return;
                            }
                        }
                        if (e.cellElement.hasClass('dx-focused'))
                            return;
                        var oldHtml = e.cellElement.html();
                        var option = instance._options.editing.dataColumn.editorOptions;

                        var dataFieldString = instance._options.editing.dataColumn.dataField;
                        option.oldData = data.data[dataFieldString];
                        var cellEditData = getPivotEditData(e.component, e.cell);
                        if (cellEditData)
                            option.value = cellEditData[dataFieldString];
                        else
                            option.value = option.oldData;

                        option.min = config.editorMinValue;

                        //var cellOnFocusOut = option.onFocusOut ? option.onFocusOut : function (e) { };
                        (function () {
                            option.onFocusOut = function (arg) {
                                var editData = instance._options.editing.editData;
                                var fieldsValue = getCellFieldsValue(instance, e.cell);
                                var cellEditData = getDxPivotGridDataSourceData(editData, fieldsValue);
                                var inputValue = arg.element.find('input.dx-texteditor-input').eq(0).val();
                                inputValue = parseFloat(inputValue);

                                i = editData.indexOf(cellEditData);
                                if (inputValue == data.data[dataFieldString]) {
                                    //开始清除cell的元素和数据
                                    if (editData.length > 0 && cellEditData) {
                                        instance._options.editing.editData.splice(i, 1);
                                    }
                                    e.cellElement.html(oldHtml).removeClass('dx-editor-cell').removeClass('dx-focused')
                                        .data('data', null);
                                    //e.element.find('.dx-datagrid-header-panel .dx-button[aria-label=save]').dxButton('instance').option('disabled', true);
                                } else {
                                    if (editData.length > 0 && i > -1) {
                                        instance._options.editing.editData[i][dataFieldString] = inputValue;
                                    } else {
                                        var cloneCellData = cloneData(data.data);
                                        cloneCellData[dataFieldString] = inputValue;
                                        instance._options.editing.editData.push(cloneCellData);
                                    }
                                }

                                if (config.editing.mode === 'cell') {
                                    // 返回所有的参数
                                    if (data.data[dataFieldString] !== inputValue) {
                                        if (typeof inputValue === 'number') {
                                            data.data[dataFieldString] = parseFloat(inputValue.toFixed(2));
                                        }
                                        config.onSave(data.data);
                                    }
                                }
                            }
                        }());
                        var textBoxId = dataFieldString + '_' + (data.data ? data.data.Id : data.Id);
                        var textBox = $('<div id="' + textBoxId + '" > ').dxNumberBox(option);
                        var outline = $('<div class="dx-highlight-outline dx-pointer-events-target">').html(textBox);
                        e.cellElement.addClass('dx-editor-cell').addClass('dx-focused').html(outline);
                        e.component.updateDimensions();
                        //选中组件
                        //$('#' + textBoxId).dxNumberBox('instance').focus();
                        e.cellElement.find('.dx-texteditor-container>input').focus();
                    }
                    onCellClick(e);
                }
            })();
        }

        // 设置toolbarItems
        var items = [];
        if (config.editing.enabled) {
            items.push({
                widget: 'dxButton',
                options: {
                    icon: 'save',
                    onClick: function (s) {
                        s.event.stopPropagation();
                        if (typeof config.onSave === 'function') {
                            config.onSave(config.editing.editData);
                        }
                    }
                },
                location: 'after'
            });
        }

        if ($.isFunction(config.onToolbarPreparing)) {
            //自定义的onToolbarPreparing 传入的两个参数
            config.onToolbarPreparing(items, config);
        }


        var onContentReady = config.onContentReady ? config.onContentReady : function (e) { };
        config.onContentReady = function (e) {
            if (e.element.find('.dx-datagrid-header-panel').length > 0) return;
            var tag = e.element.find('>div.dx-pivotgrid-container>table');//当配置了wordWrapEnabled：false 的时候，pivotgrid初始化的那个div上面 是不会有.dx-word-wrap 这个类的。
            var beforeHtml = [], centerHtml = [], afterHtml = [];
            if (tag.length) {
                tag = tag.eq(0);
                var html = '';
                $.each(items, function (i, item) {
                    if (item.widget === 'dxButton') {
                        html = $('<div style="display:inline-block;">').dxButton(item.options);
                    }
                    if (item.widget === 'dxDateBox') {
                        html = $('<div style="display:inline-block;">').dxDateBox(item.options);
                    }
                    if (item.widget === 'dxNumberBox') {
                        html = $('<div style="display:inline-block;">').dxNumberBox(item.options);
                    }
                    if (item.widget === 'dxSelectBox') {
                        html = $('<div style="margin-left:0; display:inline-block;">').dxSelectBox(item.options);
                    }
                    //if (item.widget === 'dxDateBox') {
                    //    //html = $('<div style="margin-left:0; display:inline-block;width:150px;">').dxDateBox(item.options);
                    //    html = $('<div style="display:inline-block;vertical-align: top; width:260px;">').html(item.options.template);
                    //}
                    if (item.widget === 'dxCustomLable') {
                        html = $('<div style="display:inline-block;vertical-align: top;">').html(item.options.template);
                    }
                    if (item.widget === 'dxCustomHtml') {
                        html = $('<div style="display:inline-block;vertical-align: top; width:260px;">').html(item.options.template);
                    }
                    if (item.location === 'after') {
                        afterHtml.push(html);
                    }
                    if (item.location === 'before') {
                        beforeHtml.push(html);
                    }
                    if (item.location === 'center') {
                        centerHtml.push(html);
                    }
                });
                var panel = $('<div class="dx-datagrid-header-panel"><div class="dx-toolbar dx-widget dx-visibility-change-handler dx-collection"><div class="dx-toolbar-items-container"></div></div></div>');
                var before = $('<div  class="dx-toolbar-before" onclick="void(0)"></div>');
                $.each(beforeHtml, function (i, htmlb) {
                    before.append(htmlb);
                });
                var center = $('<div  class="dx-toolbar-center" onclick="void(0)"></div>');
                $.each(centerHtml, function (i, htmlc) {
                    center.append(htmlc);
                });
                var after = $('<div  class="dx-toolbar-after" onclick="void(0)"></div>');
                $.each(afterHtml, function (i, htmla) {
                    after.append(htmla);
                });
                //var row=$('<div class='row'>').append(before).append(center).append(after);
                panel.append(before).append(center).append(after);
                panel.insertBefore(tag);
            }

            ////打印 排除打印
            //if (typeof window.checkPrintCurrentPage == 'function') window.checkPrintCurrentPage();
            onContentReady(e);
        }
        return config;
    }

    // #endregion

    //#region dxPivotGrid getCellFieldsValue获取单元格的所有字段
    window.getCellFieldsValue = function (component, cell) {
        var editing = component.option('editing');
        var columns = editing.columns,
            rows = editing.rows;
        if (!cell) return null;

        if (columns.length !== cell.columnPath.length || rows.length !== cell.rowPath.length)
            return null;
        var fieldsValue = [];
        $.each(columns,
            function (cIndex, column) {
                fieldsValue.push({ dataField: column.dataField, value: cell.columnPath[cIndex] });
            });
        $.each(rows,
            function (rIndex, row) {
                fieldsValue.push({ dataField: row.dataField, value: cell.rowPath[rIndex] });
            });

        return fieldsValue;
    };
    //#endregion

    //#region dxPivotGrid根据行字段值检查来获取数据源里的的行对象值
    window.getDxPivotGridDataSourceData = function (dataSource, fieldsValue) {
        var currentRow = {};
        if (!dataSource) dataSource = [];
        $.each(dataSource, function (i, row) {
            var check = true;//默认true
            $.each(fieldsValue, function (j, field) {
                if (row.hasOwnProperty(field.dataField)) {
                    if (row[field.dataField] !== field.value) {
                        check = false;//只要有一个字段不等，check就为假
                    }
                }
            });
            if (check) {//这行数据全部fieldsValue中的字段值都相等 check就保持为true
                currentRow = row;
                return false;
            } else {
                return true;
            }
        });
        return currentRow;
    };
    //#endregion

    //#region  dxPivotGrid 获取Cell的元数据
    window.getPivotMetaData = function (component, cell) {
        var fieldsValue = getCellFieldsValue(component, cell);
        if (!fieldsValue) return null;
        var options = component._options;
        // 开始对数据进行搜索
        //首先应该搜索cacheData
        var cacheData = options.editing.cacheData;
        var dataFromCache = true;
        var data = getDxPivotGridDataSourceData(cacheData, fieldsValue);
        if (!data) {
            data = getDxPivotGridDataSourceData(options.dataSource.store, fieldsValue);
            if (!data) return data; // 如果连DataSource里面都没有数据，那就只有return了
            else dataFromCache = false;
        }
        //var editing = component.option('editing');
        //var cellCacheData = editing.cacheData ? editing.cacheData : [];
        //判断是否已有缓存数据,如果不是则网cache里面插入数据
        if (!dataFromCache) {
            if (!options.editing.cacheData) options.editing.cacheData = [];
            options.editing.cacheData.push(data);
        }
        return { fromCache: dataFromCache, data: data };
    };
    //#endregion

    //#region dxPivotGrid 获取Cell的编辑数据
    window.getPivotEditData = function (component, cell) {
        var fieldsValue = getCellFieldsValue(component, cell);
        if (!fieldsValue) return fieldsValue;

        var options = component._options;
        var editData = options.editing.editData;
        var data = getDxPivotGridDataSourceData(editData, fieldsValue);
        return data;
    };
    //#endregion

    //#region 点击pivotGrid行/列数据时 显示数据来源：drillDownDataSource 准备使用dataGrid的方式弹出行/列数据应
    //window.showPivotGridSourcePopup = function(items, popupTitle, pivotGridCellElement, pivotGridComponent) {
    //    //准备使用datagrid的方式弹出行/列数据应
    //    var allowUpdating = false, dataSource = [], columns = [];
    //    if (!$(pivotGridCellElement[0]).hasClass('dx-total') && items && items.length === 1) {
    //        dataSource = DevExpress.data.AspNet.createStore({
    //            key: 'id',
    //            loadUrl: 'url?id=' + items[0].Id,
    //            updateUrl: 'updateUrl',
    //            onLoaded: function(e) {}
    //        });
    //        allowUpdating = true; //如果不是汇总列可以编辑数据源
    //        columns = [];
    //    } else {
    //        dataSource = items; //drillDownDataSource;
    //        columns = [];
    //    }
    //    //var wh = getAdvisableWidthAndHeight();
    //    var gridConfig = dxConfig.grid([]);
    //    gridConfig.columns = columns;
    //    gridConfig.columnFixing = { enabled: true };
    //    gridConfig.editing = {
    //        mode: 'cell',
    //        allowAdding: false,
    //        allowDeleting: false,
    //        allowUpdating: allowUpdating
    //    };
    //    gridConfig.paging.pageSize = 5;
    //    gridConfig.pager.allowedPageSizes = [5];
    //    gridConfig.maxHeight = 300;
    //    gridConfig.showLine = false;
    //    gridConfig.onRowUpdated = function(e) {
    //        //更新了年度数据需要主表刷新显示
    //        //initCorporateIncomeTaxCashflowGrid();//刷新主Grid数据
    //        //pivotGridComponent.option('dataSource').load();
    //        //pivotGridComponent.option('dataSource').store.load().done(function(res) {
    //        //    //pivotGridComponent.option('dataSource',res);
    //        //});
    //    };
    //    E('pivot-grid-popup').dxPopup({
    //        title: popupTitle,
    //        height: 400,
    //        isUpdated: false,
    //        contentTemplate: function(contentElement) {
    //            $('<div />')
    //                .addClass('drill-down')
    //                .dxDataGrid(gridConfig)
    //                .appendTo(contentElement);
    //        },
    //        onShowing: function() {
    //            $('.drill-down')
    //                .dxDataGrid('instance')
    //                .option('dataSource', dataSource);
    //        },
    //        onHiding: function(e) {
    //            //debugger;
    //        }
    //    }).dxPopup('instance').show();
    //};
    // #endregion

    // #endregion

    // #region dxForm 控件获取 selector:jquerySelector 

    window.getFormData = function (selector) {
        var component = getFormInstance(selector);
        return component && component.option() ? component.option().formData : null;
    };

    window.getFormInstance = function (selector) {
        var component = getJqueryObj(selector);
        if (component.length) {
            component = component.dxForm('instance');
            component._options.component = instance;
        }
        return component && component.option() ? component : null;
    };
    // #endregion

    // #region dxForm表单时间范围bindFormDateRanger控件初始化 参数示例： obj={formId:'dx-form-container-example', startField:'startDate', endField:'endDate'}
    window.bindFormDateRanger = function (obj, component) {
        if (!obj.startField) {
            obj.startField = 'startDate';
            obj.endField = 'endDate';
        }
        var formInstance = getFormInstance(obj.formId);
        var startInstance = formInstance._editorInstancesByField[obj.startField],
            endInstance = formInstance._editorInstancesByField[obj.endField],
            startValue = T(startInstance.option().value),
            endValue = T(endInstance.option().value);
        if (endValue) startInstance.option('max', endValue);
        if (startValue) endInstance.option('min', startValue);
    };

    // #endregion

    // #region 获取dxSelectBox 实例 获取设置选中值 数据联动 selector:jquerySelector 

    window.getSelectBoxInstance = function (selector) {
        var component = getJqueryObj(selector);
        if (component.length) {
            component = component.dxSelectBox('instance');
            component._options.component = instance;
        }
        return component && component.option() ? component : null;
    };

    window.getSelectBoxValue = function (selector) {
        var instance = getSelectBoxInstance(selector);
        if ($.isEmptyObject(instance)) return null;
        return instance.option().value;
    };

    //selector:jquerySelector(id/class,此处一般要使用id)  value:1
    window.setSelectBoxValue = function (selector, value) {
        var instance = getSelectBoxInstance(selector);
        var option = instance.option();
        option.value = value;
        $('#' + selector).dxSelectBox(option);//因该有刷新数据源的方法替换
    };

    //此方法为提供联动下拉框DxSelectBox查询业务所用 obj={targetId:'targetId', url:'/host/controller/get?id=1'}
    window.refreshDxSelectBoxByUrl = function (obj) {
        getJqueryObj(obj.targetId).dxSelectBox(dxConfig.selectBox(obj.url));
    };
    //obj={sourceId:'sourceId', targetId:'targetId', url:'/host/controller/get?id='}
    window.cascadeDxSelectBox = function (obj) {
        var instance = getSelectBoxInstance(obj.sourceId);
        if (!$.isEmptyObject(instance))
            instance.option().onSelectionChanged = function (e) {
                var value = e.component.option().value;
                if (value) refreshDxSelectBoxByUrl(obj.targetId, obj.url + value);
            };
    };
    //obj={sourceInstance:{}, targetInstance:{}, url:'/host/controller/get?id='}
    window.cascadeDxSelectBoxByInstance = function (obj) {
        obj.sourceInstance.option('onSelectionChanged',
            function (e) {
                var value = e.component.option().value;
                if (!$.isEmptyObject(value)) {
                    obj.targetInstance.option('value', null);
                    obj.targetInstance.option('dataSource', syncGet(url + value));
                }
            });
    };


    // #endregion

    // #region 获取Lookup实例 selector:jquerySelector 

    window.getLookupInstance = function (selector) {
        var component = getJqueryObj(selector);
        if (component.length) {
            component = component.dxLookup('instance');
            component._options.component = instance;
        }
        return component && component.option() ? component : null;
    };
    // selector:jquerySelector 
    window.getLookupValue = function (selector) {
        var instance = getLookupInstance(selector);
        if ($.isEmptyObject(instance)) return null;
        return instance.option().value;
    };

    // #endregion

    // #region 获取TagBox实例 selector:jquerySelector 

    window.getTagBoxInstance = function (selector) {
        var component = getJqueryObj(selector);
        if (component.length) {
            component = component.dxTagBox('instance');
            component._options.component = instance;
        }
        return component && component.option() ? component : null;
    };

    window.getDxTagBoxValue = function (selector) {
        var instance = getTagBoxInstance(selector);
        if (instance && instance.option()) {
            return instance.option().value;
        }
        return instance;
    };

    window.setDxTagBoxValue = function (selector, value) {
        var instance = getTagBoxInstance(selector);
        debugger;
        //如果设置value不成功？可以直接使用以下注释方法替换
        instance.beginUpdate();
        instance.option('value', value);
        instance.endUpdate();
        ////refresh widget
        //var option = instance.option();
        //option.value = value;
        //getJqueryObj(selector).dxTagBox(option);
    };

    // #endregion

    // #region 获取DateBox

    window.getDateBoxText = function (selector) {
        var dateBox = getJqueryObj(selector).dxDateBox('instance');
        if ($.isEmptyObject(dateBox))
            return -1;
        return dateBox.option().text;
    };
    // #endregion

    // #region 获取TreeView selector:jquerySelector 

    window.getDxTreeViewInstance = function (selector) {
        var tag = getJqueryObj(selector);
        var instance = tag.length ? tag.dxTreeView('instance') : null;
        return $.isEmptyObject(instance) ? null : instance;
    };

    window.getDxTreeViewData = function (selector) {
        var instance = getDxTreeViewInstance(selector);
        return $.isEmptyObject(instance) ? null : instance.option().dataSource;
    };

    window.getDxTreeViewSelectedItemData = function (selector) {
        var data = getDxTreeViewData(selector);
        if ($.isEmptyObject(data)) return null;
        var selectedItems = [];
        $.each(data, function (rIndex, row) { if (row.Selected) selectedItems.push(row); });
        return selectedItems;
    };

    window.getDxTreeViewClickDataId = function (selector) {
        var tag = getJqueryObj(selector);
        var dataId = tag && tag.length
            ? parseInt(tag.find('.dx-state-selected .dx-treeview-item .dx-item-content').eq(0).attr('data-dbid'))
            : null;
        return dataId;
    };

    window.getDxTreeViewClickData = function (selector) {
        var data = getDxTreeViewData(selector);
        if ($.isEmptyObject(data)) return null;
        var selectedId = getDxTreeViewClickDataId(selector);
        for (var i = 0; i < data.length; i++) {
            if (data[i].dbId === selectedId) return data[i];
        }
        return null;
    };

    window.getDxTreeViewConfig = function (config) {
        var internalConfig = dxConfig.treeView();
        config = $.extend(true, internalConfig, config);
        config.itemTemplate = function (data, index, ele) {
            var displayHtml = data.name;
            // showSelectBox 
            $(ele).attr('id', 'dx-treeview-item-' + data.dbId).attr('data-dbid', data.dbId); //设置组件item id
            if (!data.showSelectBox) {
                //$(ele).parent().css('padding-left', '6px').parent().find('.dx-checkbox').eq(0).css('display', 'none');
                $(ele).parent().parent().find('.dx-checkbox').eq(0).css('display', 'none');
            }
            //展示的方向
            if (data.displayDirection === 'x') {
                $(ele).parent().parent().css('width', 'auto').css({ 'float': 'left', 'margin-left': '10px' });
            }
            //展示备注信息
            if (data.remark && data.remark.length) {
                $(ele).attr('title', data.remark);
            }
            //展示icon
            if (data.typeIcon && data.typeIcon.length) {
                displayHtml = '<span data-dbId="' +
                    data.dbId + 'data-id="' +
                    data.id + '" > <i class="fa fa-' +
                    data.typeIcon + '" > ' +
                    data.name + '</i></span>';
            }
            if (data.disabled) {
                ele.parent().parent().find('>div.dx-state-disabled>.dx-checkbox-container>span.dx-checkbox-icon')
                    .addClass('dx-icon-close').css('line-height', '22px').css('text-align', 'center');
            }
            return displayHtml;
        };

        var itemClickFunc = $.isFunction(config.onItemClick) ? config.onItemClick : function () { };

        config.onItemClick = function (e) {
            itemClickFunc(e);
            e.component.selectItem(e.itemElement);
            $(e.element).attr('data-selectedId', e.itemData.dbId);
        };
        var selectContextMenu = $.isFunction(config.selectContextMenu) ? config.selectContextMenu : function (menu) { return menu };

        if (config.selectionMode === 'multiRecursive') {
            //将重新设置点击单元格的选中逻辑
            config.selectionMode = 'multiple';
            config.selectNodesRecursive = false;
            config.onItemSelectionChanged = function (e) {
                //判断是否有父级
                var node = $(e.itemElement).parent();
                if (e.itemData.selected) {
                    if ($.isEmptyObject($(e.element).attr('data-currentClickId')))
                        $(e.element).attr('data-currentClickId', e.itemData.dbId);
                    //将父级节点全部选择上
                    if (node.parent().parent()[0].tagName.toLowerCase() === 'li') {
                        var tag = node.parent().parent().children();
                        if (!$(tag[0]).hasClass('dx-checkbox-checked')) {
                            e.component.selectItem(tag[1]);
                        }
                    }
                    //将所有子集元素进行选中
                    if ($(e.element).attr('data-currentClickId') === e.itemData.dbId) {
                        var nodes = node.find('.dx-treeview-node-container ').eq(0).find('li');
                        for (var i = 0; i < nodes.length; i++) {
                            if (!$(nodes.eq(i).children()[0]).hasClass('dx-checkbox-checked')) {
                                e.component.selectItem(nodes.eq(i).children()[1]);
                            }
                        }
                        $(e.element).removeAttr('data-currentClickId');
                    }
                } else {
                    //将所有子集元素取消选中
                    var items = node.find('.dx-treeview-node-container ').eq(0).find('li');
                    for (var j = 0; j < items.length; j++) {
                        if ($(items.eq(j).children()[0]).hasClass('dx-checkbox-checked')) {
                            e.component.unselectItem(items.eq(j).children()[1]);
                        }
                    }
                }
            };
        }

        config.onItemContextMenu = function (e) {
            var contextMenu = cloneData(e.component.option().contextMenu);
            contextMenu = selectContextMenu(contextMenu, e);
            if (!($(e.itemElement).parent().attr('data-initContextMenu') === true)) {
                var menuConfig = {
                    dataSource: contextMenu,
                    target: '#dx-treeview-item-' + e.itemData.dbId,
                    width: 200,
                    onItemClick: function (i) {
                        i.itemData.back(e.itemData);
                    }
                };
                var tag = E('dx-treeview-item-container-' + e.itemData.dbId);
                tag.dxContextMenu(menuConfig);
                $(e.itemElement).parent().attr('data-initContextMenu', true);
            }
            //selectElement
            e.component.selectItem(e.itemElement);
            $(e.element).attr('data-selectedId', e.itemData.dbId);
        }
        return config;
    };

    window.getDxTreeViewHtml = function (config, selector) {
        config = getDxTreeViewConfig(config);
        return E(selector).dxTreeView(config);
    };

    window.showDxTreeViewPopup = function (config, widgetId, popupId, popupConfig, saveBackFunc) {
        var popConf = dxConfig.popup();
        popConf.contentTemplate = function () {
            return getDxTreeViewHtml(config);
        }
        $.extend(true, popConf, popupConfig);
        showPopupWithButtons(popConf, popupId, saveBackFunc);
    };

    window.getDxDropDownBoxConfig = function (dropDownBoxConfig, widgetConfig, type) {
        if (!type) type = 'DataGrid';
        var widgetId = widgetConfig.eleId;
        if (!widgetId) widgetId = type + 'Container';
        dropDownBoxConfig.contentTemplate = function (e) {
            widgetConfig.dataSource = e.component.option('dataSource');
            // 处理选中事件
            var result = null;
            if (type === 'DataGrid' || type === 'TreeList') {
                var onSelectionChanged = widgetConfig.onSelectionChanged ? widgetConfig.onSelectionChanged : function () { };
                widgetConfig.onSelectionChanged = function (selectedItems) {
                    var keys = selectedItems.selectedRowKeys,
                        hasSelection = keys.length;
                    // dataGrid/treeList
                    if (widgetConfig.selection.mode === 'multiple') {
                        e.component.option('value', hasSelection ? keys : null);
                    } else {
                        e.component.option('value', hasSelection ? keys[0] : null);
                    }
                    onSelectionChanged(selectedItems);
                }
                result = type === 'DataGrid'
                    ? E(widgetId).dxDataGrid(widgetConfig)
                    : E(widgetId).dxTreeList(widgetConfig);
            }
            if (type === 'TreeView') {
                widgetConfig.onItemSelectionChanged = function (args) {
                    var keys = args.component.getSelectedNodesKeys();
                    if (args.component._options.selectionMode === 'multiple') {
                        e.component.option('value', keys);
                    } else {
                        e.component.option('value', keys[0]);
                    }
                }

                var scrollView = $('<div>').html(E(widgetId).dxTreeView(widgetConfig));
                result = scrollView.dxScrollView({
                    scrollByContent: true,
                    //scrollByThumb: true,
                    useNative: true
                    //showScrollbar: 'always'
                });
            }
            return result;
        };

        return dropDownBoxConfig;
    };
    // #endregion

    // #region dxPopup hidePopup selector:jquerySelector 

    window.hidePopup = function (selector) {
        var target = getJqueryObj(selector);
        if (target.length) target.dxPopup('instance').hide();
    };

    // #endregion

    // #region confirm showPopupWithButtons / showPopup /loadContentToDxPopup 带确认取消按钮的popup 扩展......
    //扩展系统确认框 
    window.confirm = function () {
        var title, msg;
        var fn, cancelFn, completeFn = function (c) { };
        if (arguments.length === 2) {
            title = '确认框';
            msg = arguments[0];
            fn = arguments[1];
        } else if (arguments.length === 3) {
            title = arguments[0];
            msg = arguments[1];
            fn = arguments[2];
        } else if (arguments.length === 4) {
            title = arguments[0];
            msg = arguments[1];
            fn = arguments[2];
            cancelFn = arguments[3];
        } else if (arguments.length === 5) {
            title = arguments[0];
            msg = arguments[1];
            fn = arguments[2];
            cancelFn = arguments[3];
            completeFn = arguments[4];
        }
        var popup = {};
        popup.title = title;
        //popup.width = 300;
        popup.fullScreen = false;
        popup.closeOnOutsideClick = true;
        popup.resizeEnabled = true;
        popup.dragEnabled = true;
        popup.hint = title;
        popup.shading = false;
        //popup.showPopupTitle = true;
        //popup.showTitle = true;
        //popup.showCloseButton = true;//showTitle=true 才生效
        //popup.position = 'center';//默认center
        //popup.container = ;//可以设定弹窗位置 jquery对象

        //titleTemplate
        //popup.titleTemplate = function(container) {
        //    container.append($('<div class="green" />').html(title));
        //};

        popup.contentTemplate = function (container) {
            container.append($('<div class="" />').html(msg));
            return container;
        };
        //自定义toolbar
        popup.toolbarItems = [
            {
                toolbar: 'bottom',
                widget: 'dxButton',
                options: {
                    text: '确认',
                    elementAttr: {
                        //'class': 'green',
                        'style': 'background-color:#1BBC9B;' //color:white;
                    },
                    icon: 'fa fa-save',
                    hoverStateEnabled: true,
                    onClick: function (s) {
                        s.event.stopPropagation();//18.3版本中有bug 停止冒泡能保证只执行一次
                        if ($.isFunction(fn)) fn(s);
                        if ($.isFunction(completeFn)) completeFn(s);
                        window.__confirmHandler.hide();
                    }
                },
                location: 'after'
            }, {
                toolbar: 'bottom',
                widget: 'dxButton',
                options: {
                    text: '取消',
                    icon: 'fa fa-undo',
                    //elementAttr: {
                    //    'style': 'color:green;'
                    //},
                    hoverStateEnabled: true,
                    onClick: function (s) {
                        s.event.stopPropagation();
                        if ($.isFunction(cancelFn)) cancelFn(s);
                        if ($.isFunction(completeFn)) completeFn(s);
                        window.__confirmHandler.hide();
                    }
                },
                location: 'after'
            }
        ];
        popup = dxConfig.popup(popup);
        window.__confirmHandler = E('popup-container').dxPopup(popup).dxPopup('instance');
        window.__confirmHandler.show();
    };

    //一定要传递newConfig参数且包含popupId 可有toolbarItems
    window.showPopupWithButtons = function (newConfig, okFn, cancelFn, cmpFn) {
        //saveFunc = $.isEmptyObject(saveFunc) ? 'save' : saveFunc;
        // 默认自带 save cancel 删除
        var config = dxConfig.popup(newConfig);
        if (!$.isArray(config.toolbarItems)) {
            //如果config没有toolbarItems 那么默认生成确认和取消按钮
            config.toolbarItems = [
                {
                    toolbar: 'bottom',
                    widget: 'dxButton',
                    options: {
                        text: newConfig.btnOkText ? newConfig.btnOkText:'确认',
                        elementAttr: {
                            //'class': 'green',
                            'style': 'background-color:#1BBC9B;' //color:white;
                        },
                        icon: 'fa fa-save',
                        hoverStateEnabled: true,
                        onClick: function (s) {
                            s.event.stopPropagation();
                            if ($.isFunction(okFn)) okFn(s);
                            if ($.isFunction(cmpFn)) cmpFn(s);
                            hidePopup(newConfig.popupId);
                            //window.__confirmHandler.hide();
                        }
                    },
                    location: 'after'
                },
                {
                    toolbar: 'bottom',
                    widget: 'dxButton',
                    options: {
                        text: '取消',
                        icon: 'fa fa-undo',
                        //elementAttr: {
                        //    'style': 'color:green;'
                        //},
                        hoverStateEnabled: true,
                        onClick: function (s) {
                            s.event.stopPropagation();
                            if ($.isFunction(cancelFn)) cancelFn(s);
                            if ($.isFunction(cmpFn)) cmpFn(s);
                            hidePopup(newConfig.popupId);
                            //window.__confirmHandler.hide();
                        }
                    },
                    location: 'after'
                }
            ];
        }
        showPopup(config);
    };
    //根据config配置显示popup 
    window.showPopup = function (config) {
        config.popupId = config.popupId ? config.popupId : 'popup-container';
        E(config.popupId).dxPopup(config).dxPopup('instance').show();
    };
    //根据obj内容来显示popup obj有url参数
    window.loadContentToDxPopup = function (obj) {
        var config = dxConfig.popup();
        if ($.isEmptyObject(obj.width) && $.isEmptyObject(obj.height)) {
            config.fullScreen = true;
        } else {
            config.width = obj.width;
            config.height = obj.height;
        }
        config.title = obj.title;
        config.contentTemplate = function () {
            var result = syncGet(obj.url);
            return result;
        };
        E('popup-helper-container').dxPopup(config).dxPopup('instance').show();
    }
    //根据url请求数据显示popup
    window.showDxPopupByUrl = function (url, popConfig) {
        var content = syncGet(url);
        showDxPopupByContent(content, popConfig);
    }
    //根据内容显示popup
    window.showDxPopupByContent = function (content, popConfig) {
        var config = dxConfig.popup();
        if (isString(popConfig)) {
            config.title = popConfig;
        } else if (typeof popConfig === 'object') {
            for (key in popConfig) {
                config[key] = popConfig[key];
            }
        }
        config.contentTemplate = function () {
            return content;
        };
        config.pupupId = 'popup-common-container';
        showPopup(config);
    }

    // #endregion

    // #region dxLoadIndicator dxToast showLoading及系统内置notify消息提醒 

    //showLoading 防ui线程卡死，前端延缓请求后端fn(ajax) 前端线程卡死原因待调查
    window.showLoading = function (func) {
        if (!$('#load-indicator').length) {
            var html = '<div id="load-indicator" style="width:100%; height:100%; z-index:99999; position:absolute; top:0;margin-left:47%;"></div>';
            $('body').append(html);
        }
        var top = getScrollTop() + getClientHeight() * 0.5;
        $('#load-indicator').css('top', top);
        $('#load-indicator').dxLoadIndicator({
            height: 60,
            width: 60,
            visible: true
        });
        if ($.isFunction(func)) { setTimeout(function () { func(); }, 50); }
    };
    //隐藏loading遮罩层 一般放在ajax complete后
    window.hideLoading = function () {
        var tag = $('#load-indicator');
        if (tag.length) {
            var instance = tag.dxLoadIndicator('instance');
            if (instance) {
                instance.option('visible', false);
            }
        }
    };
    //消息提醒底层 根据传入消息内容text，类型type，时间time 使用dxToast控件提示消息
    window.notify = function (text, type, time) {
        if (!time) time = 2000;//提示消息设置默认停留2s
        E('toast-common-container').dxToast({
            message: text,
            displayTime: time,
            type: type,
            width: 300,
            position: {
                my: 'center',
                at: 'center',
                of: window
            }
        }).dxToast('instance').show();
    };
    //消息类型默认success，参数：消息内容text，时间time 
    window.sucNotify = function (text, time) {
        window.notify(text, 'success', time);
    };
    //消息类型默认error，参数：消息内容text，时间time 
    window.errNotify = function (text, time) {
        window.notify(text, 'error', time);
    };
    //消息类型默认warning，参数：消息内容text，时间time 
    window.warNotify = function (text, time) {
        window.notify(text, 'warning', time);
    };

    // #endregion

    // #region ajax公共请求request get post delete link linkpost 等请求扩展 

    //显示数据重复的错误消息 需要在框架设置 id='data-unique-container' 的element 
    window.showUniqueError = function (message) {
        if ($('#data-unique-container').length) {
            var type = 'error';
            var html = $('<div class="alert alert-' + type + ' alert-dismissable" style="margin-bottom:5px;padding:8px 30px 8px 8px;"></div>');
            html.append('<button onclick="removeUniqueError()" type="button" class="close" data-dismiss="alert" aria-hidden="true" style="color:white; opacity:1;" >×</button>');
            html.append('<p>' + message + '</p>');
            $('#data-unique-container').css('margin', '10px 10px 0 10px').html(html);
        }
    };
    //移除数据重复的错误消息
    window.removeUniqueError = function () {
        if ($('#data-unique-container').length) {
            $('#data-unique-container').css('margin', '0px 0px 0 0px').html('');
        }
    };

    window.syncGet = function (url, sucFn, errFn, cmpFn) {
        var data = [];
        $.ajax({
            url: url,
            async: false,
            type: 'get',
            beforeSend: function (e) {
                showLoading();
            },
            success: function (res) {
                data = res;
                if (res && res.dataUniqueError) {
                    showUniqueError(res.msg);//msg
                }
                if ($.isFunction(sucFn)) sucFn(res);//if (sucFn) sucFn(d);
            },
            error: function (e) {
                if ($.isFunction(errFn)) errFn(e);
                //else if (isString(e.responseJSON.message)) errNotify(e.responseJSON.message);//msg
            },
            complete: function (e) {
                hideLoading();
                if ($.isFunction(cmpFn)) cmpFn(e, data);
            }
        });
        return data;
    };

    window.syncPostDefault = function (url, formData, sucFn, errFn, cmpFn) {
        var data = [];
        $.ajax({
            url: url,
            async: false,//必须改为同步之后，data才会赋值
            type: 'post',
            data: formData,
            beforeSend: function (e) {
                showLoading();
            },
            success: function (res) {
                data = res;
                if (res && res.dataUniqueError) {
                    showUniqueError(res.msg);//msg
                }
                if (!res.status && errFn) errFn(res);
                else if ($.isFunction(sucFn)) sucFn(res);//if (sucFn) sucFn(d);
            },
            error: function (e) {
                if ($.isFunction(errFn)) errFn(e);
                //else if (isString(e.responseJSON.message)) errNotify(e.responseJSON.message);//msg
            },
            complete: function (e) {
                hideLoading();
                if ($.isFunction(cmpFn)) cmpFn(e, data);
            }
        });
        return data;
    };

    window.syncPost = function (url, formData, sucFn, errFn, cmpFn) {
        var data = [];
        $.ajax({
            url: url,
            async: false,//必须改为同步之后，data才会赋值
            type: 'post',
            //dataType: 'json',
            //contentType: 'application/json',
            contentType: 'application/json: charset=utf-8',
            data: JSON.stringify(formData),
            //data: formData,
            beforeSend: function (e) {
                showLoading();
            },
            success: function (res) {
                data = res;
                if (res && res.dataUniqueError) {
                    showUniqueError(res.msg);//msg
                }
                if (!res.status && errFn) errFn(res);
                else if ($.isFunction(sucFn)) sucFn(res);//if (sucFn) sucFn(d);
            },
            error: function (e) {
                if ($.isFunction(errFn)) errFn(e);
                //else if (isString(e.responseJSON.message)) errNotify(e.responseJSON.message);//msg
            },
            complete: function (e) {
                hideLoading();
                if ($.isFunction(cmpFn)) cmpFn(e, data);
            }
        });
        return data;
    };

    window.syncPut = function (url, formData, sucFn, errFn, cmpFn) {
        var data = [];
        $.ajax({
            url: url,
            async: false,
            type: 'put',
            data: formData,
            beforeSend: function (e) {
                showLoading();
            },
            success: function (res) {
                data = res;
                if (res && res.dataUniqueError) {
                    showUniqueError(res.msg);//msg
                }
                if (!res.status && errFn) errFn(res);
                else if ($.isFunction(sucFn)) sucFn(res);//if (sucFn) sucFn(d);
            },
            error: function (e) {
                if ($.isFunction(errFn)) errFn(e);
                //else if (isString(e.responseJSON.message)) errNotify(e.responseJSON.message);//msg
            },
            complete: function (e) {
                hideLoading();
                if ($.isFunction(cmpFn)) cmpFn(e, data);
            }
        });
        return data;
    };

    window.syncDelete = function (url, formData, sucFn, errFn, cmpFn) {
        var data = [];
        $.ajax({
            url: url,
            async: false,
            type: 'delete',
            data: formData,
            beforeSend: function (e) {
                showLoading();
            },
            success: function (res) {
                data = res;
                if (res && res.dataUniqueError) {
                    showUniqueError(res.msg);//msg
                }
                if (!res.status && errFn) errFn(res);
                else if ($.isFunction(sucFn)) sucFn(res);//if (sucFn) sucFn(d);
            },
            error: function (e) {
                if ($.isFunction(errFn)) errFn(e);
                else if (isString(e.responseJSON.message)) errNotify(e.responseJSON.message);//msg
            },
            complete: function (e) {
                hideLoading();
                if ($.isFunction(cmpFn)) cmpFn(e, data);
            }
        });
        return data;
    };


    //#endregion

})();

//#endregion

//#region 全局函数 cloneData exist destory E objectStr isString 全局方法
(function () {

    // #region cloneData 返回克隆的obj

    window.cloneData = function (obj) {
        if (isString(obj) || $.isNumeric(obj)) return obj;
        var data = {};
        if ($.isArray(obj)) {
            data = [];
            $.each(obj, function (i, row) {
                data.push(row);
            });
        } else $.extend(true, data, obj);//对象克隆利用extend
        return data;
    };
    //#endregion

    // #region exist判断对象是否存在/不为空
    window.exist = function (obj) {
        return !$.isEmptyObject(obj);
    };
    //#endregion

    //#region destory 根据selector 销毁某jquery 元素
    window.destory = function (selector) {
        if (getJqueryObj(selector).length) {
            getJqueryObj(selector).remove();
        }
    };

    //#endregion

    // #region element 根据id寻找element 返回jquery element对象
    window.E = function (id) {
        var target = getJqueryObj(id);
        if (!target.length) {
            $('body').append('<div id="' + id + '">');
            target = $('#' + id);
        }
        return target;
    };
    //#endregion

    //#region 对象的objectStr方法
    window.objectStr = function (obj) {
        return $.isEmptyObject(obj) ? '' : obj.toString();
    };
    //#endregion

    //#region 判断对象是否是字符串
    window.isString = function (obj) {
        return Object.prototype.toString.call(obj) === '[object String]';
    }
    //#endregion
})();
//#endregion

//#region 数组原型getByKey getFieldValues扩展
//寻找key value对应的 行
Array.prototype.getByKey = function (keyField, value) {
    //默认字段值
    keyField = defaultFmt(keyField, 'Value');
    value = defaultFmt(value, 'Text');
    var currentRow = null;
    $.each(this, function (rIndex, row) {
        if (row.hasOwnProperty(keyField)) {
            if (row[keyField] === value) {
                currentRow = row;
                return false; //找到就跳出循环
            }
        }
        return true;
    });
    return currentRow;
};
//数组中某属性列的所有值 
Array.prototype.getFieldValues = function (field) {
    field = $.isEmptyObject(field) ? 'Id' : field;
    var arr = [];
    $.each(this, function (rIndex, row) {
        if (row.hasOwnProperty(field)) {
            arr.push(row[field]);
        }
    });
    return arr;
};
//#endregion

//#region 数组根据条件获取选中行扩展getArrayDataById getArrayDataByValue
(function () {
    // #region  根据id/value获取数组行对象
    window.getArrayDataById = function (arr, id) {
        var data = {};
        if ($.isArray(arr)) {
            $.each(arr, function (rIndex, row) {
                if (row.Id && row.Id === id) {
                    data = row;
                    return false;//找到就跳出循环
                } else {
                    return true;
                }
            });
        }
        return data;
    };
    //根据value获取数据行对象
    window.getArrayDataByValue = function (arr, value) {
        var data = {};
        if ($.isArray(arr)) {
            $.each(arr, function (rIndex, row) {
                if (row.Value && row.Value === value) {
                    data = row;
                    return false;//找到就跳出循环
                } else {
                    return true;
                }
            });
        }
        return data;
    };

    // #endregion

})();
//#endregion

//#region js时间类型原型格式扩展 Format
Date.prototype.Format = function (fmt) {
    fmt = defaultFmt(fmt, 'yyyy-MM-dd hh:mm');
    var o = {
        'M+': this.getMonth() + 1, //月份 
        'd+': this.getDate(), //日 
        'h+': this.getHours(), //小时 
        'm+': this.getMinutes(), //分 
        's+': this.getSeconds(), //秒 
        'q+': Math.floor((this.getMonth() + 3) / 3), //季度 
        'S': this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + '').substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp('(' + k + ')').test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length === 1) ? (o[k]) : (('00' + o[k]).substr(('' + o[k]).length)));
    return fmt;
};
//#endregion

//#region 日期扩展T defaultFmt formatCSharpTime
(function () {
    // #region 时间格式扩展 根据传入的fmt(format) fmt='yyyy/MM/dd' 格式化日期
    window.T = function (time, fmt, fields) {
        if ($.isEmptyObject(time)) return '';
        // 1:判断浏览器
        // 1.1 兼容/ dd
        fmt = defaultFmt(fmt, 'yyyy-MM-dd hh:mm:ss');
        time = time.toString();
        if (time.indexOf('/Date') === 0) {
            return formatCSharpTime(time, fmt);
        } else {
            // 判断字符串是否只精确化到月份
            if (time.length === 4) time += '/01';
            if (time.length === 7 && time.indexOf('/') === 4) time += '/01';
            var t = new Date(time);
            return t.Format(fmt);
        }
    };
    //返回传入值/默认值
    window.defaultFmt = function (obj, value) {
        obj = $.isEmptyObject(obj) ? value : obj;
        return obj;
    };
    window.formatCSharpTime = function (value, fmt) {
        fmt = defaultFmt(fmt, 'yyyy-MM-dd hh:mm:ss');//默认格式'yyyy-MM-dd hh:mm:ss'
        if ($.isEmptyObject(value)) return '';
        value = new Date(parseInt(value.replace('/Date(', '').replace(')/', ''), 10)).Format(fmt);
        return value;
    };
    //#endregion

    //#region  时间操作函数
    window.addDate = function (date, days, format) {
        if (!days) days = 0;
        date = new Date(T(date, format ? format : 'yyyy/MM/dd hh:mm:ss'));
        date.setDate(date.getDate() + days);
        //return new Date(date.getFullYear(),date.getMonth() + 1,date.getDate());
        return date;
    };
    //#endregion

})();
//#endregion

//#region 与系统相关的全局函数

//#region 获取浏览器可视窗口参数 取滚动窗口 getScrollTop getClientHeight
function getScrollTop() {
    var scrollTop = 0;
    if (document.documentElement && document.documentElement.scrollTop) {
        scrollTop = document.documentElement.scrollTop;
    } else if (document.body) {
        scrollTop = document.body.scrollTop;
    }
    return scrollTop;
}


//取窗口可视范围的高度 
function getClientHeight() {
    var clientHeight = 0;
    if (document.body.clientHeight && document.documentElement.clientHeight) {
        clientHeight = (document.body.clientHeight < document.documentElement.clientHeight)
            ? document.body.clientHeight
            : document.documentElement.clientHeight;
    } else {
        clientHeight = (document.body.clientHeight > document.documentElement.clientHeight)
            ? document.body.clientHeight
            : document.documentElement.clientHeight;
    }
    return clientHeight;
}

//#endregion

//#region 对传入的text单双引号同时互转
function exChangeQuotationMarks(text) {
    text = text.replace(/'/g, '@');
    text = text.replace(/"/g, "%");
    text = text.replace(/@/g, '"');
    text = text.replace(/%/g, "'");
    return text;
}
//#endregion

//#region selector参数兼容有无#和.开头，可以直接传递id或者class或者首字符带或不带匹配符的jquerySelector
function getJqueryObj(selector) {
    return ($(selector).length || selector.indexOf('#') === 0 || selector.indexOf('.') === 0)
        ? $(selector)
        : $('#' + selector).length
            ? $('#' + selector)
            : $('.' + selector);
}
//#endregion

//#region 比较对象是否相等
function compareObj(item1, item2) {
    if (typeof item1 !== typeof item2) return false;
    if ($.isArray(item1)) {
        if (item1.length !== item2.length) return false;
        for (var i = 0; i < item1.length; i++) {
            if (!compareObj(item1[i], item2[i])) return false;
        }
    } else {
        // 如果是函数的话，就不参与比较
        if ($.isFunction(item1) === $.isFunction(item2)) return true;
        if ($.isEmptyObject(item1)) return item1 === item2;
        // 判断内部keys的多少
        if (Object.keys(item1).length !== Object.keys(item2).length) return false;
        for (var key in item1) {
            if (!compareObj(item1[key], item2[key])) return false;
        }
    }
    return true;
}
//#endregion

//#region 根据value获取小数与百分数  自定义格式
//根据传入的value值获取0-1的百分数
function getPercentData(value) {
    if (!value && value !== 0) {
        return { text: '', value: '' };
    }
    var number = parseFloat(value);
    if (isNaN(number)) number = 0;
    number = number < 0 ? 0 : (number > 1 ? 1 : number);
    number = Number((number * 1).toFixed(4));
    var text = (number * 100).toFixed(2);
    return { text: text + '%', value: number };
}

//根据传入value值，获取小数
function getRatioData(value) {
    if (!value && value !== 0) {
        return { text: '', value: '' };
    }
    var number = Number(value);
    if (isNaN(number)) number = 0;
    number = number < 0 ? 0 : number;
    var text = Number((number * 100).toFixed(2));
    number = Number((number * 1).toFixed(4));
    return { text: text + '%', value: number };
}
//#endregion

//GridCell里面创建a标签
function createCellTag(ele, info, func, defaultConfig) {
    var tag = $(document.createElement('a'));
    tag.html(info.text)
        .css({ cursor: 'pointer', color: 'blue' })
        .mouseover(function () {
            $(this).css('color', 'lightskyblue');
        }).mouseout(function () {
            $(this).css('color', 'blue');
        }).click(function (f) {
            if ($.isFunction(func)) func(info, defaultConfig);//defaultConfig 弹出窗口需要的控件config配置
            //一般func里面内容就是弹出一个dxPopover 初始化一个grid或者treeList或者其它控件
            //E('myPopover').dxPopover({
            //    width: 500,
            //    height: 'auto',
            //    target: ele,
            //    contentTemplate: function(contentElement) {
            //        var tree = E('tree').dxTreeView(defaultConfig);
            //        contentElement.append(tree);
            //    }
            //});
            //E('myPopover').dxPopover('instance').show();
        });
    return tag;
}

//#endregion

//#region setProxyDeleteHTML 自动生成代理标签，代理Delete标签。
//e:cellPrepared的函数传入参数  moduleName:模块名称
function setProxyDeleteHTML(e, moduleName) {
    if (e.column.command === 'edit') {
        var $getlinks = e.cellElement.find('.dx-link');
        if ($getlinks.filter('.dx-link-delete').html() === '') {//CellPrepared的时候会检测到html为空
            var deleteHtml = $getlinks.filter('.dx-link-delete')[0].outerHTML;//获取delete标签外层html
            $getlinks.filter('.dx-link-delete').css('display', 'none');//先设置为隐藏，再配置代理的标签
            var parent = $getlinks.filter('.dx-link-delete').parent().append($(deleteHtml));//在parent上插入复制的delete标签
            parent.children().last().on('click', function (s) {//当点击parent的最后一个标签时
                ////s.stopPropagation();
                ////dx控件自带的ui调用
                //var result = DevExpress.ui.dialog.confirm('删除“' + moduleName + '”的数据将会导致相关依赖数据全部被删除（无法恢复） 请确认？', '警告：');
                //result.done(function (dialogResult) {
                //    if (dialogResult) {
                //        $getlinks.filter('.dx-link-delete').click();//只需要点击确定的时候click，点击取消不click。还有，撤销按钮的事件自动将当前删除按钮还原。
                //    }
                //});

                ////全局统一使用扩展的popup弹窗
                //confirm('警告', '删除' + moduleName + '的数据将会导致相关依赖数据全部被删除(无法恢复)，请确认是否继续？', function (t) {
                //    t.event.stopPropagation();
                //    debugger;
                //    $getlinks.filter('.dx-link-delete').click();//只需要点击确定的时候click，点击撤销按钮的事件自动将当前删除按钮还原。
                //    //showLoading(function (u) {
                //    //    $getlinks.filter('.dx-link-delete').click();//只需要点击确定的时候click，点击撤销按钮的事件自动将当前删除按钮还原。
                //    //});
                //});
                $getlinks.filter('.dx-link-delete').click();

            });
        }
    }
}
//#endregion

//#region 验证时间轴上的金额是否与总和金额相等 此方法待测试
/*
 *验证时间轴上的金额是否与总和金额相等
 *@pageTimes    用于保存时间的字段
 *@gridId       datagrid的Id
 *@fieldId      总和字段名称
 *@feildName    月份字段名
 */

function validateTotalAmount(pageTimes, gridId, fieldId, feildName) {
    var instance = getDataGridInstance(gridId);
    var editData = getDataGridEditDataByInstance(instance);
    var totalAmount = 0;
    if (editData.updates.length) {
        if (editData.updates[0].oldData) {
            var oldTotalAmount = editData.updates[0].oldData[fieldId];
            if (pageTimes.length) {
                $.each(pageTimes, function (i, pageTime) {
                    if (pageTime.Children && pageTime.Children.length) {
                        $.each(pageTime.Children, function (j, child) {
                            var monthName = child[feildName];
                            var monthAmount = editData.updates[0].oldData[monthName];
                            if (!editData.updates[0].data.hasOwnProperty(monthName) && monthAmount)
                                totalAmount += monthAmount;
                            else if (editData.updates[0].data[monthName])
                                totalAmount += editData.updates[0].data[monthName];
                        });
                    }
                    //年份的数据
                    var yearAmount = editData.updates[0].oldData[pageTime[feildName]];
                    if (yearAmount) totalAmount += yearAmount;
                });
            }
            //只要误差在可接受范围内就通过 ??
            if ((oldTotalAmount * 2 - totalAmount) < 0.01 && (oldTotalAmount * 2 - totalAmount) > -0.01) {
                return true;
            } else {
                return false;
            }
        }
    }
    return true;
}

//#endregion

// #endregion

