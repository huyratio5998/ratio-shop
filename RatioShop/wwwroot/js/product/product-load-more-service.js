import * as ProductItemsService from "./product-card-items.js";
import * as ProductSearchService from "./product-search-service.js";
import * as ProductPackageItemsService from "./package-card-item.js";

// Helper function
const ResetButtonLoadMore = (nextPage) => {
  const btnLoadMore = document.querySelector("#btn-loadMore");
  if (!btnLoadMore) return;

  const totalPage = btnLoadMore.dataset.totalPage;
  if (!nextPage || !totalPage) return;

  if (nextPage > totalPage) {
    btnLoadMore.parentNode.remove();
  } else {
    btnLoadMore.dataset.nextPage = nextPage;
  }
};

// Event
const LoadMoreEvent = () => {
  const btnLoadMore = document.querySelector("#btn-loadMore");
  if (!btnLoadMore) return;

  btnLoadMore.addEventListener("click", async (e) => {
    e.preventDefault();

    // get params data
    const urlParams = new URLSearchParams(window.location.search);
    const filterItems = urlParams.get("filterItems")
      ? JSON.parse(urlParams.get("filterItems"))
      : [];
    const sortType = urlParams.get("sortType") ? urlParams.get("sortType") : 0;
    const filterItemsJson = JSON.stringify(filterItems);
    let nextPage = e.target.dataset.nextPage;
    if (nextPage <= 1) nextPage = 2;
    const pageSize = e.target.dataset.pageSize;
    const packageViewBtn = document.querySelector(".js_toggle-package-view");
    const isGetPackages = packageViewBtn
      ? packageViewBtn.dataset.isPackageView.toLowerCase() == "true"
      : false;

    // call api
    const data = await ProductSearchService.GetProducts(
      filterItemsJson,
      sortType,
      false,
      nextPage,
      pageSize,
      isGetPackages
    );

    if (!data) {
      console.error("Can't get products");
    } else {
      const productPackages = data.packages;
      const products = data.products;
      if (products || productPackages) {
        if (isGetPackages && productPackages && productPackages.length > 0) {
          ProductPackageItemsService.BuildAdditionProductPackageCartItems(
            data.packages,
            nextPage
          );
        } else if (!isGetPackages && products && products.length > 0) {
          ProductItemsService.BuildAdditionProductCartItems(
            data.products,
            nextPage
          );
        }
        // update url params
        const urlParamsToChange = [
          {
            Name: "isGetPackages",
            Value: isGetPackages,
            IsNumberAndApplyFilter: false,
            IsStringify: false,
          },
          {
            Name: "page",
            Value: nextPage,
            IsNumberAndApplyFilter: true,
            IsStringify: false,
          },
          {
            Name: "filterItems",
            Value: filterItems,
            IsNumberAndApplyFilter: false,
            IsStringify: true,
          },
        ];
        UpdateUrlParams(urlParamsToChange);
        // check still paging or not
        ResetButtonLoadMore(++nextPage);
      }
    }
  });
};

export { LoadMoreEvent };
