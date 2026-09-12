# SleepHaven design research

## Design direction

SleepHaven translates the atmosphere of Persona 3 Reload into a calm retail system rather than copying game artwork or branded assets. The useful principles are a controlled blue spectrum, asymmetrical planes, editorial numbering, direct typography, clean urban lines, and a softer water-like sense of depth. ATLUS describes Reload as a modern graphical overhaul with a signature stylish UI. Its art direction has also been described as combining seaside and death-associated blues, clean urban lines, and the soft expression of water.

The resulting direction is **deep-water editorial commerce**: midnight blue establishes quiet, clear electric blue communicates action, pale blue creates air, and full-bleed photography carries the emotional weight. Angular planes and numbered labels create identity without making product information difficult to scan.

## Sample set

The commercial review covered 12 international references across specialist bedding, home retail, and high-scale shopping apps:

| Market | Reference | Pattern worth carrying forward |
| --- | --- | --- |
| Global | [IKEA app](https://www.ikea.com/gb/en/ikea-app/) | Inspiration, personalised recommendations, lists, availability, order tracking, and room visualisation |
| United States | [Wayfair app](https://play.google.com/store/apps/details?id=com.wayfair.wayfair) | Personalised feed, app-only offers, price-drop signals, and shareable lists |
| United States | [Brooklinen](https://www.brooklinen.com/collections/sheet-bundles) | Simple bundles, explicit savings, fabric choice, and complete-bed shopping |
| United States | [Parachute](https://parachutehome.com/) | Fabric education, best sellers, material-led navigation, and editorial photography |
| United States | [Cozy Earth](https://cozyearth.com/collections/all) | Dense category filtering and bundle discovery |
| United States | [West Elm](https://www.westelm.com/pages/ideas-and-advice/room-planner/) | Room planning, saved ideas, and coordinated product presentation |
| United States | [Pottery Barn](https://www.potterybarn.com/customer-service/registry-faq.html) | Cross-brand lists, in-store scanning, and registry management |
| Japan | [MUJI app](https://www.muji.com/jp/ja/service/app/) | Inventory visibility, store pickup, favourites, price and low-stock alerts, and useful editorial content |
| Japan | [Nitori app](https://www.nitori-net.jp/ec/characteristic/App) | Store mode, image search, pickup state, and location-aware product finding |
| Spain / Global | [Zara Home](https://www.zarahome.com/us/bedroom-basic-sheets-n1799) | Material, thread count, colour, mattress depth, and immediate add actions |
| Sweden / Global | [H&M Home](https://www2.hm.com/en_us/home/shop-by-product/bed-linen.html) | Size, colour, material, product-type filters, favourites, and alternate grid density |
| United Kingdom | [DUSK](https://dusk.com/collections/natural-bedding) | Complete bundles, breathable-material education, and seasonal comfort framing |

## Shared commercial patterns

1. **Start from the sleeping need, not the catalogue structure.** Successful experiences lead with inspiration, room or climate context, feel, and material before exposing the full assortment.
2. **Make comparison effortless.** Material, size, season, care, price, colour, and availability need a stable position and consistent hierarchy.
3. **Reduce whole-bed complexity.** Bundles and coordinated edits convert better than asking people to assemble every layer independently.
4. **Keep intent across sessions.** Favourites, lists, low-stock alerts, and price-drop signals turn browsing into a durable shopping journey.
5. **Put reassurance beside the decision.** Delivery thresholds, returns or comfort trials, certifications, and care guidance belong near product actions.
6. **Use editorial imagery as navigation.** The strongest brands let styled rooms create desire, then attach clear product paths to the scene.

## Changes applied in this iteration

- Replaced passive white-card stacking with full-width blue fields, asymmetrical planes, editorial numbers, and line-based product rows.
- Removed automatic hero advancement. The hero now supports swipe plus explicit previous and next controls.
- Restored the original pale-aqua to deep-blue loading screen and its three-second pacing.
- Added a delivery threshold, comfort-promise signal, material and care reassurance, product counts, contextual weather merchandising, and a prominent save action.
- Preserved search, category browsing, seasonal recommendations, product detail, and local favourites while giving each surface a distinct visual role.

## Next commercial capabilities

The current data model is a local catalogue, so checkout, inventory, colour variants, real sizes, reviews, bundles, delivery estimates, price alerts, and order tracking should not be presented as working features yet. Those should be added only with corresponding domain models and persistence or commerce APIs.

## Persona 3 Reload sources

- [Persona 3 Reload official western website](https://persona.atlus.com/p3r/index.html?lang=en)
- [Persona 3 Reload official Japanese website](https://p3re.jp/en/)
- [Art direction interview summary](https://personacentral.com/p3r-art-design-direction-interview/)
