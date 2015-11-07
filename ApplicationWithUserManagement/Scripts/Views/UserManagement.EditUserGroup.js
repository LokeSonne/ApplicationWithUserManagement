var EditUserGroups = EditUserGroups || {};

EditUserGroups.dialog = undefined;
EditUserGroups.successCallback = function () { alert("no callback specified"); };
EditUserGroups.form = $("#userGroupEditor form");

$('#lnkSave', EditUserGroups.form).on("click", function (e) {
    var formData = EditUserGroups.form.serialize();
    var id = $("#Id", EditUserGroups.form).val();
    var url = "UserGroupManagement/EditUserGroup/" + id;
    $.ajax({
        url: url, 
        type: "post",
        data: formData,
        cache: false,
        success: function (data) {
            if (data.Succeeded === true) {
                EditUserGroups.dialog.dialog("close");
                EditUserGroups.successCallback();
            }
            else {
                $("#validations", EditUserGroups.form).empty();
                if (data.Succeeded === false && data.Errors === undefined) {
                    $("#validations").text("Ingen ændringer at gemme");
                }
                else {
                    for (var i = 0; i < data.Errors.length; i++) $("#validations", EditUserGroups.form).append(data.Errors[i]);
                }
            }
        },
        error: function (xhr, b, c) {
            alert("Fejl: " + c);
            $(EditUserGroups.form).append(xhr.responseText);
        }
    });
});

$('#lnkCancel', EditUserGroups.form).on("click", function (e) { EditUserGroups.dialog.dialog("close"); });
