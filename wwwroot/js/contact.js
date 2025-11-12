document.addEventListener('DOMContentLoaded', function() {
    const imageItems = document.querySelectorAll('.image-item');
    
    imageItems.forEach(item => {
        item.addEventListener('click', function() {
            const index = this.getAttribute('data-index');
            
            imageItems.forEach(img => {
                img.classList.remove('active');
            });
            
            this.classList.add('active');
        });
    });
});