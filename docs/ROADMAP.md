# Roadmap and issue-ready backlog

This roadmap describes intended work, not shipped functionality. Priorities may change as evidence and contributors emerge.

## Release blockers for v1.0.0

- [x] **Choose an open-source licence.** MIT was selected by the rights holder on 17 September 2026; see `LICENSE_DECISION.md`.
- [ ] **Document or replace every bundled image and font** using `ASSET_PROVENANCE.md`.
- [ ] **Capture real Windows and Android screenshots** using `DEMO.md`.
- [ ] **Run the signed APK on at least one physical ARM64 Android device** and record model, OS, install result, and demo results.
- [ ] **Enable GitHub private vulnerability reporting** or publish another private security contact.

## Near-term engineering

- [ ] **Add automated tests for seasonal recommendation boundaries.** Cover tropical behaviour, night-temperature thresholds, month boundaries, and stale-request protection.
- [ ] **Extract weather and navigation state from page code-behind.** Make recommendation logic and page state independently testable.
- [ ] **Add accessibility checks.** Verify screen-reader names, keyboard focus order, contrast, text scaling, and reduced-motion behaviour.
- [ ] **Add a seed-data validation check.** Require unique IDs, existing image references, parseable prices, and supported seasonal categories.
- [ ] **Define local-data migration policy.** Document when the SQLite file version changes and how favourites are preserved.

## Distribution and maintenance

- [ ] **Publish v1.0.0 only after release blockers close.** Attach checksums and signed Android/Windows artifacts and preserve the signing key offline.
- [ ] **Add Apple-target CI when a macOS runner budget is justified.** Current CI intentionally validates only Windows and Android.
- [ ] **Create an issue-triage cadence.** Label new reports, reproduce accepted bugs, and publish release milestones.
- [ ] **Measure real usage ethically.** Prefer opt-in, privacy-preserving evidence such as release downloads, external issues, or documented merchant pilots.

## Possible commerce work, not committed

Checkout, payments, inventory, order tracking, merchant administration, user accounts, price alerts, and reviews are outside the current product. Each needs its own domain model, security review, privacy plan, and verified integration before it can appear as a working feature.
