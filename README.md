# ProgrammingChallenge-SystemsGuild
A solution to a programming challenge given to me by Systems Guild, Inc. as a part of the job application process.

The task is to create an application that accesses the Adventure Works 2017 database from Microsoft. The application is meant to allow a user to input a zipcode and date range. This would then return the average transaction value for that zipcode in the time range provided as well as the average transaction value for the region the zipcode belongs to. 

For example, within the database the zipcode 38231 is in the Germany sales region and for the dates of 07/31/2013 to 08/25/2013 the average transaction value for the zipcode is $219.67 with 11 total transactions. The average transaction value for the region is $263.60 with 18,213 total transactions.

My solution uses C# and .Net to access an SQL Server based on information the user provides. In addition to regular C# and .Net libraries and dependencies, my program relies on the System.Data.SqlClient NuGet package.

In testing I had the Adventure Works database on my local machine and accessed it with Windows Authorization. An example of responses to the prompts for a local connection is below:

Data Source: localhost

Initial Catalogue: databaseName   i.e. AdventureWorks2017

Windows Authorization: yes

Remote database access is not yet tested and may not be feasible at this time. Based on research, a remote connection could be established by doing the following for the prompts:

Data Source: tcp/{ip address}     i.e. tcp/192.0.0.0

Initial Catalogue: databaseName   i.e. AdventureWorks2017

Windows Authorization: no

Username: username for SQL Server

Password: password for SQL Server
