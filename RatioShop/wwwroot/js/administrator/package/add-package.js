// events
const AddPackageEvent = () => {
  const btnAdds = document.querySelectorAll(".js_package-item-btn-add");
  if (!btnEdits) return;

  btnAdds.forEach((el) => {
    el.addEventListener("click", () => {
      const itemId = el.dataset.itemId;
      document.querySelector(".js_package-item-id").value = itemId;
    });
  });
};

const Init = () => {
  AddPackageEvent();
};

Init();
