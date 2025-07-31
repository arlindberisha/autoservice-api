# PowerShell script to test the Auto Service Management API
# Run this after starting the API with 'dotnet run'

$baseUrl = "https://localhost:7063"
$headers = @{
    "Content-Type" = "application/json"
}

Write-Host "Testing Auto Service Management API..." -ForegroundColor Green
Write-Host "Base URL: $baseUrl" -ForegroundColor Cyan

# Test 1: Register organization owner
Write-Host "`n1. Testing organization registration..." -ForegroundColor Yellow
$registerBody = @{
    organisationName = "Super Auto Service"
    email = "owner@superauto.com"
    password = "Password123!"
    ownerFirstName = "John"
    ownerLastName = "Doe"
    ownerPhone = "+15551234567"
} | ConvertTo-Json

try {
    $registerResponse = Invoke-RestMethod -Uri "$baseUrl/api/v1/auth/register" -Method Post -Body $registerBody -Headers $headers -SkipCertificateCheck
    Write-Host "✓ Registration successful" -ForegroundColor Green
    Write-Host "Token: $($registerResponse.token.Substring(0,20))..." -ForegroundColor Gray
    
    # Store token for subsequent requests
    $token = $registerResponse.token
    $authHeaders = @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $token"
    }
} catch {
    Write-Host "✗ Registration failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Test 2: Login
Write-Host "`n2. Testing login..." -ForegroundColor Yellow
$loginBody = @{
    email = "owner@superauto.com"
    password = "Password123!"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/v1/auth/login" -Method Post -Body $loginBody -Headers $headers -SkipCertificateCheck
    Write-Host "✓ Login successful" -ForegroundColor Green
} catch {
    Write-Host "✗ Login failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Get organization
Write-Host "`n3. Testing get organization..." -ForegroundColor Yellow
try {
    $orgResponse = Invoke-RestMethod -Uri "$baseUrl/api/v1/organisation" -Method Get -Headers $authHeaders -SkipCertificateCheck
    Write-Host "✓ Organization retrieved: $($orgResponse.name)" -ForegroundColor Green
} catch {
    Write-Host "✗ Get organization failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 4: Create client
Write-Host "`n4. Testing create client..." -ForegroundColor Yellow
$clientBody = @{
    firstName = "Jane"
    lastName = "Smith"
    phoneNumber = "+15559876543"
} | ConvertTo-Json

try {
    $clientResponse = Invoke-RestMethod -Uri "$baseUrl/api/v1/clients" -Method Post -Body $clientBody -Headers $authHeaders -SkipCertificateCheck
    Write-Host "✓ Client created: $($clientResponse.firstName) $($clientResponse.lastName)" -ForegroundColor Green
    $clientId = $clientResponse.id
} catch {
    Write-Host "✗ Create client failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 5: Create car
Write-Host "`n5. Testing create car..." -ForegroundColor Yellow
$carBody = @{
    make = "Volkswagen"
    model = "Golf"
    year = 2021
    color = "Blue"
    vin = "WVWZZZAUZMP123456"
    licensePlate = "AB 123 CD"
    insuranceNumber = "INS-987654"
    mileage = 50000
    engineType = "2.0L TDI"
    transmission = "Automatic"
    numberOfDoors = 5
    clientId = $clientId
} | ConvertTo-Json

try {
    $carResponse = Invoke-RestMethod -Uri "$baseUrl/api/v1/cars" -Method Post -Body $carBody -Headers $authHeaders -SkipCertificateCheck
    Write-Host "✓ Car created: $($carResponse.make) $($carResponse.model)" -ForegroundColor Green
    $carId = $carResponse.id
} catch {
    Write-Host "✗ Create car failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 6: Create services
Write-Host "`n6. Testing create services..." -ForegroundColor Yellow
$service1Body = @{ name = "Oil Change" } | ConvertTo-Json
$service2Body = @{ name = "Tire Rotation" } | ConvertTo-Json

try {
    $service1Response = Invoke-RestMethod -Uri "$baseUrl/api/v1/services" -Method Post -Body $service1Body -Headers $authHeaders -SkipCertificateCheck
    $service2Response = Invoke-RestMethod -Uri "$baseUrl/api/v1/services" -Method Post -Body $service2Body -Headers $authHeaders -SkipCertificateCheck
    Write-Host "✓ Services created successfully" -ForegroundColor Green
    $service1Id = $service1Response.id
    $service2Id = $service2Response.id
} catch {
    Write-Host "✗ Create services failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 7: Create bill
Write-Host "`n7. Testing create bill..." -ForegroundColor Yellow
$billBody = @{
    clientId = $clientId
    carId = $carId
    date = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ssZ")
    servicesPerformed = @(
        @{
            serviceId = $service1Id
            name = "Oil Change"
            price = 100.00
        },
        @{
            serviceId = $service2Id
            name = "Tire Rotation"
            price = 50.00
        }
    )
    discount = @{
        type = "percentage"
        value = 10
    }
    notes = "Customer requested synthetic oil."
} | ConvertTo-Json -Depth 3

try {
    $billResponse = Invoke-RestMethod -Uri "$baseUrl/api/v1/bills" -Method Post -Body $billBody -Headers $authHeaders -SkipCertificateCheck
    Write-Host "✓ Bill created: $($billResponse.billNumber) - Total: $($billResponse.totalAmount)" -ForegroundColor Green
} catch {
    Write-Host "✗ Create bill failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 8: Get income report
Write-Host "`n8. Testing income report..." -ForegroundColor Yellow
$startDate = (Get-Date).AddDays(-30).ToString("yyyy-MM-dd")
$endDate = (Get-Date).ToString("yyyy-MM-dd")

try {
    $reportResponse = Invoke-RestMethod -Uri "$baseUrl/api/v1/reports/income?startDate=$startDate&endDate=$endDate" -Method Get -Headers $authHeaders -SkipCertificateCheck
    Write-Host "✓ Income report retrieved - Total Income: $($reportResponse.totalIncome), Bills: $($reportResponse.totalBills)" -ForegroundColor Green
} catch {
    Write-Host "✗ Income report failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n🎉 API testing completed!" -ForegroundColor Green
Write-Host "Visit $baseUrl/swagger to explore the API documentation" -ForegroundColor Cyan