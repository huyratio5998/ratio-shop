const listProducts = document.querySelector(".js-listProducts");

// Api
const GetProductPackageDetailById = async (packageId) => {
  const result = await fetch(`${baseApiUrl}/package/detail/${packageId}`);
  if (result.ok) {
    return await result.json();
  }
  return null;
};

// Helpers
const BuildProductPackageCartItem = (item, modalClass) => {
  const productImage = item.imageUrl
    ? `<img src="${item.imageUrl}" alt="${item.name}">`
    : `<img src="/images/default-placeholder.jpg" alt="${item.name}">`;

  const packagePrice =
    item.manualPrice != null ? item.manualPrice : item.autoCalculatedPrice;

  const packagePriceDisplay =
    packagePrice != null ? `${VNDong.format(packagePrice)}` : `Not set`;

  const productItem = ` <div class="col-sm-6 col-md-4 col-lg-3 p-b-35 isotope-item women ratio">                        
                        <div class="block2">
                            <div class="block2-pic hov-img0">
                                ${productImage}
                                <a href="#" class="block2-btn flex-c-m stext-103 cl2 size-102 bg0 bor2 hov-btn1 p-lr-15 trans-04 ${modalClass}" data-is-package="true" data-product-id="${item.id}">
                                    Quick View
                                </a>
                            </div>

                            <div class="block2-txt flex-w flex-t p-t-14">
                                <div class="block2-txt-child1 flex-col-l ">
                                    <a href="/packages/detail/${item.id}" class="stext-104 cl4 hov-cl1 trans-04 js-name-b2 p-b-6">
                                    <p class="text-uppercase">${item.code}</p>                                        
                                        <p>${item.productFriendlyName}</p>
                                    </a>
                                    <span class="stext-105 cl3">
                                        ${packagePriceDisplay}
                                    </span>                                    
                                </div>
                            </div>
                        </div>
                    </div>`;
  return productItem;
};

const BuildAdditionProductPackageCartItems = (datas, currentPage) => {
  if (!datas) return;

  const modalClass = `js-show-modal${currentPage}`;
  let productPackages = ``;
  datas.forEach((item) => {
    const productPackage = BuildProductPackageCartItem(item, modalClass);
    productPackages += productPackage;
  });

  listProducts.insertAdjacentHTML("beforeend", productPackages);
  RefreshModalEvent(modalClass);
};

const BuildProductPackageCartItems = (datas, currentPage) => {
  if (!datas) return;

  const modalClass = `js-show-modal${currentPage}`;
  let productPackages = ``;
  datas.forEach((item) => {
    const productPackage = BuildProductPackageCartItem(item, modalClass);
    productPackages += productPackage;
  });

  listProducts.innerHTML = productPackages;
  RefreshModalEvent(modalClass);
};

const BuildProductPackagePreviewModal = (data) => {
  if (!data) return;
  const modalQuickviewArea = document.querySelector(
    ".js_productItemQuickviewModal"
  );
  if (!modalQuickviewArea) return;

  const nameDetail = modalQuickviewArea.querySelector(".js-name-detail");
  const productCode = modalQuickviewArea.querySelector(".js_product-code");
  const priceDetail = modalQuickviewArea.querySelector(".js-price-detail");
  const descriptionDetail = modalQuickviewArea.querySelector(
    ".js-description-detail"
  );
  const detailImagesQuickview = modalQuickviewArea.querySelector(
    ".js-detail-images-quickview"
  );
  const packagesElement =
    modalQuickviewArea.querySelector(".js-packages-items");
  const variantsArea = modalQuickviewArea.querySelector(".js-variants");
  if (variantsArea) variantsArea.style.display = "none";

  const numberDetail = modalQuickviewArea.querySelector(".js-variantNumber");
  const btnAddToCart = modalQuickviewArea.querySelector(".js-addcart-detail");
  // build list packages
  if (data.packageItems && data.packageItems.length > 0) {
    if (
      modalQuickviewArea.querySelector(".js-packages-area").style.display ==
      "none"
    )
      modalQuickviewArea
        .querySelector(".js-packages-area")
        .style.removeProperty("display");
    packagesElement.innerHTML = "";
    data.packageItems.forEach((e, index) => {
      const packageItemElement = `<div class="mb-3">
      <a class="row" href="/products/productdetail/${e.id}">
        <img class="col-3" src="${e.imageUrl}" alt="${e.name}" />
      <div class="col-9">
        <p>${e.name} (${e.code})</p>
        <p>Number: ${e.number}</p>
        <p>${VNDong.format(e.priceAfterDiscount)}</p>
      </div>      
      </a>
      </div>`;
      packagesElement.insertAdjacentHTML("beforeend", packageItemElement);
    });
  } else {
    modalQuickviewArea.querySelector(".js-packages-area").style.display =
      "none";
    packagesElement.innerHTML = "";
  }
  // build detail information
  nameDetail.innerHTML = `<a class="cl2" href="/packages/detail/${data.id}">${data.productFriendlyName}</a>`;
  productCode.innerHTML = data.code;
  const packagePrice =
    data.manualPrice != null ? data.manualPrice : data.autoCalculatedPrice;
  priceDetail.innerHTML = packagePrice
    ? `${VNDong.format(packagePrice)}`
    : `Not set`;

  priceDetail.dataset.itemPriceValue = packagePrice ? packagePrice : "";
  descriptionDetail.innerHTML = data.description;
  detailImagesQuickview.innerHTML = BuildPreviewGallaryImages(data);
  numberDetail.value = 1;
  if (btnAddToCart) {
    btnAddToCart.dataset.packageId = data.id;
    btnAddToCart.dataset.variantId = "";
  }

  InitSlick();
};

const BuildPreviewGallaryImages = (data) => {
  // default image from product
  const productImageDefault = data.imageUrl
    ? data.imageUrl
    : "/images/default-placeholder.jpg";

  let imageItems = `<div class="item-slick3" data-thumb="${productImageDefault}">
  <div class="wrap-pic-w pos-relative">      
  <img src="${productImageDefault}" alt="${data.productFriendlyName}" class="js-productImages" style="max-height: 750px;">
  <a class="flex-c-m size-108 how-pos1 bor0 fs-16 cl10 bg0 hov-btn3 trans-04" href="${productImageDefault}">
  <i class="fa fa-expand"></i>
  </a>
  </div>
  </div>`;

  const result = `<div class="wrap-slick3-dots"></div>
  <div class="wrap-slick3-arrows flex-sb-m flex-w"></div>
  <div class="slick3 gallery-lb">
     ${imageItems}
  </div>`;

  return result;
};

// Events
const AddProductItemQuickViewEvent = (parentElement, e) => {
  e.addEventListener("click", async () => {
    const packageId = e.dataset.productId;
    if (!packageId) console.log("empty");

    const data = await GetProductPackageDetailById(packageId);
    if (data) {
      BuildProductPackagePreviewModal(data);
    }
  });
};

const AddQuickViewProductPackageEvent = (
  parentElement = document,
  btnQuickViews = null,
  isDynamicBtns = false
) => {
  if (!isDynamicBtns)
    btnQuickViews = parentElement.querySelectorAll(".js-show-modal1");

  if (!btnQuickViews) return;

  btnQuickViews.forEach((e) => {
    if (e.dataset.isPackage == "true") {
      AddProductItemQuickViewEvent(parentElement, e);
    }
  });
};

const RefreshModalEvent = (modalClass) => {
  AddQuickViewProductPackageEvent(
    document,
    document.querySelectorAll(`.${modalClass}`),
    true
  );
  $(`.${modalClass}`).on("click", function (e) {
    e.preventDefault();
    $(".js-modal1").addClass("show-modal1");
  });
};

export {
  BuildProductPackageCartItems,
  BuildAdditionProductPackageCartItems,
  BuildProductPackageCartItem,
  AddQuickViewProductPackageEvent,
};
