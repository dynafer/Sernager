(function () {
    const CACHE_CURRENT_REPORT = "currentReport";

    const navTab = document.getElementById('nav-tab');
    const tabContent = document.getElementById('tab-content');

    const currentReport = localStorage.getItem(CACHE_CURRENT_REPORT);
    const bCurrentReportExists = currentReport !== null && reports.includes(currentReport);

    for (let i = 0; i < reports.length; ++i) {
        const report = reports[i];
        const reportId = report.replace(/\./g, "-");
        const bActivated = (bCurrentReportExists && report === currentReport) || (!bCurrentReportExists && i == 0);

        const navHTML = createNavItem(report, reportId, bActivated);
        const contentHTML = createContentItem(report, reportId, bActivated);

        navTab.innerHTML += navHTML;
        tabContent.innerHTML += contentHTML;
    }

    function createNavItem(report, reportId, bActivated) {
        return `
            <li class="nav-item" role="presentation">
                <button class="nav-link${bActivated ? " active" : ""}"
                    id="${reportId}-tab"
                    data-bs-toggle="tab"
                    data-bs-target="#${reportId}"
                    type="button"
                    role="tab"
                    onclick="localStorage.setItem('${CACHE_CURRENT_REPORT}', '${report}')"
                    aria-controls="${reportId}"
                    aria-selected="${bActivated ? " true" : "false"}">${report}</button>
            </li>
        `;
    }

    function createContentItem(report, reportId, bActivated) {
        return `
            <div class="tab-pane h-100 fade${bActivated ? " show active" : ""}"
                id="${reportId}"
                role="tabpanel"
                aria-labelledby="${reportId}-tab"
                tabindex="0">
                <iframe frameborder="0" width="100%" height="100%" src="./${report}/index.html"></iframe>
            </div>
        `;
    }
})();