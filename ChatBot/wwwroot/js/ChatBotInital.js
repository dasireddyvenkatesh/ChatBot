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
    var newEmail = document.getElementById("newEmail").value;
    var newPassword = document.getElementById("newPassword").value;
    var createUserErrorMessage = document.getElementById("createUserErrorMessage");

    if ((newUsername === "" || newUsername.length <= 3)) {
        createUserErrorMessage.style.display = "block";
    } else if (!validateEmail(newEmail)) {
        createUserErrorMessage.innerText = "Enter a valid email address";
        createUserErrorMessage.style.display = "block";
    } else if (newPassword === null || newPassword.length <= 5) {
        createUserErrorMessage.innerText = "Enter Password Min 5 Charcters";
        createUserErrorMessage.style.display = "block";
    } else {
        createUserErrorMessage.style.display = "none";
        document.getElementById("createButton").innerText = "Validating and Creating User..";
        document.getElementById("createButton").disabled = true;
        document.getElementById("createButton").style.cursor = "not-allowed"
        checkUserExists(newUsername, newEmail, newPassword);
    }
}

async function verifyEmailOtp() {
    var emailOtp = document.getElementById("emailotp").value;
    if (emailOtp.length < 6 || emailOtp.length >= 7) {
        document.getElementById("createUserErrorMessage").innerText = "Enter valid 6 digit otp";
        document.getElementById("createUserErrorMessage").style.display = "block";
    } else {
        document.getElementById("verifyEmail").disabled = true;
        document.getElementById("verifyEmail").style.cursor = "not-allowed";
        var newEmail = document.getElementById("newEmail").value;
        const apiUrl = `/VerifyEmailOtp?email=${newEmail}&emailOtp=${emailOtp}`;
        const response = await fetch(apiUrl);
        const message = await response.text();

        if (message != 'Thank you, Email Is Verified') {
            document.getElementById("createUserErrorMessage").innerText = message;
            document.getElementById("createUserErrorMessage").style.display = "block";
            document.getElementById("verifyEmail").disabled = false;
            document.getElementById("verifyEmail").style.cursor = "pointer";
        } else {
            document.getElementById("createUserErrorMessage").innerText = message;
            document.getElementById("createUserErrorMessage").style.display = "block";
            setTimeout(function () {
                var newUsername = document.getElementById("newUsername").value;
                var newPassword = document.getElementById("newPassword").value;
                document.getElementById("createUserPopup").style.display = "none";
                submitNewUserForm(newUsername, newPassword);
            }, 2000);
        }
    }
}

function validateEmail(email) {
    const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return emailPattern.test(email);
}

function closeCreateUserPopup() {
    document.getElementById("createUserPopup").style.display = "none";
    document.getElementById("errorMessage").style.display = "none";
}

function checkUserExists(newUserName, newEmail, newPassword) {
    $.ajax({
        url: '/NewUserRegister',
        type: 'POST',
        data: { newUserName: newUserName, newEmail: newEmail, newPassword: newPassword },
        success: function (data) {
            if (data != 'User registered successfully') {
                document.getElementById("createUserErrorMessage").innerText = data;
                document.getElementById("createUserErrorMessage").style.display = "block";
                document.getElementById("createButton").innerText = "Create";
                document.getElementById("createButton").disabled = false;
            } else {
                document.getElementById("createButton").innerText = "User Created..";
                document.getElementById("newUsername").style.display = "none";
                document.getElementById("newPassword").style.display = "none";
                document.getElementById("createButton").style.display = "none";
                document.getElementById("newEmail").disabled = true;
                document.getElementById('emailotp').style.display = "";
                document.getElementById('verifyEmail').style.display = "";
                document.getElementById("createUserErrorMessage").innerText = "Check your email inbox or spam folder";
                document.getElementById("createUserErrorMessage").style.display = "block";
                
            }
        }
    });
}

function submitNewUserForm(newUserName, newPassword) {
    // Create a form element
    var form = document.createElement('form');
    form.method = 'post';
    form.action = '/Login';

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
