

$(document).ready(function () {
    // Get the modal
    var modal1 = document.getElementById('modModal');

    // Get the button that opens the modal
    var btn1 = document.getElementById("modBtn");

    // Get the <span> element that closes the modal
    var span1 = document.getElementsByClassName("modClose")[0];

    // When the user clicks the button, open the modal 
    btn1.onclick = function () {
        modal1.style.display = "block";
    }

    // When the user clicks on <span> (x), close the modal
    span1.onclick = function () {
        modal1.style.display = "none";
    }

    // When the user clicks anywhere outside of the modal, close it
    window.onclick = function (event) {
        if (event.target == modal1) {
            modal1.style.display = "none";
        }
    }

    // Get the modal
    var modal2 = document.getElementById('genModal');

    // Get the button that opens the modal
    var btn2 = document.getElementById("genBtn");

    // Get the <span> element that closes the modal
    var span2 = document.getElementsByClassName("genClose")[0];

    // When the user clicks the button, open the modal 
    btn2.onclick = function () {
        modal2.style.display = "block";
    }

    // When the user clicks on <span> (x), close the modal
    span2.onclick = function () {
        modal2.style.display = "none";
    }

    // When the user clicks anywhere outside of the modal, close it
    window.onclick = function (event) {
        if (event.target == modal2) {
            modal2.style.display = "none";
        }
    }
    ////Password check
    //var check = function () {
    //    if (document.getElementById('password').value ==
    //        document.getElementById('confirm_password').value) {
    //        document.getElementById('message').style.color = 'green';
    //        document.getElementById('message').innerHTML = 'Matching';
    //    } else {
    //        document.getElementById('message').style.color = 'red';
    //        document.getElementById('message').innerHTML = 'Not Matching';
    //    }
    //}
});