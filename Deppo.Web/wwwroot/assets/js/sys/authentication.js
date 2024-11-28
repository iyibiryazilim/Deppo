"use strict";
var KTSigninGeneral = function () {

    var e, t, i; return {
        init: function () {

            e = document.querySelector("#kt_sign_in_form"),
                t = document.querySelector("#kt_sign_in_submit"),
                i = FormValidation.formValidation(e, {
                    fields: {
                        UserName: {
                            validators: { notEmpty: { message: "Kullanıcı bilgisi boş geçilemez" } }
                        },
                        Password: {
                            validators: { notEmpty: { message: "Parola bilgisi boş geçilemez" } }
                        }
                    },
                    plugins: {
                        trigger: new FormValidation.plugins.Trigger,
                        bootstrap: new FormValidation.plugins.Bootstrap5({
                            rowSelector: ".fv-row", eleInvalidClass: "", eleValidClass: ""
                        })
                    }
                }),

                t.addEventListener("click", (function (n) {
                    n.preventDefault(), i.validate().then((function (i) {

                        if (i == "Valid") {
                            t.setAttribute("data-kt-indicator", "on");
                            t.disabled = true;

                            setTimeout(function () {
                                var postUrl = 'Authentication/Authenticate';
                                $.post(postUrl, {
                                    username: e.querySelector('[name="username"]').value,
                                    password: e.querySelector('[name="password"]').value
                                },
                                    function (authResult) {
                                        t.removeAttribute("data-kt-indicator");
                                        t.disabled = false;
                                        if (authResult.result) {
                                            e.querySelector('[name="username"]').disabled = true;
                                            e.querySelector('[name="password"]').disabled = true;
                                            e.querySelector('[name="login"]').disabled = true;
                                            if (authResult.isFirst) {
                                                location.href = "../ChangePassword"
                                            } else {
                                                location.href = "../Home"
                                            }
                                            //var redirectUrl = e.getAttribute("data-kt-redirect-url");
                                            //if (redirectUrl) {
                                            //    location.href = redirectUrl;
                                            //}
                                        } else {
                                            Swal.fire({
                                                text: "Giriş işlemi başarısız, Lütfen bilgilerinizi kontrol ederek tekrar deneyiniz!",
                                                icon: "warning",
                                                buttonsStyling: false,
                                                confirmButtonText: "Tamam, Tekrar Dene!",
                                                customClass: {
                                                    confirmButton: "btn btn-primary"
                                                },
                                                charset: 'UTF-8'
                                            });
                                        }
                                    });
                            }, 2000);
                        }
                        else {
                            Swal.fire({
                                text: "Lütfen zorunlu alanları doldurduğunuzdan emin olunuz!",
                                icon: "error",
                                buttonsStyling: !1,
                                confirmButtonText: "Tamam!",
                                customClass: {
                                    confirmButton: "btn btn-primary"
                                },


                            })
                        }
                    }))
                }))
        }
    }
}();
KTUtil.onDOMContentLoaded((function () {
    KTSigninGeneral.init()
}));