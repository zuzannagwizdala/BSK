loading = false;

    form = {
        login: '',
        password: ''
    };
    

    function sendForm() {
        console.log("weszlo3");
        loading = true;
        errorMessage = '';
        if (document.getElementById('text') && document.getElementById('pwd')) {     //&& form.role
            console.log("weszlo");
            $http.post('~/Controllers/LogInLogic', {
                login: document.getElementById('text'),
                password: document.getElementById('pwd'),
                role: ''
            })
                .then(function (res) {
                    res.data.Login = form.login;
                    res.data.Role = { Name: form.role.Name, Id: form.role.Id };
                    UserService.setLoggedUser(res);
                    $location.url('/');
                    loading = false;
                })
                .catch(function (err) {
                    errorMessage = err.data.Message || 'Niepoprawne dane!';
                    loading = false;
                });
        } else if (form.login && form.password) {
            console.log("weszlo2");
            $http.post('~/Controllers/LogInLogic', {
                login: form.login,
                password: form.password,
                role: null
            })
                .then(function (res) {
                    roles = res.data;
                    form.role = roles[0];
                    loading = false;
                })
                .catch(function (err) {
                    errorMessage = err.data.Message || 'Niepoprawne dane!';
                    loading = false;
                });
        }
    };
