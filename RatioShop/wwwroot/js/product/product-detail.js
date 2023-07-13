import * as ProductItemsService from "./product-card-items.js";

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
  if (data) {
    ProductItemsService.VariantChangeEvent(data);
  }
};

// Init
const Init = () => {
  console.log("product-detail.js");
  VariantChangeEventProductDetail();
};

Init();
