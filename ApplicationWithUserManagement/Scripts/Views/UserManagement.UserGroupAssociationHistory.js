$(document).ready(function () {
    InitTable();
});

var table;

function InitTable() {
    table = $('#associationHistory').dataTable({
        "language": { "url": "/Scripts/jquery.dataTables.Danish.json" },
        "ajax": '../UserGroupManagement/GetUserGroupAssociationHistory',
        "columnDefs": [
         { "targets": 0, "visible": false }
        ]
    });

    $('#associationHistory tbody').on('click', 'tr', function () {
        $('#userGroups tbody tr').removeClass("selected");
        $(this).toggleClass('selected');
    });
};
