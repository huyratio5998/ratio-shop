﻿const btnQuickViews = document.querySelectorAll(".js-show-modal1");
const btnLoadMore = document.querySelector("#btn-loadMore");
const listProducts = document.querySelector(".js-listProducts");

// build preview view
const BuildPreviewGallaryImages = (data) => {
  const productImage = data.productImageName
    ? `images/products/${data.productImageName}`
    : `images/default-placeholder.jpg`;

  const imageItem = `<div class="item-slick3" data-thumb="${productImage}">
  <div class="wrap-pic-w pos-relative">      
      <img src="${productImage}" alt="${data.product.productFriendlyName}" class="js-productImages" style="max-height: 750px;">
      <a class="flex-c-m size-108 how-pos1 bor0 fs-16 cl10 bg0 hov-btn3 trans-04" href="${productImage}">
          <i class="fa fa-expand"></i>
      </a>
  </div>
</div>`;

  const result = `<div class="wrap-slick3-dots"></div>
  <div class="wrap-slick3-arrows flex-sb-m flex-w"></div>
  <div class="slick3 gallery-lb">
     ${imageItem}
  </div>`;

  return result;
};

const BuildProductPreviewModal = (data) => {
  if (!data) return;
  const nameDetail = document.querySelector(".js-name-detail");
  const priceDetail = document.querySelector(".js-price-detail");
  const descriptionDetail = document.querySelector(".js-description-detail");
  const detailImages = document.querySelector(".js-detail-images");
  const variantsElement = document.querySelector(".js-variants-select");
  const numberDetail = document.querySelector(".js-variantNumber");
  // build list variants
  if (data.product.variants && data.product.variants.length > 0) {
    if (document.querySelector(".js-variants").style.display == "none")
      document.querySelector(".js-variants").style.removeProperty("display");
    variantsElement.innerHTML = "";
    data.product.variants.forEach((e, index) => {
      variantsElement.insertAdjacentHTML(
        "beforeend",
        `<option value=${e.id}>${e.code}</option>`
      );
    });
  } else {
    document.querySelector(".js-variants").style.display = "none";
    variantsElement.innerHTML = "";
  }
  // build detail information
  const productVariantId = data.product.variants[0]?.id
    ? data.product.variants[0]?.id
    : data.product.id;
  const productVariantName = data.product.variants[0]?.code
    ? `(${data.product.variants[0]?.code})`
    : "";

  nameDetail.innerHTML = `<a class="cl2" href="/products/productdetail/${productVariantId}">${data.product.productFriendlyName}${productVariantName}</a>`;
  priceDetail.innerHTML = data.product.variants[0]
    ? `${VNDong.format(data.product.variants[0]?.price)}`
    : `Not set`;
  priceDetail.dataset.itemPriceValue = data.product.variants[0]
    ? data.product.variants[0]?.price
    : "";
  descriptionDetail.innerHTML = data.product.description;
  detailImages.innerHTML = BuildPreviewGallaryImages(data);
  numberDetail.value = 1;
};
// build preview error view
//

export const GetProductDetailById = async (productId) => {
  const result = await fetch(`${baseApiUrl}/product/detail/${productId}`);
  if (result.ok) {
    return await result.json();
  }
  return null;
};
// cart

//
const AddProductItemQuickViewEvent = (e, index) => {
  e.addEventListener("click", async () => {
    const productId = e.dataset.productId;
    if (!productId) console.log("empty");

    const data = await GetProductDetailById(productId);
    if (data) {
      BuildProductPreviewModal(data);
      // add variants change event
      VariantChangeEvent(data);
    }
  });
};

const DropListSelectVariantsAction = (data) => {
  const priceDetail = document.querySelector(".js-price-detail");
  const nameDetail = document.querySelector(".js-name-detail");
  const currentUrlPath = window.location.pathname;
  const selectedElement = document.querySelector(".js-variants-select");
  if (!selectedElement) return;

  const selectedValue = selectedElement.value;
  const selectedText =
    selectedElement.options[selectedElement.selectedIndex].text;

  if (currentUrlPath.includes("/products/productdetail"))
    ChangeCurrentUrlWithoutReload(
      `${RemoveLastPart(currentUrlPath)}/${selectedValue}`
    );
  const selectedVariant = data.product.variants.filter(
    (x) => x.id == selectedValue
  )[0];
  const variantPrice = selectedVariant?.price;

  if (nameDetail)
    nameDetail.innerHTML = `<a class="cl2" href="/products/productdetail/${selectedValue}">${data.product.productFriendlyName}(${selectedVariant?.code})</a>`;
  priceDetail.dataset.itemPriceValue = variantPrice;
  priceDetail.innerHTML = variantPrice
    ? VNDong.format(variantPrice)
    : `Not set`;
};

export const VariantChangeEvent = (data) => {
  const variantsElement = document.querySelector(".js-variants-select");

  if (!variantsElement) return;

  Select2EventHelper(variantsElement, DropListSelectVariantsAction, data);
};

const AddQuickViewProductEvent = (btnQuickViews) => {
  if (!btnQuickViews) return;

  btnQuickViews.forEach((e, index) => {
    AddProductItemQuickViewEvent(e);
  });
};

// Paging
const GetProducts = async (page, pageSize) => {
  const result = await fetch(
    `${baseApiUrl}/product?page=${page}&pageSize=${pageSize}`
  );
  if (result.ok) {
    return await result.json();
  }
  return null;
};

const RefreshModalEvent = (modalClass) => {
  AddQuickViewProductEvent(document.querySelectorAll(`.${modalClass}`));
  $(`.${modalClass}`).on("click", function (e) {
    e.preventDefault();
    $(".js-modal1").addClass("show-modal1");
  });
};

const BuildProductLoadMore = (datas, currentPage) => {
  let products = ``;
  const modalClass = `js-show-modal${currentPage}`;
  datas.forEach((item, index) => {
    const productImage = item.productImageName
      ? `<img src="images/products/${item.productImageName}" alt="${item.product?.name}">`
      : `<img src="images/default-placeholder.jpg" alt="${item.product?.name}">`;
    const productPrice = item.product?.variants[0]
      ? `${VNDong.format(item.product?.variants[0].price)}`
      : `Not set`;

    const product = ` <div class="col-sm-6 col-md-4 col-lg-3 p-b-35 isotope-item women ratio">
                        <!-- Block2 -->
                        <div class="block2">
                            <div class="block2-pic hov-img0">
                                ${productImage}
                                <a href="#" class="block2-btn flex-c-m stext-103 cl2 size-102 bg0 bor2 hov-btn1 p-lr-15 trans-04 ${modalClass}" data-product-id="${item.product?.id}">
                                    Quick View
                                </a>
                            </div>

                            <div class="block2-txt flex-w flex-t p-t-14">
                                <div class="block2-txt-child1 flex-col-l ">
                                    <a href="product-detail.html" class="stext-104 cl4 hov-cl1 trans-04 js-name-b2 p-b-6">
                                        ${item.product?.productFriendlyName}
                                    </a>

                                    <span class="stext-105 cl3">
                                        ${productPrice}
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>`;
    products += product;
    console.log(item);
  });

  listProducts.insertAdjacentHTML("beforeend", products);
  RefreshModalEvent(modalClass);
  //
  //
};
const ResetButtonLoadMore = (nextPage) => {
  const totalPage = btnLoadMore.dataset.totalPage;
  if (!nextPage || !totalPage) return;

  if (nextPage > totalPage) {
    btnLoadMore.parentNode.remove();
  } else {
    btnLoadMore.dataset.nextPage = nextPage;
  }
};

const UpdateUrlPagingParam = (currentPage) => {
  const url = new URL(window.location.href);
  const page = url.searchParams.has("page");
  if (page) {
    url.searchParams.set("page", currentPage);
  } else {
    url.searchParams.append("page", currentPage);
  }
  window.history.pushState({ page: currentPage }, "", url);
};

const LoadMoreEvent = () => {
  if (!btnLoadMore) return;
  btnLoadMore.addEventListener("click", async (e) => {
    e.preventDefault();
    let nextPage = e.target.dataset.nextPage;
    if (nextPage <= 1) nextPage = 2;
    const pageSize = e.target.dataset.pageSize;
    // call api
    const products = await GetProducts(nextPage, pageSize);
    if (products) {
      // append product
      BuildProductLoadMore(products, nextPage);
      // update url paging
      UpdateUrlPagingParam(nextPage);
      // check still paging or not
      ResetButtonLoadMore(++nextPage);
    }
  });
};
//
const Init = () => {
  console.log("outside: productListing.js");
  AddQuickViewProductEvent(btnQuickViews);
  LoadMoreEvent();
};

Init();
