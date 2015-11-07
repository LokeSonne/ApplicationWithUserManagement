var SetPasswords = SetPasswords || {};

SetPasswords.dialog = undefined;
SetPasswords.successCallback = function () { alert("no callback specified"); };
SetPasswords.form = $("#setPasswordEditor form");

$('#lnkSave', SetPasswords.form).on("click", function (e) {
    var formData = SetPasswords.form.serialize();
    var id = $("#Id", SetPasswords.form).val();
    var url = "/UserManagement/SetPassword/" + id;
    $.ajax({
        url: url, 
        type: "post",
        data: formData,
        cache: false,
        success: function (data) {
            if (data.Succeeded === true) {
                SetPasswords.dialog.dialog("close");
                SetPasswords.successCallback();
            }
            else
            {
                $("#validations", SetPasswords.form).empty();
                for (var i = 0; i < data.Errors.length; i++) $("#validations", SetPasswords.form).append(data.Errors[i]);
            }
        },
        error: function (xhr, b, c) {
            alert("Fejl: " + c);
            $(SetPasswords.form).append(xhr.responseText);
        }
    });
});

$('#lnkCancel', SetPasswords.form).on("click", function (e) { SetPasswords.dialog.dialog("close"); });
