const numberFormat = Intl.NumberFormat("en-US", { style: 'decimal', minimumFractionDigits: 0, maximumFractionDigits: 0 });
const percentFormat = Intl.NumberFormat('en-US', { style: 'percent', minimumFractionDigits: 1, maximumFractionDigits: 1 });
const percentFormat2Decimals = Intl.NumberFormat('en-US', { style: 'percent', minimumFractionDigits: 2, maximumFractionDigits: 2 });
// number format that receives a single parameter for decimal places
function formatNumberDecimal(value, decimalPlaces = 0) {
    return Intl.NumberFormat("en-US", { style: 'decimal', minimumFractionDigits: decimalPlaces, maximumFractionDigits: decimalPlaces }).format(value);
}

// percent format that receives a single parameter for decimal places
function formatPercent(value, decimalPlaces = 1) {
    return Intl.NumberFormat('en-US', { style: 'percent', minimumFractionDigits: decimalPlaces, maximumFractionDigits: decimalPlaces }).format(value);
}

// debounce function to limit the rate at which a function can fire.
function debounce(func, delay) {
    let timeout;
    return function(...args) {
        clearTimeout(timeout);
        timeout = setTimeout(() => func.apply(this, args), delay);
    };
}