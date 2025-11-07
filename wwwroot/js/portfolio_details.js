// Функция для обработки ошибок загрузки изображений
function handleImageError(imgElement, wrapperId, imageName) {
    const wrapper = document.getElementById(wrapperId);
    wrapper.innerHTML = `<div class="image-placeholder">Ошибка загрузки ${imageName.toLowerCase()}</div>`;
}

class PortfolioDetails {
    constructor() {
        this.currentImageType = 'main'; // По умолчанию главное изображение
        this.init();
    }

    init() {
        this.setupImageSwitcher();
    }

    setupImageSwitcher() {
        const buttons = document.querySelectorAll('.image-button:not(:disabled)');
        
        buttons.forEach(button => {
            button.addEventListener('click', () => {
                const imageType = button.dataset.type;
                this.switchImage(imageType);
            });
        });
    }

    switchImage(imageType) {
        if (this.currentImageType === imageType) return;

        // Обновляем активные кнопки
        document.querySelectorAll('.image-button').forEach(btn => {
            btn.classList.remove('active');
        });
        
        const activeButton = document.querySelector(`[data-type="${imageType}"]`);
        if (activeButton) {
            activeButton.classList.add('active');
        }

        // Скрываем все изображения
        document.querySelectorAll('.image-wrapper').forEach(wrapper => {
            wrapper.classList.remove('active');
        });

        // Показываем выбранное изображение
        const activeWrapper = document.getElementById(`${imageType}-image-wrapper`);
        if (activeWrapper) {
            activeWrapper.classList.add('active');
        }

        this.currentImageType = imageType;
    }
}

// Инициализация при загрузке страницы
document.addEventListener('DOMContentLoaded', () => {
    new PortfolioDetails();
});