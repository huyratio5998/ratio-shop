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

    // call api
    const data = await ProductSearchService.GetProducts(
      filterItemsJson,
      sortType,
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
        // update url params
        const urlParamsToChange = [
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
