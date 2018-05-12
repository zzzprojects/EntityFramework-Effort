# Export Data to Csv Files

## Introduction

Creating a CSV file could be tiresome, **Effort** provides an easy-to-use program, called [**Effort.CsvTool**](https://github.com/zzzprojects/effort/tree/develop/Main/Source/Effort.CsvTool) that export the current state of your entire database into CSV files that Effort can consume. 

It is a GUI based tool.

<img src="images/csv-tool-1.png">

## Providers

This tool supports the following database providers.

 - Odbc Data Provider
 - OleDb Data Provider
 - OracleClient Data Provider
 - SqlClient Data Provider
 - Microsoft SQL Server Compact Data Provider

The provider can be selected from Provider dropdown list, and you can set different properties value for any provider such as connection string of your database etc.

<img src="images/csv-tool-2.png">

In the **Export path** field you can specify the directory where you want to save all the **CSV** files. Click **Export** button you are done.

## Note:

The content of each database table is represented by a dedicated CSV file that has to be named as **{table name}.csv**.



