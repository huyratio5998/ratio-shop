const listProducts = document.querySelector(".js-listProducts");
// Api
const GetProductDetailById = async (productId) => {
  const result = await fetch(`${baseApiUrl}/product/detail/${productId}`);
  if (result.ok) {
    return await result.json();
  }
  return null;
};

// Helper function
const DropListSelectVariantsAction = (
  data,
  parentElement,
  isChangeUrl = false
) => {
  if (!parentElement) return;

  const priceDetail = parentElement.querySelector(".js-price-detail");
  const priceDiscountDetail = parentElement.querySelector(".js_price-discount");
  const nameDetail = parentElement.querySelector(".js-name-detail");
  const selectedElement = parentElement.querySelector(".js-variants-select");

  if (!selectedElement) return;

  const selectedValue = selectedElement.value;
  const selectedText =
    selectedElement.options[selectedElement.selectedIndex].text;

  if (isChangeUrl) {
    const currentUrlPath = window.location.pathname;
    ChangeCurrentUrlWithoutReload(
      `${RemoveLastPart(currentUrlPath)}/${selectedValue}`
    );
  }

  const selectedVariant = data.product.variants.filter(
    (x) => x.id == selectedValue
  )[0];
  const variantPrice = selectedVariant?.discountRate
    ? (+selectedVariant?.price * (100 - +selectedVariant?.discountRate)) / 100
    : selectedVariant?.price;

  if (nameDetail)
    nameDetail.innerHTML = `<a class="cl2" href="/products/productdetail/${selectedValue}">${data.product.productFriendlyName}(${selectedVariant?.code})</a>`;
  priceDetail.dataset.itemPriceValue = variantPrice;
  priceDetail.innerHTML = variantPrice
    ? VNDong.format(variantPrice)
    : `Not set`;

  if (selectedVariant?.discountRate && priceDiscountDetail)
    priceDiscountDetail.innerHTML = VNDong.format(selectedVariant?.price);
  else priceDiscountDetail.innerHTML = "";
};

const BuildPreviewGallaryImages = (data) => {
  const selectedVariantImages = data.selectedVariantImages;
  if(!selectedVariantImages || selectedVariantImages.length == 0) return '';

  let imageItems = ``;
  selectedVariantImages.forEach(el =>{
    const productImage = el.productImageName
      ? `${el.productImageName}`
      : `/images/default-placeholder.jpg`;
  
    const imageItem = `<div class="item-slick3" data-thumb="${productImage}">
    <div class="wrap-pic-w pos-relative">      
        <img src="${productImage}" alt="${el.product.productFriendlyName}" class="js-productImages" style="max-height: 750px;">
        <a class="flex-c-m size-108 how-pos1 bor0 fs-16 cl10 bg0 hov-btn3 trans-04" href="${productImage}">
            <i class="fa fa-expand"></i>
        </a>
    </div>
  </div>`;

  imageItems += imageItem;
  })

  const result = `<div class="wrap-slick3-dots"></div>
  <div class="wrap-slick3-arrows flex-sb-m flex-w"></div>
  <div class="slick3 gallery-lb">
     ${imageItems}
  </div>`;

  return result;
};

const BuildProductPreviewModal = (data) => {
  if (!data) return;
  const modalQuickviewArea = document.querySelector(
    ".js_productItemQuickviewModal"
  );
  if (!modalQuickviewArea) return;

  const nameDetail = modalQuickviewArea.querySelector(".js-name-detail");
  const productCode = modalQuickviewArea.querySelector(".js_product-code");
  const priceDetail = modalQuickviewArea.querySelector(".js-price-detail");
  const priceDiscountDetail =
    modalQuickviewArea.querySelector(".js_price-discount");
  const descriptionDetail = modalQuickviewArea.querySelector(
    ".js-description-detail"
  );
  const detailImages = modalQuickviewArea.querySelector(".js-detail-images");
  const variantsElement = modalQuickviewArea.querySelector(
    ".js-variants-select"
  );
  const numberDetail = modalQuickviewArea.querySelector(".js-variantNumber");
  // build list variants
  if (data.product.variants && data.product.variants.length > 0) {
    if (
      modalQuickviewArea.querySelector(".js-variants").style.display == "none"
    )
      modalQuickviewArea
        .querySelector(".js-variants")
        .style.removeProperty("display");
    variantsElement.innerHTML = "";
    data.product.variants.forEach((e, index) => {
      variantsElement.insertAdjacentHTML(
        "beforeend",
        `<option value=${e.id}>${e.code}</option>`
      );
    });
  } else {
    modalQuickviewArea.querySelector(".js-variants").style.display = "none";
    variantsElement.innerHTML = "";
  }
  // build detail information
  const productVariantId = data.product.variants[0]?.id
    ? data.product.variants[0]?.id
    : data.product.id;
  const productVariantName = data.product.variants[0]?.code
    ? `(${data.product.variants[0]?.code})`
    : "";
  const discountPrice = data.product.variants[0]?.discountRate
    ? (+data.product.variants[0].price *
        (100 - +data.product.variants[0]?.discountRate)) /
      100
    : data.product.variants[0]?.price;

  nameDetail.innerHTML = `<a class="cl2" href="/products/productdetail/${productVariantId}">${data.product.productFriendlyName}${productVariantName}</a>`;
  productCode.innerHTML = data.product.code;
  priceDetail.innerHTML = data.product.variants[0]
    ? `${VNDong.format(discountPrice)}`
    : `Not set`;

  if (data.product.variants[0]?.discountRate && priceDiscountDetail)
    priceDiscountDetail.innerHTML = VNDong.format(
      data.product.variants[0].price
    );
  else priceDiscountDetail.innerHTML = "";

  priceDetail.dataset.itemPriceValue = data.product.variants[0]
    ? discountPrice
    : "";
  descriptionDetail.innerHTML = data.product.description;
  detailImages.innerHTML = BuildPreviewGallaryImages(data);
  numberDetail.value = 1;
};

const BuildProductCartItem = (item, modalClass) => {
  const productImage = item.productImageName
    ? `<img src="${item.productImageName}" alt="${item.product?.name}">`
    : `<img src="/images/default-placeholder.jpg" alt="${item.product?.name}">`;

  const discountPrice =
    item.product?.variants &&
    item.product?.variants[0] &&
    item.product?.variants[0].discountRate
      ? (+item.product?.variants[0].price *
          (100 - +item.product?.variants[0].discountRate)) /
        100
      : item.product?.variants[0]?.price;

  const discountPriceDisplay =
    item.product?.variants && item.product?.variants[0]
      ? `${VNDong.format(discountPrice)}`
      : `Not set`;

  const origionalPrice =
    item.product?.variants &&
    item.product?.variants[0] &&
    item.product?.variants[0].discountRate
      ? ` <span class="stext-105 cl3 ratio-origional-price">
  ${VNDong.format(item.product?.variants[0].price)}</span>`
      : "";

  const productItem = ` <div class="col-sm-6 col-md-4 col-lg-3 p-b-35 isotope-item women ratio">                        
                        <div class="block2">
                            <div class="block2-pic hov-img0">
                                ${productImage}
                                <a href="#" class="block2-btn flex-c-m stext-103 cl2 size-102 bg0 bor2 hov-btn1 p-lr-15 trans-04 ${modalClass}" data-product-id="${item.product?.id}">
                                    Quick View
                                </a>
                            </div>

                            <div class="block2-txt flex-w flex-t p-t-14">
                                <div class="block2-txt-child1 flex-col-l ">
                                    <a href="/products/productdetail/${item.product?.id}" class="stext-104 cl4 hov-cl1 trans-04 js-name-b2 p-b-6">
                                    <p class="text-uppercase">${item.product?.code}</p>                                        
                                        <p>${item.product?.productFriendlyName}</p>
                                    </a>
                                    <span class="stext-105 cl3">
                                        ${discountPriceDisplay}
                                    </span>
                                    ${origionalPrice}
                                </div>
                            </div>
                        </div>
                    </div>`;
  return productItem;
};

const BuildAdditionProductCartItems = (datas, currentPage) => {
  if (!datas) return;

  const modalClass = `js-show-modal${currentPage}`;
  let products = ``;
  datas.forEach((item) => {
    const product = BuildProductCartItem(item, modalClass);
    products += product;
  });

  listProducts.insertAdjacentHTML("beforeend", products);
  RefreshModalEvent(modalClass);
};

const BuildProductCartItems = (datas, currentPage) => {
  if (!datas) return;

  const modalClass = `js-show-modal${currentPage}`;
  let products = ``;
  datas.forEach((item) => {
    const product = BuildProductCartItem(item, modalClass);
    products += product;
  });

  listProducts.innerHTML = products;
  RefreshModalEvent(modalClass);
};

// Events
const VariantChangeEvent = (data, parentElement) => {
  if (!parentElement) return;
  const variantsElement = parentElement.querySelector(".js-variants-select");
  if (!variantsElement) return;

  const currentUrlPath = window.location.pathname;
  let isChangeUrl = false;
  if (
    currentUrlPath.includes("/products/productdetail") &&
    parentElement?.classList?.contains("js_product-detail-area")
  )
    isChangeUrl = true;

  Select2EventHelper(
    variantsElement,
    DropListSelectVariantsAction,
    data,
    parentElement,
    isChangeUrl
  );
};

const AddProductItemQuickViewEvent = (parentElement, e) => {
  e.addEventListener("click", async () => {
    const productId = e.dataset.productId;
    if (!productId) console.log("empty");

    const data = await GetProductDetailById(productId);
    if (data) {
      BuildProductPreviewModal(data);
      VariantChangeEvent(data, parentElement);
    }
  });
};

const AddQuickViewProductEvent = (
  parentElement = document,
  btnQuickViews = null,
  isDynamicBtns = false
) => {
  if (!isDynamicBtns)
    btnQuickViews = parentElement.querySelectorAll(".js-show-modal1");

  if (!btnQuickViews) return;

  btnQuickViews.forEach((e) => {
    AddProductItemQuickViewEvent(parentElement, e);
  });
};

const RefreshModalEvent = (modalClass) => {
  AddQuickViewProductEvent(
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
  BuildAdditionProductCartItems,
  BuildProductCartItems,
  GetProductDetailById,
  VariantChangeEvent,
  AddQuickViewProductEvent,
};
