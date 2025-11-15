document.addEventListener('DOMContentLoaded', function() {
    function adjustFooterHeight() {
        const footer = document.querySelector('footer');
        const header = document.querySelector('header');
        const main = document.querySelector('main');
        
        const headerHeight = header ? header.offsetHeight : 0;
        const mainHeight = main ? main.offsetHeight : 0;
        const viewportHeight = window.innerHeight;
        
        const totalContentHeight = headerHeight + mainHeight;
        
        if (totalContentHeight < viewportHeight && viewportHeight > 1920) {
            const footerHeight = viewportHeight - totalContentHeight;
            footer.style.height = (footerHeight - 16.28) + 'px';
            footer.style.maxHeight = 'none';
        } else {
            footer.style.minHeight = 'auto';
        }
    }
    
    adjustFooterHeight();
    window.addEventListener('resize', adjustFooterHeight);
    
    const yearElement = document.getElementById('currentYear');
    if (yearElement) {
        yearElement.textContent = new Date().getFullYear();
    }
});