Feature: Version Keeper WebApi
	In order to be able to version my application or packages
	As a programmer
	I want to be able to manage my application version numbers in a web service


Scenario: Get the root	
	When When i navigate to the root of the api
	Then i should see the link allowing me to create an application
	And i should see the link allowing me to retrieve an application
