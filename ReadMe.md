
# Developement Using VS Code

* Install below extensions
    * Name: C# Publisher: Microsoft
    * Name: C# Extensions Publisher: JosKreativ
    * Name: NuGet Package Manager Publisher: jmrog

* dotnet tool install --global dotnet-ef

* Install below packages
    a. Microsoft.EntityFrameworkCore.Design
    b. Microsoft.EntityFrameworkCore.Tools

* Scafolding 
    ALL TABLES 
        dotnet ef dbcontext scaffold 'Name = ConnectionStrings:DB1Connection' Npgsql.EntityFrameworkCore.PostgreSQL -c DB1Context --context-dir DAL -o DAL/Entities -f --no-onconfiguring --no-build

    SPECIFIC TABLES
       dotnet ef dbcontext scaffold 'Name = ConnectionStrings:DB1Connection' Npgsql.EntityFrameworkCore.PostgreSQL -c DB1Context --context-dir DAL -o DAL/Entities --data-annotations --table app.group_users  -f

    NO BUILD
        add '--no-build'