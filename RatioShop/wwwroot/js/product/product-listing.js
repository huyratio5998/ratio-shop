import * as ProductItemsService from "./product-card-items.js";
import * as ProductLoadMoreService from "./product-load-more-service.js";
import * as ProductSearchService from "./product-search-service.js";

const btnQuickViews = document.querySelectorAll(".js-show-modal1");

// Init
const Init = () => {
  console.log("product-listing.js");
  ProductItemsService.AddQuickViewProductEvent(btnQuickViews);
  ProductLoadMoreService.LoadMoreEvent();
  ProductSearchService.ProductSearchEvent();
};

Init();
