const categoryFilterTable = document.querySelector("#categoryFilterTable");
const categoryFilterDisplayNameInput = document.querySelector(
  "#categoryFilter-displayName"
);
const categoryFilterValueInput = document.querySelector(
  "#categoryFilter-value"
);
const categoryFilterPositionInput = document.querySelector(
  "#categoryFilter-position"
);
const btnSaveCategoryFilter = document.querySelector("#categoryFilter-save");
const btnCreateCategoryFilterItem = document.querySelector(
  "#addCategoryFilterItem"
);
const listCategoryFilterInput = document.querySelector(
  "#list-categoryFilter-input"
);

// Helpers
const ResetCategoryFilterFormValue = () => {
  if (
    !categoryFilterDisplayNameInput ||
    !categoryFilterValueInput ||
    !categoryFilterPositionInput
  )
    return;

  categoryFilterDisplayNameInput.value = "";
  categoryFilterValueInput.value = "";
  categoryFilterPositionInput.value = "";

  btnSaveCategoryFilter.dataset.action = "";
  btnSaveCategoryFilter.dataset.index = "";
};

const HidePopup = () => {
  document.querySelector("#btn-modalCategoryFilterClose")?.click();
};

const GetListCategoryFilters = () => {
  const listCategoryFilters = listCategoryFilterInput.value
    ? JSON.parse(listCategoryFilterInput.value)
    : [];
  return listCategoryFilters;
};

const RefreshCategoryFilterTable = (tableId) => {
  const listCategoryFilters = GetListCategoryFilters();
  const tbodyRef = document
    .querySelector(tableId)
    .getElementsByTagName("tbody")[0];
  if (tbodyRef) {
    tbodyRef.innerHTML = "";
    listCategoryFilters.forEach((x, index) => {
      const newRow = tbodyRef.insertRow(-1);
      newRow.insertCell(0).appendChild(document.createTextNode(x.DisplayName));
      newRow.insertCell(1).appendChild(document.createTextNode(x.Value));
      newRow.insertCell(2).appendChild(document.createTextNode(x.Position));

      const editNode = document.createElement("a");
      editNode.classList.add("categoryFilter-edit");
      editNode.classList.add("mouse-hover");
      editNode.setAttribute("data-bs-toggle", "modal");
      editNode.setAttribute("data-bs-target", "#addCategoryFilterModal");
      editNode.appendChild(document.createTextNode("Edit"));

      const deleteNode = document.createElement("a");
      deleteNode.classList.add("categoryFilter-delete");
      deleteNode.classList.add("mouse-hover");
      deleteNode.appendChild(document.createTextNode("Delete"));

      let actionCell = newRow.insertCell(3);
      actionCell.setAttribute("data-index", index);
      actionCell.appendChild(editNode);
      actionCell.appendChild(document.createTextNode(" | "));
      actionCell.appendChild(deleteNode);
    });

    // add event after refresh table
    TableCategoryFiltersEvent();
  }
};

// Events
const CategoryFilterSaveEvent = () => {
  if (
    !categoryFilterTable ||
    !categoryFilterDisplayNameInput ||
    !categoryFilterValueInput ||
    !categoryFilterPositionInput
  )
    return;
  if (!btnSaveCategoryFilter) return;

  btnSaveCategoryFilter.addEventListener("click", () => {
    const action = btnSaveCategoryFilter.dataset.action;
    if (!action) return;

    const index = btnSaveCategoryFilter.dataset.index;
    const listCategoryFilters = GetListCategoryFilters();

    if (action == "create") {
      const categoryFilterItem = {
        DisplayName: categoryFilterDisplayNameInput.value,
        Value: categoryFilterValueInput.value,
        Position: categoryFilterPositionInput.value,
      };
      listCategoryFilters.push(categoryFilterItem);
    } else if (action == "edit" && index >= 0) {
      listCategoryFilters[index].DisplayName =
        categoryFilterDisplayNameInput.value;
      listCategoryFilters[index].Value = categoryFilterValueInput.value;
      listCategoryFilters[index].Position = categoryFilterPositionInput.value;
    }

    listCategoryFilterInput.value = JSON.stringify(listCategoryFilters);
    ResetCategoryFilterFormValue();
    RefreshCategoryFilterTable("#categoryFilterTable");
    HidePopup();
  });
};

const TableCategoryFiltersEvent = () => {
  const btnEditCategoryFilters = document.querySelectorAll(
    ".categoryFilter-edit"
  );
  const btnDeleteCategoryFilters = document.querySelectorAll(
    ".categoryFilter-delete"
  );
  let listCategoryFilters = GetListCategoryFilters();

  if (btnEditCategoryFilters) {
    btnEditCategoryFilters.forEach((element) => {
      element.addEventListener("click", (e) => {
        const index = e.currentTarget.parentNode.dataset.index;
        categoryFilterDisplayNameInput.value =
          listCategoryFilters[index].DisplayName;
        categoryFilterValueInput.value = listCategoryFilters[index].Value;
        categoryFilterPositionInput.value = listCategoryFilters[index].Position;

        btnSaveCategoryFilter.dataset.action = "edit";
        btnSaveCategoryFilter.dataset.index = index;

        CategoryFilterSaveEvent(index);
      });
    });
  }

  if (btnDeleteCategoryFilters) {
    btnDeleteCategoryFilters.forEach((element) => {
      element.addEventListener("click", (e) => {
        const index = e.currentTarget.parentNode.dataset.index;
        if (index > -1) {
          listCategoryFilters.splice(index, 1);
          listCategoryFilterInput.value = JSON.stringify(listCategoryFilters);
          RefreshCategoryFilterTable("#categoryFilterTable");
        }
      });
    });
  }
};

const AddcategoryFilterEvent = () => {
  if (!btnCreateCategoryFilterItem) return;

  btnCreateCategoryFilterItem.addEventListener("click", () => {
    ResetCategoryFilterFormValue();
    btnSaveCategoryFilter.dataset.action = "create";
    btnSaveCategoryFilter.dataset.index = -1;
  });
};

const Init = () => {
  AddcategoryFilterEvent();

  CategoryFilterSaveEvent();

  RefreshCategoryFilterTable("#categoryFilterTable");
};

Init();
