# Roadmap and issue-ready backlog

This roadmap describes intended work, not shipped functionality. Priorities may change as evidence and contributors emerge.

## Release blockers for v1.0.0

- [x] **Choose an open-source licence.** MIT was selected by the rights holder on 17 September 2026; see `LICENSE_DECISION.md`.
- [ ] **Document or replace every bundled image and font** using `ASSET_PROVENANCE.md`.
- [ ] **Capture real Windows and Android screenshots** using `DEMO.md`.
- [ ] **Run the signed APK on at least one physical ARM64 Android device** and record model, OS, install result, and demo results.
- [ ] **Enable GitHub private vulnerability reporting** or publish another private security contact.

## Near-term engineering

- [x] **Add automated tests for seasonal recommendation boundaries.** Tropical behaviour, temperature/month boundaries, request cancellation, and latest-navigation protection are covered.
- [x] **Extract weather, catalogue, search, and navigation behaviour from page code-behind.** These behaviours now live behind injectable, independently testable services.
- [ ] **Add accessibility checks.** Verify screen-reader names, keyboard focus order, contrast, text scaling, and reduced-motion behaviour.
- [x] **Add a seed-data validation check.** CI validates unique IDs, image references, non-negative decimal prices, and supported taxonomy values.
- [x] **Define local-data migration policy.** `DATA_MIGRATIONS.md` documents versioning, legacy import, and favourite preservation.

## Distribution and maintenance

- [ ] **Publish v1.0.0 only after release blockers close.** Attach checksums and signed Android/Windows artifacts and preserve the signing key offline.
- [ ] **Add Apple-target CI when a macOS runner budget is justified.** Current CI intentionally validates only Windows and Android.
- [ ] **Create an issue-triage cadence.** Label new reports, reproduce accepted bugs, and publish release milestones.
- [ ] **Measure real usage ethically.** Prefer opt-in, privacy-preserving evidence such as release downloads, external issues, or documented merchant pilots.

## Possible commerce work, not committed

Checkout, payments, inventory, order tracking, merchant administration, user accounts, price alerts, and reviews are outside the current product. Each needs its own domain model, security review, privacy plan, and verified integration before it can appear as a working feature.
