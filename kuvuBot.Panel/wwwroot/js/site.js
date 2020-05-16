window.addEventListener("scroll", () => {
    if (window.scrollY > 50) {
        if (!document.querySelector('nav').classList.contains('scrolled'))
            document.querySelector('nav').classList.add('scrolled');
    } else {
        document.querySelector('nav').classList.remove('scrolled');
    }
});

document.querySelectorAll('.commands-category').forEach(function (el) {
    el.addEventListener('click', function () {
        let categoryName = this.dataset.category ? this.dataset.category : false
        if (!categoryName) return;

        document.querySelector('.commands-category.active').classList.remove('active')
        this.classList.add('active')

        document.querySelector('.list-last').classList.add('d-none')
        document.querySelector('.list-last').classList.remove('list-last')

        let newDiv = document.querySelector(`div[data-list='${categoryName}']`)
        newDiv.classList.remove('d-none')
        newDiv.classList.add('list-last')
    })
})

document.querySelector('.navbar-toggler').addEventListener('click', function (e) {
    handleCollapseElementClick(e)
    document.querySelector('#navbar-main').classList.toggle('expanded')

})

function handleCollapseElementClick(e) {
    let el = e.currentTarget;
    let collapseTargetId = el.dataset.target || el.href || null;
    if (collapseTargetId) {
        let targetEl = document.querySelector(collapseTargetId);
        let isShown = targetEl.classList.contains('show') || targetEl.classList.contains('collapsing');
        if (!isShown) {
            targetEl.classList.remove('collapse');
            targetEl.classList.add('collapsing');
            targetEl.style.height = 0
            targetEl.classList.remove('collapsed');
            setTimeout(() => {
                targetEl.classList.remove('collapsing');
                targetEl.classList.add('collapse', 'show');
                targetEl.style.height = '';
            }, 350)
            targetEl.style.height = targetEl.scrollHeight + 'px';
        } else {
            targetEl.style.height = `${targetEl.getBoundingClientRect().height}px`
            targetEl.offsetHeight; // force reflow
            targetEl.classList.add('collapsing');
            targetEl.classList.remove('collapse', 'show');
            targetEl.style.height = '';
            setTimeout(() => {
                targetEl.classList.remove('collapsing');
                targetEl.classList.add('collapse');
            }, 350)
        }
    }
}