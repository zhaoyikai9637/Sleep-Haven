# Security Policy

## Supported versions

SleepHaven has no tagged production release yet. Security fixes currently target the latest commit on `main`. A supported-version table will be added with the first release.

## Reporting a vulnerability

Do not publish exploit details, secrets, personal data, or a proof of concept in a public issue.

Preferred reporting path:

1. Open the repository's **Security** tab.
2. Choose **Report a vulnerability** to create a private security advisory, if private vulnerability reporting is enabled.
3. Include the affected commit, platform, reproduction conditions, impact, and a minimal proof of concept.

If private reporting is unavailable, open a public issue titled `Request private security contact` without vulnerability details. The maintainer will provide an appropriate private channel when possible.

This is a single-maintainer prototype. Reports will be acknowledged and assessed as capacity allows; no response or remediation SLA is currently promised. Valid reports will be credited unless the reporter requests otherwise.

## Scope

Relevant reports include:

- unsafe local database or file handling;
- vulnerable dependency use with a practical impact on SleepHaven;
- network-response handling that permits code execution or data exposure;
- signing, package-integrity, or update-path weaknesses;
- accidental publication of secrets or personal information.

General product suggestions and availability problems belong in the public issue tracker.
