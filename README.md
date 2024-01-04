# Compiled Queries and tenancy

### Getting started
- update the CONNECTION_STRING in [Program.cs](./Program.cs) to your local postgres instance
- `dotnet restore`
- `rm -rf ./Internal && dotnet run`
- Check the generated queries under ./Internal/Generated/ (the console should output if it has the tenantId parameter or not)


### What is happening
In marten 6.0.0-rc.1, the compiled query correctly adds the tenantid filter from the session
```
builder.AddNamedParameter("tenantid", session.TenantId);
```
but all later versions do not add this value
