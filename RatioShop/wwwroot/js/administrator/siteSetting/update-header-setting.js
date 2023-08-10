const navigationTable = document.querySelector("#navigationTable");
const navigationTextInput = document.querySelector("#navigation-text");
const navigationUrlInput = document.querySelector("#navigation-url");
const navigationIdInput = document.querySelector("#navigation-id");
const navigationParentIdInput = document.querySelector("#navigation-parentId");
const btnSaveNavigation = document.querySelector("#navigation-save");
const btnCreateNavigationItem = document.querySelector("#addHeaderNavigation");
const listNavigationInput = document.querySelector("#list-navigation-input");
// Helpers
const ResetNavigationFormValue = () => {
  if (
    !navigationTextInput ||
    !navigationUrlInput ||
    !navigationIdInput ||
    !navigationParentIdInput
  )
    return;

  navigationTextInput.value = "";
  navigationUrlInput.value = "";
  navigationIdInput.value = "";
  navigationParentIdInput.value = "";

  btnSaveNavigation.dataset.action = "";
  btnSaveNavigation.dataset.index = "";
};

const HidePopup = () => {
  document.querySelector("#btn-modalNavigationClose")?.click();
};

const GetListNavigations = () => {
  const listNavigations = listNavigationInput.value
    ? JSON.parse(listNavigationInput.value)
    : [];
  return listNavigations;
};

const RefreshNavigationTable = (tableId) => {
  const listNavigations = GetListNavigations();
  const tbodyRef = document
    .querySelector(tableId)
    .getElementsByTagName("tbody")[0];
  if (tbodyRef) {
    tbodyRef.innerHTML = "";
    listNavigations.forEach((x, index) => {
      const newRow = tbodyRef.insertRow(-1);
      newRow.insertCell(0).appendChild(document.createTextNode(x.Id ?? ""));
      newRow.insertCell(1).appendChild(document.createTextNode(x.Text));
      newRow.insertCell(2).appendChild(document.createTextNode(x.Url));
      newRow
        .insertCell(3)
        .appendChild(document.createTextNode(x.ParentId ?? ""));

      const editNode = document.createElement("a");
      editNode.classList.add("navigation-edit");
      editNode.classList.add("mouse-hover");
      editNode.setAttribute("data-bs-toggle", "modal");
      editNode.setAttribute("data-bs-target", "#addHeaderNavigationModal");
      editNode.appendChild(document.createTextNode("Edit"));

      const deleteNode = document.createElement("a");
      deleteNode.classList.add("navigation-delete");
      deleteNode.classList.add("mouse-hover");
      deleteNode.appendChild(document.createTextNode("Delete"));

      let actionCell = newRow.insertCell(4);
      actionCell.setAttribute("data-index", index);
      actionCell.appendChild(editNode);
      actionCell.appendChild(document.createTextNode(" | "));
      actionCell.appendChild(deleteNode);
    });

    // add event after refresh table
    TableNavigationsEvent();
  }
};

// Events
const NavigationSaveEvent = () => {
  if (!navigationTable || !navigationTextInput || !navigationUrlInput) return;
  if (!btnSaveNavigation) return;

  btnSaveNavigation.addEventListener("click", () => {
    const action = btnSaveNavigation.dataset.action;
    if (!action) return;

    const index = btnSaveNavigation.dataset.index;
    const listNavigations = GetListNavigations();

    if (action == "create") {
      const navigationItem = {
        Text: navigationTextInput.value,
        Url: navigationUrlInput.value,
        Id: navigationIdInput.value,
        ParentId: navigationParentIdInput.value,
      };
      listNavigations.push(navigationItem);
    } else if (action == "edit" && index >= 0) {
      listNavigations[index].Text = navigationTextInput.value;
      listNavigations[index].Url = navigationUrlInput.value;
      listNavigations[index].Id = navigationIdInput.value;
      listNavigations[index].ParentId = navigationParentIdInput.value;
    }

    listNavigationInput.value = JSON.stringify(listNavigations);
    ResetNavigationFormValue();
    RefreshNavigationTable("#navigationTable");
    HidePopup();
  });
};

const TableNavigationsEvent = () => {
  const btnEditNavigations = document.querySelectorAll(".navigation-edit");
  const btnDeleteNavigations = document.querySelectorAll(".navigation-delete");
  let listNavigations = GetListNavigations();

  if (btnEditNavigations) {
    btnEditNavigations.forEach((element) => {
      element.addEventListener("click", (e) => {
        const index = e.currentTarget.parentNode.dataset.index;
        navigationTextInput.value = listNavigations[index].Text;
        navigationUrlInput.value = listNavigations[index].Url;
        navigationIdInput.value = listNavigations[index].Id;
        navigationParentIdInput.value = listNavigations[index].ParentId;

        btnSaveNavigation.dataset.action = "edit";
        btnSaveNavigation.dataset.index = index;

        NavigationSaveEvent(index);
      });
    });
  }

  if (btnDeleteNavigations) {
    btnDeleteNavigations.forEach((element) => {
      element.addEventListener("click", (e) => {
        const index = e.currentTarget.parentNode.dataset.index;
        if (index > -1) {
          listNavigations.splice(index, 1);
          listNavigationInput.value = JSON.stringify(listNavigations);
          RefreshNavigationTable("#navigationTable");
        }
      });
    });
  }
};

const AddNavigationEvent = () => {
  if (!btnCreateNavigationItem) return;

  btnCreateNavigationItem.addEventListener("click", () => {
    ResetNavigationFormValue();
    btnSaveNavigation.dataset.action = "create";
    btnSaveNavigation.dataset.index = -1;
  });
};

const Init = () => {
  AddNavigationEvent();

  NavigationSaveEvent();

  RefreshNavigationTable("#navigationTable");
};

Init();
