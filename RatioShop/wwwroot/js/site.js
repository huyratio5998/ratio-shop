import * as LoginService from "./login/login.js";

// Use
const Init = () => {
  console.log("outside: site.js");
  LoginService.AllLoginRegisterEvents();
  LoginService.ClientLogoutEvent();
};
Init();
