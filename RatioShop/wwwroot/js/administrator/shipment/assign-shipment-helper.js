//Apis
const AssignShipperToOrder = async (orderId, shipperId) => {
  try {
    const result = await fetch(
      `${baseApiUrl}/shipment/assignShipperToOrder?orderId=${orderId}&shipperId=${shipperId}`
    );
    if (result.ok) {
      return await result.json();
    }
    return null;
  } catch (e) {
    return null;
  }
};

// helper
const ResetAssignShipmentUI = () => {
  const allRatioButtons = document.querySelectorAll(
    'input[name="assignedShipper"]'
  );
  if (!allRatioButtons) return;

  allRatioButtons.forEach((el) => {
    el.checked = false;
  });
  RefreshBackgroundAssignShipmentUI();
};
const RefreshBackgroundAssignShipmentUI = () => {
  const allRatioButtons = document.querySelectorAll(
    'input[name="assignedShipper"]'
  );
  if (allRatioButtons) {
    allRatioButtons.forEach((radio) => {
      if (radio.checked == false) {
        radio.parentElement.classList.remove("bg-light");
      }
    });
  }
};

//Events
const AssignShipmentEvent = () => {
  const btnSave = document.querySelector(".js_save-AssignShipmentToShipper");
  if (!btnSave) return;

  btnSave.addEventListener("click", async () => {
    const orderId = document.querySelector(
      'input[name="currentOrderId"]'
    )?.value;
    const shipperId = document.querySelector(
      'input[name="assignedShipper"]:checked'
    )?.value;

    if (!orderId || !shipperId) {
      swal(`Error`, `Assign Error: Missing information!`, "error");
      ResetAssignShipmentUI();
    } else {
      var data = await AssignShipperToOrder(orderId, shipperId);
      if (!data) {
        swal(`Error`, `Fail to assign shipper to this order!`, "error");
        ResetAssignShipmentUI();
      } else {
        swal("Success", `Shipper assigned successfully!`, "success").then(
          (value) => {
            location.reload();
          }
        );
      }
    }
  });
};

const AssignShipmentUIEvent = () => {
  const buttonAreas = document.querySelectorAll(".js_shipperChooseArea");
  if (!buttonAreas) return;

  buttonAreas.forEach((el) => {
    el.addEventListener("click", (e) => {
      const radioButton = el.querySelector('input[name="assignedShipper"]');
      const firstChild = el.firstElementChild;
      if (!radioButton || !firstChild) return;

      if (e.target != radioButton) radioButton.checked = !radioButton.checked;
      if (radioButton.checked) firstChild.classList.add("bg-light");
      else firstChild.classList.remove("bg-light");

      RefreshBackgroundAssignShipmentUI();
    });
  });
};

export { AssignShipmentEvent, AssignShipmentUIEvent };
