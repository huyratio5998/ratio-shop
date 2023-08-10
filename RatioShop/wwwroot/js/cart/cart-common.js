import * as LoginService from "../login/login.js";

const headerItemCart = document.querySelectorAll(".js-show-cart");
const listCartItems = document.querySelector(".header-cart-wrapitem");
const headerCartTotal = document.querySelector(".header-cart-total");
const headerViewCartBtn = document.querySelector(".js_view-cart-header");

// API
const AddToCartApi = async (variantId, packageId, number) => {
  const data = JSON.stringify({
    userId: null,
    variantId: variantId,
    packageId: packageId,
    number: number,
  });
  const result = await fetch(`${baseApiUrl}/cart/addToCart`, {
    method: "POST",
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json",
    },
    body: data,
  });
  if (result.ok) {
    return await result.json();
  }
  return null;
};
const ChangeCartItem = async (variantId, packageId, number) => {
  const data = JSON.stringify({
    userId: null,
    variantId: variantId,
    packageId: packageId,
    number: number,
  });
  const result = await fetch(`${baseApiUrl}/cart/changeCartItem`, {
    method: "POST",
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json",
    },
    body: data,
  });
  if (result.ok) {
    return await result.json();
  }
  return null;
};
const GetCartDetailApi = async () => {
  try {
    const result = await fetch(`${baseApiUrl}/cart/detail`);
    if (result.ok) {
      return await result.json();
    }
    return null;
  } catch (e) {
    return null;
  }
};
const RemoveCouponCodeOnCart = async (coupon) => {
  const result = await fetch(
    `${baseApiUrl}/cart/removecouponcode?couponcode=${coupon}`
  );
  if (result.ok) {
    return await result.json();
  }
  return null;
};
//
const RefreshCartView = async () => {
  const cartDetail = await GetCartDetailApi();
  if (!cartDetail) {
    ResetCart();
    ResetCartDetail();
    headerItemCart.forEach((e) => (e.dataset.notify = 0));
    return;
  }

  const cartItems = cartDetail.cartItems;
  const totalCartItem = !cartItems
    ? 0
    : cartItems.reduce((a, element) => +a + +element.number, 0);
  // update cart icon count view
  headerItemCart.forEach((e) => (e.dataset.notify = totalCartItem));
  // update cart items view
  const totalPrice = cartDetail.totalFinalPrice;
  ResetCart();
  ResetCartDetail();
  if (cartItems) {
    cartItems.forEach((e, index) => {
      const item = BuildHeaderCartItem(e);

      listCartItems.insertAdjacentHTML("beforeend", item);
    });

    // add events
    RefreshCartDetail(cartDetail);
    CartDetailActionEvent();
    // Add remove cart item event.
    const cartItemsRemoveBtn = document.querySelectorAll(
      " .js_remove-cart-item"
    );
    if (cartItemsRemoveBtn) {
      cartItemsRemoveBtn.forEach((e, index) => {
        e.addEventListener("click", async () => {
          const variantId = e.dataset.variantId;
          const packageId = e.dataset.packageId;
          if (variantId || packageId) {
            const changeStatus = await ChangeCartItem(variantId, packageId, 0);
            if (!changeStatus) alert("Change cart failure!");
            RefreshCartView();
            console.log("remove cart item event");
          } else {
            console.error("Can't remove cart item");
          }
        });
      });
    }
  }
  headerCartTotal.innerHTML = `Total: ${VNDong.format(totalPrice)}`;
};

const BuildHeaderCartItem = (item) => {
  const discountPrice =
    item.discountRate && item.itemType == 0
      ? `<span class="ratio-origional-price">${VNDong.format(
          item.price
        )}</span>`
      : "";
  const priceDisplay = `${item.number} x ${VNDong.format(
    item.discountPrice
  )} ${discountPrice}`;
  const cartItemName = item.variableName
    ? `${item.name} <i>(${item.variableName})</i>`
    : item.name;

  let headerCartItem =
    item.itemType == 0
      ? `<li class="header-cart-item flex-w flex-t m-b-12">
                      <div class="header-cart-item-img js_remove-cart-item" data-variant-id="${item.variantId}">
                          <img src="${item.image}" alt="${item.name}">
                      </div>
                      <div class="header-cart-item-txt p-t-8">
                          <a href="/products/productdetail/${item.variantId}" class="header-cart-item-name m-b-18 hov-cl1 trans-04">
                          ${cartItemName}
                          <p class="text-uppercase">${item.productCode}</p>
                          </a>
                          <span class="header-cart-item-info">
                          ${priceDisplay}
                          </span>
                      </div>
                    </li>`
      : `<li class="header-cart-item flex-w flex-t m-b-12">
                      <div class="header-cart-item-img js_remove-cart-item" data-package-id="${item.packageId}">
                          <img src="${item.image}" alt="${item.name}">
                      </div>
                      <div class="header-cart-item-txt p-t-8">
                          <a href="/packages/detail/${item.packageId}" class="header-cart-item-name m-b-18 hov-cl1 trans-04">
                          ${cartItemName}
                          <p class="text-uppercase">${item.productCode}</p>
                          </a>
                          <span class="header-cart-item-info">
                          ${priceDisplay}
                          </span>
                      </div>
                    </li>`;
  return headerCartItem;
};

const BuildNewShippingAdderss = (cartDetail) => {
  const shippingAddressInnerArea = document.querySelector(
    ".js_default-shipping-address-inner"
  );
  if (!shippingAddressInnerArea || !cartDetail) return;

  const shippingAddressHtml = `<div><b>Reciever: </b>${cartDetail.fullName}</div>
                                <div><b>Phone number: </b>${cartDetail.phoneNumber}</div>
                                <div><b>Address: </b>${cartDetail.fullShippingAddress}</div>`;
  shippingAddressInnerArea.innerHTML = shippingAddressHtml;
};

const BuildDiscountArea = (cartDetail) => {
  const discountArea = document.querySelector(".js_cart-detail-discount-area");
  if (
    !discountArea ||
    !cartDetail ||
    !cartDetail.couponCodes ||
    cartDetail.couponCodes.length == 0
  )
    return;

  const discountHtml = `<div class="flex-w flex-t bor12 p-t-15 p-b-13">
                                <div class="size-208">
                                    <span class="stext-110 cl2">Discount:</span>
                                </div>
                                <div class="p-t-1 text-decoration-line-through">
                                    <span class="mtext-110 cl2">
                                    -${VNDong.format(
                                      +cartDetail.totalPrice +
                                        +cartDetail.shippingFee -
                                        +cartDetail.totalFinalPrice
                                    )}
                                    </span>
                                </div>
                            </div>
                            <div class="flex-w flex-t p-t-15 ratio-origional-price">
                                <div class="size-208">
                                    <span class="stext-110 cl2"></span>
                                </div>
                                <div class="">
                                    <span class="cl2">
                                    ${VNDong.format(
                                      +cartDetail.totalPrice +
                                        +cartDetail.shippingFee
                                    )}
                                    </span>
                                </div>
                            </div>`;
  discountArea.innerHTML = discountHtml;
};

const ResetCart = () => {
  listCartItems.innerHTML = "";
  headerCartTotal.innerHTML = "";
};

const ResetCartDetail = () => {
  const cartTable = document.querySelector(".js_table-shopping-cart");
  if (cartTable) {
    cartTable.innerHTML = `<tr class="table_head">
                          <th class="column-1">Product</th>
                          <th class="column-2"></th>
                          <th class="column-3">Price</th>
                          <th class="column-4">Quantity</th>
                          <th class="column-5">Total</th>
                          </tr>`;
  }

  //
  const totalPrice = document.querySelector(".js_cart-detail-total-price");
  if (totalPrice) totalPrice.innerHTML = "";
  const subTotalPriceElement = document.querySelector(
    ".js_cart-detail-subTotal-price"
  );
  const cartDetailDiscountArea = document.querySelector(
    ".js_cart-detail-discount-area"
  );
  if (subTotalPriceElement) {
    subTotalPriceElement.innerHTML = "";
    subTotalPriceElement.dataset.subTotalPrice = "";
  }

  if (cartDetailDiscountArea) {
    cartDetailDiscountArea.innerHTML = "";
  }
  // const shippingFeeEl = document.querySelector(".js_cart-detail-shippingFee");
  // if (shippingFeeEl) {
  //   shippingFeeEl.innerHTML = "";
  //   shippingFeeEl.dataset.shippingFee = "";
  // }
};

const BuildCartItemDetail = (item) => {
  const displayItemPrice =
    item.discountRate && item.itemType == 0
      ? `${VNDong.format(
          item.discountPrice
        )} <span class="ratio-origional-price">
  ${VNDong.format(item.price)}</span>`
      : VNDong.format(item.price);
  const productNameDisplay = item.variableName
    ? `${item.name} - ${item.variableName}`
    : `${item.name}`;
  const itemImage = item.image
    ? `${item.image}`
    : "/images/default-placeholder.jpg";
  const itemLink =
    item.itemType == 0
      ? `/products/productdetail/${item.variantId}`
      : `/packages/detail/${item.packageId}`;
  const itemIdData =
    item.itemType == 0
      ? `data-variant-id="${item.variantId}"`
      : `data-package-id="${item.packageId}"`;
  const itemDescription = item.description
    ? `<div class="stext-109 cl4">${item.description}</div>`
    : "";
  let cartItemDetail = ` <tr class="table_row">
                                        <td class="column-1">
                                            <div class="how-itemcart1 js_remove-cart-item" data-package-id="${
                                              item.packageId
                                            }" data-variant-id="${
    item.variantId
  }">
                                                <img src="${itemImage}" alt="${
    item.variableName
  }" />
                                            </div>
                                        </td>
                                        <td class="column-2">
                                        <a href="${itemLink}" class="header-cart-item-name m-b-18 hov-cl1 trans-04">${productNameDisplay}
                                        <p class="text-uppercase">${
                                          item.productCode
                                        }</p>
                                        </a>
                                        ${itemDescription}
                                        </td>
                                        <td class="column-3">${displayItemPrice}</td>
                                        <td class="column-4">
                                            <div class="wrap-num-product flex-w m-l-auto m-r-0" ${itemIdData}>
                                                <div class="btn-num-product-down cl8 hov-btn3 trans-04 flex-c-m js_change-cart-item-detail">
                                                    <i class="fs-16 zmdi zmdi-minus"></i>
                                                </div>
                                                <input class="mtext-104 cl3 txt-center num-product"
                                               type="number"
                                               name="num-product1"
                                               value="${item.number}" />

                                                <div class="btn-num-product-up cl8 hov-btn3 trans-04 flex-c-m js_change-cart-item-detail">
                                                    <i class="fs-16 zmdi zmdi-plus"></i>
                                                </div>
                                            </div>
                                        </td>
                                        <td class="column-5">${VNDong.format(
                                          item.discountPrice * item.number
                                        )} </td>
                                    </tr>`;
  return cartItemDetail;
};

const PartialUpdateTotalPiceView = (cartDetail) => {
  const totalPriceElement = document.querySelector(
    ".js_cart-detail-total-price"
  );
  const subTotalPriceElement = document.querySelector(
    ".js_cart-detail-subTotal-price"
  );

  if (totalPriceElement)
    totalPriceElement.innerHTML = `${VNDong.format(
      +cartDetail.totalFinalPrice
    )}`;

  if (subTotalPriceElement) {
    subTotalPriceElement.dataset.subTotalPrice = +cartDetail.totalPrice;
    subTotalPriceElement.innerHTML = `${VNDong.format(cartDetail.totalPrice)}`;
  }
};

const ChangeItemNumber = async (
  variantId,
  packageId,
  currentNumber,
  additionValue
) => {
  const changeStatus = await ChangeCartItem(
    variantId,
    packageId,
    +currentNumber + +additionValue
  );

  if (!changeStatus || changeStatus.status == "Failure") {
    let errorMessage = "Change cart failure!";
    if (changeStatus != null) errorMessage = changeStatus.message;
    swal("Error", errorMessage, "error");
    return;
  }

  RefreshCartView();
};

const RefreshCouponArea = (cartDetail) => {
  const couponArea = document.querySelector(".js_list-coupon-area");
  if (couponArea) {
    couponArea.innerHTML = "";
    if (cartDetail.couponCodes && cartDetail.couponCodes.length > 0) {
      let listCoupons = document.querySelector(".list-coupons");
      if (!listCoupons) {
        let couponElement = ` 
                          <div class="flex-w flex-t bor12 p-t-15 p-b-13">
                            <div class="size-208">
                              <span class="stext-110 cl2"> Coupons: </span>
                            </div>
                            <ul class="flex-w m-r--5 size-209 list-coupons">       
                            </ul>
                          </div>`;
        couponArea.innerHTML = couponElement;
        listCoupons = document.querySelector(".list-coupons");
      }
      cartDetail.couponCodes.forEach((coupon) => {
        listCoupons.insertAdjacentHTML(
          "beforeend",
          `<li class="flex-c-m stext-107 cl6 size-301 bor7 p-lr-15 hov-tag1 trans-04 m-r-5 m-b-5 mouse-hover js_remove-coupon-code" data-coupon-code="${coupon}">${coupon}<span class="coupon-cancel-icon">&#10006;</span></li>`
        );
      });

      // add event to remove
      BtnRemoveCouponEvent();
    }
  }
};

//Events
const RefreshCartDetail = (cartDetail) => {
  if (!cartDetail) return;

  // refresh cart table items
  const table = document.querySelector(".js_table-shopping-cart");
  if (!table) return;

  // update items
  cartDetail.cartItems.forEach((el, index) => {
    table.insertAdjacentHTML("beforeend", BuildCartItemDetail(el));
  });

  // update shipping value
  const shippingFee = document.querySelector(".js_cart-detail-shippingFee");
  if (shippingFee) {
    shippingFee.dataset.shippingFee = cartDetail.shippingFee;
    shippingFee.innerHTML = VNDong.format(cartDetail.shippingFee);
  }
  // update btn checkout data set
  const btnCheckout = document.querySelector(".js_proceed-checkout");
  if (btnCheckout) {
    btnCheckout.dataset.totalItems = cartDetail.totalItems;
    btnCheckout.dataset.shipmentValidation =
      cartDetail.isEnoughShippingInformation.toLocaleString().toLowerCase();
  }

  // refresh coupons
  RefreshCouponArea(cartDetail);
  // update new adderss
  BuildNewShippingAdderss(cartDetail);
  // update discount value
  BuildDiscountArea(cartDetail);
  // update total price
  PartialUpdateTotalPiceView(cartDetail);
};

const BtnRemoveCouponEvent = () => {
  const btnRemoveCoupons = document.querySelectorAll(".js_remove-coupon-code");
  if (!btnRemoveCoupons) return;

  btnRemoveCoupons.forEach((btnRemove) => {
    if (btnRemove) {
      btnRemove.addEventListener("click", async (e) => {
        const couponRemove = btnRemove.dataset.couponCode;
        if (couponRemove) {
          var data = await RemoveCouponCodeOnCart(couponRemove);

          if (data == null || !data) console.error("Can't remove coupon!");
          else RefreshCartView();
        }
      });
    }
  });
};

const CartDetailActionEvent = () => {
  const currentPage =
    document.querySelector(".current-page-url")?.dataset?.currentPageUrl;
  if (!currentPage || !currentPage.toLowerCase().includes("cart/cartdetail"))
    return;

  const btnCartItemDetailUp = document.querySelectorAll(".btn-num-product-up");
  const btnCartItemDetailDown = document.querySelectorAll(
    ".btn-num-product-down"
  );
  //
  if (btnCartItemDetailUp) {
    btnCartItemDetailUp.forEach((el) => {
      el.addEventListener("click", async (e) => {
        const currentNumber = el.previousElementSibling.value;
        const variantId = el.parentNode.dataset.variantId;
        const packageId = el.parentNode.dataset.packageId;
        await ChangeItemNumber(variantId, packageId, currentNumber, 1);
      });
    });
  }
  //
  if (btnCartItemDetailDown) {
    btnCartItemDetailDown.forEach((el) => {
      el.addEventListener("click", async (e) => {
        const currentNumber = el.nextElementSibling.value;
        const variantId = el.parentNode.dataset.variantId;
        const packageId = el.parentNode.dataset.packageId;
        await ChangeItemNumber(variantId, packageId, currentNumber, -1);
      });
      //
      const numbeInputElement = el.nextElementSibling;
      if (numbeInputElement) {
        numbeInputElement.addEventListener("focusout", async () => {
          const currentNumber = numbeInputElement.value;
          const variantId = el.parentNode.dataset.variantId;
          const packageId = el.parentNode.dataset.packageId;
          await ChangeItemNumber(variantId, packageId, currentNumber, 0);
        });
      }
    });
  }
};

const AddToCartEvent = (parentElement = document) => {
  const btnAddCartDetail = parentElement.querySelector(".js-addcart-detail");
  if (!btnAddCartDetail) return;

  btnAddCartDetail.addEventListener("click", async (e) => {
    e.preventDefault();
    // get data send
    let packageId = null;
    let variantId = null;
    const variantSelect = parentElement.querySelector(".js-variants-select");

    if (btnAddCartDetail.dataset.packageId) {
      packageId = btnAddCartDetail.dataset.packageId;
    } else {
      variantId = variantSelect.value;
    }
    const number = parentElement.querySelector(".js-variantNumber").value;
    if ((!variantId && !packageId) || number <= 0)
      console.error("Invalid params");

    // call api add to cart
    const data = await AddToCartApi(variantId, packageId, number);
    // show success, update cart view.
    if (!data) {
      const errorMessage = "Fail add to cart";
      console.error(errorMessage);
      swal("Error", errorMessage, "error");
    } else {
      if (data.status == "Failure") {
        if (!data.isAuthenticated) {
          const loginButton = document.querySelector(
            '[data-popup-type="login"]'
          );
          const anotherModel = document.querySelector(".js-modal1");

          if (loginButton) {
            LoginService.GenerateLoginOrRegisterPopup(false, true);
            if (anotherModel) anotherModel.classList.remove("show-modal1");
          }
        } else {
          const errorMessage = data.message;
          swal("Error", errorMessage, "error");
        }
        return;
      }
      // update cart on view
      RefreshCartView();

      // display popup success
      const itemName =
        parentElement.querySelector(".js-name-detail").textContent;
      const itemVariableName = btnAddCartDetail.dataset.packageId
        ? ""
        : variantSelect.options[variantSelect.selectedIndex].text;
      const fullItemDisplayName = itemName.includes(itemVariableName)
        ? itemName
        : `${itemName}(${itemVariableName})`;

      swal(fullItemDisplayName, "is added to cart !", "success");
    }
  });
};

const ViewCartHeaderPannelEvent = () => {
  if (headerViewCartBtn) {
    headerViewCartBtn.addEventListener("click", async (e) => {
      e.preventDefault();

      const data = await LoginService.CheckIsAuthenticated();
      if (data == true) {
        RedirectToPath(false, headerViewCartBtn.getAttribute("href"));
      } else {
        // hide pannel
        const pannelHeader = document.querySelector(".js-panel-cart");
        if (pannelHeader) pannelHeader.classList.remove("show-header-cart");

        // show login popup
        LoginService.GenerateLoginOrRegisterPopup(false, true);
      }
    });
  }
};

const InitCart = async () => {
  console.log("outside: cart-common.js");
  const isAuthentication = await LoginService.CheckIsAuthenticated();
  if (isAuthentication) RefreshCartView();
  AddToCartEvent();
  ViewCartHeaderPannelEvent();
};

InitCart();

export { RefreshCartView, AddToCartEvent };
