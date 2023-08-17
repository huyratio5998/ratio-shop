import { RefreshCartView } from "./cart-common.js";
const citySelect = document.querySelector(".js-select-city");
const districtSelect = document.querySelector(".js-select-district");
const btnUpdateShippingAddress = document.querySelector(
  ".js_updateShippingAddress"
);
const addressDetailInput = document.querySelector(".js-addressDetail");
const fullnameInput = document.querySelector(".js-fullName");
const phoneNumberInput = document.querySelector(".js-phoneNumber");
const shippingFee = document.querySelector(".js_cart-detail-shippingFee");
const btnApplyCoupon = document.querySelector(".js_apply-coupon");
const btnProceedCheckout = document.querySelector(".js_proceed-checkout");
const btnChangeDefaultAddrerss = document.querySelector(
  ".js_change-shipping-address"
);
const btnCancelChangeDefaultAddrerss = document.querySelector(
  ".js_cancelUpdateShippingAddress"
);

const UpdateCartShippingAddress = async (
  city,
  district,
  addressDetail,
  fullName,
  phoneNumber
) => {
  const result = await fetch(
    `${baseApiUrl}/address/updatecartshippingaddress?city=${city}&district=${district}&addressDetail=${addressDetail}&fullName=${fullName}&phoneNumber=${phoneNumber}`
  );
  if (result.ok) {
    return await result.json();
  }
  return null;
};

const ApplyCouponToCart = async (coupon) => {
  const result = await fetch(
    `${baseApiUrl}/cart/applycouponcode?couponcode=${coupon}`
  );
  if (result.ok) {
    return await result.json();
  }
  return null;
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

const ResetDistrict = () => {
  districtSelect.innerHTML = "<option value=''>Select district...</option>";
};

const ToggleShippingAddressView = (isShowFormUpdateShippingAddress) => {
  const defaultShippingAddressArea = document.querySelector(
    ".js_default-shipping-address"
  );
  const formUpdateShippingAddressArea = document.querySelector(
    ".js_update-shipping-addess-area"
  );
  if (!defaultShippingAddressArea || !formUpdateShippingAddressArea) return;

  if (isShowFormUpdateShippingAddress) {
    formUpdateShippingAddressArea.classList.remove("ratio-hidden");
    defaultShippingAddressArea.classList.add("ratio-hidden");
  } else {
    formUpdateShippingAddressArea.classList.add("ratio-hidden");
    defaultShippingAddressArea.classList.remove("ratio-hidden");
  }
};

//Event

const SelectCityEvent = () => {
  if (!citySelect) return;

  const cityChange = async () => {
    const selectedValue = citySelect.value;
    var data = await GetAddressByTypeValue("Address1", selectedValue);
    ResetDistrict();
    if (!data) console.error("Fail to get address");
    else {
      districtSelect.innerHTML = BuildDistrictSelect(data);
    }
  };

  // add event change city drop list
  Select2EventHelper(citySelect, cityChange, null, null, null);
};

const UpdateShippingAddressEvent = () => {
  if (!btnUpdateShippingAddress) return;

  btnUpdateShippingAddress.addEventListener("click", async (e) => {
    const updateShippingMessageElement = document.querySelector(
      ".js_updateShippingAddressMessage"
    );
    const successMessage = document.querySelector(
      ".js_updateShippingAddressSuccessMessage"
    );
    const selectedCity = citySelect.value;
    const selectedDistrict = districtSelect.value;
    const addressDetail = addressDetailInput.value;
    const fullname = fullnameInput.value;
    const phoneNumber = phoneNumberInput.value;

    if (!selectedCity || !selectedDistrict) {
      successMessage.innerHTML = "";
      DisplayMessageInMoment(
        updateShippingMessageElement,
        "Fail to upload. Please select the city and district!",
        "coupon-update-failure",
        5000
      );
      return;
    }
    if (
      IsNullOrEmptyString(addressDetail) ||
      IsNullOrEmptyString(fullname) ||
      IsNullOrEmptyString(phoneNumber)
    ) {
      successMessage.innerHTML = "";
      DisplayMessageInMoment(
        updateShippingMessageElement,
        "Fail to upload. Missing shipping information!",
        "coupon-update-failure",
        5000
      );
      return;
    }

    const data = await UpdateCartShippingAddress(
      selectedCity,
      selectedDistrict,
      addressDetail,
      fullname,
      phoneNumber
    );
    if (data == null || !data) {
      successMessage.innerHTML = "";
      DisplayMessageInMoment(
        updateShippingMessageElement,
        "Error when update shipping address!",
        "coupon-update-failure",
        5000
      );
      btnProceedCheckout.dataset.shipmentValidation = false;
    } else {
      // update shipping fee
      btnProceedCheckout.dataset.shipmentValidation = true;
      shippingFee.dataset.shippingFee = +data;
      shippingFee.innerHTML = `${VNDong.format(+data)}`;

      // update new price
      RefreshCartView();
      ToggleShippingAddressView(false);

      updateShippingMessageElement.innerHTML = "";
      DisplayMessageInMoment(
        successMessage,
        "Address has been updated successfully!",
        "coupon-update-success",
        3000
      );
    }
  });
};

const ApplyCouponEvent = () => {
  if (!btnApplyCoupon) return;

  btnApplyCoupon.addEventListener("click", async (e) => {
    // disable while call api
    btnApplyCoupon.classList.add("ratio-disable-button");
    const couponEl = document.querySelector(".js_coupon-input");
    const coupon = document.querySelector(".js_coupon-input").value;
    const cartMessage = document.querySelector(".js_apply-coupon-message");
    if (!couponEl || !coupon) return;

    var data = await ApplyCouponToCart(coupon);

    if (data == null || !data) {
      DisplayMessageInMoment(
        cartMessage,
        "Coupon incorrect. Please try another coupon!",
        "coupon-update-failure",
        3000
      );
      btnApplyCoupon.classList.remove("ratio-disable-button");
    } else {
      RefreshCartView();
      DisplayMessageInMoment(
        cartMessage,
        "Coupon applied successfully!",
        "coupon-update-success",
        3000
      );
      couponEl.value = "";
      btnApplyCoupon.classList.remove("ratio-disable-button");
    }
  });
};

const ChangeDefaultAddressEvents = () => {
  // event change city
  SelectCityEvent();
  // event when save new address
  UpdateShippingAddressEvent();

  // buton change event
  if (!btnChangeDefaultAddrerss) return;
  btnChangeDefaultAddrerss.addEventListener("click", () => {
    ToggleShippingAddressView(true);
  });
  // button cancel event
  if (!btnCancelChangeDefaultAddrerss) return;
  btnCancelChangeDefaultAddrerss.addEventListener("click", () => {
    ToggleShippingAddressView(false);
  });
};

const Init = () => {
  ApplyCouponEvent();
  ChangeDefaultAddressEvents();
};

Init();
