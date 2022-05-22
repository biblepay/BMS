function DoCallback(action, o) {
    var CTS = new Object();
    CTS.BBPAddress = "";
    CTS.ExtraData = JSON.stringify(o);
    CTS.FormData = MemorizeForm();
    CTS.Action = action;
    $.ajax({
        type: "POST",
        url: 'intel/processdocallback',
        data: JSON.stringify(CTS),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data, resObject) {
            console.log(data);
            // This object contains the return value
            if (data != null && data.length > 1) {
                var obj = JSON.parse(data);
                console.log(obj.returnbody);
                console.log(obj.field2);
                if (obj.returntype == "modal") {
                    var implant = document.getElementById("implant");

                    try {
                        removeAllChildNodes(implant);
                    }
                    catch (e) {

                    }
                    var div = document.createElement('div');
                    div.innerHTML = obj.returnbody;
                    implant.appendChild(div);
                    $('#modalid1').modal('show');
                } else if (obj.returntype == "javascript") {
                    setTimeout(obj.returnbody, 1);
                }
            }
        },
        error: function () {
            alert("Error while inserting data");
        }
    });
    return true;
}



function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}
function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}
function eraseCookie(name) {
    document.cookie = name + '=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
}




function ElementsToHTML(sType) {
    var elements = document.getElementsByTagName(sType);

    var html = "";
    for (var i = 0; i < elements.length; i++) {
        var id = elements[i].id;
        var value = elements[i].value;
        if (elements[i].type == 'radio' || elements[i].type == 'checkbox') {
            if (!elements[i].checked)
                value = "";
        }

        var row = "<col>" + id + "<col>" + value + "<row>\r\n";
        html += row;
    }
    return html;
}

function MemorizeForm() {
    var html = ElementsToHTML("input");
    var ta = ElementsToHTML("textarea");
    var td = ElementsToHTML("select");
    var h = html + ta + td;
    return h;
}

function openModal(id) {
    //document.getElementById("backdrop").style.display = "block";
    document.getElementById(id).style.display = "block";
    document.getElementById(id).classList.add("show");
}

function closeModal(id) {
    //document.getElementById("backdrop").style.display = "none"
    document.getElementById(id).style.display = "none"
    document.getElementById(id).classList.remove("show")
}

function removeAllChildNodes(parent) {
    while (parent.firstChild) {
        parent.removeChild(parent.firstChild);
    }
}


