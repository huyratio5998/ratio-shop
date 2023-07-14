import * as ProductItemsService from "./product-card-items.js";
import * as ProductLoadMoreService from "./product-load-more-service.js";

const searchInputEl = document.querySelector(".js_search-products-input");
const searchButtonEl = document.querySelector(".js_search-products-btn");
const productSearchErrorMessageEl = document.querySelector(
  ".js_product-search-error-message"
);
const loadMoreArea = document.querySelector(".js_load-more-area");

// Api
const GetProducts = async (
  filterItems,
  sortType,
  isSelectPreviousItems,
  page,
  pageSize
) => {
  const searchParams = new URLSearchParams({
    filterItems: JSON.stringify(filterItems),
    sortType: sortType,
    isSelectPreviousItems: isSelectPreviousItems,
    page: page,
    pageSize: pageSize,
  });

  const result = await fetch(
    `${baseApiUrl}/product?${searchParams.toString()}`
  );

  if (result.ok) {
    return await result.json();
  }
  return null;
};

// Helper function
const RefreshLoadMoreArea = (nextPage, pageSize, totalPage) => {
  if (!loadMoreArea) return;

  const loadMoreButton = ` <div class="flex-c-m flex-w w-full p-t-45">
                              <a href="#" class="flex-c-m stext-101 cl5 size-103 bg2 bor1 hov-btn1 p-lr-15 trans-04" id="btn-loadMore" data-next-page=${nextPage} data-page-size=${pageSize} data-total-page=${totalPage}>
                                  Load More
                              </a>
                          </div>`;
  loadMoreArea.innerHTML = loadMoreButton;

  ProductLoadMoreService.LoadMoreEvent();
};

// Events
const ProductSearchEvent = () => {
  if (!searchInputEl || !searchButtonEl || !productSearchErrorMessageEl) return;

  searchButtonEl.addEventListener("click", async (e) => {
    const pageIndex = 1;
    const searchValue = searchInputEl.value;
    const pageSize =
      searchButtonEl.dataset.pageSize == 0
        ? 5
        : searchButtonEl.dataset.pageSize;
    let filterItems = [
      {
        fieldName: "",
        value: searchValue,
        type: "FreeText",
      },
    ];
    if (!searchValue || searchValue == "") filterItems = null;
    // change common search value
    document.querySelector(".js_commonSearch").value = searchValue;

    const data = await GetProducts(filterItems, 0, false, pageIndex, pageSize);

    if (!data) {
      console.error("Can't search product!");
      DisplayMessageInMoment(
        productSearchErrorMessageEl,
        "Can't search product!",
        "",
        5000
      );
    } else {
      ProductLoadMoreService.UpdateUrlListingParam(pageIndex, filterItems);

      if (data.products.length == 0) {
        const listProducts = document.querySelector(".js-listProducts");
        const message = "Products have not found!";
        listProducts.innerHTML = `<div class="container mtext-110 cl2 p-b-12">${message}</div>`;
        loadMoreArea.innerHTML = "";
      } else {
        ProductItemsService.BuildProductCartItems(data.products, pageIndex);

        // build load more button.
        if (pageIndex < +data.totalPage) {
          const nextPage = pageIndex + 1;
          RefreshLoadMoreArea(nextPage, pageSize, data.totalPage);
        } else loadMoreArea.innerHTML = "";
      }
    }
  });

  searchInputEl.addEventListener("keyup", (e) => {
    if (e.keyCode === 13) searchButtonEl.click();
  });
};

export { GetProducts, ProductSearchEvent };
