document.addEventListener('DOMContentLoaded', function() {
    initializeCustomSelect();
    initializeFormValidation();
});

function initializeCustomSelect() {
    const elSelectCustom = document.querySelector(".js-selectCustom");
    if (!elSelectCustom) return;

    const elSelectCustomValue = elSelectCustom.querySelector(".selectCustom-trigger");
    const elSelectCustomOptions = elSelectCustom.querySelector(".selectCustom-options");
    const selectedServiceInput = document.getElementById("selectedService");
    const selectError = document.getElementById("selectError");
    const defaultLabel = "Выберите спектр услуг";

    // Проверяем начальное значение
    const initialValue = selectedServiceInput ? selectedServiceInput.value : '';
    const currentText = elSelectCustomValue.textContent.trim();

    if (initialValue && initialValue !== '' && currentText !== defaultLabel) {
        elSelectCustomValue.textContent = currentText;
        elSelectCustomValue.style.color = "#ffffff";
        elSelectCustomValue.classList.add('has-value');
    }

    // Функция выбора опции
    function selectOption(elOption) {
        const value = elOption.getAttribute("data-value");
        const text = elOption.textContent;

        if (!value) return;

        // Обновляем текст триггера
        elSelectCustomValue.textContent = text;
        elSelectCustomValue.setAttribute("data-value", value);
        
        // Обновляем скрытое поле
        if (selectedServiceInput) {
            selectedServiceInput.value = value;
        }

        // Обновляем стили
        elSelectCustomValue.style.color = "#ffffff";
        elSelectCustomValue.classList.add('has-value');

        // Снимаем выделение со всех опций и добавляем текущей
        const allOptions = elSelectCustomOptions.querySelectorAll(".selectCustom-option");
        allOptions.forEach(option => {
            option.classList.remove("isActive");
        });
        elOption.classList.add("isActive");

        // Скрываем ошибку если была
        if (selectError) {
            selectError.style.display = 'none';
        }
        elSelectCustomValue.style.borderColor = '';

        // Закрываем dropdown
        elSelectCustom.classList.remove("isActive");
    }

    // Добавляем обработчики для опций
    const options = elSelectCustomOptions.querySelectorAll(".selectCustom-option");
    options.forEach(option => {
        option.addEventListener("click", function(e) {
            e.stopPropagation();
            selectOption(this);
        });
    });

    // Открытие/закрытие dropdown
    elSelectCustomValue.addEventListener("click", function(e) {
        e.stopPropagation();
        elSelectCustom.classList.toggle("isActive");
    });

    // Закрытие при клике вне элемента
    document.addEventListener("click", function(e) {
        if (!elSelectCustom.contains(e.target)) {
            elSelectCustom.classList.remove("isActive");
        }
    });

    // Закрытие по ESC
    document.addEventListener("keydown", function(e) {
        if (e.key === "Escape" && elSelectCustom.classList.contains("isActive")) {
            elSelectCustom.classList.remove("isActive");
        }
    });
}

function initializeFormValidation() {
    const form = document.querySelector('form');
    if (!form) return;

    // Элементы формы
    const nameInput = document.getElementById("name");
    const phoneInput = document.getElementById("phone");
    const emailInput = document.getElementById("email");
    const commentInput = document.getElementById("text");
    const checkboxInput = document.getElementById("checkbox");
    const selectedServiceInput = document.getElementById("selectedService");
    const selectTrigger = document.getElementById("selectTrigger");

    // Элементы ошибок
    const nameError = document.getElementById("nameError");
    const phoneError = document.getElementById("phoneError");
    const emailError = document.getElementById("emailError");
    const commentError = document.getElementById("commentError");
    const checkboxError = document.getElementById("checkboxError");
    const selectError = document.getElementById("selectError");

    // Флаг для отслеживания валидации
    let isFormValidating = false;

    // Функции валидации
    function validateName() {
        const value = nameInput.value.trim();
        const isValid = value.length >= 2;
        
        if (!isValid) {
            showError(nameInput, nameError, 'Пожалуйста, введите ваше имя (минимум 2 символа)');
        } else {
            hideError(nameInput, nameError);
        }
        return isValid;
    }
    
    function validatePhone() {
        const value = phoneInput.value.trim();
        const phoneRegex = /^[\+]?[7-8]?[0-9\s\-\(\)]{10,15}$/;
        const isValid = phoneRegex.test(value);
        
        if (!isValid) {
            showError(phoneInput, phoneError, 'Пожалуйста, введите корректный номер телефона');
        } else {
            hideError(phoneInput, phoneError);
        }
        return isValid;
    }
    
    function validateEmail() {
        const value = emailInput.value.trim();
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        const isValid = emailRegex.test(value);
        
        if (!isValid) {
            showError(emailInput, emailError, 'Пожалуйста, введите корректный email');
        } else {
            hideError(emailInput, emailError);
        }
        return isValid;
    }
    
    function validateComment() {
        const value = commentInput.value.trim();
        const isValid = value.length >= 5;
        
        if (!isValid) {
            showError(commentInput, commentError, 'Пожалуйста, опишите ваш бизнес (минимум 5 символов)');
        } else {
            hideError(commentInput, commentError);
        }
        return isValid;
    }
    
    function validateCheckbox() {
        const isValid = checkboxInput.checked;
        
        if (!isValid) {
            showError(checkboxInput.parentElement, checkboxError, 'Необходимо согласие с политикой конфиденциальности');
        } else {
            hideError(checkboxInput.parentElement, checkboxError);
        }
        return isValid;
    }
    
    function validateSelect() {
        const value = selectedServiceInput ? selectedServiceInput.value : '';
        const currentText = selectTrigger ? selectTrigger.textContent.trim() : '';
        const defaultLabel = "Выберите спектр услуг";
        const isValid = value && value !== '' && currentText !== defaultLabel;
        
        if (!isValid) {
            showError(selectTrigger, selectError, 'Пожалуйста, выберите спектр услуг');
        } else {
            hideError(selectTrigger, selectError);
        }
        return isValid;
    }
    
    function showError(inputElement, errorElement, message) {
        if (errorElement) {
            errorElement.textContent = message;
            errorElement.style.display = 'block';
        }
        if (inputElement) {
            inputElement.style.borderColor = '#dc3545';
            inputElement.classList.add('error');
        }
    }
    
    function hideError(inputElement, errorElement) {
        if (errorElement) {
            errorElement.style.display = 'none';
        }
        if (inputElement) {
            inputElement.style.borderColor = '';
            inputElement.classList.remove('error');
        }
    }

    function validateAll() {
        const validations = [
            validateName(),
            validatePhone(),
            validateEmail(),
            validateComment(),
            validateCheckbox(),
            validateSelect()
        ];
        
        return validations.every(valid => valid === true);
    }

    // Валидация при вводе данных
    if (nameInput) nameInput.addEventListener('blur', validateName);
    if (phoneInput) phoneInput.addEventListener('blur', validatePhone);
    if (emailInput) emailInput.addEventListener('blur', validateEmail);
    if (commentInput) commentInput.addEventListener('blur', validateComment);
    if (checkboxInput) checkboxInput.addEventListener('change', validateCheckbox);

    // Валидация при отправке формы
    form.addEventListener('submit', async function(e) {
        e.preventDefault();
        
        const isValid = validateAll();
        
        if (!isValid) {
            const firstErrorElement = document.querySelector('.error');
            if (firstErrorElement) {
                firstErrorElement.scrollIntoView({ 
                    behavior: 'smooth', 
                    block: 'center' 
                });
            }
            return;
        }

        // Используем Fetch API
        try {
            const formData = new FormData(form);
            const submitBtn = form.querySelector('button[type="submit"]');
            
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.textContent = 'Отправка...';
            }

            const response = await fetch(form.action, {
                method: 'POST',
                body: formData,
                headers: {
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                }
            });

            if (response.redirected) {
                window.location.href = response.url;
            } else {
                const result = await response.text();
                document.documentElement.innerHTML = result;
            }
        } catch (error) {
            console.error('Error:', error);
            alert('Произошла ошибка при отправке формы');
            
            const submitBtn = form.querySelector('button[type="submit"]');
            if (submitBtn) {
                submitBtn.disabled = false;
                submitBtn.textContent = 'Отправить';
            }
        }
    });

    // Сброс флага валидации после отправки (на случай возврата)
    form.addEventListener('reset', function() {
        isFormValidating = false;
        const submitBtn = form.querySelector('button[type="submit"]');
        if (submitBtn) {
            submitBtn.disabled = false;
            submitBtn.textContent = 'Отправить';
        }
    });
}