import { initializeApp, getApps } from "https://www.gstatic.com/firebasejs/10.12.0/firebase-app.js";
import { getMessaging, getToken, onMessage } from "https://www.gstatic.com/firebasejs/10.12.0/firebase-messaging.js";

const ON_TOKEN_RECEIVED = 'OnTokenReceived';
const ON_PUSH_RECEIVED = 'OnPushReceived';
const ON_STATUS_CHANGED = 'OnStatusChanged';
const CONFIG_STORAGE_KEY = 'firebase_client_web_config';

class FirebaseClientManager {
    #dotNetRef;
    #messaging = null;
    #unsubscribeMessage = null;

    constructor(dotNetRef) {
        this.#dotNetRef = dotNetRef;
    }

    async initAndRegister(configJson, vapidKey) {
        try {
            if (!configJson) {
                await this.#notifyStatus('Please provide Firebase Web Configuration.', 'warning');
                return;
            }

            const config = typeof configJson === 'string' ? JSON.parse(configJson) : configJson;
            localStorage.setItem(CONFIG_STORAGE_KEY, JSON.stringify({ config, vapidKey }));

            const app = getApps().length === 0 ? initializeApp(config) : getApps()[0];
            this.#messaging = getMessaging(app);

            // Register service worker with config in query param
            const swUrl = `/firebase-messaging-sw.js?config=${encodeURIComponent(JSON.stringify(config))}`;
            const swRegistration = await navigator.serviceWorker.register(swUrl, { scope: '/' });

            // Request permission
            const permission = await Notification.requestPermission();
            if (permission !== 'granted') {
                await this.#notifyStatus('Browser notification permission was denied or dismissed.', 'error');
                return;
            }

            await this.#notifyStatus('Permission granted. Requesting FCM device token...', 'info');

            // Obtain FCM token
            const token = await getToken(this.#messaging, {
                vapidKey: vapidKey || undefined,
                serviceWorkerRegistration: swRegistration
            });

            if (token) {
                await this.#notifyToken(token);
                await this.#notifyStatus('FCM Device Token retrieved successfully!', 'success');
            } else {
                await this.#notifyStatus('No registration token available. Verify your Firebase Web configuration and VAPID key.', 'warning');
            }

            // Listen for foreground push notifications
            if (this.#unsubscribeMessage) {
                this.#unsubscribeMessage();
            }

            this.#unsubscribeMessage = onMessage(this.#messaging, async (payload) => {
                console.log('[Home.razor.js] Foreground push message received:', payload);
                const title = payload.notification?.title || payload.data?.title || 'Push Notification';
                const body = payload.notification?.body || payload.data?.body || '';
                const receivedAt = new Date().toLocaleTimeString();

                try {
                    await this.#dotNetRef.invokeMethodAsync(ON_PUSH_RECEIVED, {
                        title,
                        body,
                        receivedAt,
                        rawJson: JSON.stringify(payload)
                    });
                } catch (err) {
                    console.warn('[Home.razor.js] DotNet invokeMethodAsync error:', err);
                }
            });

        } catch (err) {
            console.error('[Home.razor.js] Error initializing Firebase:', err);
            await this.#notifyStatus(`Initialization error: ${err.message}`, 'error');
        }
    }

    async #notifyToken(token) {
        try {
            await this.#dotNetRef.invokeMethodAsync(ON_TOKEN_RECEIVED, token);
        } catch (err) {
            console.warn('[Home.razor.js] Failed to notify token to .NET:', err);
        }
    }

    async #notifyStatus(message, level) {
        try {
            await this.#dotNetRef.invokeMethodAsync(ON_STATUS_CHANGED, { message, level });
        } catch (err) {
            console.warn('[Home.razor.js] Failed to notify status to .NET:', err);
        }
    }

    dispose() {
        if (this.#unsubscribeMessage) {
            this.#unsubscribeMessage();
            this.#unsubscribeMessage = null;
        }
        this.#dotNetRef = null;
    }
}

let managerInstance = null;

export function initialize(dotNetRef) {
    managerInstance = new FirebaseClientManager(dotNetRef);
    const saved = localStorage.getItem(CONFIG_STORAGE_KEY);
    return saved || '';
}

export async function requestTokenAndListen(configJson, vapidKey) {
    if (managerInstance) {
        await managerInstance.initAndRegister(configJson, vapidKey);
    }
}

export async function copyToClipboard(text) {
    if (navigator.clipboard) {
        await navigator.clipboard.writeText(text);
        return true;
    }
    return false;
}

export function dispose() {
    managerInstance?.dispose();
    managerInstance = null;
}
