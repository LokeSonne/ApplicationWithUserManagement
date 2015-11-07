$(document).ready(function () {
    InitTable();
    InitButtons();
    InitDialogs();
});

var table;

function InitTable() {
    table = $('#userGroups').dataTable({
        "language": { "url": "/Scripts/jquery.dataTables.Danish.json" },
        "ajax": '../UserGroupManagement/GetAllUserGroups',
        "processing": true,
        "columnDefs": [
         { "targets": 0, "visible": false },
         { "targets": 2, "render": function (data, type, row) { return row[2] === "True" ? "Ja" : "Nej"; } }
        ]
    });

    $('#userGroups tbody').on('click', 'tr', function () {
        $('#userGroups tbody tr').removeClass("selected");
        $(this).toggleClass('selected');
        UpdateButtons(true);
    });
};

function InitButtons() {
    $("#lnkCreateUserGroup").on("click", function (e) {
        e.preventDefault(); 
        editUserGroup(undefined);
    });

    $("#lnkEditUserGroup").on("click", function (e) {
        e.preventDefault(); 
        var userId = getUserGroupIdOfSelectedUserGroup();
        editUserGroup(userId);
    });

    $("#lnkDeleteUserGroup").on("click", function (e) {
        e.preventDefault();
        $("#dialog-confirm-delete").dialog('open');
    });

    UpdateButtons(false);
}

function getUserGroupIdOfSelectedUserGroup() {
    var row = $("#userGroups tbody tr.selected")[0];
    var position = table.fnGetPosition(row);
    var userId = table.fnGetData(position)[0];
    return userId;
}

function editUserGroup(userId) {
    var url = userId === undefined ? "UserGroupManagement/CreateUserGroup" : "/UserGroupManagement/EditUserGroup/" + userId;
    $.ajax({
        url: url,
        type: 'Get',
        success: function (data) {
            $("#dialog-edit").empty().append(data);
            EditUserGroups.dialog = $("#dialog-edit").dialog("open");
            EditUserGroups.successCallback = updateTable;
        },
        error: function (a, b, c) { alert(c); }
    });
}

function deleteSelectedUserGroup() {
    var userId = getUserGroupIdOfSelectedUserGroup();
    var url = "UserGroupManagement/DeleteUserGroup/" + userId;
    $.ajax({
        url: url,
        type: 'Get',
        success: function (data) {
            updateTable();
        },
        error: function (a, b, c) { alert(c); }
    });
}

function UpdateButtons(userSelected) {
    $("#lnkEditUserGroup, #lnkDeleteUserGroup").toggleClass('disabled', !userSelected);
}

function InitDialogs() {
    var url = "";

    $("#dialog-edit").dialog({
        title: 'Redigér brugergruppe',
        autoOpen: false,
        resizable: false,
        width: 400,
        modal: true,
        draggable: true
    });

    $("#dialog-confirm-delete").dialog({
        autoOpen: false,
        resizable: false,
        height: 170,
        width: 350,
        modal: true,
        draggable: true,
        open: function (event, ui) {
            $(".ui-dialog-titlebar-close").hide();

        },
        buttons: {
            "OK": function () {
                $(this).dialog("close");
                deleteSelectedUserGroup();
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        }
    });

};


function updateTable() {
    table.api().ajax.reload();
    UpdateButtons(false);

}