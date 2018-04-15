# Example

A worked example of an Access Control List, implemented with [QTools](https://github.com/machonky/qtools), 
[Qapper](https://github.com/machonky/Qapper) and [QSharp](https://github.com/machonky/qsharp.netstandard).

The example is based upon the [CodeProject](https://www.codeproject.com) article [Lightning-Fast Access Control Lists in C#](https://www.codeproject.com/Articles/1056853/Lightning-Fast-Access-Control-Lists-in-Csharp) 
where the ACL is backed by a kdb+ database. 

In the example, database schema is automatically generated and defined in a kdb+ session. 
Corresponding data is also created and inserted based upon a well known collection of characters. 
The data is retreived and used to build a working Access Control List. 

