

var MENUSELECIONADO = [];
var PEDIDO = null;
var CATEGORIAS = [];

window.onload = getParams;

$(document).ready(function () {
    $("#btnAdd").click(function () {
        addItem();
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
        table.append('<tr>'+
            '<td>' + receita.Descricao +'</td> '+
            '<td class="text-end fw-700"> <input type="checkbox" name="chkComposicao" onchange="setSelected(this)" style="opacity: 1!important; position: inherit!important;" checked data-code="'+receita.Id+'"></td> '+
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
                var myModal = new bootstrap.Modal(document.getElementById('modalDetalhesMenu'), {
                    keyboard: false,
                    backdrop: "static"
                });

                $(myModal).hide();
            }
            else {
                
            }
        }
    });

}