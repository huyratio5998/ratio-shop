import * as ProductItemsService from "./product-card-items.js";
import * as CartCommonService from "../cart/cart-common.js";

// Helper function
const getLastItem = (thePath) =>
  thePath.substring(thePath.lastIndexOf("/") + 1);

const paramsHelper = new Proxy(new URLSearchParams(window.location.search), {
  get: (searchParams, prop) => searchParams.get(prop),
});

const VariantChangeEventProductDetail = async () => {
  const productId = paramsHelper.id
    ? paramsHelper.id
    : getLastItem(window.location.pathname);

  if (!productId) return;

  const data = await ProductItemsService.GetProductDetailById(productId);
  const productDetailArea = document.querySelector(".js_product-detail-area");
  if (data && productDetailArea) {
    ProductItemsService.VariantChangeEvent(data, productDetailArea);
  }
};

// Init
const Init = () => {
  console.log("product-detail.js");
  VariantChangeEventProductDetail();

  const quickViewArea = document.querySelector(".js_quickViewAreaEvent");
  if (quickViewArea) {
    ProductItemsService.AddQuickViewProductEvent(quickViewArea);
    CartCommonService.AddToCartEvent(quickViewArea);
  }
};

Init();
