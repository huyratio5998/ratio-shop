const clearCacheForm = document.querySelector("#clearCacheForm");

// events
const SubmitClearCacheEvent = () => {
  if (!clearCacheForm) return;

  clearCacheForm.addEventListener("submit", async (e) => {
    e.preventDefault();
    const formData = new FormData(clearCacheForm);
    let listClearCaches = formData.getAll("cache-setting");

    formData.append("cacheKeys", listClearCaches.join());

    const response = await fetch("/admin/cacheManager/clearCache", {
      method: "POST",
      body: formData,
    });

    const data = response.ok ? await response.json() : null;

    if (!data) {
      swal(`Error`, `Fail to clear cache!`, "error");
    } else {
      swal("Success", `Clear cache successfully!`, "success").then((value) => {
        location.reload();
      });
    }
  });
};

const ChooseItemEvent = () => {
  const cacheItems = document.querySelectorAll(".js_choose-item");
  if (!cacheItems) return;

  cacheItems.forEach((el) => {
    el.addEventListener("click", (e) => {
      const cacheItemInput = el.querySelector(`input[type="checkbox"]`);
      if (!cacheItemInput || e.target.tagName.toLowerCase() == "input") return;

      cacheItemInput.checked = !cacheItemInput.checked;
    });
  });
};

const Init = () => {
  SubmitClearCacheEvent();
  ChooseItemEvent();
};

Init();
