

$(document).ready(function () {
    setInterval(atualizarPedidosBar, 60000);
});


function atualizarPedidosBar() {
    $.ajax(
        {
            type: 'GET',
            url: hostSite() + "Admin/Sales/GetPedidosBar",
            dataType: 'html',
            cache: false,
            async: true,
            success: function (data) {
                $('#divPedidosBar').html(data);
            }
        });
}