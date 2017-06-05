DELETE FROM Transactions;
DELETE FROM ProcessingRules;
UPDATE Entry SET ParentEntryKey = null;
DELETE FROM Entry;
DELETE FROM Users;

ALTER TABLE Users auto_increment = 1;
ALTER TABLE Entry auto_increment = 1;
ALTER TABLE ProcessingRules auto_increment = 1;