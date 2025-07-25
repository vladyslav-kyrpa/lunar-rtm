/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}", // This line is important for TSX files
  ],
  theme: {
    extend: {},
  },
  plugins: [],
}