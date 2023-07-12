const CheckoutApi = async (paymentId) => {
  const data = JSON.stringify({
    PaymentId: paymentId,
  });
  const result = await fetch(`${baseApiUrl}/checkout/cartcheckout`, {
    method: "POST",
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json",
    },
    body: data,
  });
  if (result.ok) {
    return await result.json();
  }
  return null;
};

const ProceedCheckoutEvent = () => {
  const btnProceedCheckout = document.querySelector(".js_proceed-checkout");
  if (!btnProceedCheckout) return;

  btnProceedCheckout.addEventListener("click", async (e) => {
    if (btnProceedCheckout.dataset.shipmentValidation == "false") {
      // show message fill in shipment
      swal(
        `Error`,
        "Please fill up and save all shipping address information!",
        "error"
      );
      return;
    }
    if (btnProceedCheckout.dataset.totalItems == 0) {
      // show message fill in shipment
      swal(`Error`, "Cart empty!", "error");
      return;
    }
    //
    e.preventDefault();
    const paymentId = "19E5D534-34D6-4A20-C47F-08DB7AC11029";
    const data = await CheckoutApi(paymentId);

    if (data == null) {
      swal(`Error`, "Checkout failure!", "error");
      return;
    } else {
      let message = data.message;
      if (data.status == "Success" && data.orderNumber) {
        if (!message) message = "Order created successfully";
        swal("Success", `${message}!`, "success").then((value) => {
          const orderNumber = data.orderNumber;
          window.location.href = `/checkout/checkoutsuccess?ordernumber=${orderNumber}`;
        });
      } else {
        if (!message) message = "Checkout failure";
        swal("Error", `${message}!`, "error");
      }
    }
  });
};

ProceedCheckoutEvent();
