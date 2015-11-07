$(document).ready(function () {
    InitTable();
});

var table;

function InitTable() {
    table = $('#activityLog').dataTable({
        "language": { "url": "../Scripts/jquery.dataTables.Danish.json" },
        "ajax": '../UserManagement/GetAllActivities',
        "columnDefs": [
         { "targets": 0, "visible": false }
        ]
    });

    $('#activityLog tbody').on('click', 'tr', function () {
        $('#activityLog tbody tr').removeClass("selected");
        $(this).toggleClass('selected');
    });
};
    