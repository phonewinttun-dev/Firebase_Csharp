importScripts('https://www.gstatic.com/firebasejs/10.12.0/firebase-app-compat.js');
importScripts('https://www.gstatic.com/firebasejs/10.12.0/firebase-messaging-compat.js');

const params = new URLSearchParams(location.search);
const configParam = params.get('config');

if (configParam) {
    try {
        const config = JSON.parse(configParam);
        if (!firebase.apps.length) {
            firebase.initializeApp(config);
        }
    } catch (err) {
        console.error('[firebase-messaging-sw] Failed to parse config from query param:', err);
    }
}

self.addEventListener('message', (event) => {
    if (event.data && event.data.type === 'INIT_FIREBASE_SW' && event.data.config) {
        try {
            if (!firebase.apps.length) {
                firebase.initializeApp(event.data.config);
            }
        } catch (err) {
            console.error('[firebase-messaging-sw] Failed to initialize via message:', err);
        }
    }
});

try {
    if (firebase.apps.length) {
        const messaging = firebase.messaging();
        messaging.onBackgroundMessage((payload) => {
            console.log('[firebase-messaging-sw] Background message:', payload);
            const title = payload.notification?.title || payload.data?.title || 'Push Notification';
            const options = {
                body: payload.notification?.body || payload.data?.body || 'You have a new message.',
                icon: '/icon-192.png'
            };
            self.registration.showNotification(title, options);
        });
    }
} catch (err) {
    console.error('[firebase-messaging-sw] Error setting onBackgroundMessage:', err);
}
