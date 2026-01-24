<div align="center">
  <img src="Shotora.App/Assets/Shotora.png" width="200" height="200" alt="Shotora logo" />
  <h1>Shotora</h1>
  <p>A modern, privacy-focused screenshot tool with powerful annotation and local OCR capabilities.</p>

  <p>
    <a href="LICENSE">
      <img src="https://img.shields.io/badge/License-Apache--2.0-brightgreen" alt="License" />
    </a>
    <img src="https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white" alt=".NET 10" />
    <img src="https://img.shields.io/badge/C%23-13.0-239120?logo=csharp&logoColor=white" alt="C# 13" />
    <img src="https://img.shields.io/badge/Avalonia-11.3-8B44AC?logo=avalonia&logoColor=white" alt="Avalonia" />
  </p>

  <p>
    <img src="https://img.shields.io/badge/Cake-6.0.0-F5871F?logo=cakebuild&logoColor=white" alt="Cake 6.0.0" />
    <img src="https://img.shields.io/badge/Velopack-0.0.1369-00B4AB?logo=nuget&logoColor=white" alt="Velopack" />
  </p>

  <p>
    <img src="https://img.shields.io/badge/Windows-0078D6?logo=windows&logoColor=white" alt="Windows" />
    <img src="https://img.shields.io/badge/Linux-FCC624?logo=linux&logoColor=black" alt="Linux" />
    <img src="https://img.shields.io/badge/macOS-000000?logo=apple&logoColor=white" alt="macOS" />
  </p>

<!-- GitHub Release & Downloads -->
[![GitHub release](https://img.shields.io/github/v/release/Adhamura/Shotora?style=flat-square&logo=github)](https://github.com/Adhamura/Shotora/releases/latest)
[![GitHub downloads (latest)](https://img.shields.io/github/downloads/Adhamura/Shotora/latest/total?style=flat-square&logo=github)](https://github.com/Adhamura/Shotora/releases/latest)
[![GitHub downloads (total)](https://img.shields.io/github/downloads/Adhamura/Shotora/total?style=flat-square&logo=github)](https://github.com/Adhamura/Shotora/releases)

<!-- Social & Contact -->
[![LinkedIn](https://img.shields.io/badge/LinkedIn-%230077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/kazuki-kimura/)
[![Gmail](https://img.shields.io/badge/Gmail-D14836?style=for-the-badge&logo=gmail&logoColor=white)](mailto:kazuki.kimura.jp@com)
[![Line](https://img.shields.io/badge/Line-00C300?style=for-the-badge&logo=line&logoColor=white)](https://line.me/ti/p/adhamura)
[![Website](https://img.shields.io/badge/Website-000000?style=for-the-badge&logo=google-chrome&logoColor=white)](https://www.kazukikimura.com)
[![Instagram](https://img.shields.io/badge/Instagram-%23E1306C?style=for-the-badge&logo=instagram&logoColor=white)](https://www.instagram.com/kimura_kazuki_official/)

<!-- top of README.md -->

  <p>
    <a href="#-features">Features</a> •
    <a href="#-installation">Installation</a> •
    <a href="#-usage">Usage</a> •
    <a href="#-ocr-engines">OCR</a> •
    <a href="#-development">Development</a> •
    <a href="#-license">License</a>
  </p>
</div>

---

<div align="center">
  <p>Made with ❤️ for productivity</p>
  <p>
    <strong>⭐ Star this repo if you find it useful!</strong>
  </p>
</div>

---

## ✨ Features

### 🖼️ Screen Capture
- **Multi-monitor support** — Capture across all connected displays
- **Flexible modes** — Region selection, fullscreen, or active window
- **Resizable selection** — Fine-tune your capture area with precision handles
- **Smart positioning** — Movable tool palette that stays out of your way

### ✏️ Annotation Tools
| Tool                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         | Description |
|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-------------|
| ![](https://img.shields.io/badge/-white?style=plastic&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnIHZpZXdCb3g9JzQwIC03NjAgODgwIDYwMC45NDEnPjxwYXRoIGZpbGw9JyNBRDY0ZjUnIGZpbGwtcnVsZT0nZXZlbm9kZCcgZD0nTTc4MiwtNjc0TDQ0NywtMzM5TDQ5OSwtMjg3TDgzNCwtNjIyTDc4MiwtNjc0Wk0yMDAsLTY4MEMyNjguNjY3LC02NzQuNjY3LDMxOS4xNjcsLTY2MC44MzMsMzUxLjUsLTYzOC41QzM4My44MzMsLTYxNi4xNjcsNDAwLC01ODQsNDAwLC01NDJDNDAwLC01MDYuNjY3LDM4Ny4xNjcsLTQ3OSwzNjEuNSwtNDU5QzMzNS44MzMsLTQzOSwyOTgsLTQyNywyNDgsLTQyM0MyMDUuMzMzLC00MTkuNjY3LDE3My4zMzMsLTQxMS44MzMsMTUyLC0zOTkuNUMxMzAuNjY3LC0zODcuMTY3LDEyMCwtMzcwLjMzMywxMjAsLTM0OUMxMjAsLTMyNS42NjcsMTI5LjMzMywtMzA4LjgzMywxNDgsLTI5OC41QzE2Ni42NjcsLTI4OC4xNjcsMTk4LC0yODIsMjQyLC0yODBMMjM4LC0yMDBDMTcxLjMzMywtMjAzLjMzMywxMjEuNjY3LC0yMTcuMzMzLDg5LC0yNDJDNTYuMzMzLC0yNjYuNjY3LDQwLC0zMDIuMzMzLDQwLC0zNDlDNDAsLTM5Mi4zMzMsNTcuODMzLC00MjcuNSw5My41LC00NTQuNUMxMjkuMTY3LC00ODEuNSwxNzguNjY3LC00OTcuNjY3LDI0MiwtNTAzQzI2OCwtNTA1LDI4Ny41LC01MDkuMTY3LDMwMC41LC01MTUuNUMzMTMuNSwtNTIxLjgzMywzMjAsLTUzMC42NjcsMzIwLC01NDJDMzIwLC01NTkuMzMzLDMxMC4xNjcsLTU3Mi4zMzMsMjkwLjUsLTU4MUMyNzAuODMzLC01ODkuNjY3LDIzOC4zMzMsLTU5NiwxOTMsLTYwMEwyMDAsLTY4MFpNNzgyLjUsLTc2MEM4MDAuODMzLC03NjAsODE2LjY2NywtNzUzLjMzMyw4MzAsLTc0MEw5MDAsLTY3MEM5MTMuMzMzLC02NTYuNjY3LDkyMCwtNjQwLjgzMyw5MjAsLTYyMi41QzkyMCwtNjA0LjE2Nyw5MTMuMzMzLC01ODguMzMzLDkwMCwtNTc1TDUxOCwtMTkzTDM1OSwtMTYwQzM0Ny42NjcsLTE1Ny4zMzMsMzM3LjY2NywtMTYwLjMzMywzMjksLTE2OUMzMjAuMzMzLC0xNzcuNjY3LDMxNy4zMzMsLTE4Ny42NjcsMzIwLC0xOTlMMzUzLC0zNThMNzM1LC03NDBDNzQ4LjMzMywtNzUzLjMzMyw3NjQuMTY3LC03NjAsNzgyLjUsLTc2MFonLz48L3N2Zz4=) **IconPen**                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      | Freehand drawing with customizable thickness |
| ![](https://img.shields.io/badge/-white?style=plastic&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnIHZpZXdCb3g9JzIwMSAtNzYwIDU1OCA1NTknPjxwYXRoIGZpbGw9JyNBRDY0ZjUnIGZpbGwtcnVsZT0nZXZlbm9kZCcgZD0nTTcxOS41LC03NjBDNzMwLjUsLTc2MCw3NDAsLTc1Niw3NDgsLTc0OEM3NTUuMzMzLC03NDAuNjY3LDc1OSwtNzMxLjMzMyw3NTksLTcyMEM3NTksLTcwOC42NjcsNzU1LjMzMywtNjk5LjMzMyw3NDgsLTY5MkwyNjgsLTIxMkMyNjAuNjY3LC0yMDQuNjY3LDI1MS4zMzMsLTIwMSwyNDAsLTIwMUMyMjguNjY3LC0yMDEsMjE5LjMzMywtMjA0LjY2NywyMTIsLTIxMkMyMDQuNjY3LC0yMTkuMzMzLDIwMSwtMjI4LjY2NywyMDEsLTI0MEMyMDEsLTI1MS4zMzMsMjA0LjY2NywtMjYwLjY2NywyMTIsLTI2OEw2OTIsLTc0OEM2OTkuMzMzLC03NTYsNzA4LjUsLTc2MCw3MTkuNSwtNzYwWicvPjwvc3ZnPg==) **Line**                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         | Straight lines for precise markups |
| ![](https://img.shields.io/badge/-white?style=plastic&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnIHZpZXdCb3g9JzgwIC03MjAgODAwIDQ4MCc+PHBhdGggZmlsbD0nI0FENjRmNScgZmlsbC1ydWxlPSdldmVub2RkJyBkPSdNNjQwLC03MjBMODgwLC03MjBMODgwLC00ODBMODAwLC00ODBMODAwLC01ODNMNjIxLC00MDVDNTk3LjY2NywtMzgxLjY2Nyw1NjkuMzMzLC0zNzAsNTM2LC0zNzBDNTAyLjY2NywtMzcwLDQ3NC4zMzMsLTM4MS42NjcsNDUxLC00MDVMNDA0LC00NTJDMzk2LjY2NywtNDU5LjMzMywzODcuMzMzLC00NjMsMzc2LC00NjNDMzY0LjY2NywtNDYzLDM1NS4zMzMsLTQ1OS4zMzMsMzQ4LC00NTJMMTM2LC0yNDBMODAsLTI5NkwyOTIsLTUwOEMzMTUuMzMzLC01MzEuMzMzLDM0My42NjcsLTU0MywzNzcsLTU0M0M0MTAuMzMzLC01NDMsNDM4LjY2NywtNTMxLjMzMyw0NjIsLTUwOEw1MDgsLTQ2MkM1MTYsLTQ1NCw1MjUuNSwtNDUwLDUzNi41LC00NTBDNTQ3LjUsLTQ1MCw1NTcsLTQ1NCw1NjUsLTQ2Mkw3NDMsLTY0MEw2NDAsLTY0MEw2NDAsLTcyMFonLz48L3N2Zz4=) **Arrow**                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                | Directional arrows to highlight important areas |
| ![](https://img.shields.io/badge/-white?style=plastic&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnIHZpZXdCb3g9JzgwIC04MDAgODAwIDY0MCc+PHBhdGggZmlsbD0nI0FENjRmNScgZmlsbC1ydWxlPSdldmVub2RkJyBkPSdNMTYwLC03MjBMMTYwLC0yNDBMODAwLC0yNDBMODAwLC03MjBMMTYwLC03MjBaTTgwLC04MDBMODgwLC04MDBMODgwLC0xNjBMODAsLTE2MEw4MCwtODAwWicvPjwvc3ZnPg==) **Rectangle**                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    | Rectangular shapes and outlines |
| ![](https://img.shields.io/badge/-white?style=plastic&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnIHZpZXdCb3g9JzgwIC04ODAgODAwIDgwMCc+PHBhdGggZmlsbD0nI0FENjRmNScgZmlsbC1ydWxlPSdldmVub2RkJyBkPSdNNDgwLC04MDBDMzkwLjY2NywtODAwLDMxNSwtNzY5LDI1MywtNzA3QzE5MSwtNjQ1LDE2MCwtNTY5LjMzMywxNjAsLTQ4MEMxNjAsLTM5MC42NjcsMTkxLC0zMTUsMjUzLC0yNTNDMzE1LC0xOTEsMzkwLjY2NywtMTYwLDQ4MCwtMTYwQzU2OS4zMzMsLTE2MCw2NDUsLTE5MSw3MDcsLTI1M0M3NjksLTMxNSw4MDAsLTM5MC42NjcsODAwLC00ODBDODAwLC01NjkuMzMzLDc2OSwtNjQ1LDcwNywtNzA3QzY0NSwtNzY5LDU2OS4zMzMsLTgwMCw0ODAsLTgwMFpNNDgwLC04ODBDNTM1LjMzMywtODgwLDU4Ny4zMzMsLTg2OS41LDYzNiwtODQ4LjVDNjg0LjY2NywtODI3LjUsNzI3LC03OTksNzYzLC03NjNDNzk5LC03MjcsODI3LjUsLTY4NC42NjcsODQ4LjUsLTYzNkM4NjkuNSwtNTg3LjMzMyw4ODAsLTUzNS4zMzMsODgwLC00ODBDODgwLC00MjQuNjY3LDg2OS41LC0zNzIuNjY3LDg0OC41LC0zMjRDODI3LjUsLTI3NS4zMzMsNzk5LC0yMzMsNzYzLC0xOTdDNzI3LC0xNjEsNjg0LjY2NywtMTMyLjUsNjM2LC0xMTEuNUM1ODcuMzMzLC05MC41LDUzNS4zMzMsLTgwLDQ4MCwtODBDNDI0LjY2NywtODAsMzcyLjY2NywtOTAuNSwzMjQsLTExMS41QzI3NS4zMzMsLTEzMi41LDIzMywtMTYxLDE5NywtMTk3QzE2MSwtMjMzLDEzMi41LC0yNzUuMzMzLDExMS41LC0zMjRDOTAuNSwtMzcyLjY2Nyw4MCwtNDI0LjY2Nyw4MCwtNDgwQzgwLC01MzUuMzMzLDkwLjUsLTU4Ny4zMzMsMTExLjUsLTYzNkMxMzIuNSwtNjg0LjY2NywxNjEsLTcyNywxOTcsLTc2M0MyMzMsLTc5OSwyNzUuMzMzLC04MjcuNSwzMjQsLTg0OC41QzM3Mi42NjcsLTg2OS41LDQyNC42NjcsLTg4MCw0ODAsLTg4MFonLz48L3N2Zz4=) **Ellipse**                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      | Circular and oval shapes |
| ![](https://img.shields.io/badge/-white?style=plastic&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnIHZpZXdCb3g9JzgwIC04MDAgODAwIDY0MCc+PHBhdGggZmlsbD0nI0FENjRmNScgZmlsbC1ydWxlPSdldmVub2RkJyBkPSdNNTIwLC02MDBMODgwLC02MDBMODgwLC00ODBMNzYwLC00ODBMNzYwLC0xNjBMNjQwLC0xNjBMNjQwLC00ODBMNTIwLC00ODBMNTIwLC02MDBaTTgwLC04MDBMNjAwLC04MDBMNjAwLC02ODBMNDAwLC02ODBMNDAwLC0xNjBMMjgwLC0xNjBMMjgwLC02ODBMODAsLTY4MEw4MCwtODAwWicvPjwvc3ZnPg==) **Text**                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         | Add text annotations with font customization |
| ![](https://img.shields.io/badge/-white?style=plastic&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnIHZpZXdCb3g9JzYwIC04NDAgODIwIDcyMCc+PHBhdGggZmlsbD0nI0FENjRmNScgZmlsbC1ydWxlPSdldmVub2RkJyBkPSdNNDQwLC01MDRMMjQwLC0zMDRMMzQ0LC0yMDBMNTQ0LC00MDBMNDQwLC01MDRaTTY5NiwtNzYwTDQ5NywtNTYxTDYwMSwtNDU3TDgwMCwtNjU2TDY5NiwtNzYwWk02OTYsLTg0MEM3MTcuMzMzLC04NDAsNzM2LC04MzIsNzUyLC04MTZMODU2LC03MTJDODcyLC02OTYsODgwLC02NzcuMzMzLDg4MCwtNjU2Qzg4MCwtNjM0LjY2Nyw4NzIsLTYxNiw4NTYsLTYwMEw2MjksLTM3M0w0MDAsLTE0NEMzODQsLTEyOCwzNjUuMzMzLC0xMjAsMzQ0LC0xMjBDMzIyLjY2NywtMTIwLDMwNCwtMTI4LDI4OCwtMTQ0TDI4NiwtMTQ2TDI2MCwtMTIwTDYwLC0xMjBMMTg2LC0yNDZMMTg0LC0yNDhDMTY4LC0yNjQsMTYwLC0yODIuNjY3LDE2MCwtMzA0QzE2MCwtMzI1LjMzMywxNjgsLTM0NCwxODQsLTM2MEw0MTMsLTU4OUw2NDAsLTgxNkM2NTYsLTgzMiw2NzQuNjY3LC04NDAsNjk2LC04NDBaJy8+PC9zdmc+) **Highlight**                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    | Semi-transparent highlighting |
| ![](https://img.shields.io/badge/-white?style=plastic&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnIHZpZXdCb3g9JzEyMCAtODQwIDc0MCA3MjAnPjxwYXRoIGZpbGw9JyNBRDY0ZjUnIGZpbGwtcnVsZT0nZXZlbm9kZCcgZD0nTTY0MCwtMjAwTDY2OC41LC0xODguNUw2ODAsLTE2MEw2NjguNSwtMTMxLjVMNjQwLC0xMjBMNjExLjUsLTEzMS41TDYwMCwtMTYwTDYxMS41LC0xODguNUw2NDAsLTIwMFpNNDgwLC0yMDBMNTA4LjUsLTE4OC41TDUyMCwtMTYwTDUwOC41LC0xMzEuNUw0ODAsLTEyMEw0NTEuNSwtMTMxLjVMNDQwLC0xNjBMNDUxLjUsLTE4OC41TDQ4MCwtMjAwWk03MjAsLTI4MEw3NDguNSwtMjY4LjVMNzYwLC0yNDBMNzQ4LjUsLTIxMS41TDcyMCwtMjAwTDY5MS41LC0yMTEuNUw2ODAsLTI0MEw2OTEuNSwtMjY4LjVMNzIwLC0yODBaTTU2MCwtMjgwTDU4OC41LC0yNjguNUw2MDAsLTI0MEw1ODguNSwtMjExLjVMNTYwLC0yMDBMNTMxLjUsLTIxMS41TDUyMCwtMjQwTDUzMS41LC0yNjguNUw1NjAsLTI4MFpNNjQwLC0zNjBMNjY4LjUsLTM0OC41TDY4MCwtMzIwTDY2OC41LC0yOTEuNUw2NDAsLTI4MEw2MTEuNSwtMjkxLjVMNjAwLC0zMjBMNjExLjUsLTM0OC41TDY0MCwtMzYwWk00ODAsLTM2MEw1MDguNSwtMzQ4LjVMNTIwLC0zMjBMNTA4LjUsLTI5MS41TDQ4MCwtMjgwTDQ1MS41LC0yOTEuNUw0NDAsLTMyMEw0NTEuNSwtMzQ4LjVMNDgwLC0zNjBaTTg0MCwtNDIwTDg1NCwtNDE0TDg2MCwtNDAwTDg1NCwtMzg2TDg0MCwtMzgwTDgyNiwtMzg2TDgyMCwtNDAwTDgyNiwtNDE0TDg0MCwtNDIwWk03MjAsLTQ0MEw3NDguNSwtNDI4LjVMNzYwLC00MDBMNzQ4LjUsLTM3MS41TDcyMCwtMzYwTDY5MS41LC0zNzEuNUw2ODAsLTQwMEw2OTEuNSwtNDI4LjVMNzIwLC00NDBaTTU2MCwtNDQwTDU4OC41LC00MjguNUw2MDAsLTQwMEw1ODguNSwtMzcxLjVMNTYwLC0zNjBMNTMxLjUsLTM3MS41TDUyMCwtNDAwTDUzMS41LC00MjguNUw1NjAsLTQ0MFpNNjQwLC01MjBMNjY4LjUsLTUwOC41TDY4MCwtNDgwTDY2OC41LC00NTEuNUw2NDAsLTQ0MEw2MTEuNSwtNDUxLjVMNjAwLC00ODBMNjExLjUsLTUwOC41TDY0MCwtNTIwWk00ODAsLTUyMEw1MDguNSwtNTA4LjVMNTIwLC00ODBMNTA4LjUsLTQ1MS41TDQ4MCwtNDQwTDQ1MS41LC00NTEuNUw0NDAsLTQ4MEw0NTEuNSwtNTA4LjVMNDgwLC01MjBaTTg0MCwtNTgwTDg1NCwtNTc0TDg2MCwtNTYwTDg1NCwtNTQ2TDg0MCwtNTQwTDgyNiwtNTQ2TDgyMCwtNTYwTDgyNiwtNTc0TDg0MCwtNTgwWk03MjAsLTYwMEw3NDguNSwtNTg4LjVMNzYwLC01NjBMNzQ4LjUsLTUzMS41TDcyMCwtNTIwTDY5MS41LC01MzEuNUw2ODAsLTU2MEw2OTEuNSwtNTg4LjVMNzIwLC02MDBaTTU2MCwtNjAwTDU4OC41LC01ODguNUw2MDAsLTU2MEw1ODguNSwtNTMxLjVMNTYwLC01MjBMNTMxLjUsLTUzMS41TDUyMCwtNTYwTDUzMS41LC01ODguNUw1NjAsLTYwMFpNNjQwLC02ODBMNjY4LjUsLTY2OC41TDY4MCwtNjQwTDY2OC41LC02MTEuNUw2NDAsLTYwMEw2MTEuNSwtNjExLjVMNjAwLC02NDBMNjExLjUsLTY2OC41TDY0MCwtNjgwWk00ODAsLTY4MEw1MDguNSwtNjY4LjVMNTIwLC02NDBMNTA4LjUsLTYxMS41TDQ4MCwtNjAwTDQ1MS41LC02MTEuNUw0NDAsLTY0MEw0NTEuNSwtNjY4LjVMNDgwLC02ODBaTTcyMCwtNzYwTDc0OC41LC03NDguNUw3NjAsLTcyMEw3NDguNSwtNjkxLjVMNzIwLC02ODBMNjkxLjUsLTY5MS41TDY4MCwtNzIwTDY5MS41LC03NDguNUw3MjAsLTc2MFpNNTYwLC03NjBMNTg4LjUsLTc0OC41TDYwMCwtNzIwTDU4OC41LC02OTEuNUw1NjAsLTY4MEw1MzEuNSwtNjkxLjVMNTIwLC03MjBMNTMxLjUsLTc0OC41TDU2MCwtNzYwWk02NDAsLTg0MEw2NjguNSwtODI4LjVMNjgwLC04MDBMNjY4LjUsLTc3MS41TDY0MCwtNzYwTDYxMS41LC03NzEuNUw2MDAsLTgwMEw2MTEuNSwtODI4LjVMNjQwLC04NDBaTTQ4MCwtODQwTDUwOC41LC04MjguNUw1MjAsLTgwMEw1MDguNSwtNzcxLjVMNDgwLC03NjBMNDUxLjUsLTc3MS41TDQ0MCwtODAwTDQ1MS41LC04MjguNUw0ODAsLTg0MFpNMjAwLC04NDBMNDAwLC04NDBMNDAwLC0xMjBMMjAwLC0xMjBMMTY5LjM3NSwtMTI1Ljg3NUwxNDMuNSwtMTQzLjVMMTI1Ljg3NSwtMTY5LjM3NUwxMjAsLTIwMEwxMjAsLTc2MEwxMjUuODc1LC03OTAuNjI1TDE0My41LC04MTYuNUwxNjkuMzc1LC04MzQuMTI1TDIwMCwtODQwWicvPjwvc3ZnPg==) **Blur**                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 | Redact sensitive information |
| ![](https://img.shields.io/badge/-white?style=plastic&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnIHZpZXdCb3g9JzEwMCAtODYwIDc2MCA3NjAnPjxwYXRoIGZpbGw9JyNBRDY0ZjUnIGZpbGwtcnVsZT0nZXZlbm9kZCcgZD0nTTU2MCwtMTQwTDU3NCwtMTM0TDU4MCwtMTIwTDU3NCwtMTA2TDU2MCwtMTAwTDU0NiwtMTA2TDU0MCwtMTIwTDU0NiwtMTM0TDU2MCwtMTQwWk00MDAsLTE0MEw0MTQsLTEzNEw0MjAsLTEyMEw0MTQsLTEwNkw0MDAsLTEwMEwzODYsLTEwNkwzODAsLTEyMEwzODYsLTEzNEw0MDAsLTE0MFpNNzIwLC0yODBMNzQ4LjUsLTI2OC41TDc2MCwtMjQwTDc0OC41LC0yMTEuNUw3MjAsLTIwMEw2OTEuNSwtMjExLjVMNjgwLC0yNDBMNjkxLjUsLTI2OC41TDcyMCwtMjgwWk01NjAsLTI4MEw1ODguNSwtMjY4LjVMNjAwLC0yNDBMNTg4LjUsLTIxMS41TDU2MCwtMjAwTDUzMS41LC0yMTEuNUw1MjAsLTI0MEw1MzEuNSwtMjY4LjVMNTYwLC0yODBaTTQwMCwtMjgwTDQyOC41LC0yNjguNUw0NDAsLTI0MEw0MjguNSwtMjExLjVMNDAwLC0yMDBMMzcxLjUsLTIxMS41TDM2MCwtMjQwTDM3MS41LC0yNjguNUw0MDAsLTI4MFpNMjQwLC0yODBMMjY4LjUsLTI2OC41TDI4MCwtMjQwTDI2OC41LC0yMTEuNUwyNDAsLTIwMEwyMTEuNSwtMjExLjVMMjAwLC0yNDBMMjExLjUsLTI2OC41TDI0MCwtMjgwWk04NDAsLTQyMEw4NTQsLTQxNEw4NjAsLTQwMEw4NTQsLTM4Nkw4NDAsLTM4MEw4MjYsLTM4Nkw4MjAsLTQwMEw4MjYsLTQxNEw4NDAsLTQyMFpNMTIwLC00MjBMMTM0LC00MTRMMTQwLC00MDBMMTM0LC0zODZMMTIwLC0zODBMMTA2LC0zODZMMTAwLC00MDBMMTA2LC00MTRMMTIwLC00MjBaTTcyMCwtNDQwTDc0OC41LC00MjguNUw3NjAsLTQwMEw3NDguNSwtMzcxLjVMNzIwLC0zNjBMNjkxLjUsLTM3MS41TDY4MCwtNDAwTDY5MS41LC00MjguNUw3MjAsLTQ0MFpNMjQwLC00NDBMMjY4LjUsLTQyOC41TDI4MCwtNDAwTDI2OC41LC0zNzEuNUwyNDAsLTM2MEwyMTEuNSwtMzcxLjVMMjAwLC00MDBMMjExLjUsLTQyOC41TDI0MCwtNDQwWk01NjAsLTQ2MEw1ODMuMTI1LC00NTUuNjI1TDYwMi41LC00NDIuNUw2MTUuNjI1LC00MjMuMTI1TDYyMCwtNDAwTDYxNS42MjUsLTM3Ni44NzVMNjAyLjUsLTM1Ny41TDU4My4xMjUsLTM0NC4zNzVMNTYwLC0zNDBMNTM2Ljg3NSwtMzQ0LjM3NUw1MTcuNSwtMzU3LjVMNTA0LjM3NSwtMzc2Ljg3NUw1MDAsLTQwMEw1MDQuMzc1LC00MjMuMTI1TDUxNy41LC00NDIuNUw1MzYuODc1LC00NTUuNjI1TDU2MCwtNDYwWk00MDAsLTQ2MEw0MjMuMTI1LC00NTUuNjI1TDQ0Mi41LC00NDIuNUw0NTUuNjI1LC00MjMuMTI1TDQ2MCwtNDAwTDQ1NS42MjUsLTM3Ni44NzVMNDQyLjUsLTM1Ny41TDQyMy4xMjUsLTM0NC4zNzVMNDAwLC0zNDBMMzc2Ljg3NSwtMzQ0LjM3NUwzNTcuNSwtMzU3LjVMMzQ0LjM3NSwtMzc2Ljg3NUwzNDAsLTQwMEwzNDQuMzc1LC00MjMuMTI1TDM1Ny41LC00NDIuNUwzNzYuODc1LC00NTUuNjI1TDQwMCwtNDYwWk04NDAsLTU4MEw4NTQsLTU3NEw4NjAsLTU2MEw4NTQsLTU0Nkw4NDAsLTU0MEw4MjYsLTU0Nkw4MjAsLTU2MEw4MjYsLTU3NEw4NDAsLTU4MFpNMTIwLC01ODBMMTM0LC01NzRMMTQwLC01NjBMMTM0LC01NDZMMTIwLC01NDBMMTA2LC01NDZMMTAwLC01NjBMMTA2LC01NzRMMTIwLC01ODBaTTcyMCwtNjAwTDc0OC41LC01ODguNUw3NjAsLTU2MEw3NDguNSwtNTMxLjVMNzIwLC01MjBMNjkxLjUsLTUzMS41TDY4MCwtNTYwTDY5MS41LC01ODguNUw3MjAsLTYwMFpNMjQwLC02MDBMMjY4LjUsLTU4OC41TDI4MCwtNTYwTDI2OC41LC01MzEuNUwyNDAsLTUyMEwyMTEuNSwtNTMxLjVMMjAwLC01NjBMMjExLjUsLTU4OC41TDI0MCwtNjAwWk01NjAsLTYyMEw1ODMuMTI1LC02MTUuNjI1TDYwMi41LC02MDIuNUw2MTUuNjI1LC01ODMuMTI1TDYyMCwtNTYwTDYxNS42MjUsLTUzNi44NzVMNjAyLjUsLTUxNy41TDU4My4xMjUsLTUwNC4zNzVMNTYwLC01MDBMNTM2Ljg3NSwtNTA0LjM3NUw1MTcuNSwtNTE3LjVMNTA0LjM3NSwtNTM2Ljg3NUw1MDAsLTU2MEw1MDQuMzc1LC01ODMuMTI1TDUxNy41LC02MDIuNUw1MzYuODc1LC02MTUuNjI1TDU2MCwtNjIwWk00MDAsLTYyMEw0MjMuMTI1LC02MTUuNjI1TDQ0Mi41LC02MDIuNUw0NTUuNjI1LC01ODMuMTI1TDQ2MCwtNTYwTDQ1NS42MjUsLTUzNi44NzVMNDQyLjUsLTUxNy41TDQyMy4xMjUsLTUwNC4zNzVMNDAwLC01MDBMMzc2Ljg3NSwtNTA0LjM3NUwzNTcuNSwtNTE3LjVMMzQ0LjM3NSwtNTM2Ljg3NUwzNDAsLTU2MEwzNDQuMzc1LC01ODMuMTI1TDM1Ny41LC02MDIuNUwzNzYuODc1LC02MTUuNjI1TDQwMCwtNjIwWk03MjAsLTc2MEw3NDguNSwtNzQ4LjVMNzYwLC03MjBMNzQ4LjUsLTY5MS41TDcyMCwtNjgwTDY5MS41LC02OTEuNUw2ODAsLTcyMEw2OTEuNSwtNzQ4LjVMNzIwLC03NjBaTTU2MCwtNzYwTDU4OC41LC03NDguNUw2MDAsLTcyMEw1ODguNSwtNjkxLjVMNTYwLC02ODBMNTMxLjUsLTY5MS41TDUyMCwtNzIwTDUzMS41LC03NDguNUw1NjAsLTc2MFpNNDAwLC03NjBMNDI4LjUsLTc0OC41TDQ0MCwtNzIwTDQyOC41LC02OTEuNUw0MDAsLTY4MEwzNzEuNSwtNjkxLjVMMzYwLC03MjBMMzcxLjUsLTc0OC41TDQwMCwtNzYwWk0yNDAsLTc2MEwyNjguNSwtNzQ4LjVMMjgwLC03MjBMMjY4LjUsLTY5MS41TDI0MCwtNjgwTDIxMS41LC02OTEuNUwyMDAsLTcyMEwyMTEuNSwtNzQ4LjVMMjQwLC03NjBaTTU2MCwtODYwTDU3NCwtODU0TDU4MCwtODQwTDU3NCwtODI2TDU2MCwtODIwTDU0NiwtODI2TDU0MCwtODQwTDU0NiwtODU0TDU2MCwtODYwWk00MDAsLTg2MEw0MTQsLTg1NEw0MjAsLTg0MEw0MTQsLTgyNkw0MDAsLTgyMEwzODYsLTgyNkwzODAsLTg0MEwzODYsLTg1NEw0MDAsLTg2MFonLz48L3N2Zz4=) **Pixelate** | Pixelate areas for privacy |

- **Undo/Redo** — Full history support
- **Color Palette** — Quick color selection with custom picker
- **Thickness Control** — Adjustable stroke width

### 🔍 OCR (Optical Character Recognition)
- **EasyOCR** — Python-based with 80+ language support
- **Tesseract** — Lightweight native engine with language packs
- **100% Local** — No cloud services, complete privacy
- **One-click install** — Managed directly from Settings

### 🎨 Customization
- **3 Themes** — Dark, Light, and Sunset
- **73+ Languages** — Extensive localization support
- **Custom Hotkeys** — Configure keyboard shortcuts
- **Export Options** — PNG, JPEG with quality control
- **Filename Patterns** — Customizable naming templates
- **Auto-save Folder** — Set your preferred destination

### 🖥️ Cross-Platform

| Edition | Framework | Platforms |
|---------|-----------|-----------|
| **Shotora.App** | Avalonia UI 11.3 | Windows, Linux, macOS |

---

## 📸 Screenshots

### Theme Showcase

<div align="center">

| Dark Theme | Light Theme | Sunset Theme |
|:----------:|:-----------:|:------------:|
| <img src="build/screenshots/Settings_Main_Dark.png" width="280" alt="Dark Theme"/> | <img src="build/screenshots/Settings_Main_Light.png" width="280" alt="Light Theme"/> | <img src="build/screenshots/Settings_Main_Sunset.png" width="280" alt="Sunset Theme"/> |

</div>

### Editor Settings

<div align="center">

|                                         Dark                                          | Light | Sunset |
|:-------------------------------------------------------------------------------------:|:-----:|:------:|
| <img src="build/screenshots/Settings_Editor_Dark.png" width="280" alt="Editor Dark"/> | <img src="build/screenshots/Settings_Editor_Light.png" width="280" alt="Editor Light"/> | <img src="build/screenshots/Settings_Editor_Sunset.png" width="280" alt="Editor Sunset"/> |

</div>

### Hotkey Configuration

<div align="center">

| Dark | Light | Sunset |
|:----:|:-----:|:------:|
| <img src="build/screenshots/Settings_Hotkeys_Dark.png" width="280" alt="Hotkeys Dark"/> | <img src="build/screenshots/Settings_Hotkeys_Light.png" width="280" alt="Hotkeys Light"/> | <img src="build/screenshots/Settings_Hotkeys_Sunset.png" width="280" alt="Hotkeys Sunset"/> |

</div>

### UI Elements

<div align="center">

| Annotation Panel | Color Palette |
|:----------------:|:-------------:|
| <img src="build/screenshots/panel.png" width="400" alt="Panel"/> | <img src="build/screenshots/color_pallete.png" width="300" alt="Color Palette"/> |

</div>

---

## 📦 Installation

### Prerequisites

- **.NET 10 SDK** — [Download](https://dotnet.microsoft.com/download/dotnet/10.0)
- **Python 3.11+** — Required only for EasyOCR (optional)

### Build from Source

```bash
# Clone the repository
git clone https://github.com/YourUsername/Shotora.git
cd Shotora

# Build the solution
dotnet build Shotora.sln

# Run Avalonia version (Cross-platform)
dotnet run --project Shotora.App/Shotora.App.csproj
```

### Build Profiles

The following build profiles are available for different platforms and configurations:

| Platform | Architecture | Action | Command |
|:--------:|:------------:|:------:|:--------|
| <img src="https://img.shields.io/badge/Windows-0078D6?logo=windows&logoColor=white" alt="Windows"/> | x64 | Publish & Pack | `dotnet-cake --runtime=win-x64 --singlefile=true --selfcontained=true` |
| <img src="https://img.shields.io/badge/Windows-0078D6?logo=windows&logoColor=white" alt="Windows"/> | x86 | Publish & Pack | `dotnet-cake --runtime=win-x86 --singlefile=true --selfcontained=true` |
| <img src="https://img.shields.io/badge/Linux-FCC624?logo=linux&logoColor=black" alt="Linux"/> | x64 | Publish Only | `dotnet-cake --runtime=linux-x64 --singlefile=true --selfcontained=true` |
| <img src="https://img.shields.io/badge/Linux-FCC624?logo=linux&logoColor=black" alt="Linux"/> | arm64 | Publish Only | `dotnet-cake --runtime=linux-arm64 --singlefile=true --selfcontained=true` |
| <img src="https://img.shields.io/badge/macOS-000000?logo=apple&logoColor=white" alt="macOS"/> | x64 | Publish & Pack | `dotnet-cake --runtime=osx-x64 --singlefile=true --selfcontained=true` |

> 💡 **Tip**: Run `dotnet tool restore` first to install Cake and Velopack CLI tools.

<details>
<summary>📋 <strong>Detailed Build Commands</strong></summary>

#### Windows (x64) — Publish and Pack
```powershell
dotnet tool run dotnet-cake --runtime=win-x64 --singlefile=true --selfcontained=true --output=./ready/win-x64
./build/pack-from-cake.ps1 -PackDir ./ready/win-x64 -Runtime win-x64 -Channel stable
```

#### Windows (x86) — Publish and Pack
```powershell
dotnet tool run dotnet-cake --runtime=win-x86 --singlefile=true --selfcontained=true --output=./ready/win-x86
./build/pack-from-cake.ps1 -PackDir ./ready/win-x86 -Runtime win-x86 -Channel stable
```

#### Linux (x64) — Publish Only
```bash
dotnet tool run dotnet-cake --runtime=linux-x64 --singlefile=true --selfcontained=true --output=./ready/linux-x64
```

#### Linux (arm64) — Publish Only
```bash
dotnet tool run dotnet-cake --runtime=linux-arm64 --singlefile=true --selfcontained=true --output=./ready/linux-arm64
```

#### macOS (x64) — Publish & Pack
```bash
dotnet tool run dotnet-cake --runtime=osx-x64 --singlefile=true --selfcontained=true --output=./ready/osx-x64
./build/pack-from-cake.ps1 -PackDir ./ready/osx-x64 -Runtime osx-x64 -Channel stable
```

</details>

### Publish (Self-Contained)

```bash
# Windows
dotnet publish Shotora.App/Shotora.App.csproj -c Release -r win-x64

# Linux
dotnet publish Shotora.App/Shotora.App.csproj -c Release -r linux-x64

# macOS
dotnet publish Shotora.App/Shotora.App.csproj -c Release -r osx-x64
```

### Velopack Setup (Windows)

- Install the pinned Velopack CLI: `dotnet tool restore` (uses `.config/dotnet-tools.json`; sets `DOTNET_ROLL_FORWARD=LatestMajor` automatically when packing).
- Build a branded Setup.exe with the Shotora splash (`build/Installer/splash.png`) and icon (`Shotora.App/Assets/Shotora.ico`):
  - `pwsh ./build/pack-setup.ps1 -Runtime win-x64 -Channel win -Version 1.0.0`
  - The script publishes `Shotora.App`, then runs `vpk pack` with the branded assets.
- Output artifacts land in `bin/velopack/<rid>/` (including `Setup.exe` and the release/portable packages).

---

## 🚀 Usage

### Quick Start

1. **Launch** — Shotora starts minimized in the system tray
2. **Capture** — Use hotkey or right-click tray icon
3. **Select** — Draw a region on screen
4. **Annotate** — Use the toolbar to mark up your capture
5. **Export** — Copy to clipboard, save to file, or extract text with OCR

### Default Hotkeys

| Action | Shortcut |
|--------|----------|
| Region Capture | `Ctrl + Shift + PrintScreen` |
| Fullscreen Capture | `Alt + PrintScreen` |
| Active Window | `Shift + Win + S` |

> 💡 All hotkeys are configurable in Settings

### Editor Controls

| Key | Action |
|-----|--------|
| `Escape` | Deselect tool / Exit editor |
| `Ctrl + Z` | Undo |
| `Ctrl + Y` | Redo |
| `Ctrl + C` | Copy selection |
| `Ctrl + S` | Save selection |

---

## 🔍 OCR Engines

### EasyOCR
- **Languages**: 80+ including CJK, Arabic, Cyrillic
- **Installation**: Managed from Settings (requires Python)
- **Best for**: Multi-language documents, complex scripts

### Tesseract
- **Languages**: English bundled, others downloadable
- **Installation**: Built-in, no external dependencies
- **Best for**: Quick text extraction, lightweight usage

Both engines run **completely offline** — your screenshots never leave your device.

---

## 🏗️ Project Structure

```
Shotora/
├── Shotora.App/                 # Avalonia Cross-Platform Application
│   ├── Views/                   # Windows and dialogs (MainWindow, OverlayWindow, SettingsWindow, etc.)
│   ├── Controls/                # Custom UI controls
│   ├── Services/                # Platform services
│   ├── Extensions/              # Extension methods
│   ├── Styles/                  # Avalonia styles and themes
│   ├── Assets/                  # Application assets (icons, installer resources, screenshots)
│   ├── Content/                 # Fonts and content files
│   ├── Resources/               # Resource files
│   └── Properties/              # Launch settings and project properties
│
├── Shotora.App.Interfaces/      # Service contracts and abstractions
│   ├── Abstractions/            # Base interfaces and abstractions
│   ├── Adapters/                # Adapter interfaces
│   ├── Builders/                # Builder pattern interfaces
│   ├── DrawingServices/         # Drawing service interfaces
│   ├── Ocr/                     # OCR service interfaces
│   ├── PeripheralServices/      # Peripheral service interfaces
│   ├── Providers/               # Provider interfaces
│   ├── System/                  # System-level interfaces
│   └── ViewModels/              # ViewModel interfaces
│
├── Shotora.App.Models/          # Domain models and DTOs
│   ├── ViewModels/              # MVVM view models
│   ├── Enums/                   # Enumeration types
│   ├── Constants/               # Constant definitions
│   ├── Drawings/                # Drawing models
│   ├── ItemModels/              # Item-specific models
│   ├── AtomModels/              # Atomic models
│   ├── Localization/            # Localization models
│   ├── SealedModels/            # Sealed/immutable models
│   ├── System/                  # System models
│   ├── Extensions/              # Model extensions
│   └── Utilities/               # Model utilities
│
├── Shotora.App.Services/        # Business logic and services
│   ├── Abstractions/            # Service abstractions
│   ├── Adapters/                # Service adapters
│   ├── Builders/                # Builder implementations
│   ├── DrawingServices/         # Drawing service implementations
│   ├── Ocr/                     # OCR service implementations
│   ├── PeripheralServices/      # Peripheral services
│   ├── Providers/               # Provider implementations
│   ├── PythonEmbed/             # Python embedding for EasyOCR
│   ├── System/                  # System-level services
│   └── ViewModels/              # Service view models
│
├── Shotora.App.Tests/           # Unit and integration tests
│   ├── Abstractions/            # Tests for service abstractions
│   ├── Builders/                # Tests for builder implementations
│   ├── DrawingServices/         # Tests for drawing services
│   ├── PeripheralServices/      # Tests for peripheral services
│   ├── Providers/               # Tests for providers
│   ├── System/                  # Tests for system services
│   └── ViewModels/              # Tests for view models
│
├── NativeSupport/               # Native platform bindings
│   ├── Adapters/                # Native adapters
│   ├── Binaries/                # Native binaries (Linux, Mac, Windows)
│   ├── Constants/               # Native constants
│   ├── Enums/                   # Native enums
│   ├── Extensions/              # Native extensions
│   └── Services/                # Native services
│
├── Shared.Interfaces/           # Shared interface contracts
│   ├── Adapters/                # Shared adapter interfaces
│   └── Facades/                 # Shared facade interfaces
│
├── Shared.Models/               # Shared domain models
│   ├── Constants/               # Shared constants
│   └── Enums/                   # Shared enums
│
├── Shared.Services/             # Shared service implementations
│   ├── Adapters/                # Shared adapters
│   └── Facades/                 # Shared facades
│
├── build/                       # Build scripts and configurations
│   ├── pack-from-cake.ps1       # Velopack packaging script
│   ├── pack-setup.ps1           # Setup.exe builder
│   ├── pack-macos.ps1           # macOS packaging script
│   └── fix-macos-dylibs.ps1     # macOS dylib fixer
│
└── Releases/                    # Release artifacts and checksums
    └── checksums.txt            # SHA-256 checksums for release files
```

---

## 🛠️ Development

### Running Tests

```bash
dotnet test Shotora.sln
```

---

## 🔐 Release Checksums

To verify the integrity of downloaded release files, compare their SHA-256 checksums against the official values:

| File | SHA-256 Checksum |
|------|------------------|
| `Shotora_Installer_Mac_x64.pkg` | `9AE9FD005B1149EF816715E1042C23FCF9376C5302501615531C2570F61ACB9C` |
| `Shotora_Installer_Windows_x64.exe` | `B8A098E5EDC64DB26814D9ECE649702FC72200BC5C590DEA45AA4A5414F2E635` |
| `Shotora_Installer_Windows_x86.exe` | `F6E10682367358BA054D5AF45BE25D2C14695A655886A9F01DEA96ECA0862157` |
| `Shotora_Portable_Linux_arm64.App` | `3DEE5D4871A180E09F6F8AD1596E0FE2272AA0DC47811FDA7D9797D10089BE23` |
| `Shotora_Portable_Linux_x64.App` | `24F02ECC1DE1A48D8176E8CDD6CB988A8B13CBA167AECB9EBB648B83AAD807B5` |
| `Shotora_Portable_Windows_x64.exe` | `7C2B010B6F365756B68662E25CDA3EB5BD80C9F23169C8228339C67DE7900A5F` |
| `Shotora_Portable_Windows_x86.exe` | `1EB0C9B3AD054BE369EFB088E66E3765EF33ED5F422EBE53C90CA6DD5738414D` |

### Verifying Checksums

**Windows (PowerShell):**
```powershell
Get-FileHash -Algorithm SHA256 Shotora_Installer_Windows_x64.exe
```

**Linux/macOS:**
```bash
sha256sum Shotora_Portable_Linux_x64.App
# or on macOS:
shasum -a 256 Shotora_Installer_Mac_x64.pkg
```

> 💡 **Tip**: All checksums are also available in [`Releases/checksums.txt`](Releases/checksums.txt)

---

# Detailed Coverage Report

> **Generated:** 2026-01-24 22:06:06  
> **Tool:** DotCover 2025.3.1  
> **Report Type:** Detailed File-Level Analysis  
> **Minimum Threshold:** 80%

## Overall Summary

| Metric | Value |
|--------|-------|
| **Total Files** | 52 |
| **Total Statements** | 5,605 |
| **Covered Statements** | 4,703 |
| **Coverage** | ![84%](https://img.shields.io/badge/84%25-green) |

### Coverage Distribution

| Category | Files | Percentage |
|----------|-------|------------|
| **Perfect** (100%) | 19 | 36.5% |
| **High** (80-99%) | 11 | 21.2% |
| **Medium** (60-79%) | 0 | 0% |
| **Low** (1-59%) |  | 0% |
| **No Coverage** (0%) | 21 | 40.4% |

---


<details>
<summary>

### ![Project](https://img.shields.io/badge/Project-blue?style=for-the-badge&logo=nuget) **NativeSupport** ![2%](https://img.shields.io/badge/2%25-red)

> ![Warning](https://img.shields.io/badge/WARNING-Coverage_below_threshold-red?style=flat-square) | **2** / 94 statements covered

</summary>


<details>
<summary>

#### ![Folder](https://img.shields.io/badge/Folder-gray?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZmlsbD0iI2ZmZmZmZiIgZD0iTTMgNiBDMyA0LjkgMy45IDQgNSA0IEgxMCBMMTIgNiBIMTkgQzIwLjEgNiAyMSA2LjkgMjEgOCBWMTggQzIxIDE5LjEgMjAuMSAyMCAxOSAyMCBINSBDMy45IDIwIDMgMTkuMSAzIDE4IFoiLz48L3N2Zz4=) **Extensions** ![100%](https://img.shields.io/badge/100%25-brightgreen)

> **2** / 2 statements covered

</summary>

</details>


<details>
<summary>

#### ![Folder](https://img.shields.io/badge/Folder-gray?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZmlsbD0iI2ZmZmZmZiIgZD0iTTMgNiBDMyA0LjkgMy45IDQgNSA0IEgxMCBMMTIgNiBIMTkgQzIwLjEgNiAyMSA2LjkgMjEgOCBWMTggQzIxIDE5LjEgMjAuMSAyMCAxOSAyMCBINSBDMy45IDIwIDMgMTkuMSAzIDE4IFoiLz48L3N2Zz4=) **Services** ![0%](https://img.shields.io/badge/0%25-lightgrey)

> ![Warning](https://img.shields.io/badge/WARNING-Coverage_below_threshold-red?style=flat-square) | **0** / 92 statements covered

</summary>

</details>

</details>


<details>
<summary>

### ![Project](https://img.shields.io/badge/Project-blue?style=for-the-badge&logo=nuget) **Shared.Models** ![100%](https://img.shields.io/badge/100%25-brightgreen)

> **495** / 495 statements covered

</summary>


<details>
<summary>

#### ![Folder](https://img.shields.io/badge/Folder-gray?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZmlsbD0iI2ZmZmZmZiIgZD0iTTMgNiBDMyA0LjkgMy45IDQgNSA0IEgxMCBMMTIgNiBIMTkgQzIwLjEgNiAyMSA2LjkgMjEgOCBWMTggQzIxIDE5LjEgMjAuMSAyMCAxOSAyMCBINSBDMy45IDIwIDMgMTkuMSAzIDE4IFoiLz48L3N2Zz4=) **Constants** ![100%](https://img.shields.io/badge/100%25-brightgreen)

> **495** / 495 statements covered

</summary>

| File | Coverage | Statements | Progress |
|------|----------|------------|----------|
|  `LanguageCollection.cs` | ![100%](https://img.shields.io/badge/100%25-brightgreen) | 242 / 242 | `[###############]` 100% |
|  `Links.cs` | ![100%](https://img.shields.io/badge/100%25-brightgreen) | 253 / 253 | `[###############]` 100% |

</details>

</details>


<details>
<summary>

### ![Project](https://img.shields.io/badge/Project-blue?style=for-the-badge&logo=nuget) **Shotora.App.Services** ![57%](https://img.shields.io/badge/57%25-orange)

> ![Warning](https://img.shields.io/badge/WARNING-Coverage_below_threshold-red?style=flat-square) | **1,064** / 1,857 statements covered

</summary>


<details>
<summary>

#### ![Folder](https://img.shields.io/badge/Folder-gray?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZmlsbD0iI2ZmZmZmZiIgZD0iTTMgNiBDMyA0LjkgMy45IDQgNSA0IEgxMCBMMTIgNiBIMTkgQzIwLjEgNiAyMSA2LjkgMjEgOCBWMTggQzIxIDE5LjEgMjAuMSAyMCAxOSAyMCBINSBDMy45IDIwIDMgMTkuMSAzIDE4IFoiLz48L3N2Zz4=) **Abstractions** ![23%](https://img.shields.io/badge/23%25-red)

> ![Warning](https://img.shields.io/badge/WARNING-Coverage_below_threshold-red?style=flat-square) | **53** / 231 statements covered

</summary>

</details>


<details>
<summary>

#### ![Folder](https://img.shields.io/badge/Folder-gray?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZmlsbD0iI2ZmZmZmZiIgZD0iTTMgNiBDMyA0LjkgMy45IDQgNSA0IEgxMCBMMTIgNiBIMTkgQzIwLjEgNiAyMSA2LjkgMjEgOCBWMTggQzIxIDE5LjEgMjAuMSAyMCAxOSAyMCBINSBDMy45IDIwIDMgMTkuMSAzIDE4IFoiLz48L3N2Zz4=) **Adapters** ![0%](https://img.shields.io/badge/0%25-lightgrey)

> ![Warning](https://img.shields.io/badge/WARNING-Coverage_below_threshold-red?style=flat-square) | **0** / 37 statements covered

</summary>

| File | Coverage | Statements | Progress |
|------|----------|------------|----------|
|  `AvaloniaEnumAdapter.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) **!** | 0 / 9 | `[---------------]` 0% |
|  `ColorAdapter.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) **!** | 0 / 14 | `[---------------]` 0% |
|  `SkiaImageAdapter.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) **!** | 0 / 14 | `[---------------]` 0% |

</details>


<details>
<summary>

#### ![Folder](https://img.shields.io/badge/Folder-gray?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZmlsbD0iI2ZmZmZmZiIgZD0iTTMgNiBDMyA0LjkgMy45IDQgNSA0IEgxMCBMMTIgNiBIMTkgQzIwLjEgNiAyMSA2LjkgMjEgOCBWMTggQzIxIDE5LjEgMjAuMSAyMCAxOSAyMCBINSBDMy45IDIwIDMgMTkuMSAzIDE4IFoiLz48L3N2Zz4=) **Builders** ![52%](https://img.shields.io/badge/52%25-orange)

> ![Warning](https://img.shields.io/badge/WARNING-Coverage_below_threshold-red?style=flat-square) | **12** / 23 statements covered

</summary>

| File | Coverage | Statements | Progress |
|------|----------|------------|----------|
|  `AnnotationItemBuilder.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) **!** | 0 / 10 | `[---------------]` 0% |
|  `FilePickerBuilder.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) **!** | 0 / 1 | `[---------------]` 0% |
|  `PathBuilder.cs` | ![100%](https://img.shields.io/badge/100%25-brightgreen) | 12 / 12 | `[###############]` 100% |

</details>


<details>
<summary>

#### ![Folder](https://img.shields.io/badge/Folder-gray?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZmlsbD0iI2ZmZmZmZiIgZD0iTTMgNiBDMyA0LjkgMy45IDQgNSA0IEgxMCBMMTIgNiBIMTkgQzIwLjEgNiAyMSA2LjkgMjEgOCBWMTggQzIxIDE5LjEgMjAuMSAyMCAxOSAyMCBINSBDMy45IDIwIDMgMTkuMSAzIDE4IFoiLz48L3N2Zz4=) **DrawingServices** ![83%](https://img.shields.io/badge/83%25-green)

> **495** / 595 statements covered

</summary>

| File | Coverage | Statements | Progress |
|------|----------|------------|----------|
|  `AnnotationDrawingService.cs` | ![90%](https://img.shields.io/badge/90%25-brightgreen) | 114 / 126 | `[#############--]` 90% |
|  `ArrowDrawingService.cs` | ![100%](https://img.shields.io/badge/100%25-brightgreen) | 77 / 77 | `[###############]` 100% |
|  `EllipseDrawingService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) **!** | 0 / 29 | `[---------------]` 0% |
|  `GridDrawingService.cs` | ![100%](https://img.shields.io/badge/100%25-brightgreen) | 42 / 42 | `[###############]` 100% |
|  `LineDrawingService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) **!** | 0 / 11 | `[---------------]` 0% |
|  `PolylineDrawingService.cs` | ![100%](https://img.shields.io/badge/100%25-brightgreen) | 28 / 28 | `[###############]` 100% |
|  `RectangleDrawingService.cs` | ![89%](https://img.shields.io/badge/89%25-green) | 100 / 112 | `[#############--]` 89% |
|  `SaveDrawingService.cs` | ![100%](https://img.shields.io/badge/100%25-brightgreen) | 1 / 1 | `[###############]` 100% |
|  `SkiaDrawingService.cs` | ![94%](https://img.shields.io/badge/94%25-brightgreen) | 74 / 79 | `[##############-]` 94% |
|  `TextDrawingService.cs` | ![100%](https://img.shields.io/badge/100%25-brightgreen) | 59 / 59 | `[###############]` 100% |
|  `TranslationDrawingService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) **!** | 0 / 31 | `[---------------]` 0% |

</details>


<details>
<summary>

#### ![Folder](https://img.shields.io/badge/Folder-gray?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZmlsbD0iI2ZmZmZmZiIgZD0iTTMgNiBDMyA0LjkgMy45IDQgNSA0IEgxMCBMMTIgNiBIMTkgQzIwLjEgNiAyMSA2LjkgMjEgOCBWMTggQzIxIDE5LjEgMjAuMSAyMCAxOSAyMCBINSBDMy45IDIwIDMgMTkuMSAzIDE4IFoiLz48L3N2Zz4=) **Facades** ![0%](https://img.shields.io/badge/0%25-lightgrey)

> ![Warning](https://img.shields.io/badge/WARNING-Coverage_below_threshold-red?style=flat-square) | **0** / 2 statements covered

</summary>

</details>


<details>
<summary>

#### ![Folder](https://img.shields.io/badge/Folder-gray?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZmlsbD0iI2ZmZmZmZiIgZD0iTTMgNiBDMyA0LjkgMy45IDQgNSA0IEgxMCBMMTIgNiBIMTkgQzIwLjEgNiAyMSA2LjkgMjEgOCBWMTggQzIxIDE5LjEgMjAuMSAyMCAxOSAyMCBINSBDMy45IDIwIDMgMTkuMSAzIDE4IFoiLz48L3N2Zz4=) **Ocr** ![0%](https://img.shields.io/badge/0%25-lightgrey)

> ![Warning](https://img.shields.io/badge/WARNING-Coverage_below_threshold-red?style=flat-square) | **0** / 140 statements covered

</summary>


<details>
<summary>

#### ![Folder](https://img.shields.io/badge/Folder-gray?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZmlsbD0iI2ZmZmZmZiIgZD0iTTMgNiBDMyA0LjkgMy45IDQgNSA0IEgxMCBMMTIgNiBIMTkgQzIwLjEgNiAyMSA2LjkgMjEgOCBWMTggQzIxIDE5LjEgMjAuMSAyMCAxOSAyMCBINSBDMy45IDIwIDMgMTkuMSAzIDE4IFoiLz48L3N2Zz4=) **EasyOcr** ![0%](https://img.shields.io/badge/0%25-lightgrey)

> ![Warning](https://img.shields.io/badge/WARNING-Coverage_below_threshold-red?style=flat-square) | **0** / 18 statements covered

</summary>

| File | Coverage | Statements | Progress |
|------|----------|------------|----------|
|  `MaintenanceEasyOcrService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) **!** | 0 / 1 | `[---------------]` 0% |
|  `PackageDetectorEasyOcrService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) **!** | 0 / 17 | `[---------------]` 0% |

</details>


<details>
<summary>

#### ![Folder](https://img.shields.io/badge/Folder-gray?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZmlsbD0iI2ZmZmZmZiIgZD0iTTMgNiBDMyA0LjkgMy45IDQgNSA0IEgxMCBMMTIgNiBIMTkgQzIwLjEgNiAyMSA2LjkgMjEgOCBWMTggQzIxIDE5LjEgMjAuMSAyMCAxOSAyMCBINSBDMy45IDIwIDMgMTkuMSAzIDE4IFoiLz48L3N2Zz4=) **Tesseract** ![0%](https://img.shields.io/badge/0%25-lightgrey)

> ![Warning](https://img.shields.io/badge/WARNING-Coverage_below_threshold-red?style=flat-square) | **0** / 62 statements covered

</summary>

| File | Coverage | Statements | Progress |
|------|----------|------------|----------|
|  `LanguageTesseractService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) **!** | 0 / 39 | `[---------------]` 0% |
|  `TessdataTesseractService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) **!** | 0 / 23 | `[---------------]` 0% |

</details>

</details>


<details>
<summary>

#### ![Folder](https://img.shields.io/badge/Folder-gray?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZmlsbD0iI2ZmZmZmZiIgZD0iTTMgNiBDMyA0LjkgMy45IDQgNSA0IEgxMCBMMTIgNiBIMTkgQzIwLjEgNiAyMSA2LjkgMjEgOCBWMTggQzIxIDE5LjEgMjAuMSAyMCAxOSAyMCBINSBDMy45IDIwIDMgMTkuMSAzIDE4IFoiLz48L3N2Zz4=) **PeripheralServices** ![96%](https://img.shields.io/badge/96%25-brightgreen)

> **66** / 69 statements covered

</summary>

| File | Coverage | Statements | Progress |
|------|----------|------------|----------|
|  `KeyboardInteractionService.cs` | ![87%](https://img.shields.io/badge/87%25-green) | 20 / 23 | `[#############--]` 87% |
|  `MouseInteractionService.cs` | ![100%](https://img.shields.io/badge/100%25-brightgreen) | 46 / 46 | `[###############]` 100% |

</details>


<details>
<summary>

#### ![Folder](https://img.shields.io/badge/Folder-gray?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZmlsbD0iI2ZmZmZmZiIgZD0iTTMgNiBDMyA0LjkgMy45IDQgNSA0IEgxMCBMMTIgNiBIMTkgQzIwLjEgNiAyMSA2LjkgMjEgOCBWMTggQzIxIDE5LjEgMjAuMSAyMCAxOSAyMCBINSBDMy45IDIwIDMgMTkuMSAzIDE4IFoiLz48L3N2Zz4=) **Providers** ![83%](https://img.shields.io/badge/83%25-green)

> **244** / 293 statements covered

</summary>

| File | Coverage | Statements | Progress |
|------|----------|------------|----------|
|  `IconVisibilityProvider.cs` | ![100%](https://img.shields.io/badge/100%25-brightgreen) | 9 / 9 | `[###############]` 100% |
|  `LocalizationProvider.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) **!** | 0 / 34 | `[---------------]` 0% |
|  `PythonProvider.cs` | ![86%](https://img.shields.io/badge/86%25-green) | 67 / 78 | `[############---]` 86% |
|  `SettingsProvider.cs` | ![100%](https://img.shields.io/badge/100%25-brightgreen) | 51 / 51 | `[###############]` 100% |
|  `TrayMenuWindowService.cs` | ![97%](https://img.shields.io/badge/97%25-brightgreen) | 117 / 121 | `[##############-]` 97% |

</details>


<details>
<summary>

#### ![Folder](https://img.shields.io/badge/Folder-gray?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZmlsbD0iI2ZmZmZmZiIgZD0iTTMgNiBDMyA0LjkgMy45IDQgNSA0IEgxMCBMMTIgNiBIMTkgQzIwLjEgNiAyMSA2LjkgMjEgOCBWMTggQzIxIDE5LjEgMjAuMSAyMCAxOSAyMCBINSBDMy45IDIwIDMgMTkuMSAzIDE4IFoiLz48L3N2Zz4=) **System** ![17%](https://img.shields.io/badge/17%25-red)

> ![Warning](https://img.shields.io/badge/WARNING-Coverage_below_threshold-red?style=flat-square) | **51** / 293 statements covered

</summary>

| File | Coverage | Statements | Progress |
|------|----------|------------|----------|
|  `ClipboardService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) **!** | 0 / 2 | `[---------------]` 0% |
|  `HotkeyService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) **!** | 0 / 54 | `[---------------]` 0% |
|  `ScreenshotService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) **!** | 0 / 173 | `[---------------]` 0% |
|  `SettingsSystemService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) **!** | 0 / 8 | `[---------------]` 0% |
|  `StartupService.cs` | ![91%](https://img.shields.io/badge/91%25-brightgreen) | 51 / 56 | `[#############--]` 91% |

</details>


<details>
<summary>

#### ![Folder](https://img.shields.io/badge/Folder-gray?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZmlsbD0iI2ZmZmZmZiIgZD0iTTMgNiBDMyA0LjkgMy45IDQgNSA0IEgxMCBMMTIgNiBIMTkgQzIwLjEgNiAyMSA2LjkgMjEgOCBWMTggQzIxIDE5LjEgMjAuMSAyMCAxOSAyMCBINSBDMy45IDIwIDMgMTkuMSAzIDE4IFoiLz48L3N2Zz4=) **ViewModels** ![82%](https://img.shields.io/badge/82%25-green)

> **143** / 174 statements covered

</summary>

| File | Coverage | Statements | Progress |
|------|----------|------------|----------|
|  `AboutViewModel.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) **!** | 0 / 14 | `[---------------]` 0% |
|  `SettingsViewModelController.cs` | ![89%](https://img.shields.io/badge/89%25-green) | 143 / 160 | `[#############--]` 89% |

</details>

</details>


<details>
<summary>

### ![Project](https://img.shields.io/badge/Project-blue?style=for-the-badge&logo=nuget) **Shotora.App.Tests** ![100%](https://img.shields.io/badge/100%25-brightgreen)

> **2,543** / 2,553 statements covered

</summary>


<details>
<summary>

#### ![Folder](https://img.shields.io/badge/Folder-gray?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZmlsbD0iI2ZmZmZmZiIgZD0iTTMgNiBDMyA0LjkgMy45IDQgNSA0IEgxMCBMMTIgNiBIMTkgQzIwLjEgNiAyMSA2LjkgMjEgOCBWMTggQzIxIDE5LjEgMjAuMSAyMCAxOSAyMCBINSBDMy45IDIwIDMgMTkuMSAzIDE4IFoiLz48L3N2Zz4=) **DrawingServices** ![100%](https://img.shields.io/badge/100%25-brightgreen)

> **1,237** / 1,241 statements covered

</summary>

| File | Coverage | Statements | Progress |
|------|----------|------------|----------|
|  `RectangleDrawingServiceTests.cs` | ![100%](https://img.shields.io/badge/100%25-brightgreen) | 243 / 244 | `[###############]` 100% |
|  `SaveDrawingServiceTests.cs` | ![100%](https://img.shields.io/badge/100%25-brightgreen) | 219 / 220 | `[###############]` 100% |
|  `SkiaDrawingServiceTests.cs` | ![100%](https://img.shields.io/badge/100%25-brightgreen) | 259 / 260 | `[###############]` 100% |
|  `TextDrawingServiceTests.cs` | ![100%](https://img.shields.io/badge/100%25-brightgreen) | 516 / 517 | `[###############]` 100% |

</details>


<details>
<summary>

#### ![Folder](https://img.shields.io/badge/Folder-gray?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZmlsbD0iI2ZmZmZmZiIgZD0iTTMgNiBDMyA0LjkgMy45IDQgNSA0IEgxMCBMMTIgNiBIMTkgQzIwLjEgNiAyMSA2LjkgMjEgOCBWMTggQzIxIDE5LjEgMjAuMSAyMCAxOSAyMCBINSBDMy45IDIwIDMgMTkuMSAzIDE4IFoiLz48L3N2Zz4=) **PeripheralServices** ![99%](https://img.shields.io/badge/99%25-brightgreen)

> **298** / 300 statements covered

</summary>

| File | Coverage | Statements | Progress |
|------|----------|------------|----------|
|  `KeyboardInteractionServiceTests.cs` | ![100%](https://img.shields.io/badge/100%25-brightgreen) | 199 / 200 | `[###############]` 100% |
|  `MouseInteractionServiceTests.cs` | ![99%](https://img.shields.io/badge/99%25-brightgreen) | 99 / 100 | `[##############-]` 99% |

</details>


<details>
<summary>

#### ![Folder](https://img.shields.io/badge/Folder-gray?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZmlsbD0iI2ZmZmZmZiIgZD0iTTMgNiBDMyA0LjkgMy45IDQgNSA0IEgxMCBMMTIgNiBIMTkgQzIwLjEgNiAyMSA2LjkgMjEgOCBWMTggQzIxIDE5LjEgMjAuMSAyMCAxOSAyMCBINSBDMy45IDIwIDMgMTkuMSAzIDE4IFoiLz48L3N2Zz4=) **Providers** ![100%](https://img.shields.io/badge/100%25-brightgreen)

> **1,008** / 1,012 statements covered

</summary>

| File | Coverage | Statements | Progress |
|------|----------|------------|----------|
|  `IconVisibilityProviderTests.cs` | ![100%](https://img.shields.io/badge/100%25-brightgreen) | 544 / 545 | `[###############]` 100% |
|  `PythonProviderTests.cs` | ![95%](https://img.shields.io/badge/95%25-brightgreen) | 21 / 22 | `[##############-]` 95% |
|  `SettingsProviderTests.cs` | ![100%](https://img.shields.io/badge/100%25-brightgreen) | 282 / 283 | `[###############]` 100% |
|  `TrayMenuWindowServiceTests.cs` | ![99%](https://img.shields.io/badge/99%25-brightgreen) | 161 / 162 | `[##############-]` 99% |

</details>

</details>

---

## Files Needing Attention

> **20 file(s)** below 80% coverage threshold

| Priority | File | Coverage | Statements | Uncovered |
|----------|------|----------|------------|-----------|
| ![Critical](https://img.shields.io/badge/CRITICAL-red?style=flat-square) |  `Shotora.App.Services\Providers\LocalizationProvider.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) | 34 | 34 |
| ![Critical](https://img.shields.io/badge/CRITICAL-red?style=flat-square) |  `Shotora.App.Services\Ocr\EasyOcr\MaintenanceEasyOcrService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) | 1 | 1 |
| ![Critical](https://img.shields.io/badge/CRITICAL-red?style=flat-square) |  `Shotora.App.Services\System\ScreenshotService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) | 173 | 173 |
| ![Critical](https://img.shields.io/badge/CRITICAL-red?style=flat-square) |  `Shotora.App.Services\Adapters\AvaloniaEnumAdapter.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) | 9 | 9 |
| ![Critical](https://img.shields.io/badge/CRITICAL-red?style=flat-square) |  `NativeSupport\Services\BinariesLoaderService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) | 92 | 92 |
| ![Critical](https://img.shields.io/badge/CRITICAL-red?style=flat-square) |  `Shotora.App.Services\Ocr\Tesseract\TessdataTesseractService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) | 23 | 23 |
| ![Critical](https://img.shields.io/badge/CRITICAL-red?style=flat-square) |  `Shotora.App.Services\System\ClipboardService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) | 2 | 2 |
| ![Critical](https://img.shields.io/badge/CRITICAL-red?style=flat-square) |  `Shotora.App.Services\Builders\AnnotationItemBuilder.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) | 10 | 10 |
| ![Critical](https://img.shields.io/badge/CRITICAL-red?style=flat-square) |  `Shotora.App.Services\Adapters\SkiaImageAdapter.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) | 14 | 14 |
| ![Critical](https://img.shields.io/badge/CRITICAL-red?style=flat-square) |  `Shotora.App.Services\Ocr\EasyOcr\PackageDetectorEasyOcrService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) | 17 | 17 |
| ![Critical](https://img.shields.io/badge/CRITICAL-red?style=flat-square) |  `Shotora.App.Services\DrawingServices\LineDrawingService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) | 11 | 11 |
| ![Critical](https://img.shields.io/badge/CRITICAL-red?style=flat-square) |  `Shotora.App.Services\Ocr\BaseOcrService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) | 60 | 60 |
| ![Critical](https://img.shields.io/badge/CRITICAL-red?style=flat-square) |  `Shotora.App.Services\ViewModels\AboutViewModel.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) | 14 | 14 |
| ![Critical](https://img.shields.io/badge/CRITICAL-red?style=flat-square) |  `Shotora.App.Services\Builders\FilePickerBuilder.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) | 1 | 1 |
| ![Critical](https://img.shields.io/badge/CRITICAL-red?style=flat-square) |  `Shotora.App.Services\DrawingServices\EllipseDrawingService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) | 29 | 29 |
| ![Critical](https://img.shields.io/badge/CRITICAL-red?style=flat-square) |  `Shotora.App.Services\System\HotkeyService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) | 54 | 54 |
| ![Critical](https://img.shields.io/badge/CRITICAL-red?style=flat-square) |  `Shotora.App.Services\System\SettingsSystemService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) | 8 | 8 |
| ![Critical](https://img.shields.io/badge/CRITICAL-red?style=flat-square) |  `Shotora.App.Services\Facades\SevenZipFacade.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) | 2 | 2 |
| ![Critical](https://img.shields.io/badge/CRITICAL-red?style=flat-square) |  `Shotora.App.Services\DrawingServices\TranslationDrawingService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) | 31 | 31 |
| ![Critical](https://img.shields.io/badge/CRITICAL-red?style=flat-square) |  `Shotora.App.Services\Ocr\Tesseract\LanguageTesseractService.cs` | ![0%](https://img.shields.io/badge/0%25-lightgrey) | 39 | 39 |

---

*Detailed report generated by Shotora Coverage Tool | Powered by DotCover 2025.3.1*
---