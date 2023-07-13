const baseApiUrl = `${window.location.origin}/api`;
const defaultRedirectPathLocation = "/products";
const regexAllWhiteSpace = /^(?=\s*$)/;

let VNDong = new Intl.NumberFormat("vi-VN", {
  style: "currency",
  currency: "VND",
});

const IsNullOrEmptyString = (checkValue) => {
  const regex = new RegExp(regexAllWhiteSpace);
  const result = regex.test(checkValue);
  return !checkValue || result;
};

const ConvertToLocalDate = () => {
  const elements = document.querySelectorAll(".js_date-format-local");
  if (!elements) return;

  elements.forEach((el) => {
    const date = el.dataset.dateValue;
    if (date) {
      el.innerHTML = new Date(date).toLocaleDateString("en-GB");
    }
  });
};

const DisplayMessageInMoment = (element, message, className, timeOut) => {
  if (!element) return;

  element.innerHTML = message;
  if (className) element.classList.add(className);
  setTimeout(() => {
    element.innerHTML = "";
    if (className) element.classList.remove(className);
  }, timeOut);
};

const CheckRegexValidation = (pattern, inputValue) => {
  const regex = new RegExp(pattern);
  if (regex.test(inputValue)) return true;
  return false;
};

const RedirectToPath = (isToDefaultPage = false, pathName = "") => {
  let destinationPathUrl = defaultRedirectPathLocation;
  if (!isToDefaultPage) destinationPathUrl = pathName;

  window.location.pathname = destinationPathUrl;
};

const CheckAllowAnonymousUrl = () => {
  const anonymousUrlPath = ["/", "", "/products", "/products/*"];
  const currentUrlPath = window.location.pathname;
  let result = false;

  anonymousUrlPath.forEach((val) => {
    if (val.includes("*")) {
      const value = val.replace("*", "");
      if (currentUrlPath.includes(value)) result = true;
    } else if (val.includes(currentUrlPath)) result = true;
  });

  return result;
};
const RemoveLastPart = (url) => {
  return url.slice(0, url.lastIndexOf("/"));
};
const GetLastSegment = (url) => {
  return url.substring(currentPath.lastIndexOf("/") + 1);
};

// Mutation observer check class contains
const MutationObserverClassChangeHelper = (
  className,
  action,
  param1 = null,
  param2 = null,
  param3 = null,
  isContainsClass = true
) => {
  let dropdownChangeTimes = 0;
  const attrObserver = new MutationObserver((mutations) => {
    mutations.forEach((mu) => {
      if (mu.type !== "attributes" && mu.attributeName !== "class") return;

      const listClasses = mu.target.getAttribute("class");
      const validateCondition = isContainsClass
        ? listClasses.includes(className)
        : !listClasses.includes(className);

      if (validateCondition) {
        if (++dropdownChangeTimes == 1) action(param1, param2, param3);
      } else dropdownChangeTimes = 0;
    });
  });
  return attrObserver;
};

const ChangeCurrentUrlWithoutReload = (nextURL) => {
  const nextTitle = document.title;
  const nextState = { additionalInformation: "Updated the URL with JS" };
  window.history.pushState(nextState, nextTitle, nextURL);
};
// Api
const GetAddressByTypeValue = async (type, value) => {
  const result = await fetch(
    `${baseApiUrl}/address/getaddress?type=${type}&value=${value}`
  );
  if (result.ok) {
    return await result.json();
  }
  return null;
};

const GetAddressByType = async (type) => {
  const result = await fetch(
    `${baseApiUrl}/address/getaddressbytype?type=${type}`
  );
  if (result.ok) {
    return await result.json();
  }
  return null;
};

// Events

// Change city
const SelectCityEvent = (cityElement, districtElement) => {
  if (!cityElement || !districtElement) return;

  const ResetDistrict = () => {
    districtElement.innerHTML = "<option value=''>Select district...</option>";
  };

  const BuildDistrictSelect = (data) => {
    let options = ``;
    if (data.length == 0) return "<option value=''>Select district...</option>";
    data.forEach((element) => {
      const address2 = element.address2;
      options += `<option value="${address2}">${address2}</option>`;
    });
    return options;
  };

  const cityChangeAction = async () => {
    const selectedValue = cityElement.value;
    var data = await GetAddressByTypeValue("Address1", selectedValue);
    //reset district
    ResetDistrict();

    if (!data) console.error("Fail to get address");
    else {
      districtElement.innerHTML = BuildDistrictSelect(data);
    }
  };

  // add event droplist
  Select2EventHelper(cityElement, cityChangeAction);
};

// Drop list event helper
const Select2EventHelper = (
  element,
  func,
  param1 = null,
  param2 = null,
  param3 = null
) => {
  const targetElement = element.nextElementSibling;
  if (!targetElement) return;

  // init mutation
  const checkSelectClass = "select2-container--open";
  const dropListObserver = MutationObserverClassChangeHelper(
    checkSelectClass,
    func,
    param1,
    param2,
    param3,
    false
  );

  // add event obsever dropdown change.
  dropListObserver.observe(targetElement, {
    attributes: true,
    childList: false,
    subtree: false,
    attributeFilter: ["class"],
  });
};
