import * as ProductItemsService from "./product-card-items.js";
import * as ProductLoadMoreService from "./product-load-more-service.js";
import * as ProductSearchService from "./product-search-service.js";

// Init
const Init = () => {
  console.log("product-listing.js");
  ProductItemsService.AddQuickViewProductEvent();
  ProductLoadMoreService.LoadMoreEvent();
  ProductSearchService.ProductFilterAllEvent();
};

Init();
