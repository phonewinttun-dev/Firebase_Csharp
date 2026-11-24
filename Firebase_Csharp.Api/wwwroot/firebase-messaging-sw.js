// Service Worker: handles push notifications when the browser tab is in the background or closed.
importScripts('https://www.gstatic.com/firebasejs/10.12.0/firebase-app-compat.js');
importScripts('https://www.gstatic.com/firebasejs/10.12.0/firebase-messaging-compat.js');

// Read Firebase config passed from index.html via the URL query string
const params = new URLSearchParams(location.search);
const configParam = params.get('config');

if (configParam) {
    const config = JSON.parse(configParam);
    if (!firebase.apps.length) {
        firebase.initializeApp(config);
    }

    const messaging = firebase.messaging();
    messaging.onBackgroundMessage((payload) => {
        const title = payload.notification?.title || 'New Notification';
        const body = payload.notification?.body || '';
        self.registration.showNotification(title, { body });
    });
}
