const ShowToastNotification = () => {
  const toastDemo = document.querySelector(".js_ratio-toast");
  if (!toastDemo) return;
  const toast = new bootstrap.Toast(toastDemo);
  if (toast) {
    toast.show();
  }
};

ShowToastNotification();
