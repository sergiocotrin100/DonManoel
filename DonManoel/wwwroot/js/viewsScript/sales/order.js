

var MENUSELECIONADO = [];
var PEDIDO = null;
var CATEGORIAS = [];

window.onload = getParams;

$(document).ready(function () {
    debugger;
    $("input[name=chkComposicao]").change(function () {
        debugger;
        let id = $(this).data().code;
        $.each(MENUSELECIONADO.Composicao, function (idx, receita) {
            debugger;
            if (receita.Id == id) {
                receita.Selecionado = objeto.checked;
            }
        });
    });
});

function getParams() {
    if (!isNullOrEmpty(objPedido)) {
        PEDIDO = objPedido;
        CATEGORIAS = lstCategorias;
    }
}

function showMenuDetails(idmenu) {
    $(".ponto-carne").hide();
    MENUSELECIONADO = getMenu(idmenu);    
    
    $("#modalDetalhesMenu #MenuDescricao").html(MENUSELECIONADO.Descricao);
    $("#tbReceita tr").remove();
    $("#modalDetalhesMenu #tbReceita tbody").remove();

    var tbody = $('#modalDetalhesMenu #tbReceita').children('tbody');

    var table = tbody.length ? tbody : $('#modalDetalhesMenu #tbReceita');
    var contemCarne = false;
    $.each(MENUSELECIONADO.Composicao, function (idx, receita) {
        if (contemCarne == false) {
            contemCarne = receita.ContemCarne;
        }
        table.append('<tr>'+
            '<td>' + receita.Descricao +'</td> '+
            '<td class="text-end fw-700"> <input type="checkbox" name="chkComposicao" style="opacity: 1!important; position: inherit!important;" checked data-code="'+receita.Id+'"></td> '+
        '</tr>');
    });

    if (contemCarne) {
        $(".ponto-carne").show();
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