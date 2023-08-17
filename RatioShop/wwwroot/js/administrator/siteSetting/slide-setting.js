const btnCreateBannerItem = document.querySelector("#addBannerItem");
const bannerTable = document.querySelector("#bannerTable");
const listBannerInput = document.querySelector("#list-banner-input");

const bannerImageDisplay = document.querySelector("#banner-image");
const bannerImageValue = document.querySelector("#banner-image-value");
const bannerImageInput = document.querySelector("#banner-image-input");
const bannerImageAltInput = document.querySelector("#banner-image-alt");
const bannerTitleInput = document.querySelector("#banner-title");
const bannerContentInput = document.querySelector("#banner-content");
const bannerButtonTextInput = document.querySelector("#banner-button-text");
const bannerButtonLinkInput = document.querySelector("#banner-button-link");

const btnSaveBannerItem = document.querySelector("#banner-save");

// Helpers
const ResetBannerFormValue = () => {
  if (
    !bannerImageValue ||
    !bannerImageAltInput ||
    !bannerTitleInput ||
    !bannerContentInput ||
    !bannerButtonTextInput ||
    !bannerButtonLinkInput
  )
    return;

  bannerImageDisplay.setAttribute("src", "/images/default-placeholder.jpg");
  bannerImageValue.value = "";
  bannerImageAltInput.value = "";

  bannerTitleInput.value = "";
  bannerContentInput.value = "";
  bannerButtonTextInput.value = "";
  bannerButtonLinkInput.value = "";

  btnSaveBannerItem.dataset.action = "";
  btnSaveBannerItem.dataset.index = "";
};

const HidePopup = () => {
  document.querySelector("#btn-modalBannerClose")?.click();
};

const GetBanners = () => {
  const banners = listBannerInput.value
    ? JSON.parse(listBannerInput.value)
    : [];
  return banners;
};

const RefreshBannerTable = (tableId) => {
  const banners = GetBanners();
  const tbodyRef = document
    .querySelector(tableId)
    .getElementsByTagName("tbody")[0];
  if (tbodyRef) {
    tbodyRef.innerHTML = "";
    banners.forEach((x, index) => {
      const newRow = tbodyRef.insertRow(-1);
      const imageElement = document.createElement("img");
      imageElement.style.height = "100px";
      imageElement.src = x.Image.ImageSrc;
      imageElement.alt = x.Image.ImageAlt;

      newRow.insertCell(0).appendChild(imageElement);
      newRow.insertCell(1).appendChild(document.createTextNode(x.Title));
      newRow.insertCell(2).appendChild(document.createTextNode(x.Content));
      newRow
        .insertCell(3)
        .appendChild(document.createTextNode(x.ShopNow?.Text));
      newRow.insertCell(4).appendChild(document.createTextNode(x.ShopNow?.Url));

      const editNode = document.createElement("a");
      editNode.classList.add("banner-edit");
      editNode.classList.add("mouse-hover");
      editNode.setAttribute("data-bs-toggle", "modal");
      editNode.setAttribute("data-bs-target", "#bannerModal");
      editNode.appendChild(document.createTextNode("Edit"));

      const deleteNode = document.createElement("a");
      deleteNode.classList.add("banner-delete");
      deleteNode.classList.add("mouse-hover");
      deleteNode.appendChild(document.createTextNode("Delete"));

      let actionCell = newRow.insertCell(5);
      actionCell.setAttribute("data-index", index);
      actionCell.appendChild(editNode);
      actionCell.appendChild(document.createTextNode(" | "));
      actionCell.appendChild(deleteNode);
    });

    // add event after refresh table
    TableBannerEvent();
  }
};

// Events
const BannerSaveEvent = () => {
  if (
    !bannerTable ||
    !bannerImageValue ||
    !bannerImageAltInput ||
    !bannerTitleInput ||
    !bannerContentInput ||
    !bannerButtonTextInput ||
    !bannerButtonLinkInput
  )
    return;
  if (!btnSaveBannerItem) return;

  btnSaveBannerItem.addEventListener("click", async () => {
    const action = btnSaveBannerItem.dataset.action;
    if (!action) return;

    // save image
    bannerImageValue.value = !bannerImageValue.value
      ? `/images/default-placeholder.jpg`
      : bannerImageValue.value;
    const imageFile = bannerImageInput?.files[0];
    if (imageFile) {
      let data = new FormData();
      let fileName = imageFile.name;
      data.append("file", imageFile);

      const response = await fetch(
        `${baseApiUrl}/file/saveFile?folderName1=images`,
        {
          method: "POST",
          body: data,
        }
      );
      if (response.ok) {
        const result = await response.json();
        if (result) bannerImageValue.value = `/images/${fileName}`;
      }
    }

    const index = btnSaveBannerItem.dataset.index;
    const banners = GetBanners();

    if (action == "create") {
      const bannerItem = {
        Image: {
          ImageSrc: bannerImageValue.value,
          ImageAlt: bannerImageAltInput.value,
        },
        Title: bannerTitleInput.value,
        Content: bannerContentInput.value,
        ShopNow: {
          Text: bannerButtonTextInput.value,
          Url: bannerButtonLinkInput.value,
        },
      };
      banners.push(bannerItem);
    } else if (action == "edit" && index >= 0) {
      banners[index].Image.ImageSrc = bannerImageValue.value;
      banners[index].Image.ImageAlt = bannerImageAltInput.value;
      banners[index].Title = bannerTitleInput.value;
      banners[index].Content = bannerContentInput.value;
      banners[index].ShopNow.Text = bannerButtonTextInput.value;
      banners[index].ShopNow.Url = bannerButtonLinkInput.value;
    }

    listBannerInput.value = JSON.stringify(banners);
    ResetBannerFormValue();
    RefreshBannerTable("#bannerTable");
    HidePopup();
  });
};

const TableBannerEvent = () => {
  const btnEditBannerItem = document.querySelectorAll(".banner-edit");
  const btnDeleteBannerItem = document.querySelectorAll(".banner-delete");
  let banners = GetBanners();

  if (btnEditBannerItem) {
    btnEditBannerItem.forEach((element) => {
      element.addEventListener("click", (e) => {
        ResetBannerFormValue();
        const index = e.currentTarget.parentNode.dataset.index;
        bannerImageDisplay.src = banners[index].Image.ImageSrc;
        bannerImageAltInput.alt = banners[index].Image.ImageAlt;

        bannerImageValue.value = banners[index].Image.ImageSrc;
        bannerImageAltInput.value = banners[index].Image.ImageAlt;
        bannerTitleInput.value = banners[index].Title;
        bannerContentInput.value = banners[index].Content;
        bannerButtonTextInput.value = banners[index].ShopNow.Text;
        bannerButtonLinkInput.value = banners[index].ShopNow.Url;

        btnSaveBannerItem.dataset.action = "edit";
        btnSaveBannerItem.dataset.index = index;
      });
    });
  }

  if (btnDeleteBannerItem) {
    btnDeleteBannerItem.forEach((element) => {
      element.addEventListener("click", (e) => {
        const index = e.currentTarget.parentNode.dataset.index;
        if (index > -1) {
          banners.splice(index, 1);
          listBannerInput.value = JSON.stringify(banners);
          RefreshBannerTable("#bannerTable");
        }
      });
    });
  }
};

const AddBannerItemEvent = () => {
  if (!btnCreateBannerItem) return;

  btnCreateBannerItem.addEventListener("click", () => {
    ResetBannerFormValue();
    btnSaveBannerItem.dataset.action = "create";
    btnSaveBannerItem.dataset.index = -1;
  });
};

const Init = () => {
  AddBannerItemEvent();
  BannerSaveEvent();
  RefreshBannerTable("#bannerTable");
};

Init();
