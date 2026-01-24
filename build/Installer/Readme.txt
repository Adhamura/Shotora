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
[![GitHub release](https://img.shields.io/github/v/release/Adhamura/KazukiReiwa?style=flat-square&logo=github)](https://github.com/Adhamura/KazukiReiwa/releases/latest)
[![GitHub downloads (latest)](https://img.shields.io/github/downloads/Adhamura/KazukiReiwa/latest/total?style=flat-square&logo=github)](https://github.com/Adhamura/KazukiReiwa/releases/latest)
[![GitHub downloads (total)](https://img.shields.io/github/downloads/Adhamura/KazukiReiwa/total?style=flat-square&logo=github)](https://github.com/Adhamura/KazukiReiwa/releases)

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
| Tool                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Description |
|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-------------|
| <img style='width:15px;height:15px;' src='data:image/svg+xml;base64,PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnDQogICAgIHZpZXdCb3g9JzQwIC03NjAgODgwIDYwMC45NDEnDQogICAgIHByZXNlcnZlQXNwZWN0UmF0aW89J3hNaWRZTWlkIG1lZXQnPg0KICA8cGF0aCBmaWxsPScjQUQ2NGY1JyBmaWxsLXJ1bGU9J2V2ZW5vZGQnIGQ9J003ODIgLTY3NCBMNDQ3IC0zMzkgTDQ5OSAtMjg3IEw4MzQgLTYyMiBMNzgyIC02NzQgWiBNMjAwIC02ODAgQzI2OC42NjcgLTY3NC42NjcgMzE5LjE2NyAtNjYwLjgzMyAzNTEuNSAtNjM4LjUgQzM4My44MzMgLTYxNi4xNjcgNDAwIC01ODQgNDAwIC01NDIgQzQwMCAtNTA2LjY2NyAzODcuMTY3IC00NzkgMzYxLjUgLTQ1OSBDMzM1LjgzMyAtNDM5IDI5OCAtNDI3IDI0OCAtNDIzIEMyMDUuMzMzIC00MTkuNjY3IDE3My4zMzMgLTQxMS44MzMgMTUyIC0zOTkuNSBDMTMwLjY2NyAtMzg3LjE2NyAxMjAgLTM3MC4zMzMgMTIwIC0zNDkgQzEyMCAtMzI1LjY2NyAxMjkuMzMzIC0zMDguODMzIDE0OCAtMjk4LjUgQzE2Ni42NjcgLTI4OC4xNjcgMTk4IC0yODIgMjQyIC0yODAgTDIzOCAtMjAwIEMxNzEuMzMzIC0yMDMuMzMzIDEyMS42NjcgLTIxNy4zMzMgODkgLTI0MiBDNTYuMzMzIC0yNjYuNjY3IDQwIC0zMDIuMzMzIDQwIC0zNDkgQzQwIC0zOTIuMzMzIDU3LjgzMyAtNDI3LjUgOTMuNSAtNDU0LjUgQzEyOS4xNjcgLTQ4MS41IDE3OC42NjcgLTQ5Ny42NjcgMjQyIC01MDMgQzI2OCAtNTA1IDI4Ny41IC01MDkuMTY3IDMwMC41IC01MTUuNSBDMzEzLjUgLTUyMS44MzMgMzIwIC01MzAuNjY3IDMyMCAtNTQyIEMzMjAgLTU1OS4zMzMgMzEwLjE2NyAtNTcyLjMzMyAyOTAuNSAtNTgxIEMyNzAuODMzIC01ODkuNjY3IDIzOC4zMzMgLTU5NiAxOTMgLTYwMCBMMjAwIC02ODAgWiBNNzgyLjUgLTc2MCBDODAwLjgzMyAtNzYwIDgxNi42NjcgLTc1My4zMzMgODMwIC03NDAgTDkwMCAtNjcwIEM5MTMuMzMzIC02NTYuNjY3IDkyMCAtNjQwLjgzMyA5MjAgLTYyMi41IEM5MjAgLTYwNC4xNjcgOTEzLjMzMyAtNTg4LjMzMyA5MDAgLTU3NSBMNTE4IC0xOTMgTDM1OSAtMTYwIEMzNDcuNjY3IC0xNTcuMzMzIDMzNy42NjcgLTE2MC4zMzMgMzI5IC0xNjkgQzMyMC4zMzMgLTE3Ny42NjcgMzE3LjMzMyAtMTg3LjY2NyAzMjAgLTE5OSBMMzUzIC0zNTggTDczNSAtNzQwIEM3NDguMzMzIC03NTMuMzMzIDc2NC4xNjcgLTc2MCA3ODIuNSAtNzYwIFonLz4NCjwvc3ZnPg==' alt='IconPen' /> **IconPen**   | Freehand drawing with customizable thickness |
| <img style='width:15px;height:15px;' src='data:image/svg+xml;base64,PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnDQogICAgIHZpZXdCb3g9JzIwMSAtNzYwIDU1OCA1NTknDQogICAgIHByZXNlcnZlQXNwZWN0UmF0aW89J3hNaWRZTWlkIG1lZXQnPg0KICA8cGF0aCBmaWxsPScjQUQ2NGY1JyBmaWxsLXJ1bGU9J2V2ZW5vZGQnIGQ9J003MTkuNSAtNzYwIEM3MzAuNSAtNzYwIDc0MCAtNzU2IDc0OCAtNzQ4IEM3NTUuMzMzIC03NDAuNjY3IDc1OSAtNzMxLjMzMyA3NTkgLTcyMCBDNzU5IC03MDguNjY3IDc1NS4zMzMgLTY5OS4zMzMgNzQ4IC02OTIgTDI2OCAtMjEyIEMyNjAuNjY3IC0yMDQuNjY3IDI1MS4zMzMgLTIwMSAyNDAgLTIwMSBDMjI4LjY2NyAtMjAxIDIxOS4zMzMgLTIwNC42NjcgMjEyIC0yMTIgQzIwNC42NjcgLTIxOS4zMzMgMjAxIC0yMjguNjY3IDIwMSAtMjQwIEMyMDEgLTI1MS4zMzMgMjA0LjY2NyAtMjYwLjY2NyAyMTIgLTI2OCBMNjkyIC03NDggQzY5OS4zMzMgLTc1NiA3MDguNSAtNzYwIDcxOS41IC03NjAgWicvPg0KPC9zdmc+' alt='IconLine' />  **Line**      | Straight lines for precise markups |
| <img style='width:15px;height:15px;' src='data:image/svg+xml;base64,PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnDQogICAgIHZpZXdCb3g9JzgwIC03MjAgODAwIDQ4MCcNCiAgICAgcHJlc2VydmVBc3BlY3RSYXRpbz0neE1pZFlNaWQgbWVldCc+DQogIDxwYXRoIGZpbGw9JyNBRDY0ZjUnIGZpbGwtcnVsZT0nZXZlbm9kZCcgZD0nTTY0MCAtNzIwIEw4ODAgLTcyMCBMODgwIC00ODAgTDgwMCAtNDgwIEw4MDAgLTU4MyBMNjIxIC00MDUgQzU5Ny42NjcgLTM4MS42NjcgNTY5LjMzMyAtMzcwIDUzNiAtMzcwIEM1MDIuNjY3IC0zNzAgNDc0LjMzMyAtMzgxLjY2NyA0NTEgLTQwNSBMNDA0IC00NTIgQzM5Ni42NjcgLTQ1OS4zMzMgMzg3LjMzMyAtNDYzIDM3NiAtNDYzIEMzNjQuNjY3IC00NjMgMzU1LjMzMyAtNDU5LjMzMyAzNDggLTQ1MiBMMTM2IC0yNDAgTDgwIC0yOTYgTDI5MiAtNTA4IEMzMTUuMzMzIC01MzEuMzMzIDM0My42NjcgLTU0MyAzNzcgLTU0MyBDNDEwLjMzMyAtNTQzIDQzOC42NjcgLTUzMS4zMzMgNDYyIC01MDggTDUwOCAtNDYyIEM1MTYgLTQ1NCA1MjUuNSAtNDUwIDUzNi41IC00NTAgQzU0Ny41IC00NTAgNTU3IC00NTQgNTY1IC00NjIgTDc0MyAtNjQwIEw2NDAgLTY0MCBMNjQwIC03MjAgWicvPg0KPC9zdmc+' alt='IconArrow' />  **Arrow**     | Directional arrows to highlight important areas |
| <img style='width:15px;height:15px;' src='data:image/svg+xml;base64,PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnDQogICAgIHZpZXdCb3g9JzgwIC04MDAgODAwIDY0MCcNCiAgICAgcHJlc2VydmVBc3BlY3RSYXRpbz0neE1pZFlNaWQgbWVldCc+DQogIDxwYXRoIGZpbGw9JyNBRDY0ZjUnIGZpbGwtcnVsZT0nZXZlbm9kZCcgZD0nTTE2MCAtNzIwIEwxNjAgLTI0MCBMODAwIC0yNDAgTDgwMCAtNzIwIEwxNjAgLTcyMCBaIE04MCAtODAwIEw4ODAgLTgwMCBMODgwIC0xNjAgTDgwIC0xNjAgTDgwIC04MDAgWicvPg0KPC9zdmc+' alt='IconRect' />  **Rectangle**    | Rectangular shapes and outlines |
| <img style='width:15px;height:15px;' src='data:image/svg+xml;base64,PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnDQogICAgIHZpZXdCb3g9JzgwIC04ODAgODAwIDgwMCcNCiAgICAgcHJlc2VydmVBc3BlY3RSYXRpbz0neE1pZFlNaWQgbWVldCc+DQogIDxwYXRoIGZpbGw9JyNBRDY0ZjUnIGZpbGwtcnVsZT0nZXZlbm9kZCcgZD0nTTQ4MCAtODAwIEMzOTAuNjY3IC04MDAgMzE1IC03NjkgMjUzIC03MDcgQzE5MSAtNjQ1IDE2MCAtNTY5LjMzMyAxNjAgLTQ4MCBDMTYwIC0zOTAuNjY3IDE5MSAtMzE1IDI1MyAtMjUzIEMzMTUgLTE5MSAzOTAuNjY3IC0xNjAgNDgwIC0xNjAgQzU2OS4zMzMgLTE2MCA2NDUgLTE5MSA3MDcgLTI1MyBDNzY5IC0zMTUgODAwIC0zOTAuNjY3IDgwMCAtNDgwIEM4MDAgLTU2OS4zMzMgNzY5IC02NDUgNzA3IC03MDcgQzY0NSAtNzY5IDU2OS4zMzMgLTgwMCA0ODAgLTgwMCBaIE00ODAgLTg4MCBDNTM1LjMzMyAtODgwIDU4Ny4zMzMgLTg2OS41IDYzNiAtODQ4LjUgQzY4NC42NjcgLTgyNy41IDcyNyAtNzk5IDc2MyAtNzYzIEM3OTkgLTcyNyA4MjcuNSAtNjg0LjY2NyA4NDguNSAtNjM2IEM4NjkuNSAtNTg3LjMzMyA4ODAgLTUzNS4zMzMgODgwIC00ODAgQzg4MCAtNDI0LjY2NyA4NjkuNSAtMzcyLjY2NyA4NDguNSAtMzI0IEM4MjcuNSAtMjc1LjMzMyA3OTkgLTIzMyA3NjMgLTE5NyBDNzI3IC0xNjEgNjg0LjY2NyAtMTMyLjUgNjM2IC0xMTEuNSBDNTg3LjMzMyAtOTAuNSA1MzUuMzMzIC04MCA0ODAgLTgwIEM0MjQuNjY3IC04MCAzNzIuNjY3IC05MC41IDMyNCAtMTExLjUgQzI3NS4zMzMgLTEzMi41IDIzMyAtMTYxIDE5NyAtMTk3IEMxNjEgLTIzMyAxMzIuNSAtMjc1LjMzMyAxMTEuNSAtMzI0IEM5MC41IC0zNzIuNjY3IDgwIC00MjQuNjY3IDgwIC00ODAgQzgwIC01MzUuMzMzIDkwLjUgLTU4Ny4zMzMgMTExLjUgLTYzNiBDMTMyLjUgLTY4NC42NjcgMTYxIC03MjcgMTk3IC03NjMgQzIzMyAtNzk5IDI3NS4zMzMgLTgyNy41IDMyNCAtODQ4LjUgQzM3Mi42NjcgLTg2OS41IDQyNC42NjcgLTg4MCA0ODAgLTg4MCBaJy8+DQo8L3N2Zz4=' alt='IconEllipse' />  **Ellipse**    | Circular and oval shapes |
| <img style='width:15px;height:15px;' src='data:image/svg+xml;base64,PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnDQogICAgIHZpZXdCb3g9JzgwIC04MDAgODAwIDY0MCcNCiAgICAgcHJlc2VydmVBc3BlY3RSYXRpbz0neE1pZFlNaWQgbWVldCc+DQogIDxwYXRoIGZpbGw9JyNBRDY0ZjUnIGZpbGwtcnVsZT0nZXZlbm9kZCcgZD0nTTUyMCAtNjAwIEw4ODAgLTYwMCBMODgwIC00ODAgTDc2MCAtNDgwIEw3NjAgLTE2MCBMNjQwIC0xNjAgTDY0MCAtNDgwIEw1MjAgLTQ4MCBMNTIwIC02MDAgWiBNODAgLTgwMCBMNjAwIC04MDAgTDYwMCAtNjgwIEw0MDAgLTY4MCBMNDAwIC0xNjAgTDI4MCAtMTYwIEwyODAgLTY4MCBMODAgLTY4MCBMODAgLTgwMCBaJy8+DQo8L3N2Zz4=' alt='IconText' />  **Text**        | Add text annotations with font customization |
| <img style='width:15px;height:15px;' src='data:image/svg+xml;base64,PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnDQogICAgIHZpZXdCb3g9JzYwIC04NDAgODIwIDcyMCcNCiAgICAgcHJlc2VydmVBc3BlY3RSYXRpbz0neE1pZFlNaWQgbWVldCc+DQogIDxwYXRoIGZpbGw9JyNBRDY0ZjUnIGZpbGwtcnVsZT0nZXZlbm9kZCcgZD0nTTQ0MCAtNTA0IEwyNDAgLTMwNCBMMzQ0IC0yMDAgTDU0NCAtNDAwIEw0NDAgLTUwNCBaIE02OTYgLTc2MCBMNDk3IC01NjEgTDYwMSAtNDU3IEw4MDAgLTY1NiBMNjk2IC03NjAgWiBNNjk2IC04NDAgQzcxNy4zMzMgLTg0MCA3MzYgLTgzMiA3NTIgLTgxNiBMODU2IC03MTIgQzg3MiAtNjk2IDg4MCAtNjc3LjMzMyA4ODAgLTY1NiBDODgwIC02MzQuNjY3IDg3MiAtNjE2IDg1NiAtNjAwIEw2MjkgLTM3MyBMNDAwIC0xNDQgQzM4NCAtMTI4IDM2NS4zMzMgLTEyMCAzNDQgLTEyMCBDMzIyLjY2NyAtMTIwIDMwNCAtMTI4IDI4OCAtMTQ0IEwyODYgLTE0NiBMMjYwIC0xMjAgTDYwIC0xMjAgTDE4NiAtMjQ2IEwxODQgLTI0OCBDMTY4IC0yNjQgMTYwIC0yODIuNjY3IDE2MCAtMzA0IEMxNjAgLTMyNS4zMzMgMTY4IC0zNDQgMTg0IC0zNjAgTDQxMyAtNTg5IEw2NDAgLTgxNiBDNjU2IC04MzIgNjc0LjY2NyAtODQwIDY5NiAtODQwIFonLz4NCjwvc3ZnPg==' alt='IconHighlight' />  **Highlight**  | Semi-transparent highlighting |
| <img style='width:15px;height:15px;' src='data:image/svg+xml;base64,PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnDQogICAgIHZpZXdCb3g9JzEyMCAtODQwIDc0MCA3MjAnDQogICAgIHByZXNlcnZlQXNwZWN0UmF0aW89J3hNaWRZTWlkIG1lZXQnPg0KICA8cGF0aCBmaWxsPScjQUQ2NGY1JyBmaWxsLXJ1bGU9J2V2ZW5vZGQnIGQ9J002NDAgLTIwMCBDNjUxLjMzMyAtMjAwIDY2MC44MzMgLTE5Ni4xNjcgNjY4LjUgLTE4OC41IEM2NzYuMTY3IC0xODAuODMzIDY4MCAtMTcxLjMzMyA2ODAgLTE2MCBDNjgwIC0xNDguNjY3IDY3Ni4xNjcgLTEzOS4xNjcgNjY4LjUgLTEzMS41IEM2NjAuODMzIC0xMjMuODMzIDY1MS4zMzMgLTEyMCA2NDAgLTEyMCBDNjI4LjY2NyAtMTIwIDYxOS4xNjcgLTEyMy44MzMgNjExLjUgLTEzMS41IEM2MDMuODMzIC0xMzkuMTY3IDYwMCAtMTQ4LjY2NyA2MDAgLTE2MCBDNjAwIC0xNzEuMzMzIDYwMy44MzMgLTE4MC44MzMgNjExLjUgLTE4OC41IEM2MTkuMTY3IC0xOTYuMTY3IDYyOC42NjcgLTIwMCA2NDAgLTIwMCBaIE00ODAgLTIwMCBDNDkxLjMzMyAtMjAwIDUwMC44MzMgLTE5Ni4xNjcgNTA4LjUgLTE4OC41IEM1MTYuMTY3IC0xODAuODMzIDUyMCAtMTcxLjMzMyA1MjAgLTE2MCBDNTIwIC0xNDguNjY3IDUxNi4xNjcgLTEzOS4xNjcgNTA4LjUgLTEzMS41IEM1MDAuODMzIC0xMjMuODMzIDQ5MS4zMzMgLTEyMCA0ODAgLTEyMCBDNDY4LjY2NyAtMTIwIDQ1OS4xNjcgLTEyMy44MzMgNDUxLjUgLTEzMS41IEM0NDMuODMzIC0xMzkuMTY3IDQ0MCAtMTQ4LjY2NyA0NDAgLTE2MCBDNDQwIC0xNzEuMzMzIDQ0My44MzMgLTE4MC44MzMgNDUxLjUgLTE4OC41IEM0NTkuMTY3IC0xOTYuMTY3IDQ2OC42NjcgLTIwMCA0ODAgLTIwMCBaIE03MjAgLTI4MCBDNzMxLjMzMyAtMjgwIDc0MC44MzMgLTI3Ni4xNjcgNzQ4LjUgLTI2OC41IEM3NTYuMTY3IC0yNjAuODMzIDc2MCAtMjUxLjMzMyA3NjAgLTI0MCBDNzYwIC0yMjguNjY3IDc1Ni4xNjcgLTIxOS4xNjcgNzQ4LjUgLTIxMS41IEM3NDAuODMzIC0yMDMuODMzIDczMS4zMzMgLTIwMCA3MjAgLTIwMCBDNzA4LjY2NyAtMjAwIDY5OS4xNjcgLTIwMy44MzMgNjkxLjUgLTIxMS41IEM2ODMuODMzIC0yMTkuMTY3IDY4MCAtMjI4LjY2NyA2ODAgLTI0MCBDNjgwIC0yNTEuMzMzIDY4My44MzMgLTI2MC44MzMgNjkxLjUgLTI2OC41IEM2OTkuMTY3IC0yNzYuMTY3IDcwOC42NjcgLTI4MCA3MjAgLTI4MCBaIE01NjAgLTI4MCBDNTcxLjMzMyAtMjgwIDU4MC44MzMgLTI3Ni4xNjcgNTg4LjUgLTI2OC41IEM1OTYuMTY3IC0yNjAuODMzIDYwMCAtMjUxLjMzMyA2MDAgLTI0MCBDNjAwIC0yMjguNjY3IDU5Ni4xNjcgLTIxOS4xNjcgNTg4LjUgLTIxMS41IEM1ODAuODMzIC0yMDMuODMzIDU3MS4zMzMgLTIwMCA1NjAgLTIwMCBDNTQ4LjY2NyAtMjAwIDUzOS4xNjcgLTIwMy44MzMgNTMxLjUgLTIxMS41IEM1MjMuODMzIC0yMTkuMTY3IDUyMCAtMjI4LjY2NyA1MjAgLTI0MCBDNTIwIC0yNTEuMzMzIDUyMy44MzMgLTI2MC44MzMgNTMxLjUgLTI2OC41IEM1MzkuMTY3IC0yNzYuMTY3IDU0OC42NjcgLTI4MCA1NjAgLTI4MCBaIE02NDAgLTM2MCBDNjUxLjMzMyAtMzYwIDY2MC44MzMgLTM1Ni4xNjcgNjY4LjUgLTM0OC41IEM2NzYuMTY3IC0zNDAuODMzIDY4MCAtMzMxLjMzMyA2ODAgLTMyMCBDNjgwIC0zMDguNjY3IDY3Ni4xNjcgLTI5OS4xNjcgNjY4LjUgLTI5MS41IEM2NjAuODMzIC0yODMuODMzIDY1MS4zMzMgLTI4MCA2NDAgLTI4MCBDNjI4LjY2NyAtMjgwIDYxOS4xNjcgLTI4My44MzMgNjExLjUgLTI5MS41IEM2MDMuODMzIC0yOTkuMTY3IDYwMCAtMzA4LjY2NyA2MDAgLTMyMCBDNjAwIC0zMzEuMzMzIDYwMy44MzMgLTM0MC44MzMgNjExLjUgLTM0OC41IEM2MTkuMTY3IC0zNTYuMTY3IDYyOC42NjcgLTM2MCA2NDAgLTM2MCBaIE00ODAgLTM2MCBDNDkxLjMzMyAtMzYwIDUwMC44MzMgLTM1Ni4xNjcgNTA4LjUgLTM0OC41IEM1MTYuMTY3IC0zNDAuODMzIDUyMCAtMzMxLjMzMyA1MjAgLTMyMCBDNTIwIC0zMDguNjY3IDUxNi4xNjcgLTI5OS4xNjcgNTA4LjUgLTI5MS41IEM1MDAuODMzIC0yODMuODMzIDQ5MS4zMzMgLTI4MCA0ODAgLTI4MCBDNDY4LjY2NyAtMjgwIDQ1OS4xNjcgLTI4My44MzMgNDUxLjUgLTI5MS41IEM0NDMuODMzIC0yOTkuMTY3IDQ0MCAtMzA4LjY2NyA0NDAgLTMyMCBDNDQwIC0zMzEuMzMzIDQ0My44MzMgLTM0MC44MzMgNDUxLjUgLTM0OC41IEM0NTkuMTY3IC0zNTYuMTY3IDQ2OC42NjcgLTM2MCA0ODAgLTM2MCBaIE04NDAgLTQyMCBDODQ1LjMzMyAtNDIwIDg1MCAtNDE4IDg1NCAtNDE0IEM4NTggLTQxMCA4NjAgLTQwNS4zMzMgODYwIC00MDAgQzg2MCAtMzk0LjY2NyA4NTggLTM5MCA4NTQgLTM4NiBDODUwIC0zODIgODQ1LjMzMyAtMzgwIDg0MCAtMzgwIEM4MzQuNjY3IC0zODAgODMwIC0zODIgODI2IC0zODYgQzgyMiAtMzkwIDgyMCAtMzk0LjY2NyA4MjAgLTQwMCBDODIwIC00MDUuMzMzIDgyMiAtNDEwIDgyNiAtNDE0IEM4MzAgLTQxOCA4MzQuNjY3IC00MjAgODQwIC00MjAgWiBNNzIwIC00NDAgQzczMS4zMzMgLTQ0MCA3NDAuODMzIC00MzYuMTY3IDc0OC41IC00MjguNSBDNzU2LjE2NyAtNDIwLjgzMyA3NjAgLTQxMS4zMzMgNzYwIC00MDAgQzc2MCAtMzg4LjY2NyA3NTYuMTY3IC0zNzkuMTY3IDc0OC41IC0zNzEuNSBDNzQwLjgzMyAtMzYzLjgzMyA3MzEuMzMzIC0zNjAgNzIwIC0zNjAgQzcwOC42NjcgLTM2MCA2OTkuMTY3IC0zNjMuODMzIDY5MS41IC0zNzEuNSBDNjgzLjgzMyAtMzc5LjE2NyA2ODAgLTM4OC42NjcgNjgwIC00MDAgQzY4MCAtNDExLjMzMyA2ODMuODMzIC00MjAuODMzIDY5MS41IC00MjguNSBDNjk5LjE2NyAtNDM2LjE2NyA3MDguNjY3IC00NDAgNzIwIC00NDAgWiBNNTYwIC00NDAgQzU3MS4zMzMgLTQ0MCA1ODAuODMzIC00MzYuMTY3IDU4OC41IC00MjguNSBDNTk2LjE2NyAtNDIwLjgzMyA2MDAgLTQxMS4zMzMgNjAwIC00MDAgQzYwMCAtMzg4LjY2NyA1OTYuMTY3IC0zNzkuMTY3IDU4OC41IC0zNzEuNSBDNTgwLjgzMyAtMzYzLjgzMyA1NzEuMzMzIC0zNjAgNTYwIC0zNjAgQzU0OC42NjcgLTM2MCA1MzkuMTY3IC0zNjMuODMzIDUzMS41IC0zNzEuNSBDNTIzLjgzMyAtMzc5LjE2NyA1MjAgLTM4OC42NjcgNTIwIC00MDAgQzUyMCAtNDExLjMzMyA1MjMuODMzIC00MjAuODMzIDUzMS41IC00MjguNSBDNTM5LjE2NyAtNDM2LjE2NyA1NDguNjY3IC00NDAgNTYwIC00NDAgWiBNNjQwIC01MjAgQzY1MS4zMzMgLTUyMCA2NjAuODMzIC01MTYuMTY3IDY2OC41IC01MDguNSBDNjc2LjE2NyAtNTAwLjgzMyA2ODAgLTQ5MS4zMzMgNjgwIC00ODAgQzY4MCAtNDY4LjY2NyA2NzYuMTY3IC00NTkuMTY3IDY2OC41IC00NTEuNSBDNjYwLjgzMyAtNDQzLjgzMyA2NTEuMzMzIC00NDAgNjQwIC00NDAgQzYyOC42NjcgLTQ0MCA2MTkuMTY3IC00NDMuODMzIDYxMS41IC00NTEuNSBDNjAzLjgzMyAtNDU5LjE2NyA2MDAgLTQ2OC42NjcgNjAwIC00ODAgQzYwMCAtNDkxLjMzMyA2MDMuODMzIC01MDAuODMzIDYxMS41IC01MDguNSBDNjE5LjE2NyAtNTE2LjE2NyA2MjguNjY3IC01MjAgNjQwIC01MjAgWiBNNDgwIC01MjAgQzQ5MS4zMzMgLTUyMCA1MDAuODMzIC01MTYuMTY3IDUwOC41IC01MDguNSBDNTE2LjE2NyAtNTAwLjgzMyA1MjAgLTQ5MS4zMzMgNTIwIC00ODAgQzUyMCAtNDY4LjY2NyA1MTYuMTY3IC00NTkuMTY3IDUwOC41IC00NTEuNSBDNTAwLjgzMyAtNDQzLjgzMyA0OTEuMzMzIC00NDAgNDgwIC00NDAgQzQ2OC42NjcgLTQ0MCA0NTkuMTY3IC00NDMuODMzIDQ1MS41IC00NTEuNSBDNDQzLjgzMyAtNDU5LjE2NyA0NDAgLTQ2OC42NjcgNDQwIC00ODAgQzQ0MCAtNDkxLjMzMyA0NDMuODMzIC01MDAuODMzIDQ1MS41IC01MDguNSBDNDU5LjE2NyAtNTE2LjE2NyA0NjguNjY3IC01MjAgNDgwIC01MjAgWiBNODQwIC01ODAgQzg0NS4zMzMgLTU4MCA4NTAgLTU3OCA4NTQgLTU3NCBDODU4IC01NzAgODYwIC01NjUuMzMzIDg2MCAtNTYwIEM4NjAgLTU1NC42NjcgODU4IC01NTAgODU0IC01NDYgQzg1MCAtNTQyIDg0NS4zMzMgLTU0MCA4NDAgLTU0MCBDODM0LjY2NyAtNTQwIDgzMCAtNTQyIDgyNiAtNTQ2IEM4MjIgLTU1MCA4MjAgLTU1NC42NjcgODIwIC01NjAgQzgyMCAtNTY1LjMzMyA4MjIgLTU3MCA4MjYgLTU3NCBDODMwIC01NzggODM0LjY2NyAtNTgwIDg0MCAtNTgwIFogTTcyMCAtNjAwIEM3MzEuMzMzIC02MDAgNzQwLjgzMyAtNTk2LjE2NyA3NDguNSAtNTg4LjUgQzc1Ni4xNjcgLTU4MC44MzMgNzYwIC01NzEuMzMzIDc2MCAtNTYwIEM3NjAgLTU0OC42NjcgNzU2LjE2NyAtNTM5LjE2NyA3NDguNSAtNTMxLjUgQzc0MC44MzMgLTUyMy44MzMgNzMxLjMzMyAtNTIwIDcyMCAtNTIwIEM3MDguNjY3IC01MjAgNjk5LjE2NyAtNTIzLjgzMyA2OTEuNSAtNTMxLjUgQzY4My44MzMgLTUzOS4xNjcgNjgwIC01NDguNjY3IDY4MCAtNTYwIEM2ODAgLTU3MS4zMzMgNjgzLjgzMyAtNTgwLjgzMyA2OTEuNSAtNTg4LjUgQzY5OS4xNjcgLTU5Ni4xNjcgNzA4LjY2NyAtNjAwIDcyMCAtNjAwIFogTTU2MCAtNjAwIEM1NzEuMzMzIC02MDAgNTgwLjgzMyAtNTk2LjE2NyA1ODguNSAtNTg4LjUgQzU5Ni4xNjcgLTU4MC44MzMgNjAwIC01NzEuMzMzIDYwMCAtNTYwIEM2MDAgLTU0OC42NjcgNTk2LjE2NyAtNTM5LjE2NyA1ODguNSAtNTMxLjUgQzU4MC44MzMgLTUyMy44MzMgNTcxLjMzMyAtNTIwIDU2MCAtNTIwIEM1NDguNjY3IC01MjAgNTM5LjE2NyAtNTIzLjgzMyA1MzEuNSAtNTMxLjUgQzUyMy44MzMgLTUzOS4xNjcgNTIwIC01NDguNjY3IDUyMCAtNTYwIEM1MjAgLTU3MS4zMzMgNTIzLjgzMyAtNTgwLjgzMyA1MzEuNSAtNTg4LjUgQzUzOS4xNjcgLTU5Ni4xNjcgNTQ4LjY2NyAtNjAwIDU2MCAtNjAwIFogTTY0MCAtNjgwIEM2NTEuMzMzIC02ODAgNjYwLjgzMyAtNjc2LjE2NyA2NjguNSAtNjY4LjUgQzY3Ni4xNjcgLTY2MC44MzMgNjgwIC02NTEuMzMzIDY4MCAtNjQwIEM2ODAgLTYyOC42NjcgNjc2LjE2NyAtNjE5LjE2NyA2NjguNSAtNjExLjUgQzY2MC44MzMgLTYwMy44MzMgNjUxLjMzMyAtNjAwIDY0MCAtNjAwIEM2MjguNjY3IC02MDAgNjE5LjE2NyAtNjAzLjgzMyA2MTEuNSAtNjExLjUgQzYwMy44MzMgLTYxOS4xNjcgNjAwIC02MjguNjY3IDYwMCAtNjQwIEM2MDAgLTY1MS4zMzMgNjAzLjgzMyAtNjYwLjgzMyA2MTEuNSAtNjY4LjUgQzYxOS4xNjcgLTY3Ni4xNjcgNjI4LjY2NyAtNjgwIDY0MCAtNjgwIFogTTQ4MCAtNjgwIEM0OTEuMzMzIC02ODAgNTAwLjgzMyAtNjc2LjE2NyA1MDguNSAtNjY4LjUgQzUxNi4xNjcgLTY2MC44MzMgNTIwIC02NTEuMzMzIDUyMCAtNjQwIEM1MjAgLTYyOC42NjcgNTE2LjE2NyAtNjE5LjE2NyA1MDguNSAtNjExLjUgQzUwMC44MzMgLTYwMy44MzMgNDkxLjMzMyAtNjAwIDQ4MCAtNjAwIEM0NjguNjY3IC02MDAgNDU5LjE2NyAtNjAzLjgzMyA0NTEuNSAtNjExLjUgQzQ0My44MzMgLTYxOS4xNjcgNDQwIC02MjguNjY3IDQ0MCAtNjQwIEM0NDAgLTY1MS4zMzMgNDQzLjgzMyAtNjYwLjgzMyA0NTEuNSAtNjY4LjUgQzQ1OS4xNjcgLTY3Ni4xNjcgNDY4LjY2NyAtNjgwIDQ4MCAtNjgwIFogTTcyMCAtNzYwIEM3MzEuMzMzIC03NjAgNzQwLjgzMyAtNzU2LjE2NyA3NDguNSAtNzQ4LjUgQzc1Ni4xNjcgLTc0MC44MzMgNzYwIC03MzEuMzMzIDc2MCAtNzIwIEM3NjAgLTcwOC42NjcgNzU2LjE2NyAtNjk5LjE2NyA3NDguNSAtNjkxLjUgQzc0MC44MzMgLTY4My44MzMgNzMxLjMzMyAtNjgwIDcyMCAtNjgwIEM3MDguNjY3IC02ODAgNjk5LjE2NyAtNjgzLjgzMyA2OTEuNSAtNjkxLjUgQzY4My44MzMgLTY5OS4xNjcgNjgwIC03MDguNjY3IDY4MCAtNzIwIEM2ODAgLTczMS4zMzMgNjgzLjgzMyAtNzQwLjgzMyA2OTEuNSAtNzQ4LjUgQzY5OS4xNjcgLTc1Ni4xNjcgNzA4LjY2NyAtNzYwIDcyMCAtNzYwIFogTTU2MCAtNzYwIEM1NzEuMzMzIC03NjAgNTgwLjgzMyAtNzU2LjE2NyA1ODguNSAtNzQ4LjUgQzU5Ni4xNjcgLTc0MC44MzMgNjAwIC03MzEuMzMzIDYwMCAtNzIwIEM2MDAgLTcwOC42NjcgNTk2LjE2NyAtNjk5LjE2NyA1ODguNSAtNjkxLjUgQzU4MC44MzMgLTY4My44MzMgNTcxLjMzMyAtNjgwIDU2MCAtNjgwIEM1NDguNjY3IC02ODAgNTM5LjE2NyAtNjgzLjgzMyA1MzEuNSAtNjkxLjUgQzUyMy44MzMgLTY5OS4xNjcgNTIwIC03MDguNjY3IDUyMCAtNzIwIEM1MjAgLTczMS4zMzMgNTIzLjgzMyAtNzQwLjgzMyA1MzEuNSAtNzQ4LjUgQzUzOS4xNjcgLTc1Ni4xNjcgNTQ4LjY2NyAtNzYwIDU2MCAtNzYwIFogTTY0MCAtODQwIEM2NTEuMzMzIC04NDAgNjYwLjgzMyAtODM2LjE2NyA2NjguNSAtODI4LjUgQzY3Ni4xNjcgLTgyMC44MzMgNjgwIC04MTEuMzMzIDY4MCAtODAwIEM2ODAgLTc4OC42NjcgNjc2LjE2NyAtNzc5LjE2NyA2NjguNSAtNzcxLjUgQzY2MC44MzMgLTc2My44MzMgNjUxLjMzMyAtNzYwIDY0MCAtNzYwIEM2MjguNjY3IC03NjAgNjE5LjE2NyAtNzYzLjgzMyA2MTEuNSAtNzcxLjUgQzYwMy44MzMgLTc3OS4xNjcgNjAwIC03ODguNjY3IDYwMCAtODAwIEM2MDAgLTgxMS4zMzMgNjAzLjgzMyAtODIwLjgzMyA2MTEuNSAtODI4LjUgQzYxOS4xNjcgLTgzNi4xNjcgNjI4LjY2NyAtODQwIDY0MCAtODQwIFogTTQ4MCAtODQwIEM0OTEuMzMzIC04NDAgNTAwLjgzMyAtODM2LjE2NyA1MDguNSAtODI4LjUgQzUxNi4xNjcgLTgyMC44MzMgNTIwIC04MTEuMzMzIDUyMCAtODAwIEM1MjAgLTc4OC42NjcgNTE2LjE2NyAtNzc5LjE2NyA1MDguNSAtNzcxLjUgQzUwMC44MzMgLTc2My44MzMgNDkxLjMzMyAtNzYwIDQ4MCAtNzYwIEM0NjguNjY3IC03NjAgNDU5LjE2NyAtNzYzLjgzMyA0NTEuNSAtNzcxLjUgQzQ0My44MzMgLTc3OS4xNjcgNDQwIC03ODguNjY3IDQ0MCAtODAwIEM0NDAgLTgxMS4zMzMgNDQzLjgzMyAtODIwLjgzMyA0NTEuNSAtODI4LjUgQzQ1OS4xNjcgLTgzNi4xNjcgNDY4LjY2NyAtODQwIDQ4MCAtODQwIFogTTIwMCAtODQwIEw0MDAgLTg0MCBMNDAwIC0xMjAgTDIwMCAtMTIwIEMxNzggLTEyMCAxNTkuMTY3IC0xMjcuODMzIDE0My41IC0xNDMuNSBDMTI3LjgzMyAtMTU5LjE2NyAxMjAgLTE3OCAxMjAgLTIwMCBMMTIwIC03NjAgQzEyMCAtNzgyIDEyNy44MzMgLTgwMC44MzMgMTQzLjUgLTgxNi41IEMxNTkuMTY3IC04MzIuMTY3IDE3OCAtODQwIDIwMCAtODQwIFonLz4NCjwvc3ZnPg==' alt='IconPixelate' />  **Blur**        | Redact sensitive information |
| <img style='width:15px;height:15px;' src='data:image/svg+xml;base64,PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnDQogICAgIHZpZXdCb3g9JzEwMCAtODYwIDc2MCA3NjAnDQogICAgIHByZXNlcnZlQXNwZWN0UmF0aW89J3hNaWRZTWlkIG1lZXQnPg0KICA8cGF0aCBmaWxsPScjQUQ2NGY1JyBmaWxsLXJ1bGU9J2V2ZW5vZGQnIGQ9J001NjAgLTE0MCBDNTY1LjMzMyAtMTQwIDU3MCAtMTM4IDU3NCAtMTM0IEM1NzggLTEzMCA1ODAgLTEyNS4zMzMgNTgwIC0xMjAgQzU4MCAtMTE0LjY2NyA1NzggLTExMCA1NzQgLTEwNiBDNTcwIC0xMDIgNTY1LjMzMyAtMTAwIDU2MCAtMTAwIEM1NTQuNjY3IC0xMDAgNTUwIC0xMDIgNTQ2IC0xMDYgQzU0MiAtMTEwIDU0MCAtMTE0LjY2NyA1NDAgLTEyMCBDNTQwIC0xMjUuMzMzIDU0MiAtMTMwIDU0NiAtMTM0IEM1NTAgLTEzOCA1NTQuNjY3IC0xNDAgNTYwIC0xNDAgWiBNNDAwIC0xNDAgQzQwNS4zMzMgLTE0MCA0MTAgLTEzOCA0MTQgLTEzNCBDNDE4IC0xMzAgNDIwIC0xMjUuMzMzIDQyMCAtMTIwIEM0MjAgLTExNC42NjcgNDE4IC0xMTAgNDE0IC0xMDYgQzQxMCAtMTAyIDQwNS4zMzMgLTEwMCA0MDAgLTEwMCBDMzk0LjY2NyAtMTAwIDM5MCAtMTAyIDM4NiAtMTA2IEMzODIgLTExMCAzODAgLTExNC42NjcgMzgwIC0xMjAgQzM4MCAtMTI1LjMzMyAzODIgLTEzMCAzODYgLTEzNCBDMzkwIC0xMzggMzk0LjY2NyAtMTQwIDQwMCAtMTQwIFogTTcyMCAtMjgwIEM3MzEuMzMzIC0yODAgNzQwLjgzMyAtMjc2LjE2NyA3NDguNSAtMjY4LjUgQzc1Ni4xNjcgLTI2MC44MzMgNzYwIC0yNTEuMzMzIDc2MCAtMjQwIEM3NjAgLTIyOC42NjcgNzU2LjE2NyAtMjE5LjE2NyA3NDguNSAtMjExLjUgQzc0MC44MzMgLTIwMy44MzMgNzMxLjMzMyAtMjAwIDcyMCAtMjAwIEM3MDguNjY3IC0yMDAgNjk5LjE2NyAtMjAzLjgzMyA2OTEuNSAtMjExLjUgQzY4My44MzMgLTIxOS4xNjcgNjgwIC0yMjguNjY3IDY4MCAtMjQwIEM2ODAgLTI1MS4zMzMgNjgzLjgzMyAtMjYwLjgzMyA2OTEuNSAtMjY4LjUgQzY5OS4xNjcgLTI3Ni4xNjcgNzA4LjY2NyAtMjgwIDcyMCAtMjgwIFogTTU2MCAtMjgwIEM1NzEuMzMzIC0yODAgNTgwLjgzMyAtMjc2LjE2NyA1ODguNSAtMjY4LjUgQzU5Ni4xNjcgLTI2MC44MzMgNjAwIC0yNTEuMzMzIDYwMCAtMjQwIEM2MDAgLTIyOC42NjcgNTk2LjE2NyAtMjE5LjE2NyA1ODguNSAtMjExLjUgQzU4MC44MzMgLTIwMy44MzMgNTcxLjMzMyAtMjAwIDU2MCAtMjAwIEM1NDguNjY3IC0yMDAgNTM5LjE2NyAtMjAzLjgzMyA1MzEuNSAtMjExLjUgQzUyMy44MzMgLTIxOS4xNjcgNTIwIC0yMjguNjY3IDUyMCAtMjQwIEM1MjAgLTI1MS4zMzMgNTIzLjgzMyAtMjYwLjgzMyA1MzEuNSAtMjY4LjUgQzUzOS4xNjcgLTI3Ni4xNjcgNTQ4LjY2NyAtMjgwIDU2MCAtMjgwIFogTTQwMCAtMjgwIEM0MTEuMzMzIC0yODAgNDIwLjgzMyAtMjc2LjE2NyA0MjguNSAtMjY4LjUgQzQzNi4xNjcgLTI2MC44MzMgNDQwIC0yNTEuMzMzIDQ0MCAtMjQwIEM0NDAgLTIyOC42NjcgNDM2LjE2NyAtMjE5LjE2NyA0MjguNSAtMjExLjUgQzQyMC44MzMgLTIwMy44MzMgNDExLjMzMyAtMjAwIDQwMCAtMjAwIEMzODguNjY3IC0yMDAgMzc5LjE2NyAtMjAzLjgzMyAzNzEuNSAtMjExLjUgQzM2My44MzMgLTIxOS4xNjcgMzYwIC0yMjguNjY3IDM2MCAtMjQwIEMzNjAgLTI1MS4zMzMgMzYzLjgzMyAtMjYwLjgzMyAzNzEuNSAtMjY4LjUgQzM3OS4xNjcgLTI3Ni4xNjcgMzg4LjY2NyAtMjgwIDQwMCAtMjgwIFogTTI0MCAtMjgwIEMyNTEuMzMzIC0yODAgMjYwLjgzMyAtMjc2LjE2NyAyNjguNSAtMjY4LjUgQzI3Ni4xNjcgLTI2MC44MzMgMjgwIC0yNTEuMzMzIDI4MCAtMjQwIEMyODAgLTIyOC42NjcgMjc2LjE2NyAtMjE5LjE2NyAyNjguNSAtMjExLjUgQzI2MC44MzMgLTIwMy44MzMgMjUxLjMzMyAtMjAwIDI0MCAtMjAwIEMyMjguNjY3IC0yMDAgMjE5LjE2NyAtMjAzLjgzMyAyMTEuNSAtMjExLjUgQzIwMy44MzMgLTIxOS4xNjcgMjAwIC0yMjguNjY3IDIwMCAtMjQwIEMyMDAgLTI1MS4zMzMgMjAzLjgzMyAtMjYwLjgzMyAyMTEuNSAtMjY4LjUgQzIxOS4xNjcgLTI3Ni4xNjcgMjI4LjY2NyAtMjgwIDI0MCAtMjgwIFogTTg0MCAtNDIwIEM4NDUuMzMzIC00MjAgODUwIC00MTggODU0IC00MTQgQzg1OCAtNDEwIDg2MCAtNDA1LjMzMyA4NjAgLTQwMCBDODYwIC0zOTQuNjY3IDg1OCAtMzkwIDg1NCAtMzg2IEM4NTAgLTM4MiA4NDUuMzMzIC0zODAgODQwIC0zODAgQzgzNC42NjcgLTM4MCA4MzAgLTM4MiA4MjYgLTM4NiBDODIyIC0zOTAgODIwIC0zOTQuNjY3IDgyMCAtNDAwIEM4MjAgLTQwNS4zMzMgODIyIC00MTAgODI2IC00MTQgQzgzMCAtNDE4IDgzNC42NjcgLTQyMCA4NDAgLTQyMCBaIE0xMjAgLTQyMCBDMTI1LjMzMyAtNDIwIDEzMCAtNDE4IDEzNCAtNDE0IEMxMzggLTQxMCAxNDAgLTQwNS4zMzMgMTQwIC00MDAgQzE0MCAtMzk0LjY2NyAxMzggLTM5MCAxMzQgLTM4NiBDMTMwIC0zODIgMTI1LjMzMyAtMzgwIDEyMCAtMzgwIEMxMTQuNjY3IC0zODAgMTEwIC0zODIgMTA2IC0zODYgQzEwMiAtMzkwIDEwMCAtMzk0LjY2NyAxMDAgLTQwMCBDMTAwIC00MDUuMzMzIDEwMiAtNDEwIDEwNiAtNDE0IEMxMTAgLTQxOCAxMTQuNjY3IC00MjAgMTIwIC00MjAgWiBNNzIwIC00NDAgQzczMS4zMzMgLTQ0MCA3NDAuODMzIC00MzYuMTY3IDc0OC41IC00MjguNSBDNzU2LjE2NyAtNDIwLjgzMyA3NjAgLTQxMS4zMzMgNzYwIC00MDAgQzc2MCAtMzg4LjY2NyA3NTYuMTY3IC0zNzkuMTY3IDc0OC41IC0zNzEuNSBDNzQwLjgzMyAtMzYzLjgzMyA3MzEuMzMzIC0zNjAgNzIwIC0zNjAgQzcwOC42NjcgLTM2MCA2OTkuMTY3IC0zNjMuODMzIDY5MS41IC0zNzEuNSBDNjgzLjgzMyAtMzc5LjE2NyA2ODAgLTM4OC42NjcgNjgwIC00MDAgQzY4MCAtNDExLjMzMyA2ODMuODMzIC00MjAuODMzIDY5MS41IC00MjguNSBDNjk5LjE2NyAtNDM2LjE2NyA3MDguNjY3IC00NDAgNzIwIC00NDAgWiBNMjQwIC00NDAgQzI1MS4zMzMgLTQ0MCAyNjAuODMzIC00MzYuMTY3IDI2OC41IC00MjguNSBDMjc2LjE2NyAtNDIwLjgzMyAyODAgLTQxMS4zMzMgMjgwIC00MDAgQzI4MCAtMzg4LjY2NyAyNzYuMTY3IC0zNzkuMTY3IDI2OC41IC0zNzEuNSBDMjYwLjgzMyAtMzYzLjgzMyAyNTEuMzMzIC0zNjAgMjQwIC0zNjAgQzIyOC42NjcgLTM2MCAyMTkuMTY3IC0zNjMuODMzIDIxMS41IC0zNzEuNSBDMjAzLjgzMyAtMzc5LjE2NyAyMDAgLTM4OC42NjcgMjAwIC00MDAgQzIwMCAtNDExLjMzMyAyMDMuODMzIC00MjAuODMzIDIxMS41IC00MjguNSBDMjE5LjE2NyAtNDM2LjE2NyAyMjguNjY3IC00NDAgMjQwIC00NDAgWiBNNTYwIC00NjAgQzU3Ni42NjcgLTQ2MCA1OTAuODMzIC00NTQuMTY3IDYwMi41IC00NDIuNSBDNjE0LjE2NyAtNDMwLjgzMyA2MjAgLTQxNi42NjcgNjIwIC00MDAgQzYyMCAtMzgzLjMzMyA2MTQuMTY3IC0zNjkuMTY3IDYwMi41IC0zNTcuNSBDNTkwLjgzMyAtMzQ1LjgzMyA1NzYuNjY3IC0zNDAgNTYwIC0zNDAgQzU0My4zMzMgLTM0MCA1MjkuMTY3IC0zNDUuODMzIDUxNy41IC0zNTcuNSBDNTA1LjgzMyAtMzY5LjE2NyA1MDAgLTM4My4zMzMgNTAwIC00MDAgQzUwMCAtNDE2LjY2NyA1MDUuODMzIC00MzAuODMzIDUxNy41IC00NDIuNSBDNTI5LjE2NyAtNDU0LjE2NyA1NDMuMzMzIC00NjAgNTYwIC00NjAgWiBNNDAwIC00NjAgQzQxNi42NjcgLTQ2MCA0MzAuODMzIC00NTQuMTY3IDQ0Mi41IC00NDIuNSBDNDU0LjE2NyAtNDMwLjgzMyA0NjAgLTQxNi42NjcgNDYwIC00MDAgQzQ2MCAtMzgzLjMzMyA0NTQuMTY3IC0zNjkuMTY3IDQ0Mi41IC0zNTcuNSBDNDMwLjgzMyAtMzQ1LjgzMyA0MTYuNjY3IC0zNDAgNDAwIC0zNDAgQzM4My4zMzMgLTM0MCAzNjkuMTY3IC0zNDUuODMzIDM1Ny41IC0zNTcuNSBDMzQ1LjgzMyAtMzY5LjE2NyAzNDAgLTM4My4zMzMgMzQwIC00MDAgQzM0MCAtNDE2LjY2NyAzNDUuODMzIC00MzAuODMzIDM1Ny41IC00NDIuNSBDMzY5LjE2NyAtNDU0LjE2NyAzODMuMzMzIC00NjAgNDAwIC00NjAgWiBNODQwIC01ODAgQzg0NS4zMzMgLTU4MCA4NTAgLTU3OCA4NTQgLTU3NCBDODU4IC01NzAgODYwIC01NjUuMzMzIDg2MCAtNTYwIEM4NjAgLTU1NC42NjcgODU4IC01NTAgODU0IC01NDYgQzg1MCAtNTQyIDg0NS4zMzMgLTU0MCA4NDAgLTU0MCBDODM0LjY2NyAtNTQwIDgzMCAtNTQyIDgyNiAtNTQ2IEM4MjIgLTU1MCA4MjAgLTU1NC42NjcgODIwIC01NjAgQzgyMCAtNTY1LjMzMyA4MjIgLTU3MCA4MjYgLTU3NCBDODMwIC01NzggODM0LjY2NyAtNTgwIDg0MCAtNTgwIFogTTEyMCAtNTgwIEMxMjUuMzMzIC01ODAgMTMwIC01NzggMTM0IC01NzQgQzEzOCAtNTcwIDE0MCAtNTY1LjMzMyAxNDAgLTU2MCBDMTQwIC01NTQuNjY3IDEzOCAtNTUwIDEzNCAtNTQ2IEMxMzAgLTU0MiAxMjUuMzMzIC01NDAgMTIwIC01NDAgQzExNC42NjcgLTU0MCAxMTAgLTU0MiAxMDYgLTU0NiBDMTAyIC01NTAgMTAwIC01NTQuNjY3IDEwMCAtNTYwIEMxMDAgLTU2NS4zMzMgMTAyIC01NzAgMTA2IC01NzQgQzExMCAtNTc4IDExNC42NjcgLTU4MCAxMjAgLTU4MCBaIE03MjAgLTYwMCBDNzMxLjMzMyAtNjAwIDc0MC44MzMgLTU5Ni4xNjcgNzQ4LjUgLTU4OC41IEM3NTYuMTY3IC01ODAuODMzIDc2MCAtNTcxLjMzMyA3NjAgLTU2MCBDNzYwIC01NDguNjY3IDc1Ni4xNjcgLTUzOS4xNjcgNzQ4LjUgLTUzMS41IEM3NDAuODMzIC01MjMuODMzIDczMS4zMzMgLTUyMCA3MjAgLTUyMCBDNzA4LjY2NyAtNTIwIDY5OS4xNjcgLTUyMy44MzMgNjkxLjUgLTUzMS41IEM2ODMuODMzIC01MzkuMTY3IDY4MCAtNTQ4LjY2NyA2ODAgLTU2MCBDNjgwIC01NzEuMzMzIDY4My44MzMgLTU4MC44MzMgNjkxLjUgLTU4OC41IEM2OTkuMTY3IC01OTYuMTY3IDcwOC42NjcgLTYwMCA3MjAgLTYwMCBaIE0yNDAgLTYwMCBDMjUxLjMzMyAtNjAwIDI2MC44MzMgLTU5Ni4xNjcgMjY4LjUgLTU4OC41IEMyNzYuMTY3IC01ODAuODMzIDI4MCAtNTcxLjMzMyAyODAgLTU2MCBDMjgwIC01NDguNjY3IDI3Ni4xNjcgLTUzOS4xNjcgMjY4LjUgLTUzMS41IEMyNjAuODMzIC01MjMuODMzIDI1MS4zMzMgLTUyMCAyNDAgLTUyMCBDMjI4LjY2NyAtNTIwIDIxOS4xNjcgLTUyMy44MzMgMjExLjUgLTUzMS41IEMyMDMuODMzIC01MzkuMTY3IDIwMCAtNTQ4LjY2NyAyMDAgLTU2MCBDMjAwIC01NzEuMzMzIDIwMy44MzMgLTU4MC44MzMgMjExLjUgLTU4OC41IEMyMTkuMTY3IC01OTYuMTY3IDIyOC42NjcgLTYwMCAyNDAgLTYwMCBaIE01NjAgLTYyMCBDNTc2LjY2NyAtNjIwIDU5MC44MzMgLTYxNC4xNjcgNjAyLjUgLTYwMi41IEM2MTQuMTY3IC01OTAuODMzIDYyMCAtNTc2LjY2NyA2MjAgLTU2MCBDNjIwIC01NDMuMzMzIDYxNC4xNjcgLTUyOS4xNjcgNjAyLjUgLTUxNy41IEM1OTAuODMzIC01MDUuODMzIDU3Ni42NjcgLTUwMCA1NjAgLTUwMCBDNTQzLjMzMyAtNTAwIDUyOS4xNjcgLTUwNS44MzMgNTE3LjUgLTUxNy41IEM1MDUuODMzIC01MjkuMTY3IDUwMCAtNTQzLjMzMyA1MDAgLTU2MCBDNTAwIC01NzYuNjY3IDUwNS44MzMgLTU5MC44MzMgNTE3LjUgLTYwMi41IEM1MjkuMTY3IC02MTQuMTY3IDU0My4zMzMgLTYyMCA1NjAgLTYyMCBaIE00MDAgLTYyMCBDNDE2LjY2NyAtNjIwIDQzMC44MzMgLTYxNC4xNjcgNDQyLjUgLTYwMi41IEM0NTQuMTY3IC01OTAuODMzIDQ2MCAtNTc2LjY2NyA0NjAgLTU2MCBDNDYwIC01NDMuMzMzIDQ1NC4xNjcgLTUyOS4xNjcgNDQyLjUgLTUxNy41IEM0MzAuODMzIC01MDUuODMzIDQxNi42NjcgLTUwMCA0MDAgLTUwMCBDMzgzLjMzMyAtNTAwIDM2OS4xNjcgLTUwNS44MzMgMzU3LjUgLTUxNy41IEMzNDUuODMzIC01MjkuMTY3IDM0MCAtNTQzLjMzMyAzNDAgLTU2MCBDMzQwIC01NzYuNjY3IDM0NS44MzMgLTU5MC44MzMgMzU3LjUgLTYwMi41IEMzNjkuMTY3IC02MTQuMTY3IDM4My4zMzMgLTYyMCA0MDAgLTYyMCBaIE03MjAgLTc2MCBDNzMxLjMzMyAtNzYwIDc0MC44MzMgLTc1Ni4xNjcgNzQ4LjUgLTc0OC41IEM3NTYuMTY3IC03NDAuODMzIDc2MCAtNzMxLjMzMyA3NjAgLTcyMCBDNzYwIC03MDguNjY3IDc1Ni4xNjcgLTY5OS4xNjcgNzQ4LjUgLTY5MS41IEM3NDAuODMzIC02ODMuODMzIDczMS4zMzMgLTY4MCA3MjAgLTY4MCBDNzA4LjY2NyAtNjgwIDY5OS4xNjcgLTY4My44MzMgNjkxLjUgLTY5MS41IEM2ODMuODMzIC02OTkuMTY3IDY4MCAtNzA4LjY2NyA2ODAgLTcyMCBDNjgwIC03MzEuMzMzIDY4My44MzMgLTc0MC44MzMgNjkxLjUgLTc0OC41IEM2OTkuMTY3IC03NTYuMTY3IDcwOC42NjcgLTc2MCA3MjAgLTc2MCBaIE01NjAgLTc2MCBDNTcxLjMzMyAtNzYwIDU4MC44MzMgLTc1Ni4xNjcgNTg4LjUgLTc0OC41IEM1OTYuMTY3IC03NDAuODMzIDYwMCAtNzMxLjMzMyA2MDAgLTcyMCBDNjAwIC03MDguNjY3IDU5Ni4xNjcgLTY5OS4xNjcgNTg4LjUgLTY5MS41IEM1ODAuODMzIC02ODMuODMzIDU3MS4zMzMgLTY4MCA1NjAgLTY4MCBDNTQ4LjY2NyAtNjgwIDUzOS4xNjcgLTY4My44MzMgNTMxLjUgLTY5MS41IEM1MjMuODMzIC02OTkuMTY3IDUyMCAtNzA4LjY2NyA1MjAgLTcyMCBDNTIwIC03MzEuMzMzIDUyMy44MzMgLTc0MC44MzMgNTMxLjUgLTc0OC41IEM1MzkuMTY3IC03NTYuMTY3IDU0OC42NjcgLTc2MCA1NjAgLTc2MCBaIE00MDAgLTc2MCBDNDExLjMzMyAtNzYwIDQyMC44MzMgLTc1Ni4xNjcgNDI4LjUgLTc0OC41IEM0MzYuMTY3IC03NDAuODMzIDQ0MCAtNzMxLjMzMyA0NDAgLTcyMCBDNDQwIC03MDguNjY3IDQzNi4xNjcgLTY5OS4xNjcgNDI4LjUgLTY5MS41IEM0MjAuODMzIC02ODMuODMzIDQxMS4zMzMgLTY4MCA0MDAgLTY4MCBDMzg4LjY2NyAtNjgwIDM3OS4xNjcgLTY4My44MzMgMzcxLjUgLTY5MS41IEMzNjMuODMzIC02OTkuMTY3IDM2MCAtNzA4LjY2NyAzNjAgLTcyMCBDMzYwIC03MzEuMzMzIDM2My44MzMgLTc0MC44MzMgMzcxLjUgLTc0OC41IEMzNzkuMTY3IC03NTYuMTY3IDM4OC42NjcgLTc2MCA0MDAgLTc2MCBaIE0yNDAgLTc2MCBDMjUxLjMzMyAtNzYwIDI2MC44MzMgLTc1Ni4xNjcgMjY4LjUgLTc0OC41IEMyNzYuMTY3IC03NDAuODMzIDI4MCAtNzMxLjMzMyAyODAgLTcyMCBDMjgwIC03MDguNjY3IDI3Ni4xNjcgLTY5OS4xNjcgMjY4LjUgLTY5MS41IEMyNjAuODMzIC02ODMuODMzIDI1MS4zMzMgLTY4MCAyNDAgLTY4MCBDMjI4LjY2NyAtNjgwIDIxOS4xNjcgLTY4My44MzMgMjExLjUgLTY5MS41IEMyMDMuODMzIC02OTkuMTY3IDIwMCAtNzA4LjY2NyAyMDAgLTcyMCBDMjAwIC03MzEuMzMzIDIwMy44MzMgLTc0MC44MzMgMjExLjUgLTc0OC41IEMyMTkuMTY3IC03NTYuMTY3IDIyOC42NjcgLTc2MCAyNDAgLTc2MCBaIE01NjAgLTg2MCBDNTY1LjMzMyAtODYwIDU3MCAtODU4IDU3NCAtODU0IEM1NzggLTg1MCA1ODAgLTg0NS4zMzMgNTgwIC04NDAgQzU4MCAtODM0LjY2NyA1NzggLTgzMCA1NzQgLTgyNiBDNTcwIC04MjIgNTY1LjMzMyAtODIwIDU2MCAtODIwIEM1NTQuNjY3IC04MjAgNTUwIC04MjIgNTQ2IC04MjYgQzU0MiAtODMwIDU0MCAtODM0LjY2NyA1NDAgLTg0MCBDNTQwIC04NDUuMzMzIDU0MiAtODUwIDU0NiAtODU0IEM1NTAgLTg1OCA1NTQuNjY3IC04NjAgNTYwIC04NjAgWiBNNDAwIC04NjAgQzQwNS4zMzMgLTg2MCA0MTAgLTg1OCA0MTQgLTg1NCBDNDE4IC04NTAgNDIwIC04NDUuMzMzIDQyMCAtODQwIEM0MjAgLTgzNC42NjcgNDE4IC04MzAgNDE0IC04MjYgQzQxMCAtODIyIDQwNS4zMzMgLTgyMCA0MDAgLTgyMCBDMzk0LjY2NyAtODIwIDM5MCAtODIyIDM4NiAtODI2IEMzODIgLTgzMCAzODAgLTgzNC42NjcgMzgwIC04NDAgQzM4MCAtODQ1LjMzMyAzODIgLTg1MCAzODYgLTg1NCBDMzkwIC04NTggMzk0LjY2NyAtODYwIDQwMCAtODYwIFonLz4NCjwvc3ZnPg==' alt='IconBlur' />  **Pixelate** | Pixelate areas for privacy |

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