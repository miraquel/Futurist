﻿const numberFormat = Intl.NumberFormat("en-US", { style: 'decimal', minimumFractionDigits: 0, maximumFractionDigits: 0 });
const percentFormat = Intl.NumberFormat('en-US', { style: 'percent', minimumFractionDigits: 1, maximumFractionDigits: 1 });

function debounce(func, delay) {
    let timeout;
    return function(...args) {
        clearTimeout(timeout);
        timeout = setTimeout(() => func.apply(this, args), delay);
    };
}