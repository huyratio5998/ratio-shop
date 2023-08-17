//footer info
const listFooterInfos = document.querySelector("#list-footerInfos-input");
const footerInfoAddBtn = document.querySelector("#addFooterInfoItem");
const footerInfoTable = document.querySelector("#footerInfoTable");
const footerInfoTitleInput = document.querySelector("#footerInfo-title");
const footerInfoDescriptionInput = document.querySelector(
  "#footerInfo-description"
);
const footerInfoDisplayInlineInput = document.querySelector(
  "#footerInfo-displayInLine"
);
const footerInfoAddLinkItemBtn = document.querySelector(
  "#footerInfo-addLinkItem"
);
const footerInfoLinkItemsTable = document.querySelector(
  "#footerInfoLinkItemsTable"
);
const footerInfoSaveBtn = document.querySelector("#footerInfo-save");

//footer support payment
const listFooterPaymentSupports = document.querySelector(
  "#list-footerPaymentSupport-input"
);
const footerPaymentSupportAddBtn = document.querySelector(
  "#addPaymentSupportItem"
);
const footerPaymentSupportTable = document.querySelector(
  "#footerPaymentSupportTable"
);
const footerPaymentImageDisplay = document.querySelector("#payment-image");
const footerPaymentImageInput = document.querySelector("#payment-image-input");
const footerPaymentImageValue = document.querySelector("#payment-image-value");
const footerPaymentImageAlt = document.querySelector("#payment-image-alt");
const footerPaymentLink = document.querySelector("#payment-link");
const footerPaymentSaveBtn = document.querySelector("#payment-save");

// Helpers
const ResetFooterInfoModal = () => {
  footerInfoTitleInput.value = "";
  footerInfoDescriptionInput.value = "";
  footerInfoDisplayInlineInput.value = "";

  footerInfoLinkItemsTable.getElementsByTagName("tbody")[0].innerHTML = "";
  footerInfoSaveBtn.dataset.action = "";
  footerInfoSaveBtn.dataset.index = "";
};

const ResetFooterPaymentSupportModal = () => {
  footerPaymentImageDisplay.setAttribute(
    "src",
    "/images/default-placeholder.jpg"
  );
  footerPaymentImageInput.value = "";
  footerPaymentImageValue.value = "";
  footerPaymentImageAlt.value = "";
  footerPaymentLink.value = "";

  footerPaymentSaveBtn.dataset.action = "";
  footerPaymentSaveBtn.dataset.index = "";
};

const HidePopup = () => {
  document.querySelector("#btn-modalFooterInfoClose")?.click();
  document.querySelector("#btn-modalFooterPaymentSupportClose")?.click();
};

const RefreshFooterInfoLinkItemsTable = (footerInfoLinkItems) => {
  const tbodyRef = footerInfoLinkItemsTable.getElementsByTagName("tbody")[0];
  if (tbodyRef) {
    tbodyRef.innerHTML = "";
    if (footerInfoLinkItems) {
      footerInfoLinkItems.forEach((x, index) => {
        const newRow = tbodyRef.insertRow(-1);
        const text = document.createElement("input");
        text.setAttribute("type", "text");
        text.value = x.Text;
        newRow.insertCell(0).appendChild(text);

        const link = document.createElement("input");
        link.setAttribute("type", "text");
        link.value = x.Url;
        newRow.insertCell(1).appendChild(link);

        const deleteIcon = BuildBtnDeleteLinkItem("/images/delete.png");
        newRow.insertCell(2).appendChild(deleteIcon);
      });
    }
  }
};

const BuildBtnDeleteLinkItem = (iconSrc) => {
  const deleteIcon = document.createElement("img");
  deleteIcon.src = iconSrc;
  deleteIcon.alt = "delete icon";
  deleteIcon.style.width = "25px";
  deleteIcon.classList.add("mouse-hover");

  // add event click
  deleteIcon.addEventListener("click", (e) => {
    e.target.parentNode.parentNode.remove();
  });
  return deleteIcon;
};

const AddFooterInfoLinkItem = () => {
  const tbodyRef = footerInfoLinkItemsTable.getElementsByTagName("tbody")[0];
  if (tbodyRef) {
    const newRow = tbodyRef.insertRow();

    const text = document.createElement("input");
    text.setAttribute("type", "text");
    newRow.insertCell(0).appendChild(text);

    const link = document.createElement("input");
    link.setAttribute("type", "text");
    newRow.insertCell(1).appendChild(link);

    const deleteIcon = BuildBtnDeleteLinkItem("/images/delete.png");
    newRow.insertCell(2).appendChild(deleteIcon);
  }
};

const GetFooterInfoLinkItem = () => {
  let result = [];
  const tbodyRef = footerInfoLinkItemsTable.getElementsByTagName("tbody")[0];

  if (tbodyRef) {
    const listRows = tbodyRef.getElementsByTagName("tr");
    for (let item of listRows) {
      const textValue = item
        .getElementsByTagName("td")[0]
        .querySelector("input").value;

      const linkValue = item
        .getElementsByTagName("td")[1]
        .querySelector("input").value;

      result.push({
        Text: textValue,
        Url: linkValue,
      });
    }
  }
  return result;
};

// Get data
const GetListFooterInfos = () => {
  const footerInfos = listFooterInfos.value
    ? JSON.parse(listFooterInfos.value)
    : [];
  return footerInfos;
};
const GetListFooterSupportPayments = () => {
  const paymentSupports = listFooterPaymentSupports.value
    ? JSON.parse(listFooterPaymentSupports.value)
    : [];
  return paymentSupports;
};
// Refresh table
const RefreshFooterInfoTable = (tableId) => {
  const listFooterInfos = GetListFooterInfos();
  const tbodyRef = document
    .querySelector(tableId)
    .getElementsByTagName("tbody")[0];
  if (tbodyRef) {
    tbodyRef.innerHTML = "";
    listFooterInfos.forEach((x, index) => {
      const newRow = tbodyRef.insertRow(-1);
      newRow.insertCell(0).appendChild(document.createTextNode(x.Title));
      newRow.insertCell(1).appendChild(document.createTextNode(x.Description));

      const editNode = document.createElement("a");
      editNode.classList.add("footerInfo-edit");
      editNode.classList.add("mouse-hover");
      editNode.setAttribute("data-bs-toggle", "modal");
      editNode.setAttribute("data-bs-target", "#footerInfoModal");
      editNode.appendChild(document.createTextNode("Edit"));

      const deleteNode = document.createElement("a");
      deleteNode.classList.add("footerInfo-delete");
      deleteNode.classList.add("mouse-hover");
      deleteNode.appendChild(document.createTextNode("Delete"));

      let actionCell = newRow.insertCell(2);
      actionCell.setAttribute("data-index", index);
      actionCell.appendChild(editNode);
      actionCell.appendChild(document.createTextNode(" | "));
      actionCell.appendChild(deleteNode);
    });

    TableFooterInfosEvent();
  }
};
const RefreshFooterSupportPaymentTable = (tableId) => {
  const listSupportPayment = GetListFooterSupportPayments();
  const tbodyRef = document
    .querySelector(tableId)
    .getElementsByTagName("tbody")[0];
  if (tbodyRef) {
    tbodyRef.innerHTML = "";
    listSupportPayment.forEach((x, index) => {
      const newRow = tbodyRef.insertRow(-1);
      const imageElement = document.createElement("img");
      imageElement.src = x.Icon.ImageSrc;
      imageElement.alt = x.Icon.ImageAlt;
      newRow.insertCell(0).appendChild(imageElement);
      newRow.insertCell(1).appendChild(document.createTextNode(x.Url));

      const editNode = document.createElement("a");
      editNode.classList.add("supportPayment-edit");
      editNode.classList.add("mouse-hover");
      editNode.setAttribute("data-bs-toggle", "modal");
      editNode.setAttribute("data-bs-target", "#footerPaymentSupportModal");
      editNode.appendChild(document.createTextNode("Edit"));

      const deleteNode = document.createElement("a");
      deleteNode.classList.add("supportPayment-delete");
      deleteNode.classList.add("mouse-hover");
      deleteNode.appendChild(document.createTextNode("Delete"));

      let actionCell = newRow.insertCell(2);
      actionCell.setAttribute("data-index", index);
      actionCell.appendChild(editNode);
      actionCell.appendChild(document.createTextNode(" | "));
      actionCell.appendChild(deleteNode);
    });

    TableFooterSupportPaymentEvent();
  }
};

// Events
const FooterInfosSaveEvent = () => {
  if (
    !footerInfoTable ||
    !footerInfoTitleInput ||
    !footerInfoDescriptionInput ||
    !footerInfoDisplayInlineInput
  )
    return;
  if (!footerInfoSaveBtn) return;

  footerInfoSaveBtn.addEventListener("click", () => {
    const action = footerInfoSaveBtn.dataset.action;
    if (!action) return;

    const index = footerInfoSaveBtn.dataset.index;
    const footerInfos = GetListFooterInfos();

    if (action == "create") {
      const footerInfo = {
        Title: footerInfoTitleInput.value,
        Description: footerInfoDescriptionInput.value,
        ItemDisplayInline: footerInfoDisplayInlineInput.checked,
        Items: GetFooterInfoLinkItem(),
      };
      footerInfos.push(footerInfo);
    } else if (action == "edit" && index >= 0) {
      footerInfos[index].Title = footerInfoTitleInput.value;
      footerInfos[index].Description = footerInfoDescriptionInput.value;
      footerInfos[index].ItemDisplayInline =
        footerInfoDisplayInlineInput.checked;
      footerInfos[index].Items = GetFooterInfoLinkItem();
    }

    listFooterInfos.value = JSON.stringify(footerInfos);
    ResetFooterInfoModal();
    RefreshFooterInfoTable("#footerInfoTable");
    HidePopup();
  });
};
const FooterSupportPaymentSaveEvent = () => {
  if (
    !footerPaymentSupportTable ||
    !footerPaymentImageValue ||
    !footerPaymentLink
  )
    return;
  if (!footerPaymentSaveBtn) return;

  footerPaymentSaveBtn.addEventListener("click", async () => {
    const action = footerPaymentSaveBtn.dataset.action;
    if (!action) return;

    // save image
    footerPaymentImageValue.value = !footerPaymentImageValue.value
      ? `/images/default-placeholder.jpg`
      : footerPaymentImageValue.value;
    const imageFile = footerPaymentImageInput?.files[0];
    if (imageFile) {
      let data = new FormData();
      let fileName = imageFile.name;
      data.append("file", imageFile);

      const response = await fetch(
        `${baseApiUrl}/file/saveFile?folderName1=images&folderName2=icons`,
        {
          method: "POST",
          body: data,
        }
      );
      if (response.ok) {
        const result = await response.json();
        if (result) footerPaymentImageValue.value = `/images/icons/${fileName}`;
      }
    }

    const index = footerPaymentSaveBtn.dataset.index;
    const supportPayments = GetListFooterSupportPayments();

    if (action == "create") {
      const supportPayment = {
        Icon: {
          ImageSrc: footerPaymentImageValue.value,
          ImageAlt: footerPaymentImageAlt.value,
        },
        Url: footerPaymentLink.value,
      };
      supportPayments.push(supportPayment);
    } else if (action == "edit" && index >= 0) {
      supportPayments[index].Url = footerPaymentLink.value;
      supportPayments[index].Icon.ImageSrc = footerPaymentImageValue.value;
      supportPayments[index].Icon.ImageAlt = footerPaymentImageAlt.value;
    }

    listFooterPaymentSupports.value = JSON.stringify(supportPayments);
    ResetFooterPaymentSupportModal();
    RefreshFooterSupportPaymentTable("#footerPaymentSupportTable");
    HidePopup();
  });
};

const TableFooterInfosEvent = () => {
  const btnEditFooterInfo = document.querySelectorAll(".footerInfo-edit");
  const btnDeleteFooterInfo = document.querySelectorAll(".footerInfo-delete");
  let footerInfos = GetListFooterInfos();

  if (btnEditFooterInfo) {
    btnEditFooterInfo.forEach((element) => {
      element.addEventListener("click", (e) => {
        const index = e.currentTarget.parentNode.dataset.index;
        footerInfoTitleInput.value = footerInfos[index].Title;
        footerInfoDescriptionInput.value = footerInfos[index].Description;
        footerInfoDisplayInlineInput.checked =
          footerInfos[index].ItemDisplayInline;
        footerInfoSaveBtn.dataset.action = "edit";
        footerInfoSaveBtn.dataset.index = index;

        RefreshFooterInfoLinkItemsTable(footerInfos[index].Items);
      });
    });
  }

  if (btnDeleteFooterInfo) {
    btnDeleteFooterInfo.forEach((element) => {
      element.addEventListener("click", (e) => {
        const index = e.currentTarget.parentNode.dataset.index;
        if (index > -1) {
          footerInfos.splice(index, 1);
          listFooterInfos.value = JSON.stringify(footerInfos);
          RefreshFooterInfoTable("#footerInfoTable");
        }
      });
    });
  }
};
const TableFooterSupportPaymentEvent = () => {
  const btnEditSupportPayment = document.querySelectorAll(
    ".supportPayment-edit"
  );
  const btnDeleteSupportPayment = document.querySelectorAll(
    ".supportPayment-delete"
  );
  let supportPayments = GetListFooterSupportPayments();

  if (btnEditSupportPayment) {
    btnEditSupportPayment.forEach((element) => {
      element.addEventListener("click", (e) => {
        ResetFooterPaymentSupportModal();
        const index = e.currentTarget.parentNode.dataset.index;
        footerPaymentImageValue.value = supportPayments[index].Icon.ImageSrc;
        footerPaymentLink.value = supportPayments[index].Url;
        footerPaymentImageAlt.value = supportPayments[index].Icon.ImageAlt;
        footerPaymentImageDisplay.src = supportPayments[index].Icon.ImageSrc;
        footerPaymentSaveBtn.dataset.action = "edit";
        footerPaymentSaveBtn.dataset.index = index;
      });
    });
  }

  if (btnDeleteSupportPayment) {
    btnDeleteSupportPayment.forEach((element) => {
      element.addEventListener("click", (e) => {
        const index = e.currentTarget.parentNode.dataset.index;
        if (index > -1) {
          supportPayments.splice(index, 1);
          listFooterPaymentSupports.value = JSON.stringify(supportPayments);
          RefreshFooterSupportPaymentTable("#footerPaymentSupportTable");
        }
      });
    });
  }
};

const AddFooterInfoEvent = () => {
  if (!footerInfoAddBtn) return;

  footerInfoAddBtn.addEventListener("click", () => {
    ResetFooterInfoModal();
    footerInfoSaveBtn.dataset.action = "create";
    footerInfoSaveBtn.dataset.index = -1;
  });
};
const AddFooterSupportPaymentEvent = () => {
  if (!footerPaymentSupportAddBtn) return;

  footerPaymentSupportAddBtn.addEventListener("click", () => {
    ResetFooterPaymentSupportModal();
    footerPaymentSaveBtn.dataset.action = "create";
    footerPaymentSaveBtn.dataset.index = -1;
  });
};

const AddFooterInfoLinkItemEvent = () => {
  if (!footerInfoAddLinkItemBtn) return;

  footerInfoAddLinkItemBtn.addEventListener("click", () => {
    AddFooterInfoLinkItem();
  });
};

const Init = () => {
  AddFooterInfoEvent();
  AddFooterSupportPaymentEvent();

  AddFooterInfoLinkItemEvent();
  FooterInfosSaveEvent();
  FooterSupportPaymentSaveEvent();

  RefreshFooterInfoTable("#footerInfoTable");
  RefreshFooterSupportPaymentTable("#footerPaymentSupportTable");
};

Init();
