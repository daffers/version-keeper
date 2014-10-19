Feature: Version Keeper WebApi
	In order to be able to version my application or packages
	As a programmer
	I want to be able to manage my application version numbers in a web service

Scenario: Get the root	
	When When i navigate to the root of the api
	Then i should see the link allowing me to create an application
	And i should see the link allowing me to retrieve an application

Scenario: Create an application
	When I POST details of an application to the correct URL
	Then A an application with the same details appears in the storage area
	And The result view i see is the full application details
	And i see all the links for modifying the application
