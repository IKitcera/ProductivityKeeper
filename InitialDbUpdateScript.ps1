# Define variables
$projectPath = "ProductivityKeeperWeb"
$migrationsNamespace = "ProductivityKeeperWeb\Migrations" # Namespace where migrations are located
$connectionString = "Data Source=DESKTOP-6PJD0EA;Initial Catalog=prodKeepDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" # Connection string to your database

# Change directory to project path
cd $projectPath

# Get list of migrations
$migrations = dotnet ef migrations list --project $projectPath --startup-project $projectPath --namespace $migrationsNamespace --connection $connectionString --json | ConvertFrom-Json

# Sort migrations by ID
$migrations = $migrations | Sort-Object -Property ID

# Loop through migrations and apply each one
foreach ($migration in $migrations) {
    $migrationName = $migration.Name
    Write-Host "Applying migration: $migrationName"
    dotnet ef database update $migrationName --project $projectPath --startup-project $projectPath --connection $connectionString
}

# Pause execution
Read-Host -Prompt "Press Enter to exit"