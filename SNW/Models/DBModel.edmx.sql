




-- -----------------------------------------------------------
-- Entity Designer DDL Script for MySQL Server 4.1 and higher
-- -----------------------------------------------------------
-- Date Created: 04/14/2014 12:49:00
-- Generated from EDMX file: C:\Users\Donatas\Source\Repos\SoundsNiceWholefoods\SNW\SNW\Models\DBModel.edmx
-- Target version: 3.0.0.0
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- NOTE: if the constraint does not exist, an ignorable error will be reported.
-- --------------------------------------------------

--    ALTER TABLE `CustomerOrderLines` DROP CONSTRAINT `FK_ProductCustomerOrderLine`;
--    ALTER TABLE `Products` DROP CONSTRAINT `FK_ProductCategoryProduct`;
--    ALTER TABLE `Products` DROP CONSTRAINT `FK_ProductApprovalProduct`;
--    ALTER TABLE `SupplierOrders` DROP CONSTRAINT `FK_SupplierSupplierOrder`;
--    ALTER TABLE `CustomerOrderLines` DROP CONSTRAINT `FK_CustomerOrderCustomerOrderLine`;
--    ALTER TABLE `CustomerOrders` DROP CONSTRAINT `FK_CustomerCustomerOrder`;
--    ALTER TABLE `CustomerOrders` DROP CONSTRAINT `FK_CustomerOrderStateCustomerOrder`;
--    ALTER TABLE `CustomerOrders` DROP CONSTRAINT `FK_CustomerOrderPaymentMethodCustomerOrder`;
--    ALTER TABLE `Customers` DROP CONSTRAINT `FK_CustomerRoleCustomer`;
--    ALTER TABLE `SupplierOrders` DROP CONSTRAINT `FK_SupplierOrderOrderState`;
--    ALTER TABLE `CustomerOrderLines` DROP CONSTRAINT `FK_SupplierOrderCustomerOrderLine`;

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------
SET foreign_key_checks = 0;
    DROP TABLE IF EXISTS `Products`;
    DROP TABLE IF EXISTS `ProductCategories`;
    DROP TABLE IF EXISTS `ProductApprovals`;
    DROP TABLE IF EXISTS `SupplierOrders`;
    DROP TABLE IF EXISTS `CustomerOrders`;
    DROP TABLE IF EXISTS `OrderStates`;
    DROP TABLE IF EXISTS `CustomerOrderPaymentMethods`;
    DROP TABLE IF EXISTS `CustomerOrderLines`;
    DROP TABLE IF EXISTS `Suppliers`;
    DROP TABLE IF EXISTS `Customers`;
    DROP TABLE IF EXISTS `Roles`;
SET foreign_key_checks = 1;

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

CREATE TABLE `Products`(
	`productID` int NOT NULL, 
	`productCategoryID` int NOT NULL, 
	`productApprovalID` int NOT NULL, 
	`productName` longtext NOT NULL, 
	`productDescription` longtext, 
	`productOrganicStatus` longtext NOT NULL, 
	`productOrigin` char (255), 
	`productWHSQty` decimal( 10, 2 )  NOT NULL, 
	`productWHSsize` longtext NOT NULL, 
	`productWHSPrice` decimal( 10, 2 )  NOT NULL, 
	`productSNWDivisor` smallint NOT NULL, 
	`productSNWPrice` decimal( 10, 2 )  NOT NULL, 
	`productDateLastChanged` datetime NOT NULL, 
	`productPriceLastChanged` datetime NOT NULL);

ALTER TABLE `Products` ADD PRIMARY KEY (productID);




CREATE TABLE `ProductCategories`(
	`productCategoryID` int NOT NULL AUTO_INCREMENT UNIQUE, 
	`productCategoryName` longtext NOT NULL);

ALTER TABLE `ProductCategories` ADD PRIMARY KEY (productCategoryID);




CREATE TABLE `ProductApprovals`(
	`productApprovalID` int NOT NULL AUTO_INCREMENT UNIQUE, 
	`productApprovalName` longtext NOT NULL);

ALTER TABLE `ProductApprovals` ADD PRIMARY KEY (productApprovalID);




CREATE TABLE `SupplierOrders`(
	`supplierOrderID` int NOT NULL AUTO_INCREMENT UNIQUE, 
	`supplierOrderStateID` int NOT NULL, 
	`supplierID` int NOT NULL, 
	`supplierOrderDeadline` datetime, 
	`supplierOrderDate` datetime NOT NULL, 
	`supplierOrderReqDeliveryDate` datetime);

ALTER TABLE `SupplierOrders` ADD PRIMARY KEY (supplierOrderID);




CREATE TABLE `CustomerOrders`(
	`customerOrderID` int NOT NULL AUTO_INCREMENT UNIQUE, 
	`customerOrderStateID` int NOT NULL, 
	`customerID` int NOT NULL, 
	`customerOrderPaymentMethodID` int NOT NULL, 
	`customerOrderDate` datetime NOT NULL, 
	`customerOrderAdvancePayment` decimal( 10, 2 ) );

ALTER TABLE `CustomerOrders` ADD PRIMARY KEY (customerOrderID);




CREATE TABLE `OrderStates`(
	`orderStateID` int NOT NULL AUTO_INCREMENT UNIQUE, 
	`orderStateName` longtext NOT NULL);

ALTER TABLE `OrderStates` ADD PRIMARY KEY (orderStateID);




CREATE TABLE `CustomerOrderPaymentMethods`(
	`customerOrderPaymentMethodID` int NOT NULL AUTO_INCREMENT UNIQUE, 
	`customerOrderPaymentMethodName` longtext NOT NULL, 
	`customerOrderPaymentMethodDescription` longtext);

ALTER TABLE `CustomerOrderPaymentMethods` ADD PRIMARY KEY (customerOrderPaymentMethodID);




CREATE TABLE `CustomerOrderLines`(
	`customerOrderLineID` int NOT NULL AUTO_INCREMENT UNIQUE, 
	`customerOrderID` int NOT NULL, 
	`supplierOrderID` int, 
	`productID` int NOT NULL, 
	`customerOrderUnitPrice` decimal( 10, 2 ) , 
	`customerOrderQtyMin` decimal( 10, 2 )  NOT NULL, 
	`customerOrderQtyMax` decimal( 10, 2 )  NOT NULL, 
	`customerOrderQtyActual` decimal( 10, 2 )  NOT NULL);

ALTER TABLE `CustomerOrderLines` ADD PRIMARY KEY (customerOrderLineID, customerOrderID);




CREATE TABLE `Suppliers`(
	`supplierID` int NOT NULL AUTO_INCREMENT UNIQUE, 
	`supplierName` longtext NOT NULL, 
	`supplierPhone` longtext NOT NULL, 
	`supplierEmail` longtext NOT NULL, 
	`supplierAddressHouseNo` longtext, 
	`supplierAddressStreet1` longtext NOT NULL, 
	`supplierAddressStreet2` longtext, 
	`supplierAddressTown` longtext NOT NULL, 
	`supplierAddressCounty` longtext, 
	`supplierAddressPostCode` longtext, 
	`supplierAddressCountry` longtext NOT NULL);

ALTER TABLE `Suppliers` ADD PRIMARY KEY (supplierID);




CREATE TABLE `Customers`(
	`customerID` int NOT NULL AUTO_INCREMENT UNIQUE, 
	`customerRoleID` int NOT NULL, 
	`customerForename` longtext NOT NULL, 
	`customerSurname` longtext NOT NULL, 
	`customerPhone` longtext NOT NULL, 
	`customerEmail` longtext NOT NULL, 
	`customerUsername` longtext NOT NULL, 
	`customerPassword` longtext NOT NULL, 
	`customerAddressHouseNo` longtext, 
	`customerAddressStreet1` longtext NOT NULL, 
	`customerAddressStreet2` longtext, 
	`customerAddressTown` longtext NOT NULL, 
	`customerAddressCounty` longtext, 
	`customerAddressPostCode` longtext, 
	`customerAddressCountry` longtext NOT NULL);

ALTER TABLE `Customers` ADD PRIMARY KEY (customerID);




CREATE TABLE `Roles`(
	`roleID` int NOT NULL AUTO_INCREMENT UNIQUE, 
	`roleName` longtext NOT NULL, 
	`roleDescription` longtext);

ALTER TABLE `Roles` ADD PRIMARY KEY (roleID);






-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on `productID` in table 'CustomerOrderLines'

ALTER TABLE `CustomerOrderLines`
ADD CONSTRAINT `FK_ProductCustomerOrderLine`
    FOREIGN KEY (`productID`)
    REFERENCES `Products`
        (`productID`)
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductCustomerOrderLine'

CREATE INDEX `IX_FK_ProductCustomerOrderLine` 
    ON `CustomerOrderLines`
    (`productID`);

-- Creating foreign key on `productCategoryID` in table 'Products'

ALTER TABLE `Products`
ADD CONSTRAINT `FK_ProductCategoryProduct`
    FOREIGN KEY (`productCategoryID`)
    REFERENCES `ProductCategories`
        (`productCategoryID`)
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductCategoryProduct'

CREATE INDEX `IX_FK_ProductCategoryProduct` 
    ON `Products`
    (`productCategoryID`);

-- Creating foreign key on `productApprovalID` in table 'Products'

ALTER TABLE `Products`
ADD CONSTRAINT `FK_ProductApprovalProduct`
    FOREIGN KEY (`productApprovalID`)
    REFERENCES `ProductApprovals`
        (`productApprovalID`)
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductApprovalProduct'

CREATE INDEX `IX_FK_ProductApprovalProduct` 
    ON `Products`
    (`productApprovalID`);

-- Creating foreign key on `supplierID` in table 'SupplierOrders'

ALTER TABLE `SupplierOrders`
ADD CONSTRAINT `FK_SupplierSupplierOrder`
    FOREIGN KEY (`supplierID`)
    REFERENCES `Suppliers`
        (`supplierID`)
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SupplierSupplierOrder'

CREATE INDEX `IX_FK_SupplierSupplierOrder` 
    ON `SupplierOrders`
    (`supplierID`);

-- Creating foreign key on `customerOrderID` in table 'CustomerOrderLines'

ALTER TABLE `CustomerOrderLines`
ADD CONSTRAINT `FK_CustomerOrderCustomerOrderLine`
    FOREIGN KEY (`customerOrderID`)
    REFERENCES `CustomerOrders`
        (`customerOrderID`)
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CustomerOrderCustomerOrderLine'

CREATE INDEX `IX_FK_CustomerOrderCustomerOrderLine` 
    ON `CustomerOrderLines`
    (`customerOrderID`);

-- Creating foreign key on `customerID` in table 'CustomerOrders'

ALTER TABLE `CustomerOrders`
ADD CONSTRAINT `FK_CustomerCustomerOrder`
    FOREIGN KEY (`customerID`)
    REFERENCES `Customers`
        (`customerID`)
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CustomerCustomerOrder'

CREATE INDEX `IX_FK_CustomerCustomerOrder` 
    ON `CustomerOrders`
    (`customerID`);

-- Creating foreign key on `customerOrderStateID` in table 'CustomerOrders'

ALTER TABLE `CustomerOrders`
ADD CONSTRAINT `FK_CustomerOrderStateCustomerOrder`
    FOREIGN KEY (`customerOrderStateID`)
    REFERENCES `OrderStates`
        (`orderStateID`)
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CustomerOrderStateCustomerOrder'

CREATE INDEX `IX_FK_CustomerOrderStateCustomerOrder` 
    ON `CustomerOrders`
    (`customerOrderStateID`);

-- Creating foreign key on `customerOrderPaymentMethodID` in table 'CustomerOrders'

ALTER TABLE `CustomerOrders`
ADD CONSTRAINT `FK_CustomerOrderPaymentMethodCustomerOrder`
    FOREIGN KEY (`customerOrderPaymentMethodID`)
    REFERENCES `CustomerOrderPaymentMethods`
        (`customerOrderPaymentMethodID`)
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CustomerOrderPaymentMethodCustomerOrder'

CREATE INDEX `IX_FK_CustomerOrderPaymentMethodCustomerOrder` 
    ON `CustomerOrders`
    (`customerOrderPaymentMethodID`);

-- Creating foreign key on `customerRoleID` in table 'Customers'

ALTER TABLE `Customers`
ADD CONSTRAINT `FK_CustomerRoleCustomer`
    FOREIGN KEY (`customerRoleID`)
    REFERENCES `Roles`
        (`roleID`)
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CustomerRoleCustomer'

CREATE INDEX `IX_FK_CustomerRoleCustomer` 
    ON `Customers`
    (`customerRoleID`);

-- Creating foreign key on `supplierOrderStateID` in table 'SupplierOrders'

ALTER TABLE `SupplierOrders`
ADD CONSTRAINT `FK_SupplierOrderOrderState`
    FOREIGN KEY (`supplierOrderStateID`)
    REFERENCES `OrderStates`
        (`orderStateID`)
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SupplierOrderOrderState'

CREATE INDEX `IX_FK_SupplierOrderOrderState` 
    ON `SupplierOrders`
    (`supplierOrderStateID`);

-- Creating foreign key on `supplierOrderID` in table 'CustomerOrderLines'

ALTER TABLE `CustomerOrderLines`
ADD CONSTRAINT `FK_SupplierOrderCustomerOrderLine`
    FOREIGN KEY (`supplierOrderID`)
    REFERENCES `SupplierOrders`
        (`supplierOrderID`)
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SupplierOrderCustomerOrderLine'

CREATE INDEX `IX_FK_SupplierOrderCustomerOrderLine` 
    ON `CustomerOrderLines`
    (`supplierOrderID`);

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
