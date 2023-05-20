// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
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

// init tiny mce function
//const tinyInit = (element) => {
//    tinymce.init({
//        selector: `${element}`
//    });
//};

// Write your JavaScript code.
let images = document.querySelectorAll(".js_img_changeEvent");
ImageChangeAction(images);

// tiny mce init
    tinymce.init({
        selector: 'textarea'
    });