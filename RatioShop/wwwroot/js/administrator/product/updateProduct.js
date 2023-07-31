let data, variantEditIndex;
let RemoveVariants = [];
let btnEditVariants = document.querySelectorAll(".variant-edit");
let btnDeleteVariants = document.querySelectorAll(".variant-delete");
const btnSave = document.querySelector("#saveAdditionalInformation");
const btnAdvancedSetting = document.querySelector("#advance-settings");
const advancedSettingArea = document.querySelector("#advance-setting-area");

const formVariant = document.querySelector("#form-variant");
const variantModal = document.querySelector("#variantModal");
const btnModalVariantClose = document.querySelector("#btn-modalVariantClose");

const btnCreateVariants = document.querySelector("#addVariant");
const responseMessage = document.querySelector("#response-message");
// product category
let RemoveProductCategories = [];
let productCategoryEditIndex;
let btnEditProductCategories = document.querySelectorAll(
  ".productCategory-edit"
);
let btnDeleteProductCategories = document.querySelectorAll(
  ".productCategory-delete"
);
const formProductCategories = document.querySelector("#form-productCategory");
const productCategoriesModal = document.querySelector("#productCategoryModal");
const btnModalProductCategoriesClose = document.querySelector(
  "#btn-modalProductCategoryClose"
);
const btnCreateProductCategory = document.querySelector("#addProductCategory");

const updateProductAdditionalInfor = async (data) => {
  try {
    data = {
      productId: currentProductId,
      variants: productVariants,
      removeVariants: RemoveVariants,
      productCategories: productCategories,
      removeProductCategories: RemoveProductCategories,
    };
    const response = await fetch(
      "/products/UpdateProductAdditionalInformation",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
      }
    );

    const result = await response.json();
    if (result.Status) {
      productVariants = result.Variants;
      refreshVariantsTableData("#variantTable");
      responseMessage.style.color = "green";
      responseMessage.innerHTML = "Save success!!";
      setTimeout(() => {
        responseMessage.innerHTML = "";
      }, 3000);
    } else {
      responseMessage.style.color = "red";
      responseMessage.innerHTML = "Save fail!!";
    }
  } catch (error) {
    responseMessage.style.color = "red";
    responseMessage.innerHTML = "Save fail!!";
  }
};

const hidePopup = () => {
  btnModalVariantClose.click();
  btnModalProductCategoriesClose.click();
};

//Variants
const refreshVariantsTableData = (tableId) => {
  const tbodyRef = document
    .querySelector(tableId)
    .getElementsByTagName("tbody")[0];
  if (tbodyRef) {
    tbodyRef.innerHTML = "";
    productVariants.forEach((x, index) => {
      const newRow = tbodyRef.insertRow(-1);
      newRow.insertCell(0).appendChild(document.createTextNode(x.Code));
      newRow.insertCell(1).appendChild(document.createTextNode(x.Number));
      newRow
        .insertCell(2)
        .appendChild(document.createTextNode(VNDong.format(x.Price)));
      newRow.insertCell(3).appendChild(document.createTextNode(x.DiscountRate));
      //
      const editNode = document.createElement("a");
      const deleteNode = document.createElement("a");
      editNode.classList.add("variant-edit");
      editNode.classList.add("mouse-hover");
      editNode.setAttribute("data-bs-toggle", "modal");
      editNode.setAttribute("data-bs-target", "#variantModal");
      editNode.appendChild(document.createTextNode("Edit"));

      deleteNode.classList.add("variant-delete");
      deleteNode.classList.add("mouse-hover");
      deleteNode.appendChild(document.createTextNode("Delete"));

      let actionCell = newRow.insertCell(4);
      actionCell.setAttribute("data-variantId", x.Id);
      actionCell.setAttribute("data-index", index);
      actionCell.appendChild(editNode);
      actionCell.appendChild(document.createTextNode(" | "));
      actionCell.appendChild(deleteNode);
    });

    //
    btnEditVariants = document.querySelectorAll(".variant-edit");
    btnDeleteVariants = document.querySelectorAll(".variant-delete");
    // add event after refresh table
    tableVariantsEvent();
  }
};

const resetVariantsPopupField = () => {
  document.querySelector("#variant-code").value = "";
  document.querySelector("#variant-number").value = "";
  document.querySelector("#variant-price").value = "";
  document.querySelector("#variant-discountRate").value = "";
  document.querySelector("#variant-images-string").value = "";
  document.querySelector("#variant-images-string-display").innerHTML = "";
  document.querySelector("#variant-images").value = "";
  document.querySelector("#variant-type").selectedIndex = 0;
  document
    .querySelector("#variantStocksTable")
    .getElementsByTagName("tbody")[0].innerHTML = "";
  variantEditIndex = undefined;
};
const resetProductCategoriesPopupField = () => {
  document.querySelector("#categoryId").value = availableCategories[0].Id;
  productCategoryEditIndex = undefined;
};
// Stock
const buildBtnDeleteVariantStock = (iconSrc) => {
  const deleteIcon = document.createElement("img");
  deleteIcon.src = iconSrc;
  deleteIcon.alt = "delete icon";
  deleteIcon.style.width = "25px";
  deleteIcon.classList.add("mouse-hover");

  // add event click
  deleteIcon.addEventListener("click", (e) => {
    e.target.parentNode.parentNode.remove();
    updateProductVariantNumber();
  });
  return deleteIcon;
};

const refreshVariantStockTable = (productVariantStocks) => {
  const tbodyRef = document
    .querySelector("#variantStocksTable")
    .getElementsByTagName("tbody")[0];
  if (tbodyRef) {
    tbodyRef.innerHTML = "";
    if (productVariantStocks) {
      productVariantStocks.forEach((x, index) => {
        const newRow = tbodyRef.insertRow(-1);
        const inputNumber = document.createElement("input");
        inputNumber.setAttribute("type", "number");
        inputNumber.classList.add("productVariantNumber");
        inputNumber.value = x.ProductNumber;

        newRow
          .insertCell(0)
          .appendChild(buildAvailableStockElements(x.StockId, true));
        newRow.insertCell(1).appendChild(inputNumber);
        productVariantNumberEvent();

        const deleteIcon = buildBtnDeleteVariantStock("/images/delete.png");
        newRow.insertCell(2).appendChild(deleteIcon);
      });
    }
  }
};

const buildAvailableStockElements = (defaultId, isSetDefaultValue) => {
  const selectListStock = document.createElement("select");
  selectListStock.setAttribute("name", "vaiantStockSelect");

  Object.keys(availableStocks).forEach((key, index) => {
    const opt = document.createElement("option");
    opt.value = key;
    opt.innerHTML = availableStocks[key];
    if (isSetDefaultValue && defaultId == key) {
      opt.selected = true;
    }

    selectListStock.appendChild(opt);
  });

  return selectListStock;
};

const addProductVariantStock = () => {
  const tbodyRef = document
    .querySelector("#variantStocksTable")
    .getElementsByTagName("tbody")[0];
  if (tbodyRef) {
    const newRow = tbodyRef.insertRow();
    const selectListStock = buildAvailableStockElements(0, false);

    newRow.insertCell(0).appendChild(selectListStock);

    const inputNumber = document.createElement("input");
    inputNumber.setAttribute("type", "number");
    inputNumber.classList.add("productVariantNumber");
    newRow.insertCell(1).appendChild(inputNumber);
    productVariantNumberEvent();

    const deleteIcon = buildBtnDeleteVariantStock("/images/delete.png");
    newRow.insertCell(2).appendChild(deleteIcon);
  }
};

const getVariantStockValue = () => {
  let result = [];
  const tbodyRef = document
    .querySelector("#variantStocksTable")
    .getElementsByTagName("tbody")[0];

  if (tbodyRef) {
    const listRows = tbodyRef.getElementsByTagName("tr");
    for (let item of listRows) {
      const stockId = parseInt(
        item
          .getElementsByTagName("td")[0]
          .querySelector('[name="vaiantStockSelect"]').value
      );
      const productNumber = item
        .getElementsByTagName("td")[1]
        .querySelector("input").value;

      result.push({
        StockId: stockId,
        ProductNumber: productNumber,
      });
    }
  }
  return result;
};
// Categories
const refreshProductCategoriesTableData = (tableId) => {
  const tbodyRef = document
    .querySelector(tableId)
    .getElementsByTagName("tbody")[0];
  if (tbodyRef) {
    tbodyRef.innerHTML = "";
    productCategories.forEach((x, index) => {
      const newRow = tbodyRef.insertRow(-1);
      newRow.insertCell(0).appendChild(document.createTextNode(x.DisplayName));
      //
      const editNode = document.createElement("a");
      const deleteNode = document.createElement("a");
      editNode.classList.add("productCategory-edit");
      editNode.classList.add("mouse-hover");
      editNode.setAttribute("data-bs-toggle", "modal");
      editNode.setAttribute("data-bs-target", "#productCategoryModal");
      editNode.appendChild(document.createTextNode("Edit"));

      deleteNode.classList.add("productCategory-delete");
      deleteNode.classList.add("mouse-hover");
      deleteNode.appendChild(document.createTextNode("Delete"));

      let actionCell = newRow.insertCell(1);
      actionCell.setAttribute("data-productCategoryId", x.Id);
      actionCell.setAttribute("data-index", index);
      actionCell.appendChild(editNode);
      actionCell.appendChild(document.createTextNode(" | "));
      actionCell.appendChild(deleteNode);
    });

    //
    btnEditProductCategories = document.querySelectorAll(
      ".productCategory-edit"
    );
    btnDeleteProductCategories = document.querySelectorAll(
      ".productCategory-delete"
    );
    // add event after refresh table
    tableProductCategoriesEvent();
  }
};

// Event
const popupVariantsSubmitEvent = () => {
  formVariant.addEventListener("submit", async (e) => {
    e.preventDefault();
    const variantCode = document.querySelector("#variant-code").value;
    const newVariantValue = {
      Code: variantCode,
      Number: document.querySelector("#variant-number").value,
      Price: document.querySelector("#variant-price").value,
      DiscountRate: document.querySelector("#variant-discountRate").value,
      Images: document.querySelector("#variant-images-string").value,
      Type: Number.parseInt(document.querySelector("#variant-type").value),
    };

    // post images
    let variantImagesString = [];
    const variantImages = document.querySelector("#variant-images").files;
    if (variantImages && variantImages.length > 0) {
      var data = new FormData();
      for (const file of variantImages) {
        variantImagesString.push(file.name);
        data.append("variantImages", file);
      }

      const response = await fetch("/products/SubmitVariantImages", {
        method: "POST",
        body: data,
      });
      const result = await response.json();
      if (result) newVariantValue.Images = variantImagesString.join();
    }

    // update data in variable
    if (variantEditIndex) {
      productVariants[variantEditIndex].Images = newVariantValue.Images;
      productVariants[variantEditIndex].Type = Number.parseInt(
        newVariantValue.Type
      );
      productVariants[variantEditIndex].Code = newVariantValue.Code;
      productVariants[variantEditIndex].Number = Number.parseInt(
        newVariantValue.Number
      );
      productVariants[variantEditIndex].Price = Number.parseFloat(
        newVariantValue.Price
      );
      productVariants[variantEditIndex].DiscountRate = Number.parseFloat(
        newVariantValue.DiscountRate
      );

      // update stockVariant variable
      productVariants[variantEditIndex].ProductVariantStocks =
        getVariantStockValue();
    } else {
      if (!productVariants) productVariants = [];
      newVariantValue.ProductVariantStocks = getVariantStockValue();
      productVariants.push(newVariantValue);
    }

    refreshVariantsTableData("#variantTable");
    resetVariantsPopupField();
    hidePopup();
  });
};

const tableVariantsEvent = () => {
  btnEditVariants.forEach((element) => {
    element.addEventListener("click", (e) => {
      const index = e.currentTarget.parentNode.dataset.index;
      variantEditIndex = index;
      document.querySelector("#variant-code").value =
        productVariants[index].Code;
      document.querySelector("#variant-number").value =
        productVariants[index].Number;
      document.querySelector("#variant-price").value =
        productVariants[index].Price;
      document.querySelector("#variant-discountRate").value =
        productVariants[index].DiscountRate;
      document.querySelector("#variant-images-string").value =
        productVariants[index].Images;
      document.querySelector("#variant-images-string-display").innerHTML =
        productVariants[index].Images;
      document.querySelector("#variant-type").value =
        productVariants[index].Type;
      // generate variant stock table
      refreshVariantStockTable(productVariants[index].ProductVariantStocks);
    });
  });

  btnDeleteVariants.forEach((element) => {
    element.addEventListener("click", (e) => {
      const index = e.currentTarget.parentNode.dataset.index;
      const variantId = e.currentTarget.parentNode.dataset.variantid;
      if (index > -1) {
        if (variantId) {
          RemoveVariants.push(variantId);
        }
        productVariants.splice(index, 1);
        refreshVariantsTableData("#variantTable");
      }
    });
  });
};
//Product category
const popupProductCategorySubmitEvent = () => {
  formProductCategories.addEventListener("submit", (e) => {
    e.preventDefault();
    const selectedCategory = document.querySelector("#categoryId");
    const newProdutCategoryValue = {
      Id: selectedCategory.value,
      DisplayName:
        selectedCategory.options[selectedCategory.selectedIndex].text,
    };

    if (productCategoryEditIndex) {
      // if change => remove on list.
      if (
        productCategories[productCategoryEditIndex].Id !=
        newProdutCategoryValue.Id
      ) {
        RemoveProductCategories.push(
          productCategories[productCategoryEditIndex].Id
        );
      }
      productCategories[productCategoryEditIndex].Id =
        newProdutCategoryValue.Id;
      productCategories[productCategoryEditIndex].DisplayName =
        newProdutCategoryValue.DisplayName;
    } else {
      if (!productCategories) productCategories = [];
      productCategories.push(newProdutCategoryValue);
    }

    refreshProductCategoriesTableData("#productCategoryTable");
    resetProductCategoriesPopupField();
    hidePopup();
  });
};
const tableProductCategoriesEvent = () => {
  btnEditProductCategories.forEach((element) => {
    element.addEventListener("click", (e) => {
      const index = e.currentTarget.parentNode.dataset.index;
      productCategoryEditIndex = index;
      document.querySelector("#categoryId").value = productCategories[index].Id;
    });
  });

  btnDeleteProductCategories.forEach((element) => {
    element.addEventListener("click", (e) => {
      const index = e.currentTarget.parentNode.dataset.index;
      const productCategoryId =
        e.currentTarget.parentNode.dataset.productcategoryid;
      if (index > -1) {
        if (productCategoryId) {
          RemoveProductCategories.push(productCategoryId);
        }
        productCategories.splice(index, 1);
        refreshProductCategoriesTableData("#productCategoryTable");
      }
    });
  });
};

// Product stock event

const addProductStockEvent = () => {
  const btnAdd = document.querySelector("#variant-addStock");
  btnAdd.addEventListener("click", () => {
    addProductVariantStock();
  });
};

const updateProductVariantNumber = () => {
  let totalNumber = 0;
  document.querySelectorAll(".productVariantNumber").forEach((y) => {
    totalNumber += parseInt(y.value ? y.value : "0");
  });
  document.querySelector("#variant-number").value = totalNumber;
};

const productVariantNumberEvent = () => {
  const btnProductVariantNumber = document.querySelectorAll(
    ".productVariantNumber"
  );
  btnProductVariantNumber.forEach((x) => {
    x.addEventListener("input", (e) => {
      updateProductVariantNumber();
    });
  });
};
//
const initEvent = () => {
  popupVariantsSubmitEvent();
  tableVariantsEvent();
  popupProductCategorySubmitEvent();
  tableProductCategoriesEvent();
  addProductStockEvent();
  //
  btnCreateVariants.addEventListener("click", () => {
    resetVariantsPopupField();
  });
  btnCreateProductCategory.addEventListener("click", () => {
    resetProductCategoriesPopupField();
  });
  btnAdvancedSetting.addEventListener("click", () => {
    advancedSettingArea.classList.toggle("hidden");
  });
  // save all additional information
  btnSave.addEventListener("click", () => {
    updateProductAdditionalInfor(data);
  });
};

initEvent();
