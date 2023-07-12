import { GetProductDetailById, VariantChangeEvent } from "./productListing.js";

const VariantChangeEventProductDetail = async () => {
  const productId = paramsHelper.id
    ? paramsHelper.id
    : getLastItem(window.location.pathname);

  if (!productId) return;

  const data = await GetProductDetailById(productId);
  if (data) {
    VariantChangeEvent(data);
  }
};

const getLastItem = (thePath) =>
  thePath.substring(thePath.lastIndexOf("/") + 1);

const paramsHelper = new Proxy(new URLSearchParams(window.location.search), {
  get: (searchParams, prop) => searchParams.get(prop),
});

const Init = () => {
  console.log("outside: productDetail.js");
  VariantChangeEventProductDetail();
};

Init();
