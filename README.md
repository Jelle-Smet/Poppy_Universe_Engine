<div style="display: flex; align-items: center; gap: 10px;">
  <img src="Images/Poppy_Universe_Shield.png" alt="Poppy Universe Logo" width="100" style="margin-top: -5px;"/>
  <h1>Poppy Universe - Recommendation Engine</h1>
</div>


> **Personalized recommendations, layered intelligence, all in C#**

This repository contains the **Recommendation Engine** for the **Poppy Universe** project. It powers the user recommendation system using **layered calculations**. A combination of rule-based, with / or advanced Mathematical-based layers.

---

## 🚀 Tech Stack

* **Language:** C#
* **Framework:** .NET (Standard C# Project)
* **Architecture:** Layered Recommendation Engine
    * **Layer 1 & 2:** Rule-based recommendations
    * **Layer 3 & 4:** Matihimatical-based recommendations
    * **Layer 5:** Business Logic recommendations (combination of layer 1,2,3 & 4)

---

## 📂 Project Structure

```tree
Recommendation_Engine/
├── Poppy_Universe_Engine/      # C# project containing engine logic
│   ├── Layer1.cs               # Personalized Rule-based calculations
│   ├── Layer2.cs               # Popularity Rule-based calculations
│   ├── Layer3.cs               # Matrix Factorization calculations
│   ├── Layer4.cs               # Neural Network calculations
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

# 🌌 Layer 3 — Matrix Factorization  

> Layer 3 is the **type-level recommendation layer**. Its focus is on **semantic patterns across categories** like star types, planet types, and moon parent planets, rather than individual objects.  
>
> - **Purpose:** Capture **user preferences at the category level**, e.g., someone loves Gas Giants or M-type stars, and predict scores for unseen categories.  
>
> - **Inputs used:**  
>   - Processed user interactions from **Database** (or raw interaction logs)  
>   - Category definitions: Star types (O/B/A/F/G/K/M), Planet types (Terrestrial/Gas Giant/Ice Giant/Dwarf), Moon parent planets  
>   - Interaction strength (1–5)  
>
> - **How it works:**  
>   1. Builds a **User × Category matrix** where each row = user, each column = category, values = interaction strength.  
>   2. Fills missing values with 0 and optionally normalizes scores.  
>   3. Performs **matrix factorization** (collaborative filtering) to learn **latent features** representing user preferences and category characteristics.  
>   4. Uses the factorized matrices to **predict missing interactions**, i.e., estimate how likely a user is to like a category they haven’t interacted with yet.  
>
> - **Returns:**  
>   - A **predicted score for every user × category** combination  
>   - Can be used to **rank categories** for recommendation or feed into Layer 4 for more advanced modeling  
>
> - **Visibility info:**  
>   - Inherits **visibility percentages** from earlier layers (informational only)  
>   - Helps explain why a category might be more relevant to a user at a given time  

# 🌠 Layer 4 — Neural Network  

> Layer 4 is the **advanced neural network layer**. Its focus is on **learning complex, nonlinear relationships** between users and categories to refine recommendations beyond what Layer 3 captures.  
>
> - **Purpose:** Improve prediction accuracy by capturing subtle patterns and interactions in user behavior.  
>
> - **Inputs used:**  
>   - Processed user interactions from **Database** (or raw interaction logs)  
>   - One-hot encoding for users and categories  
>   - Interaction strength (1–5) as training labels  
>
> - **How it works:**  
>   1. **Encodes users and categories** as input vectors for the neural network.  
>   2. **Forward pass:** computes predicted interaction scores through multiple layers with nonlinear activations (tanh) to model complex patterns.  
>   3. **Loss calculation:** measures prediction error against actual interaction strengths.  
>   4. **Backpropagation:** updates weights and biases using gradient descent to minimize loss.  
>   5. **Training loop:** iterates for multiple epochs until the network converges, optionally using mini-batches for efficiency.  
>   6. Generates **predicted scores for all user × category combinations**, including previously unseen interactions.  
>
> - **Returns:**  
>   - Refined **user × category predictions**  
>   - Can be **ranked to recommend top categories** per user  
>   - Output saved as CSV for integration with the recommendation engine  
>
> - **Visibility info:**  
>   - Maintains **visibility percentages** from previous layers (informational only)  
>   - Layer 4 predictions can optionally be weighted by visibility for final presentation

# 🧬 Layer 5 — Genetic Algorithm Rank Fusion

> Layer 5 is the **consensus-based fusion layer**. Its focus is on **intelligently combining the rankings** from all previous layers (1-4) to produce a final, optimized recommendation list that balances multiple recommendation strategies.
>
>- **Purpose:** Find the optimal way to combine personalized, trending, matrix factorization, and neural network recommendations into a single unified ranking that minimizes disagreement across all approaches.
>
>- **Inputs used:**  
>  - Ranked lists from **Layer 1** (personalized scores)  
>  - Ranked lists from **Layer 2** (trend-boosted scores)  
>  - Ranked lists from **Layer 3** (matrix factorization predictions)  
>  - Ranked lists from **Layer 4** (neural network predictions)  
>  - Object metadata (type, brightness, visibility) from previous layers
>
>- **How it works:**  
>  1. **Converts layer outputs to rank positions**: Each object gets a rank (0 = best) in each of the four layers.  
>  2. **Genetic Algorithm optimization**: Evolves a population of weight combinations (W₁, W₂, W₃, W₄) over multiple generations to find the optimal fusion strategy.  
>  3. **Fitness function**: Minimizes the variance between the final weighted rank and individual layer ranks, finding the ranking that best represents consensus across all layers.  
>  4. **Independent optimization**: Runs three separate GA processes for Stars, Planets, and Moons since different object types may benefit from different layer weightings.  
>  5. **Constraint enforcement**: All weights sum to 1.0 and remain non-negative throughout evolution.  
>  6. **GA operators**:  
>     - **Initialization**: Random weights normalized to sum to 1.0  
>     - **Selection**: Tournament selection + elitism (top 15% survive)  
>     - **Crossover**: Blended average crossover with random alpha  
>     - **Mutation**: Gaussian perturbation with re-normalization  
>  7. Computes **final scores** using the optimal weights: `FinalScore = W₁×Rank₁ + W₂×Rank₂ + W₃×Rank₃ + W₄×Rank₄`
>
>- **Returns:**  
>  - **Three final ranked lists** (Stars, Planets, Moons), each sorted by optimized consensus score  
>  - **Optimal weight combinations** for each object type showing which layers contributed most  
>  - Each object includes its **rank in all 4 layers** for transparency and debugging
>
>- **Visibility info:**  
>  - Inherits **visibility percentages** and explanations from Layer 1 (informational only)  
>  - Final rankings can be filtered or sorted by visibility for real-time observation planning
>
>- **Why GA over simple averaging?**  
>  - Different layers may be more reliable for different object types (e.g., trending might work better for bright planets, personalization for faint stars)  
>  - GA automatically discovers the optimal balance without manual tuning  
>  - Minimizing disagreement creates rankings that are robust and well-supported by multiple algorithms  
>  - Adapts to data: if one layer has poor quality rankings, GA naturally downweights it

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

* Fully integrate with **backend, frontend, and ML modules**.
* Turn this into the **complete Poppy Universe project repo**, containing engine, frontend, backend, data, and ML.

---

## 🛠 Author

**Jelle Smet**



<p align="center">
  <img src="Images/Poppy_Universe_Logo.png" alt="Poppy Universe Logo" width="600"/>
</p>