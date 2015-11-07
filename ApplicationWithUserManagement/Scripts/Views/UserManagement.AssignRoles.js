var AssignRoles = AssignRoles || {};

AssignRoles.dialog = undefined;
AssignRoles.successCallback = function () { alert("no callback specified"); };
AssignRoles.form = $("#rolesEditor form");

$('#lnkSave', AssignRoles.form).on("click", function (e) {
    var formData = AssignRoles.form.serialize();
    var id = $("#Id", AssignRoles.form).val();
    var url = "/UserManagement/AssignRoles";
    $.ajax({
        url: url, 
        type: "post",
        data: formData,
        cache: false,
        success: function (data) {
            if (data.Succeeded === true) {
                AssignRoles.dialog.dialog("close");
                AssignRoles.successCallback();
            }
            else
            {
                $("#validations", AssignRoles.form).empty();
                for (var i = 0; i < data.Errors.length; i++) $("#validations", AssignRoles.form).append(data.Errors[i]);
            }
        },
        error: function (xhr, b, c) {
            alert("Fejl: " + c);
            $(AssignRoles.form).append(xhr.responseText);
        }
    });
});

$('#lnkCancel', AssignRoles.form).on("click", function (e) { AssignRoles.dialog.dialog("close"); });
