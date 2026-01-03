function setTableContainerMaxHeight() {
    const tableContainer = document.querySelector('.mud-table-container');
    if (tableContainer) {
        tableContainer.style.maxHeight = '615px';
    }
}

window.blurActiveElement = () => {
    document.activeElement?.blur();
};
