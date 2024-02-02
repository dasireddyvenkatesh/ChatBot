function validateForm() {
    var username = document.getElementById("username").value;
    var errorMessage = document.getElementById("errorMessage");

    if (username === "" || username.length <= 3) {
        errorMessage.style.display = "block";
        return false;
    } else {
        errorMessage.style.display = "none";
        return true;
    }
}

function showCreateUserPopup() {
    document.getElementById("errorMessage").style.display = "none";
    document.getElementById("createUserPopup").style.display = "block";
}

function validateAndCreateUser() {
    var newUsername = document.getElementById("newUsername").value;
    var createUserErrorMessage = document.getElementById("createUserErrorMessage");

    if (newUsername === "" || newUsername.length <= 3) {
        createUserErrorMessage.style.display = "block";
    } else {
        createUserErrorMessage.style.display = "none";
        checkUserExists(newUsername);
    }
}
function closeCreateUserPopup() {
    document.getElementById("createUserPopup").style.display = "none";
    document.getElementById("errorMessage").style.display = "none";
}

function checkUserExists(newUserName) {
    $.ajax({
        url: '/NewUserRegister',
        type: 'POST',
        data: { newUserName: newUserName },
        success: function (data) {

            if (data === 0) {
                document.getElementById("createUserErrorMessage").innerText = "User Already Exists";
                document.getElementById("createUserErrorMessage").style.display = "block";
            } else {
                document.getElementById("createUserErrorMessage").innerText = "User Created Redirecting..";
                document.getElementById("createUserErrorMessage").style.display = "block";
                setTimeout(function () {
                    document.getElementById("createUserPopup").style.display = "none";
                    window.location.href = '/UserChatHistory?userName=' + newUserName;
                }, 1000);

            }
        }
    });
}