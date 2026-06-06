// 1. Hàm mở Modal đặt lịch (Để hàm này tự quản lý hiển thị, logic check đăng nhập sẽ do Index.cshtml đè chặn ở vòng ngoài)
function openBooking(hospitalId, specializationId, doctorUserId) {
    const modal = document.getElementById("bookingModal");
    if (modal) {
        // Đồng bộ giao diện dạng hiển thị Block/Flex giống Index.cshtml
        modal.style.display = "block";

        // Tự động nạp giá trị khi người dùng click chọn từ ô tìm kiếm Autocomplete
        if (hospitalId) {
            const hospitalSelect = document.getElementById("hospital_id");
            if (hospitalSelect) hospitalSelect.value = hospitalId;
        }
        if (specializationId) {
            const specSelect = document.getElementById("specialization_id");
            if (specSelect) specSelect.value = specializationId;
        }

        // Nạp ID bác sĩ nếu có chỉ định bác sĩ khám
        const doctorInput = document.getElementById("doctor_user_id");
        if (doctorInput) {
            doctorInput.value = doctorUserId || "";
        }
    }
}

// 2. Hàm đóng Modal đặt lịch khám và làm sạch dữ liệu cũ
function closeBooking() {
    const modal = document.getElementById("bookingModal");
    if (modal) {
        modal.style.display = "none";
    }
    const bookingForm = document.getElementById("bookingForm");
    if (bookingForm) {
        bookingForm.reset();
    }
}

// Đóng modal khi người dùng click vào vùng trống ngoài Modal
window.onclick = function (event) {
    const modal = document.getElementById("bookingModal");
    if (event.target === modal) {
        modal.style.display = "none";
        const bookingForm = document.getElementById("bookingForm");
        if (bookingForm) {
            bookingForm.reset();
        }
    }
};

document.addEventListener("DOMContentLoaded", () => {
    // Hiệu ứng delay animation cho danh sách chuyên khoa
    const items = document.querySelectorAll(".specialties .item");
    items.forEach((item, index) => {
        item.style.animationDelay = `${index * 0.1}s`;
    });

    // ==========================================
    // TÍNH NĂNG TÌM KIẾM AUTOCOMPLETE (GIỮ LẠI & ĐỒNG BỘ ĐƯỜNG DẪN C#)
    // ==========================================
    const searchInput = document.getElementById("searchInput");
    const searchResults = document.getElementById("searchResults");
    let searchTimeout;

    if (searchInput && searchResults) {
        searchInput.addEventListener("input", function (e) {
            const query = this.value.trim();
            clearTimeout(searchTimeout);

            if (query.length < 1) {
                searchResults.innerHTML = "";
                searchResults.style.display = "none";
                searchResults.classList.remove("show");
                return;
            }

            searchTimeout = setTimeout(() => {
                // ĐÃ ĐỒNG BỘ: Chuyển hướng endpoint gọi API tìm kiếm về đúng Controller C# thay vì file .php cũ
                const url = `/Home/SearchDoctors?q=${encodeURIComponent(query)}`;

                fetch(url)
                    .then(res => {
                        if (!res.ok) throw new Error('Lỗi phản hồi mạng từ hệ thống: ' + res.status);
                        return res.json();
                    })
                    .then(data => {
                        displaySearchResults(data);
                    })
                    .catch(err => {
                        console.error("Search error:", err);
                        searchResults.innerHTML = "<div class='search-item' style='padding: 15px; color: red;'>Hệ thống tìm kiếm đang bận...</div>";
                        searchResults.style.display = "block";
                    });
            }, 200);
        });

        searchInput.addEventListener("focus", function () {
            const query = this.value.trim();
            if (query.length >= 1 && searchResults.innerHTML) {
                searchResults.style.display = "block";
            }
        });

        function escapeHtml(text) {
            if (!text) return '';
            const div = document.createElement('div');
            div.textContent = text;
            return div.innerHTML;
        }

        function displaySearchResults(results) {
            if (!results || results.length === 0) {
                searchResults.innerHTML = "<div class='search-item' style='padding: 20px; text-align: center; color: #999;'>Không tìm thấy kết quả</div>";
                searchResults.style.display = "block";
                searchResults.classList.add("show");
                return;
            }

            const limitedResults = results.slice(0, 10);

            searchResults.innerHTML = limitedResults.map(item => {
                let icon = "🏥";
                let typeLabel = "Bệnh viện";
                if (item.type === "doctor") {
                    icon = "👨‍⚕️";
                    typeLabel = "Bác sĩ";
                } else if (item.type === "specialty" || item.type === "specialization") {
                    icon = "💊";
                    typeLabel = "Chuyên khoa";
                }

                let link = item.url ? item.url : '';
                if (!link && item.type === 'doctor') {
                    link = `/Home/DoctorDetail?doctor_id=${item.id}`; // Sửa lại route C#
                }

                const inner = `
                  <div class="search-item" data-type="${item.type}" data-id="${item.id}" style="cursor: pointer;">
                    <span class="search-icon">${icon}</span>
                    <div class="search-info" style="flex: 1;">
                      <div class="search-title" style="font-weight: 600; color: #333; margin-bottom: 4px; font-size: 15px;">${escapeHtml(item.title)}</div>
                      ${item.subtitle ? `<div class="search-subtitle" style="color: #666; font-size: 13px; margin-bottom: 2px;">${escapeHtml(item.subtitle)}</div>` : ""}
                      ${item.price ? `<div class="search-price" style="color: #d93ba2; font-weight: 600; font-size: 13px;">${escapeHtml(item.price)}</div>` : ""}
                    </div>
                    <span style="color: #999; font-size: 12px; margin-left: 10px;">${typeLabel}</span>
                  </div>
                `;

                if (link) {
                    return `<a href="${link}" class="search-link" style="text-decoration:none; color:inherit; display:block;">${inner}</a>`;
                }
                return inner;
            }).join("");

            searchResults.style.display = "block";
            searchResults.classList.add("show");

            searchResults.querySelectorAll('.search-link').forEach(a => {
                a.addEventListener('click', function () {
                    searchInput.value = '';
                    searchResults.style.display = 'none';
                });
            });

            searchResults.querySelectorAll(".search-item").forEach(item => {
                item.addEventListener("click", function (e) {
                    const type = this.dataset.type;
                    const id = this.dataset.id;

                    if (type === 'doctor') {
                        searchInput.value = '';
                        searchResults.style.display = 'none';
                        return;
                    }

                    if (type === "hospital") {
                        const el = document.getElementById("hospital_id");
                        if (el) el.value = id;
                        // Gọi hàm openBooking (Kiểm tra đăng nhập từ Index.cshtml sẽ tự kích hoạt chặn)
                        if (typeof openBooking === "function") openBooking(id, null, null);
                    } else if (type === "specialization" || type === "specialty") {
                        const el = document.getElementById("specialization_id");
                        if (el) el.value = id;
                        if (typeof openBooking === "function") openBooking(null, id, null);
                    }

                    searchInput.value = "";
                    searchResults.style.display = "none";
                });
            });
        }

        document.addEventListener("click", function (e) {
            if (!searchInput.contains(e.target) && !searchResults.contains(e.target)) {
                searchResults.style.display = "none";
                searchResults.classList.remove("show");
            }
        });
    }

    // 🔥 ĐÃ XÓA BỎ: Toàn bộ khối bookingForm.addEventListener("submit") cũ của PHP ở đây!
    // Logic submit gửi dữ liệu AJAX đã được nhường quyền xử lý hoàn toàn cho file Index.cshtml để tránh lỗi đè thông báo.
});