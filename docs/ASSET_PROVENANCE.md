# Asset provenance audit

This register deliberately separates the MIT-licensed source code from bundled non-code assets. Commercial use by a shop is useful provenance evidence, but does not by itself prove that the shop owns a photograph or may sublicense it in a public repository.

| Asset group | Repository location | Current evidence | Required action |
| --- | --- | --- | --- |
| Product and bedroom photographs | `Resources/Images/*_bedroom.png`, `Resources/Images/*_folded.png` | On 17 September 2026, maintainer Zhao Yikai stated that the images were provided by family members who operate a commercial home-textile shop | Complete the shop/rights-holder record below and retain written confirmation that public repository and application redistribution are allowed |
| Navigation icons | `Resources/Images/*_on.png`, `*_off.png` | No provenance file is committed | Confirm original authorship or add the applicable licence/source |
| Application icon and splash artwork | `Resources/AppIcon/`, `Resources/Splash/` | Present in source; authorship is not documented | Record creator and permission |
| Open Sans font files | `Resources/Fonts/` | Font binaries are present; licence text is not committed | Verify the exact font distribution and include its required licence notice |

## Product photograph record

The following record applies to the paired bedroom and folded product photographs currently stored in `Resources/Images/`.

| Field | Record |
| --- | --- |
| Provider | Family members of maintainer Zhao Yikai |
| Business context | An operating commercial home-textile shop |
| Date recorded | 17 September 2026 |
| Intended project use | Display in the SleepHaven catalogue and packaged application |
| Shop or legal rights-holder name | Not yet recorded |
| Photographer/original creator | Not yet recorded |
| Rights chain | Not yet confirmed whether the shop created, commissioned, purchased, or received the images from a supplier |
| Public redistribution permission | Written confirmation not yet stored |
| Evidence location | Add non-sensitive evidence or a reference to privately retained evidence |

Before a formal public release, the maintainer should complete [`ASSET_PERMISSION_RECORD.md`](ASSET_PERMISSION_RECORD.md). If the shop received an image from a manufacturer or distributor, that party's terms must allow the image to be copied into a public repository and redistributed inside the application. If that cannot be confirmed, replace the affected image with shop-owned photography.

Design references listed in `docs/DESIGN_RESEARCH.md` are research links, not permission to copy third-party artwork. Persona artwork is not included as application content by the seasonal design implementation.

Do not describe unresolved assets as MIT-licensed. A formal GitHub Release containing them should wait until the rights-holder, rights chain, and redistribution permission fields are completed.
