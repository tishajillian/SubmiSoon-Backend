# Setting Up Secrets for SubmiSoon API

This guide explains how to configure the secrets required to run the SubmiSoon API locally.

## Overview

The SubmiSoon API requires two cryptographic secrets:
- **JWT Secret**: Used to sign and verify JSON Web Tokens for authentication
- **URL Signing Secret**: Used to generate secure, signed URLs for file downloads

These secrets are **excluded from version control** for security reasons. You must generate and configure them locally.

---

## Quick Setup

### Step 1: Copy the Template File

Copy `appsettings.json.template` to `appsettings.json`:

```powershell
# From the project root directory
Copy-Item SubmiSoonProject\appsettings.json.template SubmiSoonProject\appsettings.json
```

### Step 2: Generate Secrets

Run these PowerShell commands to generate cryptographically secure random secrets:

```powershell
# Generate JWT Secret (64 hex characters = 32 bytes)
-join ((0..31) | ForEach-Object { '{0:x2}' -f (Get-Random -Minimum 0 -Maximum 256) })

# Generate URL Signing Secret (128 hex characters = 64 bytes)
-join ((0..63) | ForEach-Object { '{0:x2}' -f (Get-Random -Minimum 0 -Maximum 256) })
```

**Example Output:**
```
JWT Secret: a3f7e9d2c1b5a8e4f6d9c2b7a1e5f8d3c6b9a2e7f1d4c8b3a6e9f2d5c1b8a4e7
URL Signing Secret: f8e3d7c2a9b5e1f4d8c3a7b2e6f9d1c5a8b4e7f2d6c9b3a1e5f8d4c7b2a9e6f3d1c5b8a4e7f2d9c6b3a1e5f8d4c7b2a9e6f3d1c5b8a4e7f2d9c6b3a1e5f8d4
```

### Step 3: Update appsettings.json

Open `SubmiSoonProject\appsettings.json` and replace the placeholder values:

**Before:**
```json
"JwtSettings": {
  "SecretKey": "REPLACE_WITH_64_CHARACTER_HEX_STRING",
  "Issuer": "SubmiSoon",
  "Audience": "SubmiSoonUsers",
  "ExpirationMinutes": 60
}
```

**After:**
```json
"JwtSettings": {
  "SecretKey": "a3f7e9d2c1b5a8e4f6d9c2b7a1e5f8d3c6b9a2e7f1d4c8b3a6e9f2d5c1b8a4e7",
  "Issuer": "SubmiSoon",
  "Audience": "SubmiSoonUsers",
  "ExpirationMinutes": 60
}
```

**Before:**
```json
"UrlSigning": {
  "SecretKey": "REPLACE_WITH_128_CHARACTER_HEX_STRING",
  "ExpirationMinutes": 30
}
```

**After:**
```json
"UrlSigning": {
  "SecretKey": "f8e3d7c2a9b5e1f4d8c3a7b2e6f9d1c5a8b4e7f2d6c9b3a1e5f8d4c7b2a9e6f3d1c5b8a4e7f2d9c6b3a1e5f8d4c7b2a9e6f3d1c5b8a4e7f2d9c6b3a1e5f8d4",
  "ExpirationMinutes": 30
}
```

### Step 4: Verify Configuration

Run the following command to verify your configuration:

```powershell
# Check that appsettings.json exists and contains your secrets
Test-Path SubmiSoonProject\appsettings.json
```

**Expected result:** `True`

---

## Important Security Notes

1. **Never commit `appsettings.json`** to version control
   - The `.gitignore` file already excludes it
   - Only commit `appsettings.json.template` with placeholder values

2. **Use unique secrets for each environment**
   - Development: Generate local secrets (as shown above)
   - Production: Use secure secret management (Azure Key Vault, AWS Secrets Manager, etc.)

3. **Share secrets securely**
   - Never send secrets via email, Slack, or other insecure channels
   - Use password managers (1Password, LastPass) or secure sharing tools

4. **Rotate secrets regularly**
   - If you suspect a secret has been exposed, generate a new one immediately
   - Update all environments using the compromised secret

---

## Alternative: Use .NET User Secrets (Development Only)

Instead of `appsettings.json`, you can use .NET User Secrets for local development:

```powershell
# Navigate to the project directory
cd SubmiSoonProject

# Initialize user secrets
dotnet user-secrets init

# Set JWT Secret
dotnet user-secrets set "JwtSettings:SecretKey" "YOUR_64_CHAR_HEX_STRING"

# Set URL Signing Secret
dotnet user-secrets set "UrlSigning:SecretKey" "YOUR_128_CHAR_HEX_STRING"
```

**Advantages:**
- Secrets stored outside the project directory
- Automatically excluded from version control
- Per-user configuration

**Disadvantages:**
- Only works in Development environment
- Requires additional setup for each developer

---

## Troubleshooting

### Error: "JWT Secret Key is too short"

**Cause:** The JWT secret must be at least 32 bytes (64 hex characters).

**Solution:** Run the PowerShell command above to generate a new 64-character secret.

---

### Error: "appsettings.json not found"

**Cause:** You haven't copied the template file yet.

**Solution:**
```powershell
Copy-Item SubmiSoonProject\appsettings.json.template SubmiSoonProject\appsettings.json
```

---

### Error: "Invalid token" when testing authentication

**Cause:** JWT secret doesn't match between token generation and validation.

**Solution:** 
1. Stop the API server
2. Clear any cached tokens (logout, clear browser cookies)
3. Restart the API server
4. Login again to get a new token

---

## Database Connection String

The default connection string connects to:
- **Server:** `localhost`
- **Database:** `SubmiSoonDB`
- **Authentication:** Windows Authentication (Integrated Security)

If you need to customize the database connection:

1. Open `SubmiSoonProject\appsettings.json`
2. Update the `ConnectionStrings:DefaultConnection` value:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DATABASE;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

**For SQL Server Authentication (username/password):**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
}
```

---

## Need Help?

If you encounter issues during setup:

1. Check that all placeholder values have been replaced
2. Verify the secret lengths (64 and 128 characters)
3. Ensure `appsettings.json` exists in the `SubmiSoonProject` directory
4. Restart Visual Studio or the terminal after making changes

For production deployment questions, consult your DevOps team or cloud provider documentation.
