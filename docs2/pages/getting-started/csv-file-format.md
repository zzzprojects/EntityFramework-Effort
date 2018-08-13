# CSV File Format

## CSV file format in Effort

**CsvDataLoader** component accepts files that follow the traditional CSV format:

 - The first row contains the header names.
 - Comma ( , ) is the separator character.
 - Double quote ( " ) is the delimiter character.
 - Double double quote ( "" ) is used to express a single double quote between delimiters.

There are some additional requirements that need to be taken into consideration.

 - Numbers and dates are parsed with invariant culture setting.
 - Binaries are encoded in base64 format.
 - Null values are represented with empty fields without delimiters.
 - Empty strings are represented by empty fields with a delimiter.
 - Backslash serves as an escape character for backslash and newline characters.

These are all the rules that need to be followed. The following example demonstrates the rules by representing a compatible CSV file.

### Sample CSV File
```csharp
id,name,birthdate,reportto,storages,photo
"JD","John Doe",01/23/1982,"MHS","\\\\server1\\share8\r\n\\\\server2\share3",
"MHS","Michael ""h4x0r"" Smith",05/12/1975,,"","ZzVlKyszZjQ5M2YzNA=="
```

The first line contains the name of the table fields, and the remaining lines represent a data row.


