var ActivateUsers = ActivateUsers || {};

ActivateUsers.dialog = undefined;
ActivateUsers.successCallback = function () { alert("No callback specified"); };
ActivateUsers.form = $("#activationEditor form");

$('#lnkAccept', ActivateUsers.form).on("click", function (e) {
    var formData = ActivateUsers.form.serialize();
    var id = $("#Id", ActivateUsers.form).val();
    var url = "/UserManagement/ActivateUser/" + id;
    $.ajax({
        url: url, 
        type: "post",
        data: formData,
        cache: false,
        success: function (data) {
            if (data.Succeeded === true) {
                ActivateUsers.dialog.dialog("close");
                ActivateUsers.successCallback();
            }
            else
            {
                $("#validations", ActivateUsers.form).empty();
                for (var i = 0; i < data.Errors.length; i++) $("#validations", ActivateUsers.form).append(data.Errors[i]);
            }
        },
        error: function (xhr, b, c) {
            alert("Fejl: " + c);
            $(ActivateUsers.form).append(xhr.responseText);
        }
    });
});

$('#lnkReject', ActivateUsers.form).on("click", function (e) {
    var formData = ActivateUsers.form.serialize();
    var id = $("#Id", ActivateUsers.form).val();
    var url = "/UserManagement/RejectUser/" + id;
    $.ajax({
        url: url,
        type: "post",
        data: formData,
        cache: false,
        success: function (data) {
            if (data.Succeeded === true) {
                ActivateUsers.dialog.dialog("close");
                ActivateUsers.successCallback();
            }
            else {
                $("#validations", ActivateUsers.form).empty();
                for (var i = 0; i < data.Errors.length; i++) $("#validations", ActivateUsers.form).append(data.Errors[i]);
            }
        },
        error: function (xhr, b, c) {
            alert("Fejl: " + c);
            $(ActivateUsers.form).append(xhr.responseText);
        }
    });
});



$('#lnkCancel', ActivateUsers.form).on("click", function (e) { ActivateUsers.dialog.dialog("close"); });
