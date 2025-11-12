document.addEventListener('DOMContentLoaded', function() {
    const imageItems = document.querySelectorAll('.image-item');
    
    imageItems.forEach(item => {
        item.addEventListener('click', function() {
            const index = this.getAttribute('data-index');
            
            // Закрываем все картинки
            imageItems.forEach(img => {
                img.classList.remove('active');
            });
            
            // Открываем выбранную картинку
            this.classList.add('active');
        });
    });
});