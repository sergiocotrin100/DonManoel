
var MENUSELECIONADO = [];
var PEDIDO = null;
var CATEGORIAS = [];

window.onload = getParams;

$(document).ready(function () {

    let viewitens = getURLParameters("viewitens");
    if (!isNullOrEmpty(viewitens)) {
        $('html, body').animate({
            scrollTop: $('#itens').offset().top
        }, 100);
    }

    $("#btnAdd").click(function () {
        addItem();
    });

    $("#btnEnviarPedido").click(function () {
        enviarPedido();
    });

    $("#btnCancelarPedido").click(function () {
        cancelarPedido();
    });

    $("#btnFecharPedido").click(function () {
        fecharConta();
    });


});

function getParams() {
    if (!isNullOrEmpty(objPedido)) {
        PEDIDO = objPedido;
        CATEGORIAS = lstCategorias;
    }
}

function setSelected(objeto) {
    let id = $(objeto).data().code;
    $.each(MENUSELECIONADO.Composicao, function (idx, receita) {
        if (receita.Id == id) {
            receita.Selecionado = objeto.checked;
        }
    });
}

function showMenuDetails(idmenu) {
    $("#txtObservacao").val("");
    $(".receita").hide();
    $(".ponto-carne").hide();

    MENUSELECIONADO = getMenu(idmenu);

    $("#modalDetalhesMenu #MenuDescricao").html(MENUSELECIONADO.Descricao);
    $("#tbReceita tr").remove();
    $("#modalDetalhesMenu #tbReceita tbody").remove();

    var tbody = $('#modalDetalhesMenu #tbReceita').children('tbody');

    var table = tbody.length ? tbody : $('#modalDetalhesMenu #tbReceita');
    var contemCarne = false;
    var contemComposicao = false;
    $.each(MENUSELECIONADO.Composicao, function (idx, receita) {
        receita.Selecionado = true;
        if (contemCarne == false) {
            contemCarne = receita.ContemCarne;
        }
        table.append('<tr>' +
            '<td>' + receita.Descricao + '</td> ' +
            '<td class="text-end fw-700"> <input type="checkbox" name="chkComposicao" onchange="setSelected(this)" style="opacity: 1!important; position: inherit!important;" checked data-code="' + receita.Id + '"></td> ' +
            '</tr>');

        contemComposicao = true;
    });

    if (contemCarne) {
        $(".ponto-carne").show();
    }

    if (contemComposicao) {
        $(".receita").show();
    }

    var myModal = new bootstrap.Modal(document.getElementById('modalDetalhesMenu'), {
        keyboard: false,
        backdrop: "static"
    });

    $(myModal).modal({ backdrop: true, keyboard: false, show: true });

}

function getMenu(idMenu) {
    let menu = [];

    $.each(CATEGORIAS, function (idx, categoria) {
        let item = $.grep(categoria.Menu, function (e) { return e.Id == idMenu; })[0];
        if (!isNullOrEmpty(item) && item.Id > 0) {
            menu = item;
            return menu;
        }

    });
    return menu;
}

function addItem() {
    var observacao = $("#txtObservacao").val();
    var idmesa = $("#hdIdMesa").val();
    MENUSELECIONADO.Valor = MENUSELECIONADO.ValorFormatado;

    $.ajax({
        type: "post",
        dataType: 'json',
        data: {
            'idmesa': idmesa, 'pontoCarne': $("input[name='pontocarne']:checked").val(), 'observacao': observacao, 'menu': MENUSELECIONADO
        },
        url: hostSite() + "Sales/AddItem",
        success: function (data) {
            if (data.success) {
                var url = hostSite() + "Sales/Order?idmesa=" + idmesa + "&idorder=" + data.result + "&viewitens=1";
                window.location.href = url;
            } else {
                toastr.error(data.message, "Erro");
            }
        }
    });

}

function enviarPedido() {
    $.confirm({
        title: 'Atenção!',
        icon: 'fa fa-user',
        animation: 'scale',
        closeAnimation: 'scale',
        content: 'Deseja realmente enviar esse pedido para a cozinha?',
        buttons: {
            Sim: {
                btnClass: 'btn-danger',
                keys: ['enter', 'shift'],
                action: function () {
                    $.ajax({
                        type: "post",
                        dataType: 'json',
                        data: {
                            'idpedido': $("#hdIdPedido").val(), 'status': 3
                        },
                        url: hostSite() + "Sales/ChangeStatus",
                        success: function (data) {
                            if (data.success) {
                                toastr.success("Solicitação efetuada com sucesso!", "Sucesso");
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

function cancelarPedido() {
    $.confirm({
        title: 'Atenção!',
        icon: 'fa fa-user',
        animation: 'scale',
        closeAnimation: 'scale',
        content: 'Deseja realmente cancelar esse pedido?',
        buttons: {
            Sim: {
                btnClass: 'btn-danger',
                keys: ['enter', 'shift'],
                action: function () {
                    $.ajax({
                        type: "post",
                        dataType: 'json',
                        data: {
                            'idpedido': $("#hdIdPedido").val(), 'status': 7
                        },
                        url: hostSite() + "Sales/ChangeStatus",
                        success: function (data) {
                            if (data.success) {
                                toastr.success("Pedido cancelado com sucesso!", "Sucesso");
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

function cancelarItem(id) {
    $.confirm({
        title: 'Atenção!',
        icon: 'fa fa-user',
        animation: 'scale',
        closeAnimation: 'scale',
        content: 'Deseja realmente cancelar esse item?',
        buttons: {
            Sim: {
                btnClass: 'btn-danger',
                keys: ['enter', 'shift'],
                action: function () {
                    $.ajax({
                        type: "post",
                        dataType: 'json',
                        data: {
                            'idpedidoitem': id, 'status': 4
                        },
                        url: hostSite() + "Sales/ChangeStatusItem",
                        success: function (data) {
                            if (data.success) {
                                toastr.success("Item cancelado com sucesso!", "Sucesso");
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

function setItemEntregue(id) {
    $.confirm({
        title: 'Atenção!',
        icon: 'fa fa-user',
        animation: 'scale',
        closeAnimation: 'scale',
        content: 'Esse item foi entregue?',
        buttons: {
            Sim: {
                btnClass: 'btn-danger',
                keys: ['enter', 'shift'],
                action: function () {
                    $.ajax({
                        type: "post",
                        dataType: 'json',
                        data: {
                            'idpedidoitem': id, 'status': 5
                        },
                        url: hostSite() + "Sales/ChangeStatusItem",
                        success: function (data) {
                            if (data.success) {
                                toastr.success("Item entregue com sucesso!", "Sucesso");
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

function duplicarItem(id) {
    $.confirm({
        title: 'Atenção!',
        icon: 'fa fa-user',
        animation: 'scale',
        closeAnimation: 'scale',
        content: 'Deseja realmente duplicar esse item?',
        buttons: {
            Sim: {
                btnClass: 'btn-danger',
                keys: ['enter', 'shift'],
                action: function () {
                    $.ajax({
                        type: "post",
                        dataType: 'json',
                        data: {
                            'idpedidoitem': id
                        },
                        url: hostSite() + "Sales/DuplicateItem",
                        success: function (data) {
                            if (data.success) {
                                toastr.success("Item duplicado com sucesso!", "Sucesso");
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

function fecharConta() {
    $.confirm({
        title: 'Atenção!',
        icon: 'fa fa-dollar',
        animation: 'scale',
        closeAnimation: 'scale',
        content: 'Deseja realmente fechar a conta?',
        buttons: {
            Sim: {
                btnClass: 'btn-danger',
                keys: ['enter', 'shift'],
                action: function () {
                    $.ajax({
                        type: "post",
                        dataType: 'json',
                        data: {
                            'idpedido': $("#hdIdPedido").val(), 'status': 5
                        },
                        url: hostSite() + "Sales/ChangeStatus",
                        success: function (data) {
                            if (data.success) {
                                 $("#modalImpressao #numeropedidoimpressao").html(("00000" + data.result.id).slice(-5));
                                $("#modalImpressao #numeromesaimpressao").html(("00" + data.result.idMesa).slice(-2));
                                $("#modalImpressao #datapedidoimpressao").html(data.result.dataPedidoImpressao);
                                $("#modalImpressao #horapedidoimpressao").html(data.result.horaPedidoImpressao);

                                $("#tableItensImpressao tbody").remove();
                                var tbody = $('#tableItensImpressao').children('tbody');

                                var table = tbody.length ? tbody : $('#tableItensImpressao');
                                $.each(data.result.itensImpressao, function (idx, item) {
                                    table.append('<tr>' +
                                        '<td>' + item.quantidade + '</td> ' +
                                        '<td style="max-width: 17ch; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;">' + item.descricao + '</td> ' +
                                        '<td>' + item.valorFormatado + '</td> ' +
                                        '<td>' + item.valorTotalFormatado + '</td> ' +
                                        '</tr>');
                                });

                                jQuery('#modalImpressao').modal('show')
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