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
const myAccountContentArea = document.querySelector(".js_MyAccountContentArea");

const GetPersonalInfoApi = async () => {
  const result = await fetch(`${baseApiUrl}/account/getPersonalInformation`);
  if (result.ok) {
    return await result.json();
  }
  return null;
};

const GetOrderHistoryApi = async () => {
  const result = await fetch(`${baseApiUrl}/account/getOrderHistory`);
  if (result.ok) {
    return await result.json();
  }
  return null;
};

const GetOrderDetailApi = async (id) => {
  const result = await fetch(`${baseApiUrl}/order/detail?id=${id}`);
  if (result.ok) {
    return await result.json();
  }
  return null;
};
//

const BuildTableItems = (data) => {
  const cartItems = data.cartDetail.cartItems;
  if (!cartItems || cartItems.length == 0) return "";

  let itemsHtml = `<tr class="table_head">
                      <th class="column-1">Product</th>
                      <th class="column-2"></th>
                      <th class="column-3">Price</th>
                      <th class="column-4">Quantity</th>
                      <th class="column-5">Total</th>
                  </tr>`;
  cartItems.forEach((el) => {
    const itemImage = el.image
      ? `/images/products/${el.image}`
      : "/images/default-placeholder.jpg";
    const itemRow = `<tr class="table_row">
                        <td class="column-1">
                            <div class="how-itemcart1" data-variant-id="${
                              el.variantId
                            }">
                                <img src="${itemImage}" alt="${el.name} ${
      el.variableName
    }">
                            </div>
                        </td>
                        <td class="column-2"><a href="/products/productdetail/${
                          el.variantId
                        }" class="header-cart-item-name m-b-18 hov-cl1 trans-04">${
      el.name
    } - ${el.variableName}</a></td>
                        <td class="column-3">${VNDong.format(el.price)}</td>
                        <td class="column-4">
                            <div class="wrap-num-product flex-w m-l-auto m-r-0" data-variant-id="${
                              el.variantId
                            }">
                                <div class="ratio-btn-num-product-down cl8 hov-btn3 trans-04 flex-c-m">
                                    <i class="fs-16 zmdi zmdi-minus"></i>
                                </div>
                                <input class="mtext-104 cl3 txt-center num-product" disabled type="number" name="num-product1" value="${
                                  el.number
                                }">

                                <div class="ratio-btn-num-product-up cl8 hov-btn3 trans-04 flex-c-m">
                                    <i class="fs-16 zmdi zmdi-plus"></i>
                                </div>
                            </div>
                        </td>
                        <td class="column-5">${VNDong.format(
                          +el.price * +el.number
                        )}</td>
                    </tr>`;
    itemsHtml += itemRow;
  });

  return itemsHtml;
};

const BuildCouponArea = (data) => {
  const couponsApplied = data.cartDetail.couponCodes;
  if (!couponsApplied || couponsApplied.length == 0) return "";

  let couponsHtml = ``;
  couponsApplied.forEach((el) => {
    couponsHtml += `<li class="flex-c-m stext-107 cl6 size-301 bor7 p-lr-15 hov-tag1 trans-04 m-r-5 m-b-5 mouse-hover" data-coupon-code="${el}">${el}</li>`;
  });
  const couponArea = `<div class="flex-w flex-t bor12 p-t-15 p-b-13">
                        <div class="size-208">
                            <span class="stext-110 cl2"> Coupons: </span>
                        </div>
                        <ul class="flex-w m-r--5 size-209 list-coupons">
                            ${couponsHtml}
                        </ul>
                    </div>`;

  return couponArea;
};

const BuildShippingAddressArea = (data) => {
  if (
    !data.cartDetail.fullShippingAddress ||
    !data.cartDetail.fullName ||
    !data.cartDetail.phoneNumber
  )
    return "";

  const shippingAddressArea = ` <div class="mb-2 mt-2">
                                    <div><b>Reciever: </b>${data.cartDetail.fullName}</div>
                                    <div><b>Phone number: </b>${data.cartDetail.phoneNumber}</div>
                                    <div><b>Address: </b>${data.cartDetail.fullShippingAddress}</div>
                                </div>`;
  return shippingAddressArea;
};

const BuildDiscountArea = (data) => {
  if (!data.cartDetail.couponCodes || data.cartDetail.couponCodes.length == 0)
    return "";

  const discountPrice =
    +data.cartDetail.totalPrice + +data.shipmentFee - +data.totalMoney;
  let discountArea = `<div class="flex-w flex-t bor12 p-t-15 p-b-13">
                          <div class="size-208">
                              <span class="stext-110 cl2">Discount:</span>
                          </div>
                          <div class="p-t-1 text-decoration-line-through">
                              <span class="mtext-110 cl2">
                                  -${VNDong.format(discountPrice)}
                              </span>
                          </div>
                      </div>
                      <div class="flex-w flex-t p-t-15 ratio-origional-price">
                          <div class="size-208">
                              <span class="stext-110 cl2"></span>
                          </div>
                          <div>
                              <span class="cl2">
                                  ${VNDong.format(
                                    +data.cartDetail.totalPrice +
                                      +data.shipmentFee
                                  )}
                              </span>
                          </div>
                      </div>`;

  return discountArea;
};

//Events

const EventHideOrderDetailPopup = () => {
  const hideBtns = document.querySelectorAll(".js_hideOrderDetailPopup");
  const orderDetailPopup = document.querySelector(".js_orderDetailPopup");
  if (!hideBtns || !orderDetailPopup) return;

  hideBtns.forEach((el) => {
    el.addEventListener("click", () => {
      orderDetailPopup.classList.remove("show-loginPopup");
    });
  });
};

const OrderDetailEvent = () => {
  const orderItems = document.querySelectorAll(".js_myaccount-order-detail");
  const orderDetailPopup = document.querySelector(".js_orderDetailPopup");
  const orderDetailPopupArea = document.querySelector(
    ".js_orderDetailContentArea"
  );
  if (!orderItems || !orderDetailPopup || !orderDetailPopupArea) return;
  //
  const orderNumberEl = document.querySelector(".js_myaccount-order-number");
  const orderCreatedDateEl = document.querySelector(
    ".js_myaccount-order-createdDate"
  );
  const orderTableEl = document.querySelector(
    ".js_myaccount-table-shopping-cart"
  );
  const orderSubTotalEl = document.querySelector(".js_myaccount-subTotalPrice");
  const orderCouponAreaEl = document.querySelector(".js_myaccount-coupon-area");
  const orderShippingFeeEl = document.querySelector(
    ".js_myaccount-detail-shippingFee"
  );
  const orderShippingAddressAreaEl = document.querySelector(
    ".js_myaccount-shipping-address"
  );
  const orderDiscountAreaEl = document.querySelector(
    ".js_myaccount-discount-area"
  );
  const orderTotalPriceEl = document.querySelector(".js_myaccount-total-price");

  orderItems.forEach((el) => {
    el.addEventListener("click", async (e) => {
      e.preventDefault();
      const orderId = e.target.parentElement.parentElement.dataset.orderId;
      if (orderId) {
        var data = await GetOrderDetailApi(orderId);
        if (!data) {
          swal(`Error`, "Can't load cart detail!", "error");
        } else {
          // action
          orderNumberEl.innerHTML = data.orderNumber;
          orderCreatedDateEl.innerHTML = data.createdDate;
          orderTableEl.innerHTML = BuildTableItems(data);
          orderSubTotalEl.innerHTML = VNDong.format(data.cartDetail.totalPrice);
          orderCouponAreaEl.innerHTML = BuildCouponArea(data);
          orderShippingFeeEl.innerHTML = VNDong.format(data.shipmentFee);
          orderShippingAddressAreaEl.innerHTML = BuildShippingAddressArea(data);
          orderDiscountAreaEl.innerHTML = BuildDiscountArea(data);
          orderTotalPriceEl.innerHTML = VNDong.format(data.totalMoney);

          // show order detail popup
          orderDetailPopup.classList.add("show-loginPopup");
        }
      } else {
        console.error("Can't found orderId");
      }
    });
  });
};
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
  // add event when close popup.
  EventHideOrderDetailPopup();
  ChangeCityEvent();
  ToggleFormUpdatePersonalInformationEvent();
  UpdatePersonalInfoSubmit();
  OrderDetailEvent();
};

Init();
