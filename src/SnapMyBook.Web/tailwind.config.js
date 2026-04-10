/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Views/**/*.cshtml",
    "./wwwroot/js/**/*.js"
  ],
  theme: {
    extend: {
      colors: {
        ink: "#132033",
        sky: "#0c7ff2",
        mint: "#00c2a8"
      },
      boxShadow: {
        glass: "0 18px 60px rgba(14, 31, 53, 0.12)"
      },
      borderRadius: {
        shell: "28px"
      }
    }
  },
  plugins: [
    require("daisyui")
  ],
  daisyui: {
    themes: [
      {
        snapmybook: {
          "primary": "#0c7ff2",
          "secondary": "#132033",
          "accent": "#00c2a8",
          "neutral": "#132033",
          "base-100": "#f8fbff",
          "base-200": "#eef3f8",
          "base-300": "#dde7f0",
          "info": "#46a4ff",
          "success": "#00c2a8",
          "warning": "#ff8a3d",
          "error": "#d64545"
        }
      }
    ]
  }
};
