param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$Name
)

dotnet ef migrations add $Name `
    --project src/Ucms.Infrastructure `
    --startup-project src/Ucms.Api `
    --output-dir Migrations
