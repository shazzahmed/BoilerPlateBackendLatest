# How to Access Logs on SmarterASP.NET Shared Hosting

## 📋 **Updated Code for Shared Hosting**

I've updated the `DashboardService.cs` to write logs to a file (`dashboard_debug.log`) that you can easily access via FTP.

---

## 🔍 **Method 1: Access Custom Debug Log File (Easiest)**

### **Step 1: Deploy Updated Code**

1. Build your solution
2. Publish to SmarterASP.NET
3. The file `dashboard_debug.log` will be created automatically when dashboard is accessed

### **Step 2: Access the Log via FTP**

1. **Open FileZilla or any FTP client**

2. **Connect to SmarterASP:**
   - Host: `ftp://ftp.[yourdomain].com` or use IP from control panel
   - Username: Your SmarterASP username
   - Password: Your SmarterASP password
   - Port: 21

3. **Navigate to your application root folder:**
   ```
   /wwwroot/
   ```

4. **Look for file:** `dashboard_debug.log`

5. **Download and open** the file with Notepad or any text editor

6. **Look for lines like:**
   ```
   [2025-10-29 23:30:45] 🕐 Dashboard: Server Local Time: 2025-10-29 18:30:45, UTC Time: 2025-10-29 18:30:45, Timezone: (UTC) Coordinated Universal Time, TenantId: 1
   ```

---

## 🔍 **Method 2: SmarterASP Control Panel Logs**

### **Step 1: Login to SmarterASP Control Panel**

1. Go to: https://cp.smarterasp.net/
2. Enter your username and password
3. Click **Login**

### **Step 2: Navigate to Web Logs**

1. From the main menu, click **"Web Site"**
2. Click **"Web Site Logs"** (or "IIS Logs")
3. You'll see a list of log files by date

### **Step 3: Download Today's Log**

1. Find today's date in the list
2. Click **Download** icon next to the file
3. Save the file (usually named like: `u_ex[YYMMDD].log`)

### **Step 4: Search the Log**

1. Open the downloaded file with Notepad++, VS Code, or any text editor
2. Press `Ctrl+F` and search for:
   - `Dashboard`
   - `GetKeyMetrics`
   - `Server Local Time`

**Note:** IIS logs might not show `Console.WriteLine` output, which is why we added file logging.

---

## 🔍 **Method 3: Direct FTP to Check Application Files**

### **Access via FTP:**

1. **Server:** Your FTP hostname (from SmarterASP control panel)
2. **Username:** Your hosting username
3. **Password:** Your hosting password
4. **Port:** 21 (FTP) or 22 (SFTP if available)

### **Check these locations:**

```
/wwwroot/dashboard_debug.log          ← Our custom log file
/wwwroot/logs/stdout_*.log            ← ASP.NET Core stdout logs
/LogFiles/                            ← IIS logs folder
```

---

## 📊 **What You're Looking For**

After calling the dashboard API, check the log file for:

```
[2025-10-29 23:30:45] 🕐 Dashboard: Server Local Time: 2025-10-29 18:30:45, UTC Time: 2025-10-29 18:30:45, Timezone: (UTC) Coordinated Universal Time, TenantId: 1
```

### **Identify the Problem:**

| Log Shows | Your Local Time | Problem |
|-----------|-----------------|---------|
| `18:30:45 UTC` | `23:30:45 PKT` | ✅ Server is 5 hours behind (UTC) |
| `23:30:45` | `23:30:45 PKT` | ✅ Server timezone is correct |
| `Timezone: (UTC)` | Any | 🔴 Server is in UTC, needs fix |
| `Timezone: Pakistan Standard Time` | Any | ✅ Server timezone correct |

---

## 🔧 **Fix for SmarterASP.NET Shared Hosting**

### **Option 1: Web.config Environment Variable (Recommended)**

Add this to your `web.config` file:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <aspNetCore processPath="dotnet" 
                  arguments=".\SMSBACKEND.Presentation.dll" 
                  stdoutLogEnabled="false" 
                  stdoutLogFile=".\logs\stdout" 
                  hostingModel="inprocess">
        <environmentVariables>
          <!-- ✅ Set timezone to Pakistan -->
          <environmentVariable name="TZ" value="Asia/Karachi" />
          <environmentVariable name="WEBSITE_TIME_ZONE" value="Pakistan Standard Time" />
        </environmentVariables>
      </aspNetCore>
    </system.webServer>
  </location>
</configuration>
```

### **Option 2: Request Timezone Change via Support Ticket**

If environment variables don't work:

1. **Login to SmarterASP Control Panel**
2. **Open Support Ticket:**
   - Category: Technical Support
   - Subject: "Request Server Timezone Change to Pakistan Standard Time"
   - Message:
   ```
   Hello,
   
   I need the timezone for my hosting account to be changed to:
   - Timezone: Pakistan Standard Time (Asia/Karachi, UTC+5)
   
   My application requires local time for date comparisons.
   Account: [Your Username]
   Domain: [Your Domain]
   
   Thank you.
   ```

### **Option 3: Application-Level Timezone Handling (If above don't work)**

If SmarterASP doesn't allow timezone changes, we can handle it in code. Let me know if you need this approach.

---

## ✅ **Verification Steps**

After implementing the fix:

### **Step 1: Clear the Log File**

Via FTP, delete or rename `dashboard_debug.log`

### **Step 2: Access Dashboard**

Visit: `https://yourdomain.com/Api/Dashboard/Admin?dateRange=month`

### **Step 3: Check New Log**

Download `dashboard_debug.log` and verify:
```
[2025-10-29 23:30:45] 🕐 Dashboard: Server Local Time: 2025-10-29 23:30:45, UTC Time: 2025-10-29 18:30:45, Timezone: Pakistan Standard Time, TenantId: 1
```

If you see `Pakistan Standard Time` - ✅ Fixed!

### **Step 4: Test Dashboard**

- Check if today's attendance shows correctly
- Check if today's fee collection displays
- Verify dashboard shows current data, not yesterday's

---

## 🚨 **Troubleshooting**

### **Problem: Log file not created**

**Solution:**
1. Check write permissions on `/wwwroot/` folder
2. Contact SmarterASP support to enable file write permissions
3. Alternatively, create the file manually via FTP and set permissions

### **Problem: Environment variables not working**

**Cause:** Some shared hosting providers restrict environment variable changes

**Solution:**
1. Open a support ticket with SmarterASP
2. Request timezone change at the account level
3. Or implement application-level timezone handling (let me know if needed)

### **Problem: Still showing wrong time after fix**

**Check:**
1. Did you recycle the application pool? (may need support ticket)
2. Did you clear browser cache?
3. Is the web.config properly formatted?
4. Check the log to confirm timezone is actually changed

---

## 📞 **SmarterASP Support**

If you need help:

1. **Support Ticket System:** https://cp.smarterasp.net/support/
2. **Live Chat:** Available in control panel
3. **Email:** support@smarterasp.net

**What to ask:**
```
Subject: Enable Pakistan Standard Time for my hosting account

Body:
Hello,

I need assistance configuring my account timezone. My .NET Core application 
requires the server timezone to be set to "Pakistan Standard Time" (Asia/Karachi, UTC+5).

Could you please:
1. Change the server timezone for my account to Pakistan Standard Time
2. OR enable environment variable support for TZ and WEBSITE_TIME_ZONE
3. OR recycle my application pool after I update web.config

Account: [Your Username]
Domain: [Your Domain]

Thank you.
```

---

## 📋 **Quick Reference: FTP Details**

Get your FTP details from SmarterASP Control Panel:

1. Login to: https://cp.smarterasp.net/
2. Go to: **"Web Site" > "FTP Accounts"**
3. Note down:
   - FTP Host
   - FTP Username
   - FTP Password
   - FTP Port (usually 21)

---

## 🎯 **Expected Result**

After fixing timezone, your dashboard should:
- ✅ Show today's attendance data
- ✅ Show today's fee collections
- ✅ Display correct dates in all charts
- ✅ Recent activities show current items

---

**Need help?** Let me know what you see in the log file and I can help diagnose further!

