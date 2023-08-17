const storeNameElement = document.querySelector(".js_store-name");

// Helpers
const ImageChangeAction = (images) => {
  for (const image of images) {
    const elements = image.parentElement.children;
    const targetImage = [...elements].filter((el) =>
      el.classList.contains("js_img_changeTarget")
    )[0];

    const targetElement = [...targetImage.children].filter(
      (el) => el.tagName === "IMG"
    )[0];

    if (targetImage && targetElement) {
      image.addEventListener("change", () => {
        const imgFile = image.files[0];
        if (imgFile == null)
          targetElement.src = `/images/default-placeholder.jpg`;
        else targetElement.src = URL.createObjectURL(imgFile);
      });
    }
  }
};

const OpenImageInNewTab = () => {
  const images = document.querySelectorAll(".js_open-image-newTab");
  if (!images) return;

  images.forEach((el) => {
    el.addEventListener("click", () => {
      const url = el.getAttribute("src");
      window.open(url, "Image");
    });
  });
};

// Write your JavaScript code.
let images = document.querySelectorAll(".js_img_changeEvent");
ImageChangeAction(images);

// tiny mce init
tinymce.init({
  selector: "textarea",
});

//
ConvertToLocalDate();
ConvertToLocalDateTime();
