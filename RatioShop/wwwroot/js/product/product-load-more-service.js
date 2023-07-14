import * as ProductItemsService from "./product-card-items.js";
import * as ProductSearchService from "./product-search-service.js";

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

const UpdateUrlListingParam = (currentPage, filterItems) => {
  const url = new URL(window.location.href);

  // search text
  if (filterItems) {
    const searchFilterItems = url.searchParams.has("filterItems");
    const filterItemsJson = JSON.stringify(filterItems);
    if (searchFilterItems) {
      url.searchParams.set("filterItems", filterItemsJson);
    } else {
      url.searchParams.append("filterItems", filterItemsJson);
    }
  } else {
    url.searchParams.delete("filterItems");
  }

  // paging
  if (currentPage == 1 || currentPage == 0) {
    url.searchParams.delete("page");
  } else {
    const page = url.searchParams.has("page");
    if (page) {
      url.searchParams.set("page", currentPage);
    } else {
      url.searchParams.append("page", currentPage);
    }
  }

  window.history.pushState(
    { filterItems: JSON.stringify(filterItems), page: currentPage },
    "",
    url
  );
};

// Event
const LoadMoreEvent = () => {
  const btnLoadMore = document.querySelector("#btn-loadMore");
  if (!btnLoadMore) return;

  btnLoadMore.addEventListener("click", async (e) => {
    e.preventDefault();
    let nextPage = e.target.dataset.nextPage;
    if (nextPage <= 1) nextPage = 2;

    const pageSize = e.target.dataset.pageSize;
    const searchInputEl = document.querySelector(".js_search-products-input");
    let filterItems = [
      {
        fieldName: "",
        value: !searchInputEl ? "" : searchInputEl.value,
        type: "FreeText",
      },
    ];

    if (!searchInputEl || searchInputEl.value == "") filterItems = null;
    // call api

    const data = await ProductSearchService.GetProducts(
      filterItems,
      0,
      false,
      nextPage,
      pageSize
    );

    if (!data) {
      console.error("Can't get products");
    } else {
      const products = data.products;
      if (products) {
        // append product
        ProductItemsService.BuildAdditionProductCartItems(products, nextPage);
        // update url paging
        UpdateUrlListingParam(nextPage, filterItems);
        // check still paging or not
        ResetButtonLoadMore(++nextPage);
      }
    }
  });
};

export { LoadMoreEvent, UpdateUrlListingParam };
