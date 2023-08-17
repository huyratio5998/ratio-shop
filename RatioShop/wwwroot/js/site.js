import * as LoginService from "./login/login.js";

// Use
const Init = () => {
  LoginService.AllLoginRegisterEvents();
  LoginService.ClientLogoutEvent();
};
Init();
