
function recalcularValorPedido(checked, idpedido, totalitens, taxaservico) {
    var totalpedido = totalitens;
    if (checked) {
        totalpedido = calcularTotalPedidoComTaxaServico(totalitens, taxaservico);
    }

    $("#totalpedido_" + idpedido).html(formatMoney(totalpedido, false));
}

function setPedidoPago(idpedido) {
    var checked = $("#chkTaxaServico_" + idpedido).is(':checked');
    var taxaservico = checked ? 'S' : 'N';
    $.confirm({
        title: 'Atenção!',
        icon: 'fa fa-dollar',
        animation: 'scale',
        closeAnimation: 'scale',
        content: 'Pedido foi recebido pelo caixa?',
        buttons: {
            Sim: {
                btnClass: 'btn-danger',
                keys: ['enter', 'shift'],
                action: function () {
                    $.ajax({
                        type: "post",
                        dataType: 'json',
                        data: {
                            'idpedido': idpedido, 'status': 6, taxaservico: taxaservico
                        },
                        url: hostSite() + "Sales/ChangeStatus",
                        success: function (data) {
                            if (data.success) {
                                toastr.success("Pedido recebido com sucesso!", "Sucesso");
                                window.location.reload();
                            }
                            else {
                                toastr.error(data.message, "Erro");
                            }
                        }
                    });
                }
            },
            Não: function () {

            }
        }
    });
}