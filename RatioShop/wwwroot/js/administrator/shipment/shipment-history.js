import * as AssignShipmentHelper from "./assign-shipment-helper.js";

//Apis
const GetShipmentHistoryDetail = async (id) => {
  try {
    const result = await fetch(
      `${baseApiUrl}/shipment/shipmentHistoryDetail?id=${id}`
    );
    if (result.ok) {
      return await result.json();
    }
    return null;
  } catch (e) {
    return null;
  }
};

// Helpers
const BuildHistoryDetailPopupData = (data, parentElement = document) => {
  const detailPopupDataArea = parentElement.querySelector(
    ".js_shipment-history-detail-popup-data"
  );

  if (!detailPopupDataArea) return;

  if (!data) return;

  let modalContent = ``;
  // handle logic
  const baseImgUrl = detailPopupDataArea.dataset.baseImgUrl;
  const imgUrl = `${baseImgUrl}/${data.images}`;
  const finalImg = `<img src="${imgUrl}" alt="${data.images}" style="max-height:200px" class="js_open-image-newTab" />`;
  modalContent += `<dt class="col-sm-3">
                            Images
                        </dt>
                        <dd class="col-sm-9" > 
                        <a href="#">
                        ${finalImg}                                                       
                        </a>
                        </dd>`;

  modalContent += `<dt class="col-sm-3">
                            Reasons
                        </dt>
                        <dd class="col-sm-9">                                                        
                        ${data.reasons ? data.reasons : ""}
                        </dd>`;

  modalContent += ` <dt class="col-sm-3">
                            System Message
                        </dt>
                        <dd class="col-sm-9">                                                        
                        ${data.systemMessage ? data.systemMessage : ""}
                        </dd>`;

  detailPopupDataArea.innerHTML = modalContent;
  OpenImageInNewTab();
};

//Events
const HistoryDetailRecordEvent = () => {
  const btnDetails = document.querySelectorAll(".js_shipment-history-detail");
  if (!btnDetails) return;

  btnDetails.forEach((el) => {
    el.addEventListener("click", async () => {
      const detailId = el.dataset.id;
      if (!detailId) return;
      const data = await GetShipmentHistoryDetail(detailId);

      if (!data) {
        swal(`Error`, `Can't get detail information!`, "error").then(
          (value) => {
            location.reload();
          }
        );
      } else {
        // validation logic
        const detailModal = document.querySelector("#shipmentHistoryDetail");
        if (!detailModal) {
          console.log("Modal empty!");
          return;
        }

        BuildHistoryDetailPopupData(data, detailModal);
      }
    });
  });
};

const Init = () => {
  AssignShipmentHelper.AssignShipmentEvent();
  AssignShipmentHelper.AssignShipmentUIEvent();
  HistoryDetailRecordEvent();
};

Init();
