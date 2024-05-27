// wwwroot/js/webrtc.js

$(document).ready(function () {
    let localStream;
    let remoteStream;
    let peerConnection;

    const localVideo = document.getElementById('localVideo');
    const remoteVideo = document.getElementById('remoteVideo');
    const startButton = document.getElementById('startButton');
    const hangupButton = document.getElementById('hangupButton');

    startButton.addEventListener('click', start);
    hangupButton.addEventListener('click', hangup);

    async function start() {
        try {
            localStream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
            localVideo.srcObject = localStream;

        } catch (err) {
            console.error(err);
        }
    }

    async function hangup() {
        try {
            localStream.getTracks().forEach(track => track.stop());
            localVideo.srcObject = null;
            remoteVideo.srcObject = null;

            peerConnection.close();
        } catch (err) {
            console.error(err);
        }
    }
});
