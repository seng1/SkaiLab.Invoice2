
app.controller("PaymentControler", function ($http) {
    var vm = this;
    vm.couponCode = "";
    vm.init = function () {
        vm.data = JSON.parse($("#hdData").val());
        console.log(vm.data);
    };
    vm.onPlanChange = function () {
        vm.reCalculate();
    };
    vm.onSubscriptionChange = function () {
        vm.reCalculate();
    };
    vm.reCalculate = function () {
        var selectedPlan = null;
        var selectedSubscriptionType = null;
        for (var i = 0; i < vm.data.plans.length; i++){
            if (vm.data.plans[i].id === vm.data.planId) {
                selectedPlan = vm.data.plans[i];
            }
        }
        for (var i = 0; i < vm.data.subscriptionTypes.length; i++) {
            if (vm.data.subscriptionTypes[i].id === vm.data.subscriptionId) {
                selectedSubscriptionType = vm.data.subscriptionTypes[i];
            } 
        }
        vm.data.paymentDescription = "Skai Invoice " + selectedPlan.name + " plan for 1 ";
        if (selectedSubscriptionType.id === 1) {
            vm.data.paymentDescription += " month."
        }
        else {
            vm.data.paymentDescription += " year."
        }
        if (selectedSubscriptionType.id === 2) {
            vm.data.total = selectedPlan.yearlyPrice;
        }
        else {
            vm.data.total = selectedPlan.monthlyPrice;
        }
        vm.data.totalAfterDiscount = vm.data.total;
        vm.data.totalDiscount = null;
        if (vm.data.discountRate !== null) {
            vm.data.totalDiscount = (vm.data.total * vm.data.discountRate) / 100;
            vm.data.totalAfterDiscount -=  vm.data.totalDiscount;
        }
        vm.data.taxAmount = 0;
        if (vm.data.isTax) {
            vm.data.taxAmount = (vm.data.totalAfterDiscount * 10) / 100;
        }
        vm.data.totalIncludeTax = vm.data.totalAfterDiscount + vm.data.taxAmount;
    }
    vm.onNextClick = function () {
        showProgressBar();
        $http.post("/MakePayment/SaveSubscription" , vm.data).then(function success(response) {
            hideProgressBar();
            window.location.href = "/MakePayment/Checkout?id=" + vm.data.userId + "&culture=" + $("#hdculture").val();
        }, function error(response) {
            showError(response.data);
        });
    };
    vm.onShowCouplCodeModal = function () {
        vm.couponCode = "";
        $('#CouponCodeModal').modal('show'); 
    };
    vm.onApplyCouponCode = function () {
        $('#CouponCodeModal').modal('toggle');
        if (vm.couponCode.length === 0) {
            showError("Promotion code is require");
            return;
        }
        showProgressBar();
        $http.get("/MakePayment/ApplyPromotionCode?id=" + vm.data.paymentId + "&code=" + vm.couponCode).then(function success(response) {
            hideProgressBar();
            vm.data.couponCode = response.data.couponCode;
            vm.data.discountRate = response.data.discountRate;
            vm.reCalculate();
        }, function error(response) {
                showError(response.data);
        });
    };
    vm.onRemoveCouponCode = function () {
        showProgressBar();
        $http.get("/MakePayment/RemovePromotionCode?id=" + vm.data.paymentId).then(function success(response) {
            hideProgressBar();
            vm.data.couponCode = null;
            vm.data.discountRate = null;
            vm.reCalculate();
        }, function error(response) {
            showError(response.data);
        });
    };
});