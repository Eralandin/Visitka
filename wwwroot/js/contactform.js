document.addEventListener('DOMContentLoaded', function() {
    const elSelectCustom = document.getElementsByClassName("js-selectCustom")[0];
    const elSelectCustomValue = elSelectCustom.children[0];
    const elSelectCustomOptions = elSelectCustom.children[1];
    const defaultLabel = "Выберите спектр услуг"; // Дефолтный текст
    const selectedServiceInput = document.getElementById("selectedService");
    
    // Функция для выбора опции
    function selectOption(elOption) {
        const value = elOption.getAttribute("data-value");
        const text = elOption.textContent;
        
        // Обновляем текст триггера
        elSelectCustomValue.textContent = text;
        elSelectCustomValue.setAttribute("data-value", value);
        
        // Обновляем скрытое поле формы
        if (selectedServiceInput) {
            selectedServiceInput.value = value;
        }
        
        // Добавляем стили для выбранного значения
        elSelectCustomValue.classList.add('has-value');
        elSelectCustomValue.style.color = "#ffffff";
        
        // Снимаем выделение со всех опций и добавляем текущей
        Array.from(elSelectCustomOptions.children).forEach(option => {
            option.classList.remove("isActive");
        });
        elOption.classList.add("isActive");
        
        // Закрываем dropdown
        elSelectCustom.classList.remove("isActive");
    }

    // Проверяем, есть ли выбранная услуга при загрузке страницы
    const selectedServiceName = '@ViewData["SelectedServiceName"]';
    const currentText = elSelectCustomValue.textContent.trim();
    
    // Устанавливаем белый цвет ТОЛЬКО если есть выбранная услуга И текст не дефолтный
    if (selectedServiceName && selectedServiceName !== '' && currentText !== defaultLabel) {
        elSelectCustomValue.style.color = "#ffffff";
        elSelectCustomValue.classList.add('has-value');
        
        // Автоматически выбираем опцию если передан serviceName
        const targetOption = Array.from(elSelectCustomOptions.children).find(option => 
            option.getAttribute("data-value") === selectedServiceName
        );
        
        if (targetOption) {
            selectOption(targetOption);
        }
    } else {
        // Если нет выбранной услуги - устанавливаем стандартный цвет
        elSelectCustomValue.style.color = ""; // или ваш исходный цвет
        elSelectCustomValue.classList.remove('has-value');
    }

    // Обработчики кликов по опциям
    Array.from(elSelectCustomOptions.children).forEach(function (elOption) {
        elOption.addEventListener("click", (e) => {
            e.stopPropagation();
            selectOption(elOption);
        });
    });

    // Открытие/закрытие dropdown
    elSelectCustomValue.addEventListener("click", (e) => {
        e.stopPropagation();
        elSelectCustom.classList.toggle("isActive");
    });

    // Закрытие при клике вне элемента
    document.addEventListener("click", (e) => {
        const didClickedOutside = !elSelectCustom.contains(e.target);
        if (didClickedOutside) {
            elSelectCustom.classList.remove("isActive");
        }
    });

    // Закрытие по ESC
    document.addEventListener("keydown", (e) => {
        if (e.key === "Escape" && elSelectCustom.classList.contains("isActive")) {
            elSelectCustom.classList.remove("isActive");
        }
    });

    // Добавляем обработчик для обновления скрытого поля при изменении выбора
    elSelectCustomValue.addEventListener('DOMSubtreeModified', function() {
        if (selectedServiceInput) {
            const currentValue = elSelectCustomValue.getAttribute("data-value");
            const currentText = elSelectCustomValue.textContent.trim();
            
            // Обновляем цвет в зависимости от выбранного значения
            if (currentText !== defaultLabel && currentValue) {
                elSelectCustomValue.style.color = "#ffffff";
                elSelectCustomValue.classList.add('has-value');
            } else {
                elSelectCustomValue.style.color = "";
                elSelectCustomValue.classList.remove('has-value');
            }
            
            if (currentValue && currentValue !== defaultLabel) {
                selectedServiceInput.value = currentValue;
            }
        }
    });
});