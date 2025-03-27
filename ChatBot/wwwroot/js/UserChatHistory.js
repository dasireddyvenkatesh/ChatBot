let selectedValue = "";
var statusChangeBool = true;
function showPopup() {
    document.getElementById("popupContainer").style.display = "flex";
    $('.dropdownSelection').select2();

    $('.dropdownSelection').on('select2:select', function (e) {
        selectedValue = e.params.data.id;
    });
}

function closePopup() {
    document.getElementById("popupContainer").style.display = "none";
}

function redirectToPage(event) {
    if (selectedValue == 0) {
        event.preventDefault();
        return;
    }

    // Create a hidden form
    const form = document.createElement('form');
    form.method = 'POST';
    form.action = '/ChatDetails';  // The action URL for the POST request

    // Create hidden input fields for each of the data items
    const fromUserIdInput = document.createElement('input');
    fromUserIdInput.type = 'hidden';
    fromUserIdInput.name = 'fromUserId';
    fromUserIdInput.value = fromUserId;

    const toUserIdInput = document.createElement('input');
    toUserIdInput.type = 'hidden';
    toUserIdInput.name = 'toUserId';
    toUserIdInput.value = selectedValue;

    const newUserInput = document.createElement('input');
    newUserInput.type = 'hidden';
    newUserInput.name = 'newUser';
    newUserInput.value = 'true';

    // Append the hidden input fields to the form
    form.appendChild(fromUserIdInput);
    form.appendChild(toUserIdInput);
    form.appendChild(newUserInput);

    // Append the form to the body
    document.body.appendChild(form);

    // Submit the form
    form.submit();

    closePopup();
}

function submitChatForm(fromUser, toUser) {
    // Create a form element
    var form = document.createElement('form');
    form.method = 'post';
    form.action = '/ChatDetails';

    // Function to create hidden input field
    function createHiddenInput(name, value) {
        var input = document.createElement('input');
        input.type = 'hidden';
        input.name = name;
        input.value = value;
        return input;
    }
    
    // Create hidden input fields for parameters
    form.appendChild(createHiddenInput('fromUserId', fromUser));
    form.appendChild(createHiddenInput('toUserId', toUser));

    // Append the form to the document body
    document.body.appendChild(form);

    // Submit the form
    form.submit();
}

setInterval(async function () {

    if (statusChangeBool) {

        statusChangeBool = false;

        var listItems = document.querySelectorAll("#chat-History li");

        for (const list of listItems) {

            const lastMessageId = list.querySelector("#lastMessageId").value;
            const idArray = list.id.split(',');

            if (idArray != undefined && idArray[0] != undefined && idArray[1] != undefined && lastMessageId != undefined) {

                const apiUrl = `/LastMessageStatus?fromUserId=${idArray[0]}&toUserId=${idArray[1]}&lastMessageId=${lastMessageId}`;
                const response = await fetch(apiUrl);
                const data = await response.json();
                LastSeenUpdate(list.id, data.lastSeenStatus, data.lastSeenStatusColor);
                if (data.statusChange) {
                    moveLiToTop(list.id, data.latestMessageId, data.latestMessage);
                }
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