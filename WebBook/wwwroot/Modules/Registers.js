$(document).ready(function () {
    $('#tableRole').DataTable();
});

function Delete(id) {
    Swal.fire({
        title: 'هل انتا متأكد ؟',
        text: 'لن تتمكن من التراجع عن هذا!!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = `/Admin/Accounts/DeleteUser?userId=${id}`;
            Swal.fire(
                'تم الحذف!',
                'تم حذف مجموعة المستخدم.',
                'success'
            )
        }
    })
}

Edit = (id, name, image, email, active, role) => {
    //alert(image);
    document.getElementById("title").innerHTML = "تعديل مجموعة المستخدم";
    document.getElementById("btnSave").value = "تعديل";
    document.getElementById("UserId").value = id;
    document.getElementById("UserName").value = name;
    document.getElementById("UserEmail").value = email;
    document.getElementById("ddlUserRole").value = role;
    var Active = document.getElementById("UserActive");
    if (active == "True")
        Active.checked = true;
    else
        Active.checked = false;
    $('#grPassword').hide();
    $('#grcomPassword').hide();
    document.getElementById("UserPassword").value = "$$$$$$";
    document.getElementById("UserCompare").value = "$$$$$$";
    document.getElementById("image").hidden = false;
    document.getElementById("image").src = "/Images/Users/" + image;
    document.getElementById("imgeHide").value = image;
   
}


Rest = () => {
    document.getElementById("title").innerHTML = "اضف مجموعة جديدة";
    document.getElementById("btnSave").value = "حفظ";
    document.getElementById("UserId").value = "";
    document.getElementById("UserName").value = "";
    document.getElementById("UserEmail").value = "";
    //document.getElementById("userImage").value = "";
    document.getElementById("ddlUserRole").value = "";
    document.getElementById("UserActive").checked = false;
    $('#grPassword').show();
    $('#grcomPassword').show();
    document.getElementById("UserPassword").value = "";
    document.getElementById("UserCompare").value = "";
    document.getElementById("image").hidden = true;
    document.getElementById("imgeHide").value = "";
   
    
}