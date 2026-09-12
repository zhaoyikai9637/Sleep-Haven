# Seasonal browsing design

The home page now treats a season as a focused shopping scene, not a stack of expandable cards. A wide layout shows a foreground product photograph with subdued previews of the neighbouring seasons. Below it, the current season's products sit on a quieter second surface. Narrow layouts hide the side previews and retain explicit Previous and Next controls.

| Season | Light scene | Light text | Action colour | Dark scene |
| --- | --- | --- | --- | --- |
| Spring | `#DCE9DF` | `#213F34` | `#456955` | `#213B32` |
| Summer | `#D8E9EC` | `#1D3B49` | `#416675` | `#1C3640` |
| Autumn | `#E8DED4` | `#48382F` | `#705844` | `#3E312C` |
| Winter | `#DDE3EB` | `#293950` | `#4E637D` | `#26364B` |

The season colours are intentionally muted. Measured light-theme contrast ranges from 8.41:1 to 9.46:1 for titles, 4.78:1 to 5.00:1 for accent text, and at least 6.17:1 for white text on action buttons. Dark-theme foregrounds also exceed 10:1 against their scene backgrounds.

Switching the season changes the scene, image, neighbouring previews, product list, and count together. It is entirely user-driven via buttons, side previews, or horizontal swipes. There is no `CarouselView` or automatic timer. The weather result selects an initial season, but a manual season choice takes precedence over a pending weather response. Choosing a different city deliberately requests a new recommendation.

Product images are already part of this repository; no game artwork or new downloaded dependencies are used. The surrounding navigation remains SleepHaven blue, so the seasonal colours communicate browsing context instead of changing the entire application theme.
