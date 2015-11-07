var EditUsers = EditUsers || {};

EditUsers.dialog = undefined;
EditUsers.successCallback = function () { alert("no callback specified"); };
EditUsers.form = $("#userEditor form");

$('#lnkSave', EditUsers.form).on("click", function (e) {
    var formData = EditUsers.form.serialize();
    var id = $("#Id", EditUsers.form).val();
    var url = "/UserManagement/EditUser/" + id;
    $.ajax({
        url: url, 
        type: "post",
        data: formData,
        cache: false,
        success: function (data) {
            if (data.Succeeded === true) {
                EditUsers.dialog.dialog("close");
                EditUsers.successCallback();
            }
            else
            {
                $("#validations", EditUsers.form).empty();
                for (var i = 0; i < data.Errors.length; i++) $("#validations", EditUsers.form).append(data.Errors[i]);
            }
        },
        error: function (xhr, b, c) {
            alert("Fejl: " + c);
            $(EditUsers.form).append(xhr.responseText);
        }
    });
});

$('#lnkCancel', EditUsers.form).on("click", function (e) { EditUsers.dialog.dialog("close"); });
