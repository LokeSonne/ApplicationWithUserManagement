var AssignGroups = AssignGroups || {};

AssignGroups.dialog = undefined;
AssignGroups.successCallback = function () { alert("no callback specified"); };
AssignGroups.form = $("#groupsEditor form");

$('#lnkSave', AssignGroups.form).on("click", function (e) {
    var formData = AssignGroups.form.serialize();
    var id = $("#Id, AssignGroups.form").val();
    var url = "/UserManagement/AssignGroups";
    $.ajax({
        url: url, 
        type: "post",
        data: formData,
        cache: false,
        success: function (data) {
            if (data.Succeeded === true) {
                AssignGroups.dialog.dialog("close");
                AssignGroups.successCallback();
            }
            else
            {
                $("#validations", AssignGroups.form).empty();
                for (var i = 0; i < data.Errors.length; i++) $("#validations", AssignGroups.form).append(data.Errors[i]);
            }
        },
        error: function (xhr, b, c) {
            alert("Fejl: " + c);
            $(AssignGroups.form).append(xhr.responseText);
        }
    });
});

$('#lnkCancel', AssignGroups.form).on("click", function (e) { AssignGroups.dialog.dialog("close"); });
