# NutriTrack Shared

This package contains the shared code used by both NutriTrack mobile and desktop apps:

- Core HTTP client (`ApiClient`) and error handling
- Token storage (`TokenStore`)
- Common models (groceries, exercises, auth)
- Repositories for calling the main NutriTrack API

It does not contain any UI code or state management.  
Both apps reference this package via a local path dependency.
