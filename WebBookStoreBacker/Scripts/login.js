/// <reference path="jquery-1.10.2.min.js" />

$(function () {
    //重置功能
    $("#btnClear").click(function () {
        $("#txtUserName").val("");
        $("#txtPwd").val("");
    });
    //登录功能
    $("#btnLogin").click(function () {
        var username = $("#txtUserName").val();
        var pwd = $("#txtPwd").val();
        if (username == "" || pwd == "") {
            alert("用户名或密码不能为空");
        }
        else {
            $.post("/User/Login", { "username": username, "pwd": pwd }, function (data) {
                if (data == "登陆成功") {
                    location.href = "../User/Main";
                }
                else {
                    alert("用户名或者密码错误或者没有权限");
                }
            });
        }
    });
});