const CACHE_NAME = "learntrack-v1";
const OFFLINE_URL = "/offline.html";

// Install service worker
self.addEventListener("install", event => {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => cache.add(OFFLINE_URL))
            .then(() => self.skipWaiting())
    );
});

// Activate service worker
self.addEventListener("activate", event => {
    event.waitUntil(
        caches.keys().then(keys =>
            Promise.all(
                keys
                    .filter(key => key !== CACHE_NAME)
                    .map(key => caches.delete(key))
            )
        ).then(() => self.clients.claim())
    );
});

// Handle offline navigation
self.addEventListener("fetch", event => {
    const request = event.request;
    const url = new URL(request.url);

    if (request.method !== "GET" ||
        url.origin !== self.location.origin) {
        return;
    }

    // Do not cache private account pages
    if (url.pathname.startsWith("/Account") ||
        url.pathname.startsWith("/Identity")) {
        return;
    }

    if (request.mode === "navigate") {
        event.respondWith(
            fetch(request).catch(async () => {
                return await caches.match(OFFLINE_URL);
            })
        );
    }
});