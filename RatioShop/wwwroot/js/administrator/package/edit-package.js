// events
const EditPackageEvent = () => {
  const btnEdits = document.querySelectorAll(".js_package-item-btn-edit");
  if (!btnEdits) return;

  btnEdits.forEach((el) => {
    el.addEventListener("click", () => {
      const itemId = el.dataset.itemId;
      const number = el.dataset.number;
      document.querySelector(".js_package-item-id").value = itemId;
      document.querySelector(".js_package-item-number").value = number;
    });
  });
};

const Init = () => {
  EditPackageEvent();
};

Init();
