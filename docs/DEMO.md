# Reproducible demo

This walkthrough is the canonical demo until a recorded video is available. It uses only features present in the repository.

## Setup

1. Build and start the Windows or Android target using `README.md`.
2. Allow the loading screen to complete.
3. An internet connection is optional for catalogue browsing and required for a fresh weather recommendation.

## Walkthrough

1. On **Discover**, use `PREV`, `NEXT`, or a horizontal swipe to move through the deterministic hero items.
2. Select **Singapore** or **Qingdao** and observe the weather card. The selected seasonal edit uses the upcoming local night temperature; a failed request leaves manual browsing available.
3. In **Seasonal comfort**, use the side previews, buttons, or swipe to move through Spring, Summer, Autumn, and Winter. Manual selection is not replaced by an older pending weather response.
4. Open a featured or listed product and inspect its product detail page.
5. Save the product, then open **Saved** and verify that it persists after leaving the page.
6. Open **Shop** to browse the seeded categories.
7. Return to **Discover**, search for a product name or material, and open a suggestion.
8. Switch the operating-system theme and confirm the seasonal surfaces remain readable.

## Screenshot set

Capture from a real build and place files in `docs/screenshots/` using these names:

- `01-discover.png`
- `02-seasonal-edit.png`
- `03-product-detail.png`
- `04-search.png`
- `05-saved.png`
- `06-android-phone.png`

Record the commit SHA, platform, device/window size, and capture date in `docs/screenshots/README.md`. Do not use design references or generated mockups as evidence of the running application.
