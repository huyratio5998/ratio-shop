import * as AssignShipmentHelper from "../shipment/assign-shipment-helper.js";

const cancelOrderForm = document.querySelector(".js_cancelOrderForm");
const completeOrderForm = document.querySelector(".js_completeOrderForm");

// api
const UpdateOrderStatus = async (elementForm, statusUpdate) => {
  const formData = new FormData(elementForm);
  const orderId = formData.get("orderId");
  const status = formData.get("status");

  const response = await fetch(
    `${baseApiUrl}/order/updateOrderStatus?orderId=${orderId}&status=${status}`,
    {
      method: "POST",
      body: formData,
    }
  );

  const data = response.ok ? await response.json() : null;

  if (!data || !data.status) {
    swal(`Error`, `${data?.message ?? `Can't update status`}!`, "error");
  } else {
    swal("Success", `${statusUpdate} successfully!`, "success").then(
      (value) => {
        location.reload();
      }
    );
  }
};

// events
const CancelOrderFormEvent = () => {
  if (!cancelOrderForm) return;

  cancelOrderForm.addEventListener("submit", async (e) => {
    e.preventDefault();
    UpdateOrderStatus(cancelOrderForm, "Cancel");
  });
};

const CompleteOrderFormEvent = () => {
  if (!completeOrderForm) return;

  completeOrderForm.addEventListener("submit", async (e) => {
    e.preventDefault();
    UpdateOrderStatus(completeOrderForm, "Complete");
  });
};

const Init = () => {
  CancelOrderFormEvent();
  CompleteOrderFormEvent();
  AssignShipmentHelper.AssignShipmentEvent();
  AssignShipmentHelper.AssignShipmentUIEvent();
};

Init();
