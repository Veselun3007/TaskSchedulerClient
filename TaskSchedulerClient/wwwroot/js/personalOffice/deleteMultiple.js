$(".btnDelete").on("click", function (e) {
    var confirmation = confirm("Ви впевнені?")
    if (confirmation) {
        e.preventDefault();
        var data = [];
        $("input[name='chkDeleteName']:checked").each(function () {
            data.push($(this).val());
        });

        $.ajax({
            type: "POST",
            url: '/Assignment/DeleteSelected',
            data: { deleteList: data },
            traditional: true,
            success: function (result) {
                alert("Видалення успішне")
                location.reload();
            }
        })
    }
    else
        alert("Delete Canceled")
})