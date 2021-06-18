


var PEDIDO = null;
var CATEGORIAS = [];

/*
$(document).ready(function () {

});*/

window.onload = getParams;


function getParams() {
    if (!isNullOrEmpty(objPedido)) {
        PEDIDO = objPedido;
        CATEGORIAS = lstCategorias;
    }
}

function showMenuDetails(idmenu) {
    
    let menu = getMenu(idmenu);
    

    $("#modalDetalhesMenu #MenuDescricao").html(menu.Descricao);

    $("#modalDetalhesMenu #tbReceita tbody").remove();
   // $("#modalDetalhesMenu #tbReceita > tbody:result").append("<tr><td>row content</td></tr>");

    //Try to get tbody first with jquery children. works faster!
    var tbody = $('#tbReceita').children('tbody');

    //Then if no tbody just select your table 
    var table = tbody.length ? tbody : $('#tbReceita');

    //Add row
    table.append('<tr><td>hello</td></tr>');
    

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