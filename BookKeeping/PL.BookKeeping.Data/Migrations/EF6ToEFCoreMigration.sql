CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

INSERT INTO __efmigrationshistory
VALUES("20190127110421_InitialCreate", "2.2.1-servicing-10028"),
("20190127123417_EntryActivePeriod", "2.2.1-servicing-10028"),
("20190127125055_AddDateRules", "2.2.1-servicing-10028");

DROP TABLE __migrationhistory;
