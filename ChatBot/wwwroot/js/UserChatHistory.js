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

function redirectToPage(event) {

    if (selectedValue == 0) {
        event.preventDefault();
        return;
    }

    const data = {
        fromUserId: fromUserId,
        toUserId: selectedValue,
        newUser: true
    };

    fetch('/ChatDetails', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })

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