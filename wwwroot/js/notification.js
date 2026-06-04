document.addEventListener("DOMContentLoaded", () => {

    const btn = document.getElementById("notification-btn");
    const menu = document.getElementById("notification-menu");
    const list = document.getElementById("notification-list");
    const badge = document.getElementById("notification-badge");

    if (!btn) return;

    btn.addEventListener("click", async() => {

        menu.style.display =
            menu.style.display === "block"
                ? "none"
                : "block";

        loadNotifications();

        await fetch("/Notification/MarkAsRead", {
            method: "POST"
        });

        badge.style.display = "none";
    });

    async function loadNotifications() {

        const response =
            await fetch("/Notification/GetNotifications");

        const data =
            await response.json();

        if (!data.success) return;

        badge.innerText = data.unreadCount;

        badge.style.display =
            data.unreadCount > 0
                ? "flex"
                : "none";

        let html = "";

        data.notifications.forEach(n => {

            html += `
            <div class="notification-item">
                <div>${n.message}</div>
                <small>
                    ${new Date(n.createdAt)
                    .toLocaleString("vi-VN")}
                </small>
            </div>
            `;
        });

        list.innerHTML = html;
    }

    loadNotifications();

    setInterval(loadNotifications, 30000);

});