
// validating and highligh unvalidated
function validatingUsingClass(id) {
    var returnval = 0;
    var x = document.getElementById(id).getElementsByClassName('validate-required');
    for ($i = 0; $i < x.length; $i++) {
        if (x[$i].value.includes('Select') || x[$i].value === "" || x[$i].value === "0.00") {
            $('#' + x[$i].getAttribute('id')).attr('placeholder', 'This is a required field');
            x[$i].style.border = '1px solid red';
            returnval += 1;
        }
        else {
            $('#' + x[$i].getAttribute('id')).attr('placeholder', '');
            x[$i].style.border = '1px solid #eeeeee';
        }
    }
    return returnval;
}
function CustomAlert(title, text, type) {
    swal({
        title: title,
        text: text,
        type: type,
        showCancelButton: true,
        showConfirmButton: true,
        confirmButtonClass: 'btn btn-quickSWap'
    });
}
function validateNumber(id) {
    var num = $('#' + id).val();
    if (isNaN(num)) {
        $('#' + id).val('');
        $('#' + id).attr('placeholder', 'Only number required');
    } else {
        $('#' + id).attr('placeholder', '');
    }
}
function datetimeConverter(dt) {

    var _dateInt = new Date(parseInt(dt.substr(6)));
    var _date = _dateInt.toLocaleString();
    console.log(_dateInt);
    return _date;
}
function makeid() {
    length = 5;
    var text = "";
    var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    for (var i = 0; i < length; i++)
        text += possible.charAt(Math.floor(Math.random() * possible.length));

    return text;
}
function customPost(text, url, postObj, modalName, formName, actionName, populatingClass) {
    //$('#loader').show();
    swal({
        title: "ARE YOU SURE?",
        text: text,
        type: 'warning',
        showCancelButton: true,
        showConfirmButton: true,
        confirmButtonClass: 'btn btn-success',
        cancelButtonClass: 'btn btn-danger',
        confirmButtonText: 'Yes!',
        closeOnConfirm: false,
        closeOnCancel: false
    }, function (isConfirm) {
            if (isConfirm) {
            $.ajax({
                url: url,
                data: JSON.stringify(postObj),
                contentType: "application/json;charst=utf-8",
                dataType: 'JSON',
                type: 'POST',
                success: function (data) {
                    if (data.Status === "success") {
                        if (modalName !== null) {
                            $('#' + modalName).modal('hide');
                        }
                        $(':input', '#' + formName)
                            .not(':button, :submit, :reset, :hidden')
                            .val('')
                            .prop('checked', false)
                            .prop('selectedIndex', 0);
                        populatingClass();
                       // $('#bady').loading('stop');
                        CustomAlert(actionName, data.Message, data.Status);
                    } else {
                        CustomAlert(actionName, data.Message, data.Status);
                    }
                }
            });

        } else {
            swal("Cancelled", "Your have cancelled :)", "error");
        }
    });
}
function GetDate() {
    var d = new Date();

    var month = d.getMonth() + 1;
    var day = d.getDate();

    var output =(('' + day).length < 2 ? '0' : '') + day + '/' +
        (('' + month).length < 2 ? '0' : '') + month + '/' +
        d.getFullYear()
       ;

    return output;
}