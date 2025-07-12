document.addEventListener("DOMContentLoaded", () => {
    const cards = document.querySelectorAll(".video-card");

    cards.forEach(card => {
        const video = card.querySelector(".preview-video");

        if (!video) return;

        card.addEventListener("mouseenter", () => {
            video.play().catch(() => { }); // يشغّل الفيديو عند المرور
        });

        card.addEventListener("mouseleave", () => {
            video.pause();
            video.currentTime = 0; // يرجعه للبداية
        });
    });
});
