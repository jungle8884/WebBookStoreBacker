/// <reference path="jquery.jqprint-0.3.js" />
/// <reference path="jquery-1.10.2.min.js" />

$(function () {
    GetInformList();
    $("#win").hide();//默认情况下，隐藏窗体
});

function GetInformList() {
    $("#dg").datagrid({
        url: "/Inform/GetInformList",//后台页面地址
        queryParams: { selKeyWords: $.trim($("#selKeyWords").val()) },//传到后台的参数
        pagination: true,//是否允许分页
        rownumbers: true,//是否显示行号
        singleSelect: false,//是否只选择一行
        pageSize: 15,//每一页默认显示多少条数据
        checkOnSelect: false,//选中某一行的是否复选框是否可以勾上
        pageList: [5, 10, 15, 20, 25],
        columns: [[
            {
                field: "InformId",
                checkbox: true
            },
            {
                field: "InformText",
                title: "通知信息",
                align: "center",
                width: 400
            },
            {
                field: "IsVisible",
                title: "是否显示",
                align: "center",
                width: 100,
                formatter: function (val, row) {
                    var s = "";
                    switch (val) {
                        case 0:
                            s = "隐藏";
                            break;
                        case 1:
                            s = "显示";
                            break;
                    }
                    return s;
                }
            },
            {
                field: "InfoType",
                title: "信息类型",
                align: "center",
                width: 150,
                formatter: function (val, row) {
                    var s = "";
                    switch (val) {
                        case 0:
                            s = "书籍信息";
                            break;
                        case 1:
                            s = "系统信息";
                            break;
                    }
                    return s;
                }
            }
        ]]
    });
}

//查询
function Sel() {
    GetInformList();
}

//显示添加/更新窗体
var editinformid = 0;
function ShowAddEditBox(i) {
    $("#win").show();
    //添加
    if (i == 1) {
        ClearForm();
        $("#btnAdd").show();
        $("#btnEdit").hide();
        $('#win').dialog({
            title: '添加公告',
            width: 400,
            height: 200,
            closed: false,
            cache: false,
            modal: true
        });
    }
    //更新
    if (i == 2) {
        $("#btnAdd").hide();
        $("#btnEdit").show();
        var rows = $("#dg").datagrid("getSelections");//获取到选中行的数据
        if (!rows || rows.length == 0) {
            $.messager.alert("提示", "请选择要修改的数据");
            return;
        }
        if (rows.length > 1) {
            $.messager.alert("提示", "只能够选择一行数据进行编辑");
            return;
        }
        $.each(rows, function (i, n) {
            //n就表示选中行的数据对象
            $("#txtInformText").val(n.InformText);
            $("#txtIsVisible").val(n.IsVisible);
            $("#txtInfoType").val(n.InfoType);
            editinformid = n.InformId;
        });
        $('#win').dialog({
            title: '更新公告',
            width: 400,
            height: 200,
            closed: false,
            cache: false,
            modal: true
        });
    }
}

//重置表单
function ClearForm() {
    $("#fm").form("reset");
}
//关闭窗体
function CloseForm() {
    $("#win").dialog("close");
}

function AddInform() {
    var InformText = $.trim($("#txtInformText").val());
    if (InformText == "") {
        $.messager.alert('警告', '内容不能为空');
    }
    else {
        $("#fm").form("submit", {
            url: "/Inform/AddInform",
            success: function (data) {
                var data = eval('(' + data + ')');//把json格式转成js数组
                if (data.success) {
                    window.location.reload();
                }
                else {
                    $.messager.alert("提示", data.infor);
                }
            }
        }
        )
    };
}

function EditInform() {
    var InformText = $.trim($("#txtInformText").val());
    if (InformText == "") {
        $.messager.alert('警告', '内容不能为空');
    }
    else {
        $("#fm").form("submit", {
            url: "/Inform/UpdateInform/" + editinformid,
            success: function (data) {
                var data = eval('(' + data + ')');//把json格式转成js对象
                if (data.success) {
                    window.location.reload();
                }
                else {
                    $.messager.alert("提示", data.infor);
                }
            }
        }
        )
    };
}

function DelInform() {
    var rows = $("#dg").datagrid("getSelections");//获取到选中行的数据
    if (!rows || rows.length == 0) {
        $.messager.alert("提示", "请选择要修改的数据");
        return;
    }
    var delinformids = "";
    //i是行数，n是这一行的数据对象
    $.each(rows, function (i, n) {
        //若删除多行则用_连接
        if (i == 0) {
            delinformids = n.InformId;
            return;
        }
        else {
            delinformids += "_" + n.InformId;
            return;
        }
    });
    $.messager.confirm("提示", "你确定删除吗？", function (r) {
        if (r) {
            $.post("/Inform/DelInform", { "delinformids": delinformids }, function (data) {
                if (data.success) {
                    window.location.reload();
                }
                else {
                    $.messager.alert("提示", data.infor);
                }
            });
        }
    });
}

//数据导出Excel
function ExportData() {
    $.post("/Inform/GetExcel", null, function (data) {
        $("#aexcel").attr("href", data);//data为文件路径，设置给它的超链接，点击直接下载
        $.messager.alert("提示", "生成成功");
    });
}

//打印
function Print() {
    $(".datagrid").jqprint();
}
