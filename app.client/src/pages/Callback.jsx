import React, { useEffect, useState } from 'react';

const Callback = (props) => {
    const [token, setToken] = useState('');
    const i = 1;

    useEffect(() => {
        // Получаем параметры из URL
        const urlParams = new URLSearchParams(window.location.search);
        // Извлекаем токен из параметров
        const tokenFromCallback = urlParams.get('token');

        // Если токен получен, сохраняем его в состоянии компонента
        if (tokenFromCallback) {
        setToken(tokenFromCallback);
        }
    }, []);

    
    
    return (
        <div>callback {i}</div>
    );
};

export {Callback};