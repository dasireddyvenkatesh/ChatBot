let selectedValue = 0;
var statusChangeBool = true;
function showPopup() {
    document.getElementById("popupContainer").style.display = "flex";
    $('.dropdownSelection').select2();

    $('.dropdownSelection').on('select2:select', function (e) {
        selectedValue = parseInt(e.params.data.id);
    });
}

function closePopup() {
    document.getElementById("popupContainer").style.display = "none";
}

function redirectToPage() {
    window.location.href = '/ChatDetails?fromUserId=' + fromUserId + '&toUserId=' + selectedValue + '&newUser=true';
    closePopup();
}

setInterval(async function () {

    if (statusChangeBool) {

        statusChangeBool = false;

        var listItems = document.querySelectorAll("#chat-History li");

        for (const list of listItems) {

            const lastMessageId = list.querySelector("#lastMessageId").value;
            const idArray = list.id.split(',');
            const apiUrl = `/LastMessageStatus?fromUserId=${idArray[0]}&toUserId=${idArray[1]}&lastMessageId=${lastMessageId}`;
            const response = await fetch(apiUrl);
            const data = await response.json();
            LastSeenUpdate(list.id, data.lastSeenStatus, data.lastSeenStatusColor);
            if (data.statusChange) {
                moveLiToTop(list.id, data.latestMessageId, data.latestMessage);
            }
        }

        statusChangeBool = true;
    }

}, 5000);

function LastSeenUpdate(id, status, statusColor) {
    var ul = document.getElementById("chat-History");
    var liToMove = document.getElementById(id);
    var laststatus = liToMove.querySelector("#LastStatus");
    laststatus.textContent = status;
    laststatus.style.color = statusColor;
}

function moveLiToTop(id, latestMessageId, latestMessage) {
    var ul = document.getElementById("chat-History");
    var liToMove = document.getElementById(id);
    var chatLastMessage = liToMove.querySelector("#chat-LastMessage");
    chatLastMessage.textContent = latestMessage;
    chatLastMessage.style.fontWeight = "bold";
    var messageElement = liToMove.querySelector("#unread-Messages");
    var messageCount = messageElement.textContent;
    if (messageCount > 0) {
        const existingMessageCount = parseInt(messageElement.textContent, 10);
        messageElement.textContent = (existingMessageCount + 1).toString();
    } else {
        messageElement.classList.add("unread-Messages");
        messageElement.textContent = "1";
    }
    var lastMessageId = liToMove.querySelector("#lastMessageId");
    lastMessageId.value = latestMessageId;

    if (liToMove) {
        liToMove.parentNode.removeChild(liToMove);
        ul.insertBefore(liToMove, ul.firstChild);
    }
}