// JavaScript function to remove specified text and hide the containing element

var checkInterval = true;
function removeTextAndHideElement(targetText) {
    // Select all elements containing text nodes
    var elements = document.querySelectorAll('*');

    // Iterate through each element
    elements.forEach(function (element) {
        // Iterate through child nodes of the current element
        for (var i = 0; i < element.childNodes.length; i++) {
            var node = element.childNodes[i];

            // Check if the node is a text node and contains the target text
            if (node.nodeType === 3 && node.nodeValue.includes(targetText)) {
                // Hide the parent element by setting its display to "none"
                element.style.display = 'none';
                // Exit the loop since we found and hid the element
                return;
            }
        }
    });
}

function removeDiv() {
    var divToRemove1 = document.querySelector('div[style="opacity: 0.9; z-index: 2147483647; position: fixed; left: 0px; bottom: 0px; height: 65px; right: 0px; display: block; width: 100%; background-color: #202020; margin: 0px; padding: 0px;"]');
    var divToRemove2 = document.querySelector('div[style="position: fixed; z-index: 2147483647; left: 0px; bottom: 0px; height: 65px; right: 0px; display: block; width: 100%; background-color: transparent; margin: 0px; padding: 0px;"]');
    if (divToRemove1 && divToRemove2) {
        divToRemove1.remove();
        divToRemove2.remove();
        clearInterval(checkInterval);
    }
}

function removeAnotherDiv() {
    var divToRemove = document.querySelector('div[style="position: fixed; z-index: 2147483647; left: 0px; bottom: 0px; height: 65px; right: 0px; display: block; width: 100%; background-color: transparent; margin: 0px; padding: 0px;"]');
    if (divToRemove) {
        divToRemove.remove();
    }
}

// Call the function when the DOM content is loaded
document.addEventListener('DOMContentLoaded', function () {
    // Specify the target text to remove and the containing element to hide
    var targetText = 'Web hosting by Somee.com';

    // Call the function to remove the target text and hide the containing element
    removeTextAndHideElement(targetText);
    checkInterval = setInterval(removeDiv, 20);
});
