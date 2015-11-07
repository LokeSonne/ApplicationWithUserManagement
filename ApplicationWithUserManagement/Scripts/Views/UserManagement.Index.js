//$(document).ready(function () {
    InitTable();
    InitButtons();
    InitDialogs();
//});

var table;

function InitTable() {
    table = $('#users').dataTable({
        "language": { "url": "/Scripts/jquery.dataTables.Danish.json" },
        "ajax": '../UserManagement/GetAllUsers',
        "processing": true,
        "columnDefs": [
         { "targets": 0, "visible": false },
         { "targets": 2, "render": function (data, type, row) { return row[2]==="True" ? "Ja" : "Nej"; } }
        ]
    });

    $('#users tbody').on('click', 'tr', function () {
        $('#users tbody tr').removeClass("selected");
        $(this).toggleClass('selected');
        UpdateButtons(true);
    });
};

function InitButtons() {
    $("#lnkCreateUser").on("click", function (e) {
        e.preventDefault(); 
        editUser(undefined);
    });

    $("#lnkEditUser").on("click", function (e) {
        e.preventDefault(); 
        var userId = getUserIdOfSelectedUser();
        editUser(userId);
    });

    $("#lnkAssignRoles").on("click", function (e) {
        e.preventDefault();
        var userId = getUserIdOfSelectedUser();
        assignRoles(userId);
    });

    $("#lnkAssignGroups").on("click", function (e) {
        e.preventDefault();
        var userId = getUserIdOfSelectedUser();
        assignGroups(userId);
    });

    $("#lnkDeleteUser").on("click", function (e) {
        e.preventDefault();
        $("#dialog-confirm-delete").dialog('open');
    });

    $("#lnkSetPassword").on("click", function (e) {
        e.preventDefault();
        var userId = getUserIdOfSelectedUser();
        setUserPassword(userId);
    });


    UpdateButtons(false);
}

function getUserIdOfSelectedUser() {
    var row = $("#users tbody tr.selected")[0];
    var position = table.fnGetPosition(row);
    var userId = table.fnGetData(position)[0];
    return userId;
}

function editUser(userId) {
    var url = userId === undefined ? "/UserManagement/CreateUser" : "/UserManagement/EditUser/" + userId;
    $.ajax({
        url: url,
        type: 'Get',
        success: function (data) {
            $("#dialog-edit").empty().append(data);
            EditUsers.dialog = $("#dialog-edit").dialog("open");
            EditUsers.successCallback = updateTable;
        },
        error: function (a, b, c) { alert(c); }
    });
}

function setUserPassword(userId) {
    var url = "/UserManagement/SetPassword/" + userId;
    $.ajax({
        url: url,
        type: 'Get',
        success: function (data) {
            $("#dialog-set-password").empty().append(data);
            SetPasswords.dialog = $("#dialog-set-password").dialog("open");
            SetPasswords.successCallback = updateTable;
        },
        error: function (a, b, c) { alert(c); }
    });
}

function activateUser(userId) {
    var url = "/UserManagement/ProcessUserActivationRequest/" + userId;
    $.ajax({
        url: url,
        type: 'Get',
        success: function (data) {
            $("#dialog-confirm-activate").empty().append(data);
            ActivateUsers.dialog = $("#dialog-confirm-activate").dialog("open");
            ActivateUsers.successCallback = updateTable;
        },
        error: function (a, b, c) { alert(c); }
    });
}


function assignRoles(userId) {
    var url = "/UserManagement/AssignRoles/" + userId;
    $.ajax({
        url: url,
        type: 'Get',
        success: function (data) {
            $("#dialog-roles").empty().append(data);
            AssignRoles.dialog = $("#dialog-roles").dialog("open");
            AssignRoles.successCallback = updateTable;
        },
        error: function (a, b, c) { alert(c); }
    });
}

function assignGroups(userId) {
    var url = "/UserManagement/AssignGroups/" + userId;
    $.ajax({
        url: url,
        type: 'Get',
        success: function (data) {
            $("#dialog-groups").empty().append(data);
            AssignGroups.dialog = $("#dialog-groups").dialog("open");
            AssignGroups.successCallback = updateTable;
        },
        error: function (a, b, c) { alert(c); }
    });
}


function deleteSelectedUser() {
    var userId = getUserIdOfSelectedUser();
    var url = "/UserManagement/DeleteUser/" + userId;
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
    $("#lnkEditUser, #lnkDeleteUser, #lnkAssignRoles, #lnkAssignGroups").toggleClass('disabled', !userSelected);
}

function InitDialogs() {
    var url = "";

    $("#dialog-edit").dialog({
        title: 'Redigér bruger',
        autoOpen: false,
        resizable: false,
        width: 500,
        modal: true,
        draggable: true
    });

    $("#dialog-roles").dialog({
        title: 'Tildel roller',
        autoOpen: false,
        resizable: false,
        width: 450,
        modal: true,
        draggable: true
    });

    $("#dialog-groups").dialog({
        title: 'Tildel gruppe',
        autoOpen: false,
        resizable: false,
        width: 450,
        modal: true,
        draggable: true
    });


    $("#dialog-set-password").dialog({
        title: 'Skift password',
        autoOpen: false,
        resizable: false,
        width: 450,
        modal: true,
        draggable: true
    });

    $("#dialog-confirm-delete").dialog({
        autoOpen: false,
        resizable: false,
        height: 200,
        width: 350,
        modal: true,
        draggable: true,
        open: function (event, ui) {
            $(".ui-dialog-titlebar-close").hide();
        },
        buttons: {
            "OK": function () {
                $(this).dialog("close");
                deleteSelectedUser();
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        }
    });

    $("#dialog-confirm-activate").dialog({
        title: 'Bruger ønsker adgang til systemet',
        autoOpen: false,
        resizable: false,
        width: 500,
        modal: true,
        draggable: true
    });

};


function updateTable() {
    table.api().ajax.reload();
    UpdateButtons(false);
}