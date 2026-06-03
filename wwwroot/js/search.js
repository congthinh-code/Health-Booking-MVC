document.addEventListener("DOMContentLoaded", () => {

    const searchInput = document.getElementById("searchInput");
    const searchResults = document.getElementById("searchResults");

    if (!searchInput || !searchResults) return;

    let timeout;

    searchInput.addEventListener("input", function () {

        const query = this.value.trim();

        clearTimeout(timeout);

        if (query.length < 1) {
            searchResults.innerHTML = "";
            searchResults.style.display = "none";
            return;
        }

        timeout = setTimeout(() => {

            fetch(`/Search/GetSearch?q=${encodeURIComponent(query)}`)
                .then(res => res.json())
                .then(data => {

                    if (!data || data.length === 0) {
                        searchResults.innerHTML = `
                            <div class="search-item">
                                ❌ Không tìm thấy kết quả
                            </div>`;
                        searchResults.style.display = "block";
                        return;
                    }

                    searchResults.innerHTML = data.map(item => {

                        let icon = "🏥";
                        if (item.type === "doctor") icon = "👨‍⚕️";
                        if (item.type === "specialization") icon = "💊";

                        return `
                        <a href="${item.url}" class="search-link">
                            <div class="search-item">
                                <div class="search-icon">${icon}</div>

                                <div class="search-info">
                                    <div class="search-title">
                                        ${item.title}
                                    </div>

                                    <div class="search-subtitle">
                                        ${item.subtitle ?? ""}
                                    </div>
                                </div>
                            </div>
                        </a>`;
                    }).join("");

                    searchResults.style.display = "block";
                });

        }, 250);
    });

    document.addEventListener("click", function (e) {
        if (!searchInput.contains(e.target) &&
            !searchResults.contains(e.target)) {
            searchResults.style.display = "none";
        }
    });
});