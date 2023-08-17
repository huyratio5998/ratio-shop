import * as PackageItemsService from "./package-card-item.js";
import * as CartCommonService from "../cart/cart-common.js";

// Init
const Init = () => {
  const quickViewArea = document.querySelector(".js_quickViewAreaEvent");
  if (quickViewArea) {
    PackageItemsService.AddQuickViewProductPackageEvent(quickViewArea);
    CartCommonService.AddToCartEvent(quickViewArea);
  }
};

Init();
