# Boole-algebra és Igazságtábla Elemző Segédeszköz

Ez a projekt a **Tutorálás** tárgy keretében készült oktatási segédeszközként mérnökinformatikus hallgatók számára, a **Digitális technika** tantárgy logikai alapozásának támogatására.

## Funkcionalitás
- **Szintaktikai elemzés:** Tetszőleges logikai kifejezések feldolgozása a Shunting-yard algoritmus segítségével (Infix formából Reverse Polish Notation / RPN formátumba konvertálva).
- **Automatikus változófelismerés:** Detektálja az egyedi változókat (pl. A, B, C).
- **Igazságtábla generálás:** Legenerálja a $2^n$ soros állapotteret és kiszámítja a hozzá tartozó kimeneteket.
- **Algebrai értékelés:** Kigyűjti a mintermeket ($\Sigma m$), maxtermeket ($\Pi M$), és felírja a kanonikus diszjunktív normálformát (SOP).

## Támogatott operátorok
- `!` vagy `~` : Tagadás (NOT)
- `&` : ÉS művelet (AND)
- `|` : VAGY művelet (OR)
- `^` : Kizáró VAGY (XOR)
- `(` és `)` : Zárójelek

## Futtatás lokálisan
Követelmény: **.NET 8 SDK**

1. Klónozd a tárolót és lépj be a könyvtárba:
   ```bash
   git clone https://github.com/FordUlok/bool-eval-edu.git
   cd bool-eval-edu
   ```

2. Fordítsd le és indítsd el az alkalmazást:
   ```bash
   dotnet run
   ```