var App = App || {};
(function () {

    var appLocalizationSource = abp.localization.getSource('OTel');
    App.localize = function () {
        return appLocalizationSource.apply(this, arguments);
    };

})(App);