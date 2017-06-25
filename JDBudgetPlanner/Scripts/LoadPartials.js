$(document).ready(function () {

    function LoadPartialView(element, action, controller) {
        $(document).on('click', element, function (evt) {
            var $id = $(this).attr("id");
            var $actionDIV = $('#action');
            var $actionURL = '/' + controller + '/' + action + '/' + $id;
            var url = $(this).data($actionURL);
            $actionDIV.load($actionURL, function () {
                $('#Amount').maskMoney({ thousands: '', decimal: '.' });
                $('#ActualBalance').maskMoney({ thousands: '', decimal: '.' });
            });
        })
    }

    LoadPartialView('.renameAccount', '_renameaccount', 'transactions');
    LoadPartialView('.archiveAccount', '_archiveaccount', 'transactions');

    LoadPartialView('.reconcileAccount', '_reconcileaccount', 'transactions');
    LoadPartialView('.addTransaction', '_addtransaction', 'transactions');
    LoadPartialView('.deleteTransaction', '_deletetransaction', 'transactions');
    LoadPartialView('.editTransaction', '_edittransaction', 'transactions');

    LoadPartialView('.addBudgetItem', '_addbudgetitem', 'budget');
    LoadPartialView('.editBudgetItem', '_editbudgetitem', 'budget');
    LoadPartialView('.deleteBudgetItem', '_deletebudgetitem', 'budget');

});