importScripts("https://storage.googleapis.com/workbox-cdn/releases/4.3.1/workbox-sw.js");

if (workbox) {

    workbox.core.setCacheNameDetails({ prefix: "idops-ui" });

    self.__precacheManifest = [].concat(self.__precacheManifest || []);
    workbox.precaching.precacheAndRoute(self.__precacheManifest, {});

    workbox.routing.registerRoute(
        /^https:\/\/fonts\.gstatic\.com/,
        new workbox.strategies.CacheFirst({
            cacheName: 'google-fonts-webfonts',
            plugins: [
                new workbox.cacheableResponse.Plugin({
                    statuses: [0, 200],
                }),
                new workbox.expiration.Plugin({
                    maxAgeSeconds: 60 * 60 * 24 * 365,
                    maxEntries: 30,
                }),
            ],
        })
    )
    workbox.routing.registerRoute(
        ({ url }) => url.pathname.startsWith('/session'),
        new workbox.strategies.NetworkOnly()
    );

    workbox.routing.registerRoute(
        ({ url }) => url.pathname === '/',
        new workbox.strategies.NetworkFirst()
    );
}

self.addEventListener('message', (event) => {
    if (event.data && event.data.type === 'SKIP_WAITING') {
        self.skipWaiting()
    }
});

setInterval(() => {

    fetch('api/session').then(response => {
        response.json().then(data => {

            if (!data.isAuthenticated) {
                self.clients.matchAll().then(clients => {
                    clients.forEach(client => {
                        sendClientAction(client, { action: 'ROUTE', value: 'SessionExpired' })
                    });
                })
            }
        })
    })

}, 1000 * 60 * 5)

addEventListener('fetch', event => {

    const requestUrl = new URL(event.request.url);

    if (requestUrl.pathname.startsWith("/graphql")) {

        event.respondWith(
            fetch(event.request).then(function (response) {
                if (response.status === 403) {

                    response.text().then(x => {
                        if (x === "Access denied!") {
                            self.clients.get(event.clientId).then(client => {
                                sendClientAction(client, { action: 'ROUTE', value: 'AccessDenied' })
                            });
                        }
                        return;
                    });
                    self.clients.get(event.clientId).then(client => {
                        sendClientAction(client, { action: 'ROUTE', value: 'SessionExpired' })
                    });
                }

                return response;
            }));
    }
});

const sendClientAction = (client, message) => {
    client.postMessage(message);
}

const isGraphQLNotAuth = (data) => {

    if (data.errors && data.errors.length > 0) {

        if (data.errors[0].extensions.code === "AUTH_NOT_AUTHENTICATED") {
            return true;
        }
    }

    return false;
}