# Asset provenance audit

Asset rights are a release blocker until the maintainer supplies evidence. This document records only what is verifiable from the repository.

| Asset group | Repository location | Current evidence | Required action |
| --- | --- | --- | --- |
| Product and bedroom photographs | `Resources/Images/*.png` | No source, creator, licence, or permission record is committed | Add source and redistribution permission for every image, or replace/remove it |
| Navigation icons | `Resources/Images/*_on.png`, `*_off.png` | No provenance file is committed | Confirm original authorship or add the applicable licence/source |
| Application icon and splash artwork | `Resources/AppIcon/`, `Resources/Splash/` | Present in source; authorship is not documented | Record creator and permission |
| Open Sans font files | `Resources/Fonts/` | Font binaries are present; licence text is not committed | Verify the exact font distribution and include its required licence notice |

Design references listed in `docs/DESIGN_RESEARCH.md` are research links, not permission to copy third-party artwork. Persona artwork is not included as application content by the seasonal design implementation.

Do not publish a GitHub Release containing these assets until the table is resolved.
