<div style="display: flex; align-items: center; gap: 10px;">
  <img src="../Images/Poppy_Universe_Logo.png" alt="Poppy Universe Logo" width="100" style="margin-top: -5px;"/>
  <h1>Poppy Universe - Recommendation Engine</h1>
</div>


> **Personalized recommendations, layered intelligence, all in C#**

This repository contains the **Recommendation Engine** for the **Poppy Universe** project. It powers the user recommendation system using **layered calculations**. Currently rule-based, with advanced matrix-based layers planned for the future.

---

## 🚀 Tech Stack

* **Language:** C#
* **Framework:** .NET (Standard C# Project)
* **Architecture:** Layered Recommendation Engine
    * **Layer 1 & 2:** Rule-based recommendations
    * **Layer 3 & 4:** ...

---

## 📂 Project Structure

```tree
Recommendation_Engine/
├── Poppy_Universe_Engine/      # C# project containing engine logic
│   ├── Layer1.cs               # Personalized Rule-based calculations
│   ├── Layer2.cs               # Popularity Rule-based calculations
│   ├── Layer3.cs               # Placeholder for future enhancements
│   ├── Layer...                # Placeholder for future enhancements
│   ├── WeatherWhecker.cs       # Weather conditions checker
│   └── Program.cs              # Entry point
├── .gitignore
└── README.md
```

---


## 🏗️ Layer Explanations

### 🌓 Layer 1 — Personalized Objects

> Layer 1 is the **core personalized recommendation layer**. Its focus is on **individual user preferences** and calculating a **matching score** for each celestial object.
>
> - **Recommended objects:** Moons, stars, planets  
> - **Inputs used:**  
>   - User location  
>   - (Current) date and time  
>   - User’s liked objects  
>
> - **How it works:**  
>   1. Calculates the **exact location** of each object and checks its **visibility**.  
>   2. Uses a **weighted rule-based scoring system** with **various variables**:  
>      - Example variables: Brightness has a weight of 5 points, calculated as `5 - brightness` (lower number = higher brightness), and whether the user has liked the object.  
>      - These are just examples; the layer uses multiple other factors to compute the score.    
>   3. Produces a **matching score and matching percentage** for each object.
>
> - **Returns:**  
>   - A **list of all visible objects**, ranked from **highest matching score to lowest**.
>
> - **Visibility info:**  
>   - Each recommendation includes a **visibility percentage** (chance the object is visible in the next 12 hours) and an **explanation** for this percentage.  
>   - This is **informational only** and does not affect the score.
>   - For more information check **Visibility Info** under layer Explanations

### 🌓 Layer 2 — Poppy's Trend Booster

> Layer 2 is the **trend-based recommendation layer**. Its focus is on **popular interactions** and **trending celestial objects**, enhancing the personalized recommendations from Layer 1.
>
> - **Purpose:** Boost the scores of objects that are trending among users or have high interaction rates, based on **machine-learned signals** rather than raw interactions.
>
> - **Inputs used:**  
>   - Recommendations from **Layer 1** (objects + scores)  
>   - **Processed interaction and popularity data** for objects (total interactions, trending score), handled and normalized by ML models
>
> - **How it works:**  
>   1. Calculates a **weighted boost** for each object:  
>      - **User interactions** (60% weight): How much an object has been interacted with by the community (ML-processed).  
>      - **Trending score** (40% weight): Normalized score indicating current popularity (ML-processed).  
>   2. Caps the **maximum boost** at 25% of the maximum possible score for objects already in Layer 1.  
>   3. Objects missing in the processed interaction data are **set to 0 boost**.  
>   4. Updates the **final score, match percentage**, and sets a **BoostDescription** (e.g., "Boosted by 10%" or "No boost").
>
> - **Returns:**  
>   - **Top N stars, planets, and moons** per type (default 5), ranked by final boosted score.  
>   - Optionally, a **combined top N list** across all object types.
>
> - **Visibility info:**  
>   - Inherits **visibility percentages** and explanations from Layer 1 (informational only, does not affect the score).


### 🚀 Layer 3 — Matrix-Based Intelligence (Future)
...
### 🌠 Layer 4+ — Future Extensions
...

---

## 🌟 Visibility Info
Each recommendation includes **visibility information**:  
- Uses an external API to get weather predictions.  
- Calculates the chance (percentage) that a moon, star, or planet will be visible in the next 12 hours.  
- This is **informational only**, it does **not influence the recommendation score**.  
- Each recommendation explains why it received that percentage.

---

## ⚙️ Setup & Usage

> **Important:** This engine relies on data provided by the backend, so it’s designed to run **as part of the full Poppy Universe project**. Standalone execution is possible for testing but may not produce meaningful recommendations.

1.  **Clone** this repository:
    ```bash
    git clone [https://github.com/Jelle-Smet/Poppy_Universe_Engine.git](https://github.com/Jelle-Smet/Poppy_Universe_Engine.git)
    ```
2.  Open **Poppy_Universe_Engine** in **Visual Studio**.
3.  **Build** the project as a standard C# project.
4.  Run `Program.cs` to see the engine in action.

*In the full project, the backend will call this engine dynamically with real user data.*

---

## 🌟 Future Plans

* Add **matrix-based recommendation layers** (Layer 3 & 4).
* Fully integrate with backend, frontend, and ML modules.
* Turn this into the **complete Poppy Universe project repo**, containing engine, frontend, backend, data, and ML.

---

## 🛠 Author

**Jelle Smet**



<p align="center">
  <img src="../Images/Poppy_Universe_Logo.png" alt="Poppy Universe Logo" width="600"/>
</p>