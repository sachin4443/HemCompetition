const CHANNEL_NAME = "HEM_EXAM_SECURITY_CHANNEL";

let channel = null;

const listeners = new Set();

export function getChannel() {

    if (!channel) {

        channel = new BroadcastChannel(CHANNEL_NAME);

        channel.addEventListener("message", (event) => {

            listeners.forEach(listener => {

                listener(event.data);

            });

        });

    }

    return channel;

}

export function sendMessage(type, payload = {}) {

    getChannel().postMessage({

        type,

        payload,

        timestamp: Date.now()

    });

}

export function subscribe(callback) {

    getChannel();

    listeners.add(callback);

    return () => {

        listeners.delete(callback);

    };

}

export function closeChannel() {

    if (!channel)
        return;

    channel.close();

    channel = null;

    listeners.clear();

}