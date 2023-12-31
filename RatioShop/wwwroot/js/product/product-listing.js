﻿import * as ProductItemsService from "./product-card-items.js";
import * as PackageItemsService from "./package-card-item.js";
import * as ProductLoadMoreService from "./product-load-more-service.js";
import * as ProductSearchService from "./product-search-service.js";

// Init
const Init = () => {
  ProductItemsService.AddQuickViewProductEvent();
  PackageItemsService.AddQuickViewProductPackageEvent();
  ProductLoadMoreService.LoadMoreEvent();
  ProductSearchService.ProductFilterAllEvent();
};

Init();
