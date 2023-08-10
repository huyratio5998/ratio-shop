import * as ProductItemsService from "./product-card-items.js";
import * as ProductPackageItemsService from "./package-card-item.js";
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
  pageSize,
  isGetPackages = false
) => {
  const searchParams = new URLSearchParams({
    filterItems: filterItems,
    sortType: sortType,
    isSelectPreviousItems: isSelectPreviousItems,
    page: page,
    pageSize: pageSize,
    isGetPackages: isGetPackages,
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

const RefreshListProducts = (data, pageIndex, pageSize) => {
  if (!data) return;

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
};

const RefreshListProductPackages = (data, pageIndex, pageSize) => {
  if (!data) return;

  if (data.packages.length == 0) {
    const listProducts = document.querySelector(".js-listProducts");
    const message = "Packages have not found!";
    listProducts.innerHTML = `<div class="container mtext-110 cl2 p-b-12">${message}</div>`;
    loadMoreArea.innerHTML = "";
  } else {
    ProductPackageItemsService.BuildProductPackageCartItems(
      data.packages,
      pageIndex
    );

    // build load more button.
    if (pageIndex < +data.totalPage) {
      const nextPage = pageIndex + 1;
      RefreshLoadMoreArea(nextPage, pageSize, data.totalPage);
    } else loadMoreArea.innerHTML = "";
  }
};

// Events
const ProductFilterItemEvent = (
  filterItems,
  filterName,
  filterType,
  activeClass
) => {
  if (!filterItems || !filterType || !filterName) return;

  const ResetFilterActive = () => {
    if (!filterItems) return;

    filterItems.forEach((el) => {
      el.classList.remove(activeClass);
    });
  };

  const ActiveFilterLink = (el) => {
    if (!el) return;

    ResetFilterActive();
    el.classList.add(activeClass);
  };

  filterItems.forEach((el) => {
    el.addEventListener("click", async (e) => {
      e.preventDefault();
      if (el.classList.contains(activeClass)) return;

      const urlParams = new URLSearchParams(window.location.search);
      // get current filter item by type and name
      const ListFilterItemsApplied = urlParams.get("filterItems")
        ? JSON.parse(urlParams.get("filterItems"))
        : [];

      const currentFilterItemTypeApplied = ListFilterItemsApplied?.filter(
        (x) => x.fieldName == filterName && x.type == filterType
      );
      // prevent call api many times
      const selectedFilterTypeValue = el.dataset.value;
      if (currentFilterItemTypeApplied[0]?.value == selectedFilterTypeValue)
        return;

      ActiveFilterLink(el);
      // get params data
      let filterItems;
      if (ListFilterItemsApplied && ListFilterItemsApplied.length > 0) {
        if (
          currentFilterItemTypeApplied &&
          currentFilterItemTypeApplied.length > 0
        ) {
          const filterIndex = ListFilterItemsApplied.findIndex(
            (x) => x.fieldName == filterName && x.type == filterType
          );

          ListFilterItemsApplied[filterIndex].value = selectedFilterTypeValue;
        } else {
          ListFilterItemsApplied.push({
            fieldName: filterName,
            value: selectedFilterTypeValue,
            type: filterType,
          });
        }
        filterItems = ListFilterItemsApplied;
      } else {
        filterItems = [
          {
            fieldName: filterName,
            value: selectedFilterTypeValue,
            type: filterType,
          },
        ];
      }
      const filterItemsJson = JSON.stringify(filterItems);
      const currentSortType =
        urlParams.get("sortType") == null ? 0 : urlParams.get("sortType");
      const page = 1;
      const buttonSearch = document.querySelector(".js_search-products-btn");
      const pageSize = buttonSearch ? buttonSearch.dataset.pageSize : 8;
      const packageViewBtn = document.querySelector(".js_toggle-package-view");
      const isGetPackages = packageViewBtn
        ? packageViewBtn.dataset.isPackageView.toLowerCase() == "true"
        : false;

      // call api filter
      const data = await GetProducts(
        filterItemsJson,
        currentSortType,
        false,
        page,
        pageSize,
        isGetPackages
      );

      if (!data) {
        console.error("Can't search product!");
        DisplayMessageInMoment(
          productSearchErrorMessageEl,
          "Can't search product!",
          "",
          5000
        );
      } else {
        // change url
        const urlParamsToChange = [
          {
            Name: "isGetPackages",
            Value: isGetPackages,
            IsNumberAndApplyFilter: false,
            IsStringify: false,
          },
          {
            Name: "filterItems",
            Value: filterItems,
            IsNumberAndApplyFilter: false,
            IsStringify: true,
          },
          {
            Name: "page",
            Value: page,
            IsNumberAndApplyFilter: true,
            IsStringify: false,
          },
        ];
        UpdateUrlParams(urlParamsToChange);
        if (isGetPackages) RefreshListProductPackages(data, page, pageSize);
        else RefreshListProducts(data, page, pageSize);
      }
    });
  });
};

const ProductSortEvent = () => {
  const sortLists = document.querySelectorAll(".js_product-sortBy");
  if (!sortLists) return;

  const ResetSortActive = () => {
    if (!sortLists) return;

    sortLists.forEach((el) => {
      el.classList.remove("filter-link-active");
    });
  };

  const ActiveSortLink = (el) => {
    if (!el) return;

    ResetSortActive();
    el.classList.add("filter-link-active");
  };

  sortLists.forEach((el) => {
    el.addEventListener("click", async (e) => {
      e.preventDefault();
      const urlParams = new URLSearchParams(window.location.search);
      const currentSortType = urlParams.get("sortType");
      const sortTypeValue = el.dataset.value;
      const commonSearchSortType = document.querySelector(".js_commonSortType");
      if (commonSearchSortType) commonSearchSortType.value = sortTypeValue;
      if (sortTypeValue == currentSortType) return;

      ActiveSortLink(el);
      // get params data
      const filterItemsJson = urlParams.get("filterItems")
        ? urlParams.get("filterItems")
        : "";

      const page = 1;
      const packageViewBtn = document.querySelector(".js_toggle-package-view");
      const isGetPackages = packageViewBtn
        ? packageViewBtn.dataset.isPackageView.toLowerCase() == "true"
        : false;
      const buttonSearch = document.querySelector(".js_search-products-btn");
      const pageSize = buttonSearch ? buttonSearch.dataset.pageSize : 8;
      // call api to sort.
      const data = await GetProducts(
        filterItemsJson,
        sortTypeValue,
        false,
        page,
        pageSize,
        isGetPackages
      );

      if (!data) {
        console.error("Can't search product!");
        DisplayMessageInMoment(
          productSearchErrorMessageEl,
          "Can't search product!",
          "",
          5000
        );
      } else {
        // change url
        const urlParamsToChange = [
          {
            Name: "isGetPackages",
            Value: isGetPackages,
            IsNumberAndApplyFilter: false,
            IsStringify: false,
          },
          {
            Name: "sortType",
            Value: sortTypeValue,
            IsNumberAndApplyFilter: false,
            IsStringify: false,
          },
          {
            Name: "page",
            Value: page,
            IsNumberAndApplyFilter: true,
            IsStringify: false,
          },
        ];
        UpdateUrlParams(urlParamsToChange);
        if (isGetPackages) RefreshListProductPackages(data, page, pageSize);
        else RefreshListProducts(data, page, pageSize);
      }
    });
  });
};

const ProductSearchEvent = () => {
  if (!searchInputEl || !searchButtonEl || !productSearchErrorMessageEl) return;

  searchButtonEl.addEventListener("click", async (e) => {
    const previousValue = searchInputEl.dataset.previousValue;
    const searchValue = searchInputEl.value;
    if (previousValue == searchValue) return;
    searchInputEl.dataset.previousValue = searchValue;

    const urlParams = new URLSearchParams(window.location.search);

    // get current filter item by type and name
    const ListFilterItemsApplied = urlParams.get("filterItems")
      ? JSON.parse(urlParams.get("filterItems"))
      : [];

    const currentFilterItemTypeApplied = ListFilterItemsApplied?.filter(
      (x) => x.fieldName == "Name" && x.type == "FreeText"
    );
    // prevent call api many times
    if (currentFilterItemTypeApplied[0]?.value == searchValue) return;

    let filterItems;
    if (ListFilterItemsApplied && ListFilterItemsApplied.length > 0) {
      if (
        currentFilterItemTypeApplied &&
        currentFilterItemTypeApplied.length > 0
      ) {
        const filterIndex = ListFilterItemsApplied.findIndex(
          (x) => x.fieldName == "Name" && x.type == "FreeText"
        );
        ListFilterItemsApplied[filterIndex].value = searchValue;
      } else {
        ListFilterItemsApplied.push({
          fieldName: "Name",
          value: searchValue,
          type: "FreeText",
        });
      }
      filterItems = ListFilterItemsApplied;
    } else {
      filterItems = [
        {
          fieldName: "Name",
          value: searchValue,
          type: "FreeText",
        },
      ];
    }

    // build param call api
    const filterItemsJson = JSON.stringify(filterItems);
    const pageIndex = 1;
    const pageSize =
      searchButtonEl.dataset.pageSize == 0
        ? 5
        : searchButtonEl.dataset.pageSize;
    const currentSortType =
      urlParams.get("sortType") == null ? 0 : urlParams.get("sortType");
    const packageViewBtn = document.querySelector(".js_toggle-package-view");
    const isGetPackages = packageViewBtn
      ? packageViewBtn.dataset.isPackageView.toLowerCase() == "true"
      : false;

    // change common search value
    document.querySelector(".js_commonSearch").value = searchValue;

    const data = await GetProducts(
      filterItemsJson,
      currentSortType,
      false,
      pageIndex,
      pageSize,
      isGetPackages
    );

    if (!data) {
      console.error("Can't search product!");
      DisplayMessageInMoment(
        productSearchErrorMessageEl,
        "Can't search product!",
        "",
        5000
      );
    } else {
      const urlParamsToChange = [
        {
          Name: "isGetPackages",
          Value: isGetPackages,
          IsNumberAndApplyFilter: false,
          IsStringify: false,
        },
        {
          Name: "filterItems",
          Value: filterItems,
          IsNumberAndApplyFilter: false,
          IsStringify: true,
        },
        {
          Name: "page",
          Value: pageIndex,
          IsNumberAndApplyFilter: true,
          IsStringify: false,
        },
      ];
      UpdateUrlParams(urlParamsToChange);
      if (isGetPackages) RefreshListProductPackages(data, pageIndex, pageSize);
      else RefreshListProducts(data, pageIndex, pageSize);
    }
  });

  searchInputEl.addEventListener("keyup", (e) => {
    if (e.keyCode === 13) searchButtonEl.click();
  });
};

const PackageToggleEvent = () => {
  const togglePackageBtn = document.querySelector(".js_toggle-package-view");
  if (!togglePackageBtn) return;

  togglePackageBtn.addEventListener("click", async () => {
    // toogle class active
    togglePackageBtn.classList.toggle("how-active1");
    togglePackageBtn.dataset.isPackageView = !(
      togglePackageBtn.dataset.isPackageView.toLowerCase() == "true"
    );

    // get params data
    const urlParams = new URLSearchParams(window.location.search);
    const currentSortType = urlParams.get("sortType") ?? 0;
    const filterItemsJson = urlParams.get("filterItems")
      ? urlParams.get("filterItems")
      : "";
    const page = 1;
    const buttonSearch = document.querySelector(".js_search-products-btn");
    const pageSize = buttonSearch ? buttonSearch.dataset.pageSize : 8;
    const isGetPackages =
      togglePackageBtn.dataset.isPackageView.toLowerCase() == "true";

    // call api to sort.
    const data = await GetProducts(
      filterItemsJson,
      currentSortType,
      false,
      page,
      pageSize,
      isGetPackages
    );

    if (!data) {
      console.error("Can't search product!");
      DisplayMessageInMoment(
        productSearchErrorMessageEl,
        "Can't search product!",
        "",
        5000
      );
    } else {
      // change url
      const urlParamsToChange = [
        {
          Name: "isGetPackages",
          Value: isGetPackages,
          IsNumberAndApplyFilter: false,
          IsStringify: false,
        },
        {
          Name: "sortType",
          Value: currentSortType,
          IsNumberAndApplyFilter: false,
          IsStringify: false,
        },
        {
          Name: "page",
          Value: page,
          IsNumberAndApplyFilter: true,
          IsStringify: false,
        },
      ];
      UpdateUrlParams(urlParamsToChange);
      if (isGetPackages) RefreshListProductPackages(data, page, pageSize);
      else RefreshListProducts(data, page, pageSize);
    }
  });
};

const ProductFilterAllEvent = () => {
  ProductSearchEvent();
  ProductSortEvent();

  // add event for filter category
  const categoryFilterElements = document.querySelectorAll(
    ".js_category-filter"
  );
  ProductFilterItemEvent(
    categoryFilterElements,
    "Category",
    "Text",
    "how-active1"
  );

  // add event for filter price
  const priceFitlerElements = document.querySelectorAll(
    ".js_priceRange-filter"
  );
  ProductFilterItemEvent(
    priceFitlerElements,
    "Price",
    "NumberRange",
    "filter-link-active"
  );

  // add event for packages
  PackageToggleEvent();
};

export { GetProducts, ProductFilterAllEvent };
