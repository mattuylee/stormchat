DROP TABLE IF EXISTS `message`;
CREATE TABLE `message` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `When` varchar(44) NOT NULL,
  `From` varchar(12) NOT NULL,
  `To` varchar(12) NOT NULL,
  `Msg` text,
  PRIMARY KEY (`Id`)
) AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

LOCK TABLES `message` WRITE;
UNLOCK TABLES;

DROP TABLE IF EXISTS `user`;
CREATE TABLE `user` (
  `User` varchar(12) NOT NULL,
  `Pwd` varchar(16) NOT NULL,
  `NickName` varchar(36) NOT NULL DEFAULT '',
  `Motto` varchar(144) NOT NULL DEFAULT '',
  `UGroup` enum('User','Vip','Admin','Group') DEFAULT 'User',
  `Photo` mediumtext,
  PRIMARY KEY (`User`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

LOCK TABLES `user` WRITE;
INSERT INTO `user` VALUES ('mattuy','mattuy','Mattuy','','Admin',NULL);
UNLOCK TABLES;

