# Local data migration policy

SleepHaven uses one stable database name, `SleepHaven.db3`, and SQLite's `PRAGMA user_version` as the schema version. The current schema version is `2`.

## Version 1 / legacy import

Earlier application builds stored products in `SleepHaven_v8.db3` and encoded price and category as display strings such as `$129.00` and `BeddingSets - Spring`.

On first launch of the new build:

1. if `SleepHaven.db3` does not exist and `SleepHaven_v8.db3` does, the legacy database is copied to the stable path;
2. favourite states are read by product ID;
3. the legacy product table is replaced with the structured schema;
4. current seed products are inserted with `Price`, `Currency`, `ProductType`, `Season`, and `Material` fields;
5. favourites for matching product IDs are restored;
6. `PRAGMA user_version` is set to `2`.

The legacy file is not deleted, so it remains available as a local recovery copy. No customer identity, account, payment, or analytics data is stored.

## Seed catalogue updates

Seed synchronization is keyed by the stable product ID. Updating product descriptions or adding products must not change an existing ID. Before replacing seed rows, the migration layer reads `Id` and `IsFavorite`, then reapplies the favourite state to matching IDs.

Removing or changing an ID intentionally removes the association with the old favourite. Such a change must be called out in release notes and covered by a migration test when retention is required.

## Future schema changes

For every persistent schema change:

- increment `DatabaseService.CurrentSchemaVersion`;
- add a forward-only migration from the previous version;
- preserve user-owned state separately from replaceable catalogue data;
- add an integration test that constructs the previous schema and verifies the migrated result;
- do not use a new database filename as the migration mechanism.
