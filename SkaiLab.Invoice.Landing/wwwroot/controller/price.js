
app.controller("PriceControler", function ($http) {
    var vm = this;
    vm.monthlySelected = true;
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
            vm.saveExtraText = vm.saveExtraText.replace("{%}", vm.items[0].yearDiscountRate + "%")
            for (var i = 0; i <= vm.items.length; i++) {
                vm.items[i].yearPrice = vm.items[i].price * 12;
                vm.items[i].yearPrice -= (vm.items[i].yearPrice * vm.items[i].yearDiscountRate) / 100;
            }
            
        }).catch(function (error) {

        });
    };
    vm.toggleClick = function () {
        vm.monthlySelected = !vm.monthlySelected;
    }
});