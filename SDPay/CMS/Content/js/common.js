window.CommonCtrl = {
    PopupArchive: function (id) {

        $.ajax({
            type: 'GET',
            url: "/AdminTest/PopupArchive?id=" + id,
            data: {},
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                bootbox.dialog({
                    message: data,
                    buttons: {
                        success: {
                            label: "Lưu Lại",
                            className: "btn-success",
                            callback: function () {
                                //window.location.reload();
                            }
                        }
                    }
                });
            }
        });
    },
    PopupManagerPage: function (pageurl) {
        $.ajax({
            type: 'GET',
            url: pageurl,
            data: {},
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                bootbox.dialog({
                    message: data,
                    buttons: {
                        success: {
                            label: "Đóng",
                            className: "btn-success",
                            callback: function () {
                               
                            }
                        }
                    }
                });
            }
        });
    },
    PopupManagerNewsRef: function (imageChoose,datanews) {
        var datanews =$("#" + imageChoose).val()
        $.ajax({
            type: 'GET',
            url: "/AdminNews2/Reference?ids=" + datanews,
            data: {},
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                bootbox.dialog({
                    message: data,
                    buttons: {
                        success: {
                            label: "Lưu Lại",
                            className: "btn-success",
                            callback: function () {
                                var params = "";
                                $("#SelectedNews option").each(function () {
                                    params = params + $(this).val() + ",";
                                });
                                $("#" + imageChoose).val(params);
                            }
                        }
                    }
                });
            }
        });
    },
    PopupManagerImages: function (imageChoose) {
        $.ajax({
            type: 'GET',
            url: "/Admin/PopupManagerImages",
            data: {},
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                bootbox.dialog({
                    message: data,
                    buttons: {
                        success: {
                            label: "Lưu Lại",
                            className: "btn-success",
                            callback: function () {
                                var txtImageUrl = $('#txtImageUrl').val();
                                if (txtImageUrl != null && txtImageUrl != '') {
                                    $("#" + imageChoose).val(txtImageUrl);
                                    if ($("#" + imageChoose.replace('txt', '')).length)
                                    {
                                        $("#" + imageChoose.replace('txt', '')).attr('src', txtImageUrl);
                                    }
                                    
                                }
                            }
                        }
                    }
                });
            }
        });
    },
    ListImages: function (currPage) {
       
        $.ajax({
            type: 'GET',
            url: "/Admin/ListImages",
            data: {
               
                Month: $("#ddlMonth").val(),
                currPage: currPage == null ? 1 : currPage
            },
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#ListImages").html(data);
            }
        });
    },
    BtnDelete: function (name,month) {
        bootbox.confirm("Bạn chắc chắn muốn xóa ảnh này ? ", function (result) {
            if (result == true) {
              
                Utils.Loading();
                $.ajax({
                    type: 'POST',
                    url: '/Admin/FileDelete',
                    data: {
                        month: month,
                        filename: name
                    },
                    success: function (data) {
                        Utils.UnLoading();
                        bootbox.alert('Xóa ảnh thành công');
                        CommonCtrl.ListImages();
                    }
                });
            }
        });
    },
    SetFile: function (src) {
        //$("#srcImage").attr("src", src);
        $("#txtImageUrl").val(src);
    },
    SetImages: function (src) {
        $("#srcImage").attr("src", src);
        $("#txtImageUrl").val(src);
    },
    GetFileInfo: function (src) {
        $("#ImageInfo").attr("src", src);
        $("#txtImageUrl").val(src);
    }
},
window.FileManagerCtr = {

    ListFileManager: function (currPage) {
      
        $.ajax({
            type: 'GET',
            url: '/AdminImages/ListImage',
            data: {

                Month: $("#ddlMonth").val(),
                currPage: currPage == null ? 1 : currPage
            },
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                Utils.UnLoading();
                $("#ListFile").html(data);
            }
        });
    },


    BtnDelete: function (name, month) {
        bootbox.confirm("Bạn chắc chắn muốn xóa ảnh này ? ", function (result) {
            if (result == true) {
                Utils.Loading();
                $.ajax({
                    type: 'POST',
                    url: '/AdminImages/FileDelete',
                    data: {
                        month: month,
                        filename: name
                    },
                    success: function (data) {
                        Utils.UnLoading();
                        bootbox.alert("Xóa ảnh thành công");
                        FileManagerCtr.ListFileManager();
                    }
                });
            }
        });
    },

   
}