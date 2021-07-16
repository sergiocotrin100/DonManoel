//const { stat } = require("fs");

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

    //$(".enviar-pedido").click(function () {
    //    enviarPedido();
    //});

    $(".cancelar-pedido").click(function () {
        cancelarPedido();
    });

    $(".fechar-pedido").click(function () {
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
        url: hostSite() + "Admin/Sales/AddItem",
        success: function (data) {
            if (data.success) {
                var url = hostSite() + "Admin/Sales/Order?idmesa=" + idmesa + "&idorder=" + data.result + "&viewitens=1";
                window.location.href = url;
            } else {
                erro(data.message);
            }
        }
    });

}

function enviarPedido() {
    var idOrder = 0;
    var url_string = window.location.href
    var url = new URL(url_string);
    idOrder = url.searchParams.get("idorder");
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
                        dataType: 'html',
                        data: {
                            'idpedido': $("#hdIdPedido").val(), 'status': 3, 'idorder': idOrder
                        },
                        url: hostSite() + "Admin/Sales/ChangeStatus",
                        success: function (data) {
                            sucesso("Pedido enviado para cozinha!");
                            $("#resultadoItens").html('');
                            $("#resultadoItens").html(data);
                        }
                        , error: function (request, status, error) {
                            erro(request.responseText);
                        }

                    });
                }
            },
            Não: function () {
                aviso("Pedido nao enviado para cozinha.");
            }
        }
    });
}

function cancelarPedido() {
    var idOrder = 0;
    var url_string = window.location.href
    var url = new URL(url_string);
    idOrder = url.searchParams.get("idorder");
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
                        dataType: 'html',
                        data: {
                            'idpedido': $("#hdIdPedido").val(), 'status': 7, 'idorder': idOrder
                        },
                        url: hostSite() + "Admin/Sales/ChangeStatus",
                        success: function (data) {
                            sucesso("Pedido cancelado com sucesso!");
                            $("#resultadoItens").html('');
                            $("#resultadoItens").html(data);
                        },
                        error: function (request, status, error) {
                            erro(request.responseText);
                        }
                    });
                }
            },
            Não: function () {
                aviso("Pedido não cancelado.");
            }
        }
    });
}

function cancelarItem(id) {
    var idOrder = 0;
    var url_string = window.location.href
    var url = new URL(url_string);
    idOrder = url.searchParams.get("idorder");
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
                        dataType: 'html',
                        data: {
                            'idpedidoitem': id, 'status': 4, 'idorder': idOrder
                        },
                        url: hostSite() + "Admin/Sales/ChangeStatusItem",
                        success: function (data) {
                            sucesso("Item cancelado com sucesso!");
                            $("#resultadoItens").html('');
                            $("#resultadoItens").html(data);

                        },
                        error: function (request, status, error) {
                            erro(request.responseText);
                        }
                    });
                }
            },
            Não: function () {
                aviso("Item não cancelado.");
            }
        }
    });
}

function cancelarItemBar(nome, status) {
    //statusFase=status para cancelar no status certo
    var idOrder = 0;
    var url_string = window.location.href
    var url = new URL(url_string);
    idOrder = url.searchParams.get("idorder");
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
                        dataType: 'html',
                        data: {
                            'nomepedidoitem': nome, 'idPedido': idOrder, 'status': 4, 'idorder': idOrder, 'statusFase': status
                        },
                        url: hostSite() + "Admin/Sales/ChangeStatusItemBar",
                        success: function (data) {
                            sucesso("Item cancelado com sucesso!");
                            $("#resultadoItens").html('');
                            $("#resultadoItens").html(data);

                        },
                        error: function (request, status, error) {
                            erro(request.responseText);
                        }
                    });
                }
            },
            Não: function () {
                aviso("Item não cancelado.");
            }
        }
    });
}


function cancelarItemQuantoEmPreparo(id) {
    var idOrder = 0;
    var url_string = window.location.href
    var url = new URL(url_string);
    idOrder = url.searchParams.get("idorder");
    $.confirm({
        title: 'Atenção!',
        icon: 'fa fa-user',
        animation: 'scale',
        closeAnimation: 'scale',
        content: 'O item já foi enviado para a cozinha, antes de confirmar o cancelamento, verifique se o prato esta pronto, caso não esteja pronto, clique em "sim" para cancelar.',
        buttons: {
            Sim: {
                btnClass: 'btn-danger',
                keys: ['enter', 'shift'],
                action: function () {
                    $.ajax({
                        type: "post",
                        dataType: 'html',
                        data: {
                            'idpedidoitem': id, 'status': 4, 'idorder': idOrder
                        },
                        url: hostSite() + "Admin/Sales/ChangeStatusItem",
                        success: function (data) {
                            sucesso("Item cancelado com sucesso!");
                            $("#resultadoItens").html('');
                            $("#resultadoItens").html(data);

                        },
                        error: function (request, status, error) {
                            erro(request.responseText);
                        }
                    });
                }
            },
            Não: function () {
                aviso("Item não cancelado.");
            }
        }
    });
}

function setItemEntregue(id) {
    var idOrder = 0;
    var url_string = window.location.href
    var url = new URL(url_string);
    idOrder = url.searchParams.get("idorder");
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
                        dataType: 'html',
                        data: {
                            'idpedidoitem': id, 'status': 5, 'idorder': idOrder
                        },
                        url: hostSite() + "Admin/Sales/ChangeStatusItem",
                        success: function (data) {
                            sucesso("Item entregue com sucesso!");
                            $("#resultadoItens").html('');
                            $("#resultadoItens").html(data);
                        },
                        error: function (request, status, error) {
                            erro(request.responseText);
                        }
                    });
                }
            },
            Não: function () {
                aviso("Pedido não enviado para cozinha");
            }
        }
    });
}


function setItemBarEntregue(nome, status) {
    //statusFase=status para cancelar no status certo
    var idOrder = 0;
    var url_string = window.location.href
    var url = new URL(url_string);
    idOrder = url.searchParams.get("idorder");
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
                        dataType: 'html',
                        data: {
                            'nomePedidoitem': nome, 'status': 5, 'idorder': idOrder, 'statusFase': status
                        },
                        url: hostSite() + "Admin/Sales/ChangeStatusItemBar",
                        success: function (data) {
                            sucesso("Item entregue com sucesso!");
                            $("#resultadoItens").html('');
                            $("#resultadoItens").html(data);
                        },
                        error: function (request, status, error) {
                            erro(request.responseText);
                        }
                    });
                }
            },
            Não: function () {
                aviso("Pedido não enviado para cozinha");
            }
        }
    });
}

function duplicarItem(id) {
    var idOrder = 0;
    var url_string = window.location.href
    var url = new URL(url_string);
    idOrder = url.searchParams.get("idorder");
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
                        dataType: 'html',
                        data: {
                            'idpedidoitem': id, 'idorder': idOrder
                        },
                        url: hostSite() + "Admin/Sales/DuplicateItem",
                        success: function (data) {
                            sucesso("Item duplicado com sucesso!");
                            $("#resultadoItens").html('');
                            $("#resultadoItens").html(data);
                        },
                        error: function (request, status, error) {
                            erro(request.responseText);
                        }
                    });
                }
            },
            Não: function () {
                aviso("Item não foi duplicado.");
            }
        }
    });
}


function duplicarItemBar(nome) {
    var idOrder = 0;
    var url_string = window.location.href
    var url = new URL(url_string);
    idOrder = url.searchParams.get("idorder");
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
                        dataType: 'html',
                        data: {
                            'nomePedidoitem': nome, 'idorder': idOrder
                        },
                        url: hostSite() + "Admin/Sales/DuplicateItemBar",
                        success: function (data) {
                            sucesso("Item duplicado com sucesso!");
                            $("#resultadoItens").html('');
                            $("#resultadoItens").html(data);
                        },
                        error: function (request, status, error) {
                            erro(request.responseText);
                        }
                    });
                }
            },
            Não: function () {
                aviso("Item não foi duplicado.");
            }
        }
    });
}

function mostraFilhos(item) {

    if ($("." + item).hasClass("classRowDisplay"))
        $("." + item).removeClass("classRowDisplay");
    else
        $("." + item).addClass("classRowDisplay");
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
                        url: hostSite() + "Admin/Sales/ChangeStatus",
                        success: function (data) {
                            if (data.success) {
                                printOrder(data.result);
                            }
                            else {
                                erro(data.message);
                            }
                        }
                    });
                }
            },
            Não: function () {
                aviso("Conta não foi fechada.");
            }
        }
    });
}

function erro(msg) {
    $.toast({
        heading: 'Erro',
        text: "Ocorreu um erro fatal, informe o administrador" + msg,
        position: 'top-right',
        loaderBg: '#ff6849',
        icon: 'error',
        hideAfter: 4000

    });
}
function info(msg) {
    $.toast({
        heading: 'informação',
        text: msg,
        position: 'top-right',
        loaderBg: '#ff6849',
        icon: 'info',
        hideAfter: 4000,
        stack: 6
    });
}

function aviso(msg) {
    $.toast({
        heading: 'Aviso',
        text: msg,
        position: 'top-right',
        loaderBg: '#ff6849',
        icon: 'warning',
        hideAfter: 3500,
        stack: 6
    });
}
function sucesso(msg) {
    $.toast({
        heading: 'Sucesso',
        text: msg,
        position: 'top-right',
        loaderBg: '#ff6849',
        icon: 'success',
        hideAfter: 3500,
        stack: 6
    });
}



