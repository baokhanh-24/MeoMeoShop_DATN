$(document).ready(function () {
    // Sidebar toggle
    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
        $('#content').toggleClass('active');
    });

    // Close sidebar on mobile when clicking outside
    $(document).click(function (e) {
        var container = $("#sidebar, #sidebarCollapse");
        if (!container.is(e.target) && container.has(e.target).length === 0) {
            if ($(window).width() <= 768) {
                $('#sidebar').removeClass('active');
                $('#content').removeClass('active');
            }
        }
    });

    // Active menu item
    const currentPath = window.location.pathname;
    $('nav#sidebar ul li a').each(function () {
        const menuPath = $(this).attr('href');
        if (currentPath.startsWith(menuPath)) {
            $(this).parent().addClass('active');
        }
    });

    // DataTable initialization if present
    if ($.fn.DataTable) {
        $('.admin-table').DataTable({
            responsive: true,
            language: {
                url: '//cdn.datatables.net/plug-ins/1.10.24/i18n/Vietnamese.json'
            }
        });
    }

    // Form validation
    $('form.admin-form').on('submit', function (e) {
        if (!this.checkValidity()) {
            e.preventDefault();
            e.stopPropagation();
        }
        $(this).addClass('was-validated');
    });

    // Image preview
    $('input[type="file"][accept="image/*"]').on('change', function (e) {
        const file = e.target.files[0];
        const reader = new FileReader();
        reader.onload = function (e) {
            $('#imagePreview').attr('src', e.target.result);
        }
        if (file) {
            reader.readAsDataURL(file);
        }
    });

    // Toast notifications
    function showToast(message, type = 'success') {
        const toast = $(`
            <div class="toast" role="alert">
                <div class="toast-body ${type}">
                    ${message}
                </div>
            </div>
        `);

        $('.toast-container').append(toast);
        toast.toast({ delay: 3000 }).toast('show');
    }

    // Show toast if there's a message in TempData
    if (tempMessage) {
        showToast(tempMessage);
    }
});