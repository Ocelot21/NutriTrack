# NutriTrack (Backend)

## Seeded login credentials
The database seeder creates a few predefined accounts you can use to log in:

- Username: `user` / Password: `user`
- Username: `desktop` / Password: `desktop`
- Username: `mobile` / Password: `mobile`
- Username: `admin` / Password: `admin`

## Grocery recommendations
Grocery recommendations are implemented in the infrastructure layer by the `GroceryRecommender` service (`src/NutriTrack.Infrastructure/Services/Groceries/GroceryRecommender.cs`).
It fetches a candidate set of groceries using existing repository filters/visibility rules, then ranks them in-memory.
Ranking uses a goal-aware heuristic: it determines the user’s current (in-progress) nutrition goal (falling back to the profile goal) and prefers groceries whose macros better match that goal.
It also incorporates cohort popularity by counting how often groceries are consumed by users with the same goal type, and uses that as an additional signal when ordering results.
