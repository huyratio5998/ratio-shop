import { RefreshCartView } from "../cart/cart-common.js";

// Generate client login popup
// call api
const GetLoginOrRegsiterPopupApi = async (isLoginPopup) => {
  const result = await fetch(
    `${baseApiUrl}/account/loginOrRegisterPopup?isLoginPopup=${isLoginPopup}`
  );
  if (result.ok) {
    return await result.json();
  }
  return null;
};

const CheckAuthenticated = async () => {
  const result = await fetch(`${baseApiUrl}/account/authenticated`);
  if (result.ok) {
    return await result.json();
  }
  return null;
};

// helper: update top bar header
const UpdateTopBarHeader = (isAuthenticated, userName) => {
  const topBarDesktop = document.querySelector(".js_top-bar-desktop");
  const topBarMobile = document.querySelector(".js_top-bar-mobile");
  const topBarDesktopAnonymous = `<a href="#" class="flex-c-m trans-04 p-lr-25 js_login-register-popup" data-popup-type="login">Login</a><a href="#" class="flex-c-m p-lr-10 trans-04 js_login-register-popup" data-popup-type="register">Register</a><a href="#" class="flex-c-m trans-04 p-lr-25">EN</a>`;
  const topBarMobileAnonymous = `<a href="#" class="flex-c-m p-lr-10 trans-04 js_login-register-popup" data-popup-type="login">Login</a><a href="#" class="flex-c-m p-lr-10 trans-04 js_login-register-popup" data-popup-type="register">Register</a> <a href="#" class="flex-c-m p-lr-10 trans-04">EN</a>`;
  const topBarDesktopAuthenticated = `<a href="/myaccount" class="flex-c-m trans-04 p-lr-25">Hello ${userName} </a><a href="#" class="flex-c-m trans-04 p-lr-25 js_logout-client"> Logout </a><a href="#" class="flex-c-m trans-04 p-lr-25">EN</a>`;
  const topBarMobileAuthenticated = `<a href="/myaccount" class="flex-c-m p-lr-10 trans-04">Hello ${userName} </a><a href="#" class="flex-c-m p-lr-10 trans-04 js_logout-client"> Logout </a><a href="#" class="flex-c-m p-lr-10 trans-04">EN</a>`;

  if (topBarDesktop) {
    if (isAuthenticated) topBarDesktop.innerHTML = topBarDesktopAuthenticated;
    else topBarDesktop.innerHTML = topBarDesktopAnonymous;
  }
  if (topBarMobile) {
    if (isAuthenticated) topBarMobile.innerHTML = topBarMobileAuthenticated;
    else topBarMobile.innerHTML = topBarMobileAnonymous;
  }
  // Add event
  if (isAuthenticated) ClientLogoutEvent();
  else AllLoginRegisterEvents();
};
const HideAuthenticationPopup = (isLoginPopup) => {
  if (isLoginPopup) {
    const loginModal = document.querySelector(".js-loginPopup");
    if (loginModal) loginModal.classList.remove("show-loginPopup");
  } else {
    const registerModal = document.querySelector(".js-registerPopup");
    if (registerModal) registerModal.classList.remove("show-loginPopup");
  }
};
const ResetFormInputContent = () => {
  const loginForm = document.querySelector("#js_login-form");
  const registerForm = document.querySelector("#js_register-form");

  const errorMessageEl = document.querySelector(
    ".js_authentication-popup-error-message"
  );
  if (loginForm) {
    loginForm.reset();
    loginForm.elements["rememberMe"].checked = true;
  }

  if (registerForm) {
    registerForm.reset();
    registerForm.elements["district"].innerHTML =
      "<option value=''>Select district...</option>";
  }

  if (errorMessageEl) errorMessageEl.innerHTML = "";
};
//

// external login-register event
const ExternalLoginOrRegisterEvent = () => {
  const currentForm = document.querySelector("#external-submit-form");
  const externalProviders = document.querySelectorAll(
    ".js_external-login-submit"
  );
  if (!externalProviders) return;

  externalProviders.forEach((el) => {
    el.addEventListener("click", async (e) => {
      e.preventDefault();

      currentForm.elements["provider"].value = el.getAttribute("value");
      currentForm.submit();
    });
  });
};

// event for register form
const RegisterFormEvent = () => {
  const citySelect = document.querySelector(".js_popup-city");
  const districtSelect = document.querySelector(".js_popup-district");

  const BuildDistrictSelect = (data) => {
    let options = ``;
    if (data.length == 0) return "<option value=''>Select district...</option>";
    data.forEach((element) => {
      const address2 = element.address2;
      options += `<option value="${address2}">${address2}</option>`;
    });
    return options;
  };

  const ResetDistrict = () => {
    districtSelect.innerHTML = "<option value=''>Select district...</option>";
  };

  const SelectCityEvent = () => {
    if (!citySelect) return;

    const cityChangeAction = async () => {
      const selectedValue = citySelect.value;
      var data = await GetAddressByTypeValue("Address1", selectedValue);
      ResetDistrict();
      if (!data) console.error("Fail add to cart");
      else {
        districtSelect.innerHTML = BuildDistrictSelect(data);
      }
    };

    // add event droplist
    Select2EventHelper(citySelect, cityChangeAction);
  };

  const RegisterEvent = () => {
    const registerBtn = document.querySelector("#register-button");
    registerBtn.addEventListener("click", async (e) => {
      e.preventDefault();

      const registerForm = document.querySelector("#js_register-form");
      const errorMessageEl = document.querySelector(
        ".js_authentication-popup-error-message"
      );
      const registerFormData = new FormData(registerForm);
      // validation data
      if (
        !registerFormData.get("userName") ||
        !registerFormData.get("password") ||
        !CheckRegexValidation(
          "^[a-zA-Z0-9]+$",
          registerFormData.get("userName")
        )
      ) {
        DisplayMessageInMoment(
          errorMessageEl,
          "Username or password can not be empty! Username not allow specific characters",
          "",
          5000
        );
        return;
      }
      // handle data
      registerFormData.set("isexternallogin", false);
      //
      const response = await fetch(`${baseApiUrl}/account/register`, {
        method: "POST",
        body: registerFormData,
      });

      if (!response.ok) {
        console.error("Login failure!");
      } else {
        const data = await response.json();
        if (data.status == "Success") {
          UpdateTopBarHeader(true, data.userName);
          RefreshCartView();
          HideAuthenticationPopup(false);
        } else {
          const errorMessage =
            "Your username or password is incorrect! Please try again!";
          DisplayMessageInMoment(errorMessageEl, errorMessage, "", 5000);
        }
      }
    });
  };
  // use
  SelectCityEvent();
  RegisterEvent();
  ExternalLoginOrRegisterEvent();
};
// event for login form
const LoginFormEvent = () => {
  const LoginEvent = () => {
    const loginBtn = document.querySelector("#login-button");
    loginBtn.addEventListener("click", async (e) => {
      e.preventDefault();

      const loginForm = document.querySelector("#js_login-form");
      const errorMessageEl = document.querySelector(
        ".js_authentication-popup-error-message"
      );
      const loginFormData = new FormData(loginForm);
      // validation data
      if (
        !loginFormData.get("userName") ||
        !loginFormData.get("password") ||
        !CheckRegexValidation("^[a-zA-Z0-9]+$", loginFormData.get("userName"))
      ) {
        DisplayMessageInMoment(
          errorMessageEl,
          "Username or password can not be empty! Username not allow specific characters",
          "",
          5000
        );
        return;
      }
      // handle data
      const rememberMe = loginFormData.get("rememberMe") == "on";
      loginFormData.set("rememberMe", rememberMe);
      loginFormData.set("isexternallogin", false);
      //
      const response = await fetch(`${baseApiUrl}/account/login`, {
        method: "POST",
        body: loginFormData,
      });

      if (!response.ok) {
        console.error("Login failure!");
      } else {
        const data = await response.json();
        if (data.status == "Success") {
          UpdateTopBarHeader(true, data.userName);
          RefreshCartView();
          HideAuthenticationPopup(true);
        } else {
          const errorMessage =
            "Your username or password is incorrect! Please try again!";
          DisplayMessageInMoment(errorMessageEl, errorMessage, "", 5000);
        }
      }
    });
  };

  // use
  LoginEvent();
  ExternalLoginOrRegisterEvent();
};
// event for logout
const ClientLogoutEvent = () => {
  const logoutBtns = document.querySelectorAll(".js_logout-client");
  if (!logoutBtns || logoutBtns.length <= 0) return;

  logoutBtns.forEach((e) => {
    e.addEventListener("click", async () => {
      const response = await fetch(`${baseApiUrl}/account/logout`);

      if (response.ok) {
        const data = await response.json();
        if (data) {
          if (CheckAllowAnonymousUrl()) {
            UpdateTopBarHeader(false, data.userName);
            RefreshCartView();
          } else return RedirectToPath(true);
        }
      }
    });
  });
};

const buildCityHtml = (data) => {
  if (!data || !data.listCities || data.listCities.length == 0)
    return "<option value=''>Select city...</option>";

  let options = `<option value=''>Select city...</option>`;
  data.listCities.forEach((element) => {
    options += `<option value="${element}">${element}</option>`;
  });
  return options;
};
// build html address for popup
const BuildAddressComponentRegister = (data, isLoginPopup) => {
  if (!isLoginPopup) {
    const sharedComponentArea = document.querySelector(
      ".js_address-shared-component"
    );
    if (sharedComponentArea) {
      const cityElement = document.querySelector(".js_popup-city");
      if (!cityElement) return;

      cityElement.innerHTML = buildCityHtml(data);
    }
  }
};
// build html popup
const BuildLoginPopup = (data, isShowImage, isLoginPopup) => {
  // show popup
  if (!data) return "";

  const popupTitle = isLoginPopup ? "Sign In to" : "Register to";
  const loginImage = isShowImage
    ? `  <div class="col-md-6 order-md-2"><img src="/login/images/undraw_file_sync_ot38.svg" alt="Image" class="img-fluid"></div>`
    : "";

  // build external login html
  let externalLoginsHtml = "";
  if (data.externalLogins && data.externalLogins.length > 0) {
    const prefixHtml = `<form method="post" id="external-submit-form" action="/api/account/externallogin?returnUrl=${window.location.pathname}"><div class="social-login text-center"><input type="hidden" value="" name="provider"/>
                          `;
    const posFixHtml = `</div></form>`;
    let externalLoginItemsHtml = "";
    data.externalLogins.forEach((e) => {
      const providerName = e.toLowerCase();
      externalLoginItemsHtml += `<a href="#" type="submit" class="${providerName} mouse-hover js_external-login-submit" value="${e}"><span class="icon-${providerName} mr-3"></span></a> `;
    });
    externalLoginsHtml = prefixHtml + externalLoginItemsHtml + posFixHtml;
  }

  const popupForm = `<form action="/" method="post" id="js_login-form">
        <div class="form-group first">
          <label for="username">Username</label>
          <input type="text" name="userName" class="form-control" />
        </div>
        <div class="form-group last mb-4">
          <label for="password">Password</label>
          <input type="password" name="password" class="form-control" />
        </div>
        <div class="d-flex mb-5 align-items-center">
          <label class="control control--checkbox mb-0">
            <span class="caption">Remember me</span>
            <input type="checkbox" name="rememberMe" checked="checked" />
            <div class="control__indicator"></div>
          </label>
          <span class="ml-auto"><a href="#" class="ratio-forgot-pass">Forgot Password</a></span>
        </div>
        <div class="text-danger js_authentication-popup-error-message mb-2"></div>
        <input type="submit" value="Log In" id="login-button" class="login-popup-btn-login btn btn-pill text-white btn-block btn-primary mouse-hover" />
      </form>
      <span class="d-block text-center my-4 text-muted"> or sign in with</span>
      ${externalLoginsHtml}
      <div class="text-center">
        <a href="#" class="js_switch-to-register-popup" title="Click here to register an account">Don't have an account?</a>
      </div>`;

  // build popup html
  const loginHtml = `<!-- Style -->
    <link rel="stylesheet" href="/login/fonts/icomoon/style.css">
    <link rel="stylesheet" href="/login/css/style.css">

    <!-- Login Popup -->
    <div class="wrap-modal1 js-loginPopup p-t-60 p-b-20" data-is-login-popup="${isLoginPopup}">
       <div class="overlay-modal1 js-hide-loginPopup"></div>
       <div class="login-popup-container">
          <div class="bg-login-popup p-t-60 p-b-30 p-lr-15-lg how-pos3-parent">
             <button class="how-pos3 hov3 trans-04 js-hide-loginPopup">
             <img src="/images/icons/icon-close.png" alt="CLOSE">
             </button>
             <div class="row justify-content-center">
               ${loginImage}
                <div class="col-md-6 contents">
                   <div class="row justify-content-center">
                      <div class="col-md-12">
                         <div class="form-block">
                            <div class="mb-4">
                               <h3>${popupTitle} <strong>${data.storeName}</strong></h3>
                               <p class="mb-4">${data.description}</p>
                            </div>
                            ${popupForm}
                         </div>
                      </div>
                   </div>
                </div>
             </div>
          </div>
       </div>
    </div>`;

  return loginHtml;
};

const BuildRegisterPopup = (data, isShowImage) => {
  // show popup
  if (!data) return;

  const popupTitle = "Register to";
  const loginImage = isShowImage
    ? `  <div class="col-md-6 order-md-2"><img src="/login/images/undraw_file_sync_ot38.svg" alt="Image" class="img-fluid"></div>`
    : "";

  // build external login html
  let externalLoginsHtml = "";
  if (data.externalLogins && data.externalLogins.length > 0) {
    const prefixHtml = `<form method="post" id="external-submit-form" action="/api/account/externallogin?returnUrl=${window.location.pathname}"><div class="social-login text-center"><input type="hidden" value="" name="provider"/>`;

    const posFixHtml = `</div></form>`;
    let externalLoginItemsHtml = "";
    data.externalLogins.forEach((e) => {
      const providerName = e.toLowerCase();
      externalLoginItemsHtml += `<a href="#" type="submit" class="${providerName} mouse-hover js_external-login-submit" value="${e}"><span class="icon-${providerName} mr-3"></span></a> `;
    });
    externalLoginsHtml = prefixHtml + externalLoginItemsHtml + posFixHtml;
  }

  // append to html
  //login image
  const loginImageArea = document.querySelector(".js_login-image");
  if (loginImage) loginImageArea.innerHTML = loginImage;
  //register header
  const registerHeaderArea = document.querySelector(
    ".js_register-header-popup"
  );
  if (registerHeaderArea)
    registerHeaderArea.innerHTML = `<h3>${popupTitle} <strong>${data.storeName}</strong></h3><p class="mb-4">${data.description}</p>`;
  //external login
  const externalLoginArea = document.querySelector(".js_external-login-html");
  if (externalLoginArea) externalLoginArea.innerHTML = externalLoginsHtml;
};

// wrap api call and => generate popup.
const GenerateLoginOrRegisterPopup = async (isShowImage, isLoginPopup) => {
  const modalArea = document.querySelector(".js_login-popup-area");
  const modalRegisterArea = document.querySelector(".js_register-popup-area");

  if (!modalArea || !modalRegisterArea) return;

  const loginModal = document.querySelector(".js-loginPopup");
  const registerModal = document.querySelector(".js-registerPopup");

  // handle register reopen
  if (
    !isLoginPopup &&
    registerModal &&
    registerModal.dataset.registerFirstLoad != "true"
  ) {
    if (loginModal) modalArea.innerHTML = "";
    ResetFormInputContent();
    registerModal.classList.add("show-loginPopup");
    return;
  }
  // handle login reopen
  if (loginModal && isLoginPopup) {
    if (registerModal) registerModal.classList.remove("show-loginPopup");
    ResetFormInputContent();
    loginModal.classList.add("show-loginPopup");
    return;
  }

  // call api get popup info
  const data = await GetLoginOrRegsiterPopupApi(isLoginPopup);
  if (!data) {
    console.error("Fail to load login popup!");
    return;
  }

  if (isLoginPopup) {
    // hide register form
    registerModal.classList.remove("show-loginPopup");

    // show login popup
    modalArea.innerHTML = BuildLoginPopup(data, isShowImage, isLoginPopup);
    const loginModal = document.querySelector(".js-loginPopup");
    loginModal.classList.add("show-loginPopup");
    LoginOrRegisterUIEvents(registerModal, isLoginPopup);
  } else {
    // hide login
    modalArea.innerHTML = "";

    // show register popup
    BuildRegisterPopup(data, isShowImage);
    BuildAddressComponentRegister(data, isLoginPopup);
    registerModal.classList.add("show-loginPopup");
    // add events
    LoginOrRegisterUIEvents(registerModal, isLoginPopup);
    registerModal.dataset.registerFirstLoad = false;
  }
};

const CheckIsAuthenticated = async () => {
  const data = await CheckAuthenticated();
  return data;
};
//events
// Login or register UI events
const LoginOrRegisterUIEvents = (registerModal, isLoginPopup) => {
  // add event
  // switch register or switch login button
  const isAddEventForRegisterAction =
    registerModal.dataset.registerFirstLoad.toLowerCase() == "true" &&
    !isLoginPopup;

  if (isLoginPopup || isAddEventForRegisterAction) {
    const switchPopupButton = isLoginPopup
      ? document.querySelector(".js_switch-to-register-popup")
      : document.querySelector(".js_switch-to-login-popup");
    if (switchPopupButton) {
      switchPopupButton.addEventListener("click", (e) => {
        e.preventDefault();
        GenerateLoginOrRegisterPopup(false, !isLoginPopup);
      });
    }
    // hide popup
    const hidePopups = document.querySelectorAll(".js-hide-loginPopup");
    if (!hidePopups) return;

    hidePopups.forEach((el) => {
      el.addEventListener("click", () => {
        HideAuthenticationPopup(isLoginPopup);
      });
    });
    // separate event
    if (!isLoginPopup) RegisterFormEvent();
    else {
      LoginFormEvent();
    }
  }
};
// add event to all button.
const AllLoginRegisterEvents = () => {
  const getAllLoginRegisterButtons = document.querySelectorAll(
    ".js_login-register-popup"
  );
  if (getAllLoginRegisterButtons) {
    getAllLoginRegisterButtons.forEach((el) => {
      el.addEventListener("click", () => {
        const isLoginPopup = el.dataset.popupType == "login";
        GenerateLoginOrRegisterPopup(false, isLoginPopup);
      });
    });
  }
};

export {
  ClientLogoutEvent,
  AllLoginRegisterEvents,
  GenerateLoginOrRegisterPopup,
  CheckIsAuthenticated,
};
