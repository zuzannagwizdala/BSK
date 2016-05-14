loading = false;

    LogInZapytanie = function(login, password, rola){
        this.login = login;
        this.password = password;
        this.rola = rola;
    };

    function sendForm() {

        
        debugger;
        var login = document.getElementById("text");
        var password = document.getElementById("pwd");

        var logInReq = new LogInZapytanie(login, password, null);
        var postData = { values: logInReq };

        $.ajax({
            type: "POST",
            url: '@Url.Action("Post","LogInView")',
            data: postData,
            success: function (data) {
                alert(data.Result);
            },
            dataType: "HttpResponseMessage",
            traditional: true
        });
    };
