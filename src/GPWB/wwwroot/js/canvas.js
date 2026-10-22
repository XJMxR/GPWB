window.repoCanvas = {
    attachPointerCapture(element) {
        element.addEventListener("pointerdown", event => {
            if (event.target.closest("a, button, input, textarea, [contenteditable='true']")) {
                return;
            }

            const card = event.target.closest(".board-card");
            (card || element).setPointerCapture(event.pointerId);
        }, true);
    },
    worldPoint(element, clientX, clientY, panX, panY, zoom) {
        const bounds = element.getBoundingClientRect();
        return {
            x: (clientX - bounds.left - panX) / zoom,
            y: (clientY - bounds.top - panY) / zoom
        };
    },
    focus(elementId) {
        document.getElementById(elementId)?.focus();
    },
    loadAnnotations(key) {
        return localStorage.getItem(key);
    },
    saveAnnotations(key, value) {
        localStorage.setItem(key, value);
    }
};
