/// <reference path="jquery.jqprint-0.3.js" />
/// <reference path="jquery-1.10.2.min.js" />
$(function () {
    var type = $("#UserType").text();
    //alert(type);
    GetUserList(type);
    $("#win").hide();//默认情况下，隐藏窗体
});

//获取用户信息列表
function GetUserList(type) {
    $("#dg").datagrid({
        url: "/User/GetUserList",//后台页面地址
        queryParams: { sUserName: $.trim($("#selUserName").val()), iType: $.trim($("#UserType").text()) },//传到后台的参数
        pagination: true,//是否允许分页
        rownumbers: true,//是否显示行号
        singleSelect: false,//是否只选择一行
        pageSize: 15,//每一页默认显示多少条数据
        checkOnSelect: false,//选中某一行的是否复选框是否可以勾上
        pageList: [5, 10, 15, 20, 25],
        columns: [[
            {
                field: "UserId",
                checkbox: true
            },
            {
                field: "UserName",
                title: "用户名",
                align: "center",
                width: 100
            },
            {
                field: "Pwd",
                title: "密码",
                align: "center",
                width: 100
            },
            {
                field: "Gender",
                title: "性别",
                align: "center",
                width: 50,
                formatter: function (val, rowData, rowIndex) {
                    var gender = "";
                    if (val == "男") {
                        gender = "男";
                    }
                    else {
                        gender = "女";
                    }
                    return gender;
                }
            },
            {
                field: "Email",
                title: "Email",
                align: "center",
                width: 200
            },
            {
                field: "Tel",
                title: "手机号",
                align: "center",
                width: 100
            },
            {
                field: "QQ",
                title: "QQ",
                align: "center",
                width: 100
            },
            {
                field: "Type",
                title: "用户类型",
                align: "center",
                width: 100,
                formatter: function (val, row) {
                    var s = "";
                    switch (val) {
                        case 0:
                            s = "普通用户";
                            break;
                        case 1:
                            s = "系统管理员";
                            break;
                    }
                    return s;
                }
            },
            {
                field: "CreatedTime",
                title: "创建时间",
                align: "center",
                width: 300,
                formatter: function (val, row) {
                    var str1 = val.indexOf("(")
                    var str2 = val.lastIndexOf(")");
                    var dateValue = val.substring(str1 + 1, str2);
                    var date = new Date(parseInt(dateValue));
                    return date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate() + " " + date.getHours() + ":" + date.getMinutes();
                }
            }
        ]]
    });
}

//查询
function Sel() {
    GetUserList();
}

//显示添加/更新窗体
var edituserid = 0;
function ShowAddEditBox(i) {
    $("#win").show();
    //添加
    if (i == 1) {
        ClearForm();
        $("#btnAdd").show();
        $("#btnEdit").hide();
        $('#win').dialog({
            title: '添加用户',
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
            $("#txtUserName").val(n.UserName);
            $("#txtPwd").val(n.Pwd);
            $("#txtGender").val(n.Gender);
            $("#txtEmail").val(n.Email);
            $("#txtTel").val(n.Tel);
            $("#txtQQ").val(n.QQ);
            $("#txtCreatedTime").val(n.CreatedTime);
            edituserid = n.UserId;
        });
        $('#win').dialog({
            title: '更新用户',
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

//添加用户
function AddUser() {
    var username = $.trim($("#txtUserName").val());
    var pwd = $.trim($("#txtPwd").val());
    if (username == "" || pwd == "") {
        $.messager.alert('警告', '用户名或者密码不能为空');
    }
    else {
        $("#fm").form("submit", {
            url: "/User/AddUser",
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

//更新用户
function EditUser() {
    var username = $.trim($("#txtUserName").val());
    var pwd = $.trim($("#txtPwd").val());
    if (username == "" || pwd == "") {
        $.messager.alert('警告', '用户名或者密码不能为空');
    }
    else {
        $("#fm").form("submit", {
            url: "/User/UpdateUser/" + edituserid,
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

//删除用户（支持批量删除）
function DelUser() {
    var rows = $("#dg").datagrid("getSelections");//获取到选中行的数据
    if (!rows || rows.length == 0) {
        $.messager.alert("提示", "请选择要修改的数据");
        return;
    }
    var deluserids = "";
    //i是行数，n是这一行的数据对象
    $.each(rows, function (i, n) {
        //若删除多行则用_连接
        if (i == 0) {
            deluserids = n.UserId;
            return;
        }
        else {
            deluserids += "_" + n.UserId;
            return;
        }
    });
    $.messager.confirm("提示", "你确定删除吗？", function (r) {
        if (r) {
            $.post("/User/DelUser", { "deluserids": deluserids }, function (data) {
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
    $.post("/User/GetExcel", null, function (data) {
        $("#aexcel").attr("href", data);//data为文件路径，设置给它的超链接，点击直接下载
        $.messager.alert("提示", "生成成功");
    });
}

//打印
function Print() {
    $(".datagrid").jqprint();
}