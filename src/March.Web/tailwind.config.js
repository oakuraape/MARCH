/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./*.razor",
        "./Features/Server/StaticFiles/*.html",
        "./Features/Server/StaticFiles/*.js",
        "./Features/Client/**/*.razor",
    ],
    daisyui: {
        themes: ["light", "dark", "cupcake"],
    },
    darkMode: 'selector',
    theme: {
        extend: {},
    },
    plugins: [
        require('daisyui'),
    ],
}

