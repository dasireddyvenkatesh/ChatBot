function validateForm() {
    var username = document.getElementById("username").value;
    var password = document.getElementById("password").value;
    var errorMessage = document.getElementById("errorMessage");

    if (username === "" || username.length <= 3) {
        errorMessage.style.display = "block";
        return false;
    }
    else if (password === null || password.length <= 5) {
        document.getElementById("errorMessage").innerText = "Enter Password Min 5 Charcters"
        errorMessage.style.display = "block";
        return false;
    }
    else {
        errorMessage.style.display = "none";
        return true;
    }

    return true;
}

function showCreateUserPopup() {
    document.getElementById("errorMessage").style.display = "none";
    document.getElementById("createUserPopup").style.display = "block";
}

function validateAndCreateUser() {
    var newUsername = document.getElementById("newUsername").value;
    var newPassword = document.getElementById("newPassword").value;
    var createUserErrorMessage = document.getElementById("createUserErrorMessage");

    if ((newUsername === "" || newUsername.length <= 3)) {
        createUserErrorMessage.style.display = "block";
    } else if (newPassword === null || newPassword.length <= 5) {
        document.getElementById("createUserErrorMessage").innerText = "Enter Password Min 5 Charcters";
        createUserErrorMessage.style.display = "block";
    } else {
        createUserErrorMessage.style.display = "none";
        checkUserExists(newUsername, newPassword);
    }
}
function closeCreateUserPopup() {
    document.getElementById("createUserPopup").style.display = "none";
    document.getElementById("errorMessage").style.display = "none";
}

function checkUserExists(newUserName, newPassword) {
    $.ajax({
        url: '/NewUserRegister',
        type: 'POST',
        data: { newUserName: newUserName, newPassword: newPassword },
        success: function (data) {

            if (data === 0) {
                document.getElementById("createUserErrorMessage").innerText = "User Already Exists";
                document.getElementById("createUserErrorMessage").style.display = "block";
            } else {
                document.getElementById("createUserErrorMessage").innerText = "User Created Redirecting..";
                document.getElementById("createUserErrorMessage").style.display = "block";
                setTimeout(function () {
                    document.getElementById("createUserPopup").style.display = "none";
                    window.location.href = '/UserChatHistory?userName=' + newUserName + '&passWord=' + newPassword;
                    submitNewUserForm(newUserName, newPassword);
                }, 2000);

            }
        }
    });
}

function submitNewUserForm(newUserName, newPassword) {
    // Create a form element
    var form = document.createElement('form');
    form.method = 'post';
    form.action = '/UserChatHistory';

    // Function to create hidden input field
    function createHiddenInput(name, value) {
        var input = document.createElement('input');
        input.type = 'hidden';
        input.name = name;
        input.value = value;
        return input;
    }

    // Create hidden input fields for parameters
    form.appendChild(createHiddenInput('userName', newUserName));
    form.appendChild(createHiddenInput('passWord', newPassword));

    // Append the form to the document body
    document.body.appendChild(form);

    // Submit the form
    form.submit();
}
