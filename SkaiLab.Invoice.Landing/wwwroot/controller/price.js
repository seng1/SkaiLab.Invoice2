
app.controller("PriceControler", function ($http) {
    var vm = this;
    vm.monthlySelected = false;
    vm.items = [];
    vm.init = function () {
        vm.saveExtraText = $("#htSaveExtra").val();
        $http({
            method: 'GET',
            url: '/plan/GetInvoicePlans',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            mimeType: 'application/json'
        }).then(function (result) {
            vm.items = result.data;
            
        }).catch(function (error) {

        });
    };
    vm.toggleClick = function () {
        vm.monthlySelected = !vm.monthlySelected;
    }
});