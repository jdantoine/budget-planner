$(document).ready(function () {

        //datepicker
        $('.datepicker').datepicker()

        //datatables
        $('.data-table').DataTable({
            "order": [[0, 'desc']]
        })

    //disable/enable budget and category dropdowns on transaction view
    $('body').on('change', '#budgetBool', function () {
        if ($(this).is(':checked')) {
            $('#category').show('slow');
            $('.budget-item').prop('disabled', true).hide('slow');
        }
        if (!$(this).is(':checked')) {
            $('#category').hide("slow");
            $('.budget-item').prop('disabled', false).show('slow');
        }
    })

    //$('body').on('click', '.editTrans', function () {
        //$('#editView').load('/Transactions/_Edit/' + $(this).data('id'), null, function () {
            //if ($('#BudgetItemId').disabled == "disabled") {
                //console.log('inside disabled');
                //$('.budget-item').hide()
                //$('#category').show()
                //$('#budgetBool').prop(':checked', true)
            //}
            //console.log('in edit trans' + $('#temp').text);
            //if ($('#temp').text == "" || $('#temp').text == null) {
            //    console.log('in if cond');
            //    $('.budget-item').hide()
            //    $('.budget-item').prop('disabled', true)
            //    $('#category').show()
            //    $('#budgetBool').prop(':checked', true)
            //}
        //});
        
    //})

    //colors
    $('.balance').each(function(i) {
        var content = parseInt($(this).text().replace('$', ''), 10);
        var balance = parseInt(content, 10);
        if (balance <= 0) {
            $(this).removeClass("text-succ").addClass("text-dang");
        }
        else {
            $(this).removeClass("text-dang").addClass("text-succ");
        }
    });


    //section scroll
    $('.section-scroll').on('click', function (e) {
        var target = this.hash;
        var $target = $(target);

        $('html, body').stop().animate({
            'scrollTop': $target.offset().top
        }, 900, 'swing');

        e.preventDefault();
    });


    //toggle boolean buttons
    //function TogglePrettyButton(divContain, target, checkbox, textElement, textFalse, textTrue)
    //{
    //    divContain = $(divContain),
    //    target = $(target),
    //    checkbox = $(checkbox),
    //    textElement = $(textElement)

    //    divContain.on('click', target, function() {
    //        if(checkbox.is(':checked')) {
    //            checkbox.prop('checked', false).prop('value', false);
    //            target.toggleClass('btn-success');
    //            target.toggleClass('btn-danger');
    //            textElement.text(textFalse)
    //        }
    //        else {
    //            checkbox.prop('checked', true).prop('value', true);
    //            target.toggleClass('btn-danger');
    //            target.toggleClass('btn-success');
    //            textElement.text(textTrue)
    //        }
    //    })

    //TogglePrettyButton('#editView', '#allow-btn', '#allow-ck', '#allow-text', 'Only I Can Edit', 'Anyone Can Edit');
    //TogglePrettyButton('#editView', '#income-btn', '#income-ck', '#income-text', 'Expense', 'Income');
    //TogglePrettyButton('#editView', '#recon-btn', '#recon-ck', '#recon-text', 'Unreconciled', 'Reconciled');

    $('#editView').on('click', '#allow-btn', function () {
        if ($('#allow-ck').is(':checked'))

        {
            $('#allow-ck').prop('checked', false).prop('false');
            $('#allow-btn').removeClass('btn-success').addClass('btn-danger');
            $('#allow-text').text('Only I Can Edit');
        }
        else {
            $('#allow-ck').prop('checked', true).val('true');
            $('#allow-btn').removeClass('btn-danger').addClass('btn-success');
            $('#allow-text').text('Anyone Can Edit');
        }
    })

    $('#editView').on('click', '#income-btn', function () {
        if ($('#income-ck').is(':checked')) {
            $('#income-ck').prop('checked', false).val('false');
            $('#income-btn').removeClass('btn-success').addClass('btn-danger');
            $('#income-text').text('Expense');
        }
        else {
            $('#income-ck').prop('checked', true).val('true');
            $('#income-btn').removeClass('btn-danger').addClass('btn-success');
            $('#income-text').text('Income');
        }
    });

    $('#editView').on('click', '#recon-btn', function () {
        if ($('#recon-ck').is(':checked')) {
            $('#recon-ck').prop('checked', false).val('false');
            $('#recon-btn').removeClass('btn-success').addClass('btn-danger');
            $('#recon-text').text('UnReconciled');
        }
        else {
            $('#recon-ck').prop('checked', true).val('true');
            $('#recon-btn').removeClass('btn-danger').addClass('btn-success');
            $('#recon-text').text('Reconciled');
        }
    })

    $('#editView').on('click', '#budget-btn', function () {
        if ($('#budget-ck').is(':checked')) {
            $('#budget-ck').prop('checked', false).val('false');
            $('#budget-btn').removeClass('btn-success').addClass('btn-danger');
            $('#budget-text').text('Spending Limit');
            $('#amt-editor').attr("placeholder", "Enter goal spending limit amount");
        }
        else {
            $('#budget-ck').prop('checked', true).val('true');
            $('#budget-btn').removeClass('btn-danger').addClass('btn-success');
            $('#budget-text').text('Expected Income');
            $('#amt-editor').attr("placeholder", "Enter expected income");
        }
    });

    //partial views handling
    function AssignPartialViewHandler(divContain, divRender, target, controllerName, actionName, hasDataTag) {
        var loadUrl = "/" + controllerName + "/" + actionName;

        $(divContain).on('click', target, function () {
            $(divRender).load(loadUrl + (hasDataTag ? ('/' + $(this).data('id')) : ""));
        })
    }

    AssignPartialViewHandler('#editView', '#editView', '.cancel-acct', 'BankAccounts', '_Create', false);
    AssignPartialViewHandler('#tableAcct', '#editView', '.editAcct', 'BankAccounts', '_Edit', true);
    AssignPartialViewHandler('#tableAcct', '#editView', '.deleteAcct', 'BankAccounts', '_Delete', true);
    AssignPartialViewHandler('#tableAcct', '#viewTrans', '.viewAcctTrans', 'BankAccounts', '_Transactions', true);
    AssignPartialViewHandler('#editView', '#editView', '.cancel-budg', 'BudgetItems', '_Create', false);
    AssignPartialViewHandler('#tableBudg', '#editView', '.editBudg', 'BudgetItems', '_Edit', true);
    AssignPartialViewHandler('#tableBudg', '#editView', '.deleteBudg', 'BudgetItems', '_Delete', true)
    AssignPartialViewHandler('#tableBudg', '#viewTrans', '.viewBudgTrans', 'BudgetItems', '_Transactions', true);
    AssignPartialViewHandler('#accountsRender', '#editView', '.editTrans', 'Transactions', '_Edit', true);
    AssignPartialViewHandler('#accountsRender', '#editView', '.deleteTrans', 'Transactions', '_Delete', true);
    AssignPartialViewHandler('#catsRender', '#editView', '.editCat', 'Categories', '_Edit', true);
    AssignPartialViewHandler('#catsRender', '#editView', '.deleteCat', 'Categories', '_Delete', true);
    AssignPartialViewHandler('#invitedUsers', "#inviteUserPartial", '.invite', 'InvitedUsers', '_Create', false);

    //cancels
    $('#invitedUsers').on('click', '.cancel-invite', function () {
        $('.removeIUser').detach();
    })

    $('#catsRender').on('click', '.cancel-cat', function () {
        $('.removeCat').detach();
    })

    //manually change checkbox values (in partial views)
    function ManualCheckbox(target) {
        $('body').on('change', target, function () {
            if ($(this).is(':checked')) {
                $(this).val(true);

            }
            if (!$(this).is(':checked')) {
                $(this).val(false);
            }
        });
    }

    ManualCheckbox("#income-ck");
    ManualCheckbox('#recon-ck')
    ManualCheckbox("#adminCk");

    //data to modals
    function PassIdToModal(target, dataElement) {
        $('body').on('click', target, function () {
            $(dataElement).val($(this).data('id'));
        });
    }

    function PassAttributeToModal(target, dataElement, attribute) {
        dataElement = $(dataElement)
        $('body').on('click', target, function () {
            if (attribute == 'income' && ($(this).data(attribute)) == "True") {
                dataElement.text = ""
                dataElement.html("<i class=\"fa fa-plus\"></i>")
            }
            else if (attribute == 'income' && ($(this).data(attribute)) == "False") {
                dataElement.text = ""
                dataElement.html("<i class=\"fa fa-minus\"></i>")
            }
            else if (attribute == 'recon' && ($(this).data(attribute)) == "True") {
                dataElement.text = ""
                dataElement.html("<i class=\"fa fa-check\"></i>")
            }
            else if (attribute == 'recon' && ($(this).data(attribute)) == "False") {
                dataElement.text = ""
                dataElement.html("<i class=\"fa fa-times\"></i>")
            }
            else {
                dataElement.text($(this).data(attribute));
            }
        });

    }

    PassIdToModal('.delete', '#rescindId');
    PassIdToModal('.expel', '#expelId');
    PassIdToModal('.leave', '#leaveId');

    PassAttributeToModal('.tranDetails', '#transId', 'id')
    PassAttributeToModal('.transDetails', '#transAccount', 'account');
    PassAttributeToModal('.transDetails', '#transCategory', 'category');
    PassAttributeToModal('.transDetails', '#transUser', 'user');
    PassAttributeToModal('.transDetails', '#transTransacted', 'transacted');
    PassAttributeToModal('.transDetails', '#transEntered', 'entered');
    PassAttributeToModal('.transDetails', '#transAmount', 'amount');
    PassAttributeToModal('.transDetails', '#transDesc', 'desc');
    PassAttributeToModal('.transDetails', '#transIncome', 'income');
    PassAttributeToModal('.transDetails', '#transRecon', 'recon');

    PassAttributeToModal('.transDetailsB', '#transId', 'id')
    PassAttributeToModal('.transDetailsB', '#transAccount', 'account');
    PassAttributeToModal('.transDetailsB', '#transBudget', 'budget');
    PassAttributeToModal('.transDetailsB', '#transCategory', 'category');
    PassAttributeToModal('.transDetailsB', '#transUser', 'user');
    PassAttributeToModal('.transDetailsB', '#transTransacted', 'transacted');
    PassAttributeToModal('.transDetailsB', '#transEntered', 'entered');
    PassAttributeToModal('.transDetailsB', '#transAmount', 'amount');
    PassAttributeToModal('.transDetailsB', '#transDesc', 'desc');
    PassAttributeToModal('.transDetailsB', '#transIncome', 'income');
    PassAttributeToModal('.transDetailsB', '#transRecon', 'recon');
});