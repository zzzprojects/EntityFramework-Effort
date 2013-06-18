Effort is a powerful tool that enables a convenient way to create unit tests for
Entity Framework based applications.

It is basicly an ADO.NET provider that executes all the data operations on 
a lightweight in-process main memory database instead of a traditional external
database. It provides some intuitive helper methods too that make really easy to
use this provider with existing ObjectContext or DbContext classes. A simple 
addition to existing code might be enough to create data driven unit tests 
that can be run without the presence of the external database.