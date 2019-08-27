(function () {
    function request(method, url, dataType, contentType, headers, data, success, error, complete) {
        $.ajax({
            url: url,
            type: method,
            dataType: dataType,
            contentType: contentType,
            headers: headers || {},
            data: JSON.stringify(data),
            success: function (data) {
                success && success(data);
            },
            error: function (jqXhr, err, msg) {
                error && error(JSON.parse(jqXhr.responseText).error_description);

            },
            complete: complete
        });
    }

    function setJwt(key) {
        swaggerUi.api.clientAuthorizations.authz = {};
        swaggerUi.api.clientAuthorizations.add("key", new SwaggerClient.ApiKeyAuthorization("token", key, "header"));
    }

    function getJwt() {
        var authz = swaggerUi.api.clientAuthorizations.authz;
        if (authz && authz.key && authz.key.value) {
            return authz.key.value;
        }

        return null;
    }

    $(function () {
        (function () {
            var styles = `
                <style>
                #sa-setting{
                    display:inline-block;
                    position: absolute;
                    background: #89BF04;
                    right: 20px;
                    top: 48px;
                    padding: 10px;
                    box-shadow: 0 2px 1px rgba(0,0,0,0.5);
                    display: none;
                }
                 .sa-btn{
                    display: inline-block;
                    color: #FFF;
                    font-weight: bold;
                    background: #547F00;
                    border-radius: 3px;
                    padding: 6px 8px;
                    font-family: "Droid Sans", sans-serif;
                    font-size: 0.9em;
                    cursor: pointer;
                }
                #sa-setting input{
                    margin-bottom: 5px;
                    padding: 3px;
                    border: 2px inset;
                }
            </style>
            `;
            $('head').append(styles);
            var settingTemplate = `
                <div id="sa-setting">
                    Login: <input id="sa-path" placeholder="Path" value="/api/v1/commands/login">
                    <br>
                    Logout: <input id="sa-logout-path" placeholder="Path" value="/api/v1/commands/logout">
                    <br>
                    <input id="sa-username" placeholder="Username">
                    <br>
                    <input id="sa-password" type="password" placeholder="Password">
                    <br>
                    <span id="sa-btn-login" class="sa-btn">Login</span>
                    <span id="sa-btn-logout" class="sa-btn" style="display: none">Logout</span>
                </div>
            `;
            $('body').append(settingTemplate);

            $('<div id="sa-btn-setting" class="sa-btn">Login</div>')
                .click(function () {
                    $('#sa-setting').fadeToggle();
                })
                .appendTo('#api_selector');

            $("#input_apiKey").hide();
        })();


        function login(loginData) {
            $('#sa-btn-setting').text('Working...');

            function success(res) {
                $('#sa-btn-setting').text('Hi ' + loginData.user).css('background', '#0f6ab4');
                $('#sa-setting').fadeOut();
                $('#sa-btn-logout').fadeIn();
                setJwt(res.Token);
            }

            function error(err) {
                $('#sa-btn-setting').text('Failed').css('background', '#a41e22');
                setJwt('');
                alert(err);
            }

            request("POST", loginData.path, "json", "application/json;charset=utf-8", null, {
                userId: loginData.user,
                password: loginData.pass
            }, success, error, null);
        }

        function logout(success, error, complete) {
            var url = $("#sa-logout-path").val();
            var token = getJwt();

            if (token) {
                request("POST", url, "json", "application/json;charset=utf-8", {
                    "token": token
                }, null, success, error, complete);
            }
        };

        $('#sa-btn-logout').click(function () {
            logout(null, null, function () {
                setJwt('');
                window.localStorage.removeItem('sa-login-data');
                $('#sa-username').val('');
                $('#sa-password').val('');
                $(this).fadeOut();
            });
        });

        $('#sa-btn-login').click(function () {
            var loginData = {
                path: $('#sa-path').val(),
                user: $('#sa-username').val(),
                pass: $('#sa-password').val()
            };
            window.localStorage.setItem('sa-login-data', JSON.stringify(loginData));
            login(loginData);
        });

        window.addEventListener("beforeunload", function () {
            logout();
        });

        //Auto login
        (function () {
            var oldLoginData = window.localStorage.getItem('sa-login-data');
            if (oldLoginData) {
                oldLoginData = JSON.parse(oldLoginData);
                $('#sa-path').val(oldLoginData.path);
                $('#sa-username').val(oldLoginData.user);
                $('#sa-password').val(oldLoginData.pass);
                login(oldLoginData);
            }

        })();

    });
})();
