(function () {

    //let create = abp.auth.isGranted('DeviceAnalogRule.Create');
    let update = abp.auth.isGranted('DeviceAnalogRules.Update');
    let remove = abp.auth.isGranted('DeviceAnalogRules.Delete');
    var MF = {
        Init: function () {
            $('#bootstrap_table').bootstrapTable({
                columns: [
                    {
                        field: 'fakeNo',
                        title: '编码',
                        align: 'center',
                        valign: 'middle'
                    },
                    {
                        field: 'deviceName',
                        title: '设备名称',
                        sortable: true,
                        align: 'center',
                        width: '300',
                        valign: 'middle'
                    },
                    {
                        field: 'period',
                        title: '发送频率(s)',
                        sortable: true,
                        align: 'center',
                        width: '300',
                        valign: 'middle'
                    },
                    {
                        field: 'isEnabled',
                        title: '是否启用',
                        align: 'center',
                        valign: 'middle',
                        formatter: function (value, row, index) {
                            var isEnabledButton = "";
                            if (row.isEnabled) {
                                isEnabledButton = '<input value="' + row.id + '" type="checkbox" name="enable-checkbox" checked />';
                            }
                            else {
                                isEnabledButton = '<input value="' + row.id + '" type="checkbox" name="enable-checkbox" />';
                            }
                            return isEnabledButton;
                        }
                    },
                    {
                        field: 'lastSendTime',
                        title: '最后发送',
                        align: 'center',
                        valign: 'middle'
                    },
                    {
                        field: 'remark',
                        title: '备注',
                        align: 'center',
                        valign: 'middle'
                    },
                    {
                        field: 'operate',
                        title: '操作',
                        align: 'center',
                        valign: 'middle',
                        events: {
                            'click .remove': function (e, value, row, index) {
                                swal({
                                    title: "您确定?",
                                    text: "目标删除后将无法恢复!",
                                    type: "warning",
                                    showCancelButton: true,
                                    confirmButtonColor: "#dd6b55",
                                    confirmButtonText: "仍然删除!",
                                    cancelButtonText: "取消删除!",
                                    closeOnConfirm: false,
                                    closeOnCancel: false
                                }, function (isConfirm) {
                                    if (isConfirm) {
                                        //$.post("/deviceAnalogRule-remove", { id: row.id }, function (result) {
                                        //    $('#bootstrap_table').bootstrapTable('remove', {
                                        //        field: 'name',
                                        //        values: [row.name]
                                        //    });
                                        //    swal("成功!", "该目标已被成功删除.", "success");
                                        //    $('#bootstrap_table').bootstrapTable('refresh');
                                        //}, "json").fail(function () {
                                        //    swal("失败", "无法删除该目标 :(", "error");
                                        //});
                                        $.ajax({
                                            type: 'delete',
                                            dataType: 'json',
                                            url: '/api/ops/deviceAnalogRule',
                                            contentType: 'application/x-www-form-urlencoded',
                                            data: { id: row.id },
                                            success: function (d) {
                                                swal("成功!", "该目标已被成功删除.", "success");
                                                $('#bootstrap_table').bootstrapTable('refresh');
                                            },
                                            error: function (d) {
                                                swal("失败", "无法删除该目标", "error");
                                            }
                                        });
                                    } else {
                                        swal("取消", "您取消了该操作 :)", "error");
                                    }
                                });
                            }
                        },
                        formatter: function (value, row, index) {
                            var array = [];
                            var edit = '<a class="btn btn-outline btn-sm btn-success btn-circle edit" title="编辑" target="_blank" href="/deviceAnalogRule-edit/' + row.id + '"><i class="ti-pencil-alt"></i></a>';
                            if (update) {
                                array.push(edit);
                            }

                            var deleteStr = '<button class="btn btn-outline btn-sm btn-danger btn-circle m-l-5 remove" title="删除"><i class="ti-trash"></i></button>';
                            if (remove) {
                                array.push(deleteStr);
                            }
                            return array.join('');
                        }
                    }
                ],

                onLoadSuccess: function () {
                    $('[name="enable-checkbox"]').bootstrapSwitch({
                        onText: "启用",
                        offText: "禁用",
                        onColor: "success",
                        offColor: "danger",
                        size: "small",
                        disabled: !update,
                        onSwitchChange: function (event, state) {
                            var rouleId = this.value;
                            if (state) {
                                $.ajax({
                                    type: 'POST',
                                    url: '/api/ops/deviceAnalogRule/setEnabled',
                                    contentType: 'application/json; charset=utf-8',
                                    data: JSON.stringify({ id: rouleId, isEnabled: state }),
                                    success: function (data) {
                                        if (data) {
                                            swal("启用成功", "", "success");
                                        } else {
                                            swal("启用失败", "", "error");
                                        }
                                    },
                                    error: function (XMLHttpRequest, textStatus) {
                                        console.log(textStatus);
                                    }
                                });
                            } else {
                                $.ajax({
                                    type: 'POST',
                                    url: '/api/ops/deviceAnalogRule/setEnabled',
                                    contentType: 'application/json; charset=utf-8',
                                    data: JSON.stringify({ id: rouleId, isEnabled: state }),
                                    success: function (data) {
                                        if (!data) {
                                            swal("禁用成功", "", "success");
                                        } else {
                                            swal("禁用失败", "", "error");
                                        }
                                    },
                                    error: function (XMLHttpRequest, textStatus) {
                                        console.log(textStatus);
                                    }
                                });
                            }
                        }
                    });
                    //if (!update) {
                    //    //swal("没有更新模拟数据规则的权限，禁止设置规则状态", "", "error");
                    //    $('[name="enable-checkbox"]').bootstrapSwitch('setActive', false);  // true || false
                    //}
                },
                queryParams: function (params) {
                    //params.key = $("#type").val();
                    params.filter = $("#filter").val();
                    params.skipCount = params.pageSize * (params.pageNumber - 1);
                    params.maxResultCount = params.pageSize;

                    return params;
                },
                responseHandler: function (res) {
                    return { rows: res.items, total: res.totalCount };
                }
            });

            $("#filter").change(function () {
                $('#bootstrap_table').bootstrapTable('refresh');
            });
            $("#clear").click(function () {
                $("#filter").val('');
                $("#filter").trigger('change');
            });
        }
    };

    $(function () {
        MF.Init();
    });
}()); 