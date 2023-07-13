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

const UpdateUrlListingParam = (currentPage, searchValue) => {
  const url = new URL(window.location.href);

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

  // search text
  if (searchValue) {
    const searchText = url.searchParams.has("search");
    if (searchText) {
      url.searchParams.set("search", searchValue);
    } else {
      url.searchParams.append("search", searchValue);
    }
  } else {
    url.searchParams.delete("search");
  }

  window.history.pushState({ page: currentPage, search: searchValue }, "", url);
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
    const searchText = !searchInputEl ? "" : searchInputEl.value;

    // call api
    const data = await ProductSearchService.GetProducts(
      searchText,
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
        UpdateUrlListingParam(nextPage, searchText);
        // check still paging or not
        ResetButtonLoadMore(++nextPage);
      }
    }
  });
};

export { LoadMoreEvent, UpdateUrlListingParam };
