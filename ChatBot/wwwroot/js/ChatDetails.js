var updateStatus = true;
var deliveredStatus = false;
var seenStatus = false;
var sentStatus = false;
const sentBadge = '✓';
const deliveredBadge = '✓✓';
const seenBadge = '🗹';
const sentText = 'Sent';
const deliveredText = 'Delivered';
const seenText = 'Seen';
const statusIndicators = {
    Online: { text: "Online", color: "green" }
};

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub", { transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling })
    .withAutomaticReconnect()
    .build();

connection.serverTimeoutInMilliseconds = 6000000; 
connection.keepAliveIntervalInMilliseconds = 3000000;

connection.start();

setInterval(async function () {
    await connection.invoke('GetUserStatus', toUser).then(function (data) {
        LastSeenStatusUpdate(data, sentStatus);
        deliveredStatus = data;
    });

    await connection.invoke('GetUserActiveStatus', fromUser, toUser).then(function (data) {
        sentStatus = data;
        LastSeenStatusUpdate(deliveredStatus, data);
        if (data && updateStatus) {
            updateMessageStatus(seenText, seenBadge);
        } else if (deliveredStatus && updateStatus) {
            updateMessageStatus(deliveredText, deliveredBadge);
        }       
    });

}, 5000);

setInterval(async () => {
    if (connection.state === signalR.HubConnectionState.Disconnected) {
        await connection.start();
    }
}, 3000);

async function LastSeenStatusUpdate(deliveredStatus, sentStatus) {
    if (!(deliveredStatus && sentStatus)) {
        await connection.invoke('LastSeenStatus', toUser).then(function (data) {
            var usernameStatus = document.getElementById("chatUserName");
            usernameStatus.style.marginTop = "-14Px";
            var usernameStatus = document.getElementById("chatLastSeen");
            usernameStatus.textContent = data;
            usernameStatus.style.fontSize = "11px";
            usernameStatus.style.color = "black";
        });
    }

    if (deliveredStatus || sentStatus) {       
        var usernameStatus = document.getElementById("chatUserName");
        usernameStatus.style.marginTop = "-14Px";
        var usernameStatus = document.getElementById("chatLastSeen");
        usernameStatus.textContent = statusIndicators.Online.text;
        usernameStatus.style.fontSize = "11px";
        usernameStatus.style.color = statusIndicators.Online.color;
        
    }

    await connection.invoke('UpdateSeenStatus', fromUser);
}

function sendMessage() {
    const message = document.getElementById("messageInput").value;
    const formData = new FormData();
    formData.append('fromUserId', fromUser);
    formData.append('toUserId', toUser);
    formData.append('message', message);

    const imageFile = document.getElementById("imageInput").files[0];
    if (imageFile) {
        formData.append('imageFile', imageFile);
    }

    fetch('/SendMessage', {
        method: 'POST',
        body: formData,
    }).then(() => {

        document.getElementById("imageInput").value = "";
        document.getElementById("messageInput").value = "";
        document.getElementById("messageInput").disabled = false;

    });
}
sendButton.addEventListener('click', function (event) {

    const messageInput = document.getElementById('messageInput');
    const sendButton = document.getElementById("sendButton").textContent;
    if (messageInput.value.trim() || sendButton.trim() === "Send Image") {
        sendMessage();
    }
    event.preventDefault();
});

messageInput.addEventListener('keypress', function (event) {
    const messageInput = document.getElementById('messageInput');
    const sendButton = document.getElementById("sendButton").textContent;
    if (event.key === 'Enter' && (messageInput.value.trim() || sendButton.trim() === "Send Image")) {
        sendMessage();
        event.preventDefault();
    }
});

messageInput.addEventListener('input', function () {
    sendButton.disabled = !messageInput.value.trim();
});
//function submitForm() {
//    // Create a form element
//    var form = document.createElement('form');
//    form.method = 'post';
//    form.action = '/UserChatHistory';

//    // Function to create hidden input field
//    function createHiddenInput(name, value) {
//        var input = document.createElement('input');
//        input.type = 'hidden';
//        input.name = name;
//        input.value = value;
//        return input;
//    }

//    // Create hidden input fields for parameters
//    form.appendChild(createHiddenInput('userName', model[0].loginUserName));
//    form.appendChild(createHiddenInput('passWord', 'passWord')); 
//    form.appendChild(createHiddenInput('loggedInUser', 'true'));

//    // Append the form to the document body
//    document.body.appendChild(form);

//    // Submit the form
//    form.submit();
//}
function toggleMessageInput() {
    const imageInput = document.getElementById("imageInput");
    const messageInput = document.getElementById("messageInput");
    const sendButton = document.getElementById("sendButton");

    if (imageInput.files.length > 0) {
        sendButton.textContent = "Send Image";
        document.getElementById("messageInput").value = "";
        sendButton.disabled = false;
        messageInput.disabled = true;
        messageInput.style.cursor = "not-allowed";
    } else {
        messageInput.disabled = false;
        sendButton.textContent = "Send";
        messageInput.style.cursor = "default";
    }
}

document.getElementById("imageInput").addEventListener("change", toggleMessageInput);


connection.on("Receive", function (fromUserId, toUserId, message, imageBase64, messageId) {
    const sendButton = document.getElementById("sendButton");
    sendButton.textContent = "Send";
    sendButton.disabled = true;

    const isMessageFromSender = fromUserId === fromUser && toUserId === toUser;
    const isMessageToSender = fromUserId === toUser && toUserId === fromUser;
    const isMessageFromCurrentUser = fromUserId === fromUser;

    if ((isMessageFromSender) || (isMessageToSender)) {

        const todayDate = new Date().toLocaleString("en-Uk", { weekday: 'short', day: '2-digit', month: 'short' });
        const messageDates = document.querySelectorAll("#messagesList .messageDate");
        const lastChatDate = messageDates[messageDates.length - 1].innerHTML;

        if (lastChatDate !== todayDate) {
            const dateLi = document.createElement("li");
            dateLi.textContent = todayDate;
            dateLi.dataset.messageIds = messageId;
            dateLi.classList.add("messageDate");
            messagesList.appendChild(dateLi);
        } else {

            const lastMessageDate = messageDates[messageDates.length - 1];
            const existingMessageIds = lastMessageDate.getAttribute('data-message-ids');
            const newMessageIds = existingMessageIds + ',' + messageId;
            lastMessageDate.dataset.messageIds = newMessageIds;
        }

        if (message !== null && message !== undefined) {

            appendChatBubble(message, new Date().toLocaleTimeString('en-US', { timeStyle: 'short' }),
                isMessageFromCurrentUser, sentText, messageId, new Date().toDateString());

            messagesList.scrollTop = messagesList.scrollHeight;
        }
        else {
            appendImageBubble(isMessageFromCurrentUser, imageBase64, messageId, sentText,
                new Date().toLocaleTimeString('en-US', { timeStyle: 'short' }));

            setTimeout(function () {
                messagesList.scrollTop = messagesList.scrollHeight;
            }, 0);
        }

        connection.invoke('GetUserActiveStatus', fromUser, toUser).then(function (data) {
            if (data && isScrollAtBottom()) {
                updateLastMessageStatus(seenBadge, seenText);
                connection.invoke('UpdateMessageStatus', messageId, seenText);
            } else if (deliveredStatus) {
                updateLastMessageStatus(deliveredBadge, deliveredText);
                connection.invoke('UpdateMessageStatus', messageId, deliveredText);

            }
        });
    }

});

function updateLastMessageStatus(badge, status) {
    const lastmessageList = document.getElementById("messagesList").lastElementChild;
    const contentContainer = lastmessageList.querySelector('.contentContainer');
    if (contentContainer) {
        const statusBadge = contentContainer.querySelector('.statusBadge');
        if (statusBadge) {

            statusBadge.textContent = badge;
            statusBadge.title = status;
        }
    }
}

async function updateMessageStatus(status, badge) {
    updateStatus = false;
    const messageList = document.getElementById("messagesList");
    const messageItems = messageList.querySelectorAll('.message');

    for (const messageItem of messageItems) {
        const contentContainer = messageItem.querySelector('.contentContainer');
        if (contentContainer) {
            const statusBadge = contentContainer.querySelector('.statusBadge');
            if (statusBadge && statusBadge.textContent !== badge
                && statusBadge.textContent !== seenBadge) {
                statusBadge.textContent = badge;
                statusBadge.title = status;
                var messageId = parseInt(messageItem.dataset.messageId, 10);
                await connection.invoke('UpdateMessageStatus', messageId, status);
            }
        }
    }

    updateStatus = true;
}

async function openImagePopup(messageId) {
    const popup = document.getElementById("imagePopup");
    const popupImage = document.getElementById("popupImage");

    const apiUrl = `/ImageBase?messageId=${messageId}`;
    const response = await fetch(apiUrl);
    const base64 = await response.text();
    popupImage.src = `data:image/png;base64,${base64}`;
    popup.style.display = "block";
    document.getElementById("base64ImageSource").value = base64;
    document.getElementById("popupDeleteMessageId").value = messageId;
}

function closeImagePopup() {
    const popup = document.getElementById("imagePopup");
    popup.style.display = "none";
}

function downloadImage() {
    const imageBase64 = document.getElementById("base64ImageSource").value;
    const link = document.createElement('a');
    link.href = `data:image/png;base64,${imageBase64}`;
    link.download = generateFilename();
    link.click();
}

async function deleteImage() {
    const messageId = parseInt(document.getElementById("popupDeleteMessageId").value, 10);
    await connection.invoke('DeleteMessage', messageId);
    deleteMessageLiById(messageId.toString());
    closeImagePopup();
}

function appendImageBubble(isCurrentUser, imageBase64, messageId, messageStatus, time) {
    const messagesList = document.getElementById("messagesList");
    const li = document.createElement("li");
    li.dataset.messageId = messageId;
    li.classList.add("message");

    const messageDiv = document.createElement("div");
    messageDiv.classList.add(isCurrentUser ? "imageFrom" : "imageTo");

    const img = document.createElement('img');
    img.src = `data:image/png;base64,${imageBase64}`;
    img.height = 100;
    img.style.borderRadius = "50px";
    img.addEventListener("click", () => openImagePopup(messageId));

    const container = document.createElement("div");
    container.classList.add("contentContainer");
    container.style.display = 'flex';
    container.style.float = isCurrentUser ? "right" : "left";

    const timeSpan = document.createElement("span");
    timeSpan.textContent = `${time}`;
    timeSpan.classList.add("messageTime");
    timeSpan.style.cursor = "default";
    container.appendChild(timeSpan);

    if (isCurrentUser) {
        const statusBadge = appendStatusMessage(messageStatus);
        const statusSpan = document.createElement("span");
        statusSpan.textContent = statusBadge;
        statusSpan.title = messageStatus;
        statusSpan.classList.add("statusBadge");
        statusSpan.style.cursor = "default";
        container.appendChild(statusSpan);
    }

    messageDiv.appendChild(img);
    li.appendChild(messageDiv);
    li.appendChild(container);

    messagesList.appendChild(li);

}

function generateFilename() {
    const currentDate = new Date();
    const formattedDate = currentDate.toISOString().split('T')[0];
    const formattedTime = currentDate.toTimeString().split(' ')[0];
    return `Image_${formattedDate}_${formattedTime}.png`;
}

function appendStatusMessage(status) {
    let badge = '';

    if (status === sentText) {
        badge = sentBadge;
    } else if (status === deliveredText) {
        badge = deliveredBadge;
    } else if (status === seenText) {
        badge = seenBadge;
    }

    return badge;
}

function openPopup(messageId) {
    document.getElementById("popupContainer").style.display = "flex";
    const apiUrl = `/MessageDetails?messageId=${messageId}`;
    fetch(apiUrl)
        .then(response => response.json())
        .then(data => {
            document.getElementById("deliveryStatus").textContent = `Delivered: ${data.deliveryDate}`;
            document.getElementById("seenStatus").textContent = `Seen: ${data.seenDate}`;
            document.getElementById("popupMessageId").value = messageId;
        })
}

function closePopup() {
    document.getElementById("popupContainer").style.display = "none";
}

async function deleteMessage() {
    const messageId = parseInt(document.getElementById("popupMessageId").value, 10);
    await connection.invoke('DeleteMessage', messageId);
    deleteMessageLiById(messageId.toString());
    closePopup();
}

function deleteMessageLiById(messageId) {

    const messageElement = document.querySelector(`[data-message-id="${messageId}"]`);

    if (messageElement && messageElement.parentNode) {

        messageElement.parentNode.removeChild(messageElement);

        const messageDateElements = document.querySelectorAll('.messageDate');

        for (const messageDateElement of messageDateElements) {

            const messageIdsAttribute = messageDateElement.getAttribute('data-message-ids');

            const messageIdsArray = messageIdsAttribute.split(',');

            const indexToRemove = messageIdsArray.indexOf(messageId);

            if (indexToRemove !== -1) {
                messageIdsArray.splice(indexToRemove, 1);
                messageDateElement.dataset.messageIds = messageIdsArray;
            }

            if (messageIdsArray.length === 0) {
                messageDateElement.parentNode.removeChild(messageDateElement);
            }
        }
    }
}

function appendChatBubble(message, time, isCurrentUser, messageStatus, messageId) {
    const messagesList = document.getElementById("messagesList");
    const li = document.createElement("li");

    const messageDiv = document.createElement("div");
    messageDiv.classList.add(isCurrentUser ? "userMessage" : "chatbotMessage");
    const clickableButton = document.createElement("button");
    clickableButton.textContent = message;
    clickableButton.classList.add(isCurrentUser ? "rightClickableButton" : "leftClickableButton");
    clickableButton.addEventListener("click", () => openPopup(messageId));
    messageDiv.appendChild(clickableButton);

    li.appendChild(messageDiv);

    const contentContainer = document.createElement("div");
    contentContainer.classList.add("contentContainer");
    contentContainer.style.float = isCurrentUser ? "right" : "left";

    const timeSpan = document.createElement("span");
    timeSpan.textContent = `${time}`;
    timeSpan.classList.add("messageTime");
    timeSpan.style.cursor = "default";
    contentContainer.appendChild(timeSpan);

    if (isCurrentUser) {
        const statusBadge = appendStatusMessage(messageStatus);
        const statusSpan = document.createElement("span");
        statusSpan.textContent = statusBadge;
        statusSpan.title = messageStatus;
        statusSpan.classList.add("statusBadge");
        statusSpan.style.cursor = "default";
        statusSpan.dataset.messageId = messageId;
        contentContainer.appendChild(statusSpan);
    }

    li.appendChild(contentContainer);
    li.dataset.messageId = messageId;
    li.classList.add("message");
    messagesList.appendChild(li);
}

function loadChatHistory(chatHistoryList) {
    let lastDate = null;

    chatHistoryList.forEach(chat => {
        if (chat.messageDate !== lastDate) {
            const messagesList = document.getElementById("messagesList");
            const dateLi = document.createElement("li");
            dateLi.textContent = chat.messageDate;
            dateLi.dataset.messageIds = chat.messageIds;
            dateLi.classList.add("messageDate");
            messagesList.appendChild(dateLi);
            lastDate = chat.messageDate;
        }

        for (let i = 0; i < chat.messages.length; i++) {

            if (chat.messages[i] === '') {
                appendImageBubble(chat.isCurrentUser[i],
                    chat.imageBase64[i],
                    chat.messageIds[i],
                    chat.messageStatus[i],
                    chat.messageTime[i]
                );

            } else {
                appendChatBubble(
                    chat.messages[i],
                    chat.messageTime[i],
                    chat.isCurrentUser[i],
                    chat.messageStatus[i],
                    chat.messageIds[i],
                    chat.messageDate
                );
            }

        }
    });

    messagesList.scrollTop = messagesList.scrollHeight;

    const loader = document.getElementById('loader');
    loader.style.display = 'none';
}

window.onload = function () {
    loadChatHistory(model);

};

function isScrollAtBottom() {

    const messagesList = document.getElementById("messagesList");
    return messagesList.scrollHeight - messagesList.clientHeight <= messagesList.scrollTop + 1;
}
