let player = null;
let playerName = "Windows UWP";
let accessToken = null;
let isReady = false;

window.onSpotifyWebPlaybackSDKReady = function() {
    isReady = true;
    isWindowReady();
};

function isWindowReady() {
    let readyStatus = {
        windowReady: isReady
    };
    notifyJson(readyStatus);
}

function setPlayerName(name) {
    playerName = name;
}

function setAccessToken(token) {
    accessToken = token;
}

function getAccessToken() {
    return accessToken;
}

function createPlayer() {
    player = new Spotify.Player({
        name: playerName,
        getOAuthToken: callback => {
            requestAccessToken().then(function(token) {
                callback(token);
            });
        }
    });

    player.addListener('initialization_error', function(webPlaybackError) {
        notifyJson(createError('initialization_error', webPlaybackError['message']));
    });
    player.addListener('authentication_error', function(webPlaybackError) {
        notifyJson(createError('authentication_error', webPlaybackError['message']));
    });
    player.addListener('account_error', function(webPlaybackError) {
        notifyJson(createError('account_error', webPlaybackError['message']));
    });
    player.addListener('playback_error', function(webPlaybackError) {
        notifyJson(createError('playback_error', webPlaybackError['message']));
    });
    player.addListener('player_state_changed', function(webPlaybackState) {
        let playerStatus = {
            player: {
                state: webPlaybackState
            }
        };
        notifyJson(playerStatus);
    });
    player.addListener('ready', function(webPlaybackPlayer) {
        let playerStatus = {
            player: {
                ready: true,
                deviceId: webPlaybackPlayer['device_id']
            }
        };
        notifyJson(playerStatus);
    });
    player.addListener('not_ready', function(webPlaybackPlayer) {
        let playerStatus = {
            player: {
                ready: false,
                deviceId: webPlaybackPlayer['device_id']
            }
        };
        notifyJson(playerStatus);
    });
}

function connectPlayer() {
    player.connect().then(success => {
        let playerStatus = {
            player: {
                connected: success
            }
        };
        notifyJson(playerStatus);
    });
}

function disconnectPlayer() {
    player.disconnect();
}

function createError(errorType, errorMessage) {
    return {
        errorType: errorType,
        errorMessage: errorMessage
    };
}

function testRequestAccessToken() {
    requestAccessToken();
}

function requestAccessToken() {
    accessToken = null;
    let request = {
        request: 'accessToken'
    };
    notifyJson(request);
    return new Promise((resolve, reject) => {
        let intervalId = window.setInterval(function() {
            if (accessToken != null) {
                window.clearInterval(intervalId);
                resolve(accessToken);
            }
        }, 10);
    });
}

function notify(str) {
    window.external.notify(str);
}

function notifyJson(obj) {
    notify(JSON.stringify(obj));
}
