

$(document).ready(function () {
    setInterval(atualizarPedidosCozinha, 60000);
});


function atualizarPedidosCozinha() {
    $.ajax(
        {
            type: 'GET',
            url: hostSite() + "Sales/GetPedidosCozinha",
            dataType: 'html',
            cache: false,
            async: true,
            success: function (data) {
                $('#divPedidosCozinha').html(data);
            }
        });
}