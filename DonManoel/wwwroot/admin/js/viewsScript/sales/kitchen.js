

$(document).ready(function () {
    setInterval(atualizarPedidosCozinha, 10000);
});


function atualizarPedidosCozinha() {
    $.ajax(
        {
            type: 'GET',
            url: hostSite() + "Admin/Sales/GetPedidosCozinha",
            dataType: 'html',
            cache: false,
            async: true,
            success: function (data) {
                $('#divPedidosCozinha').html(data);
            }
        });
}