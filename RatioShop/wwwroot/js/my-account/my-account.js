const btnUpdatePersonalInformation = document.querySelector(
  ".js_updatePersonalInfo"
);
const fullNameElement = document.querySelector(".js_personalInfoFullName");
const addressElement = document.querySelector(".js_personalInfoAddress");
const phoneNumberElement = document.querySelector(
  ".js_personalInfoPhoneNumber"
);

const myAccountAddressComponent = document.querySelector(
  ".js_myAccountAddressComponent"
);
const pesonalInfoDisplayArea = document.querySelector(".js_personalInfoArea");
const pesonalInfoFormArea = document.querySelector(".js_personalInfoFormArea");
const pesonalInfoForm = document.querySelector("#js_personalInfoForm");
const btnCancelUpdate = document.querySelector(".js_cancelUpdateMyAccountInfo");
const btnSaveInfo = document.querySelector(".js_submitUpdatePersonalInfo");

const GetPersonalInfoApi = async () => {
  const result = await fetch(`${baseApiUrl}/account/getPersonalInformation`);
  if (result.ok) {
    return await result.json();
  }
  return null;
};

// Events

const ChangeCityEvent = () => {
  if (!myAccountAddressComponent) return;

  const cityElement =
    myAccountAddressComponent.querySelector(".js-select-city");
  const districtElement = myAccountAddressComponent.querySelector(
    ".js-select-district"
  );
  if (!cityElement || !districtElement) return;

  // add event when change city
  SelectCityEvent(cityElement, districtElement);
};

const RefreshPersonalInfoDisplay = async () => {
  if (
    !pesonalInfoDisplayArea ||
    !fullNameElement ||
    !addressElement ||
    !phoneNumberElement
  )
    return;

  var data = await GetPersonalInfoApi();
  if (!data) {
    console.error("Can not get latest personal data");
    return;
  }

  // update new data
  fullNameElement.innerHTML = data.fullName;
  phoneNumberElement.innerHTML = data.phoneNumber;
  addressElement.innerHTML = data.address.addressDetail;

  pesonalInfoFormArea.classList.add("ratio-hidden");
  pesonalInfoDisplayArea.classList.remove("ratio-hidden");
};

const UpdatePersonalInfoSubmit = () => {
  if (!pesonalInfoForm || !btnSaveInfo) return;

  btnSaveInfo.addEventListener("click", async (e) => {
    e.preventDefault();

    const errorMessageEl = document.querySelector(
      ".js_personal-info-error-message"
    );
    const personalInfoData = new FormData(pesonalInfoForm);
    const response = await fetch(
      `${baseApiUrl}/account/updatePersonalInformation`,
      {
        method: "POST",
        body: personalInfoData,
      }
    );

    if (!response.ok) {
      const errorMessage = "Fail to update personal info!";
      DisplayMessageInMoment(errorMessageEl, errorMessage, "", 5000);
    } else {
      const data = await response.json();
      if (data) {
        swal("Success", `Your personal info has been updated!`, "success").then(
          (value) => {
            RefreshPersonalInfoDisplay();
          }
        );
      } else {
        const errorMessage = "Fail to update personal info!";
        DisplayMessageInMoment(errorMessageEl, errorMessage, "", 5000);
      }
    }
  });
};

const ToggleFormUpdatePersonalInformationEvent = () => {
  if (!btnUpdatePersonalInformation || !btnCancelUpdate) return;

  btnUpdatePersonalInformation.addEventListener("click", async () => {
    if (!pesonalInfoFormArea) return;

    pesonalInfoDisplayArea.classList.add("ratio-hidden");
    pesonalInfoFormArea.classList.remove("ratio-hidden");
  });

  btnCancelUpdate.addEventListener("click", () => {
    if (!pesonalInfoFormArea) return;
    pesonalInfoFormArea.classList.add("ratio-hidden");
    pesonalInfoDisplayArea.classList.remove("ratio-hidden");
  });
};

const Init = () => {
  console.log("Init event my account");
  ChangeCityEvent();
  ToggleFormUpdatePersonalInformationEvent();
  UpdatePersonalInfoSubmit();
};

Init();
