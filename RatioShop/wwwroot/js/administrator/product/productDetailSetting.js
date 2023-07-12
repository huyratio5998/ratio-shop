const btnShowMoreVariants = document.querySelector("#variants-detail");
const variantsTable = document.querySelector(".table");
// add event
btnShowMoreVariants.addEventListener("click", () => {
    variantsTable.classList.toggle("hidden");
})