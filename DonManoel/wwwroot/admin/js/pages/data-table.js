//[Data Table Javascript]

//Project:	Riday Admin - Responsive Admin Template
//Primary use:   Used only for the Data Table

$(function () {
    "use strict";

	$('#example1').DataTable({
        language: {
            decimal: ",",
            thousands: ".",
            loadingRecords: "Carregando...",
            processing: "Processando...",
            lengthMenu: "Mostrar _MENU_",
            zeroRecords: "Nenhum registro encontrado",
            emptyTable: "Nenhum registro encontrado",
            info: "Registros _START_ a _END_ de _TOTAL_",
            infoEmpty: "",//"Nada para mostrar",
            infoFiltered: "(filtrados de _MAX_)",
            infoPostFix: "",
            url: "",
            paginate: {
                First: "Primeiro",
                Previous: "Anterior",
                Next: "Seguinte",
                Last: "Último"
            },
            buttons: {
                pageLength: {
                    _: '<i class="fal fa-align-justify" data-toggle="tooltip" data-placement="top" title="%d Linhas"></i>',
                    '-1': "Mostrar Tudo"
                }
            },
            search: "",
            searchPlaceholder: "Pesquisar...",
            select: true,
            deferRender: true,
            fixedHeader: {
                header: true,
                footer: false
            }
            , bAutoWidth: true

        },
        drawCallback: function (settings) {
            var pagination = $(this).closest('.dataTables_wrapper').find('.dataTables_paginate');
            pagination.toggle(this.api().page.info().pages > 1);
            var info = $(this).closest('.dataTables_wrapper').find('.dataTables_info');
            info.toggle(this.api().page.info().pages > 1);
            $(".dataTables_paginate a").addClass("paginate_button");
        }
	});
	//$('#example1').FWDataTableDefault();
    $('#example2').DataTable({
      'paging'      : true,
      'lengthChange': false,
      'searching'   : false,
      'ordering'    : true,
      'info'        : true,
      'autoWidth'   : false
    });
	
	
	$('#example').DataTable( {
		dom: 'Bfrtip',
		buttons: [
			'copy', 'csv', 'excel', 'pdf', 'print'
		]
	} );
	
	$('#tickets').DataTable({
	  'paging'      : true,
	  'lengthChange': true,
	  'searching'   : true,
	  'ordering'    : true,
	  'info'        : true,
	  'autoWidth'   : false,
	});
	
	$('#productorder').DataTable({
	  'paging'      : true,
	  'lengthChange': true,
	  'searching'   : true,
	  'ordering'    : true,
	  'info'        : true,
	  'autoWidth'   : false,
	});
	

	$('#complex_header').DataTable();
	
	//--------Individual column searching
	
    // Setup - add a text input to each footer cell
    $('#example5 tfoot th').each( function () {
        var title = $(this).text();
        $(this).html( '<input type="text" placeholder="Search '+title+'" />' );
    } );
 
    // DataTable
    var table = $('#example5').DataTable();
 
    // Apply the search
    table.columns().every( function () {
        var that = this;
 
        $( 'input', this.footer() ).on( 'keyup change', function () {
            if ( that.search() !== this.value ) {
                that
                    .search( this.value )
                    .draw();
            }
        } );
    } );
	
	
	//---------------Form inputs
	var table = $('#example6').DataTable();
 
    $('#data-update').click( function() {
        var data = table.$('input, select').serialize();
        alert(
            "The following data would have been submitted to the server: \n\n"+
            data.substr( 0, 120 )+'...'
        );
        return false;
    } );
	
	
	
	
  }); // End of use strict