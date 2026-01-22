# Student Module - Quick Reference Guide

## 🎯 Quick Overview

**What:** Comprehensive student management system with parent linking, validation, and multi-tenancy support.

**Tech Stack:** Angular 17 + .NET 8 + SQL Server + Material Design

**Pattern:** Service-Repository with Dependency Injection

---

## 📋 Recent Changes Summary

| Feature | Status | Description |
|---------|--------|-------------|
| **Phone Validation** | ✅ Complete | Accepts both `03018210558` and `0301-8210558` formats |
| **CNIC/ParentId Validation** | ✅ Complete | Tenant-configurable: accepts CNIC or numeric ParentId |
| **Admission Number Sequence** | ✅ Complete | Auto-initializes from existing data, continues from max |
| **UI 4-Column Layout** | ✅ Complete | Responsive grid with inline image upload |
| **Form Steps Consolidation** | ✅ Complete | 4 steps instead of 5 (removed Contact Info step) |
| **UpdateStudent Functionality** | ✅ Complete | Full CRUD with change detection and transaction safety |
| **ESC Key & Z-Index** | ✅ Complete | Dialog improvements for better UX |
| **Menu Loading Fix** | ✅ Complete | Re-initializes menu after login |

---

## 🔧 Configuration (Per Tenant)

```csharp
public class Tenant
{
    // Admission Number Management
    public int LastStudentSequence { get; set; } = 0;
    public string AdmissionNoFormat { get; set; } = "{SEQ}";
    
    // CNIC/ParentId Validation
    public bool AllowParentIdInCnicField { get; set; } = true;
}
```

### Admission Number Format Examples

| Format | Output | Description |
|--------|--------|-------------|
| `{SEQ}` | `501543` | Plain sequence |
| `ADM-{YEAR}-{SEQ:0000}` | `ADM-2025-0543` | Year prefix with 4-digit padding |
| `STU{SEQ}` | `STU501543` | Custom prefix |
| `{YEAR}{SEQ:000}` | `2025543` | Year + 3-digit padding |

---

## 🚀 API Endpoints

### Core Operations
```http
GET    /Api/Students/GetAll
POST   /Api/Students/CreateStudent
POST   /Api/Students/UpdateStudent
DELETE /Api/Students/DeleteStudent/{id}
POST   /Api/Students/RestoreStudent/{id}
```

### Validation
```http
POST /Api/Students/ValidateParent
POST /Api/Students/ValidateStudent
GET  /Api/Students/GetNextAdmissionNumber  ← Returns next sequence number
```

### Bulk Import
```http
POST /Api/Students/StartBulkImport
GET  /Api/Students/GetImportStatus/{jobId}
GET  /Api/Students/DownloadImportTemplate
```

---

## 📊 Database Entities

```
Student (main)
├── StudentInfo (1:1) - Basic info, admission number, name
├── StudentAddress (1:N) - Contact info, addresses
├── StudentParent (1:N) - Father, mother, guardian info
├── StudentMedicalInfo (1:N) - Health records
└── StudentClassAssignment (M:M) - Class/section assignments
```

---

## ✅ Validation Rules

### Frontend (Angular)

**Phone Number:**
- ✅ `03018210558` (11 digits)
- ✅ `0301-8210558` (4-7 mobile format)
- ✅ `021-12345678` (3-8 landline format)

**CNIC/ParentId (configurable):**
- ✅ `12345-1234567-1` (CNIC format - always accepted)
- ✅ `151`, `14` (ParentId - if `AllowParentIdInCnicField = true`)

### Backend (C#)

**Required Fields:**
- FirstName, LastName, Gender
- ClassId, SectionId, SessionId
- FatherCNIC, FatherName
- GuardianType, GuardianName, GuardianPhone

**Format Validation:**
- Email: Standard RFC format
- Phone: 11 digits starting with 03
- CNIC: 13 digits in xxxxx-xxxxxxx-x format
- ParentId: Numeric only (if enabled)

---

## 🎨 Form Layout

### Step 1: Student Information (4-column grid)

```
┌─────────────┬─────────────┬─────────────┬─────────────┐
│   Image     │ Admission # │  Roll No    │  Adm. Date  │
├─────────────┴─────────────┴─────────────┴─────────────┤
│ First Name  │ Last Name   │  DOB        │  Gender     │
├─────────────┴─────────────┴─────────────┴─────────────┤
│ B Form #    │ Religion    │  Caste      │  Category   │
└─────────────┴─────────────┴─────────────┴─────────────┘

Contact Information (full width):
- Phone, Mobile, Email, Pickup Person
- City, State, Zip Code
- Current Address, Permanent Address

Medical Information (full width):
- Height, Weight, Measurement Date
- Blood Group, Medical History, Medical Notes
```

---

## 🔄 Admission Number Flow

### 1. Form Load (Preview)
```
Frontend → GET /Api/Students/GetNextAdmissionNumber
         ← Returns "501543" (peek, no increment)
```

### 2. Student Creation
```
Frontend → POST /Api/Students/CreateStudent { AdmissionNo: "" }
Backend  → Checks Tenant.LastStudentSequence = 0
         → Scans database for max admission number = 501542
         → Initializes Tenant.LastStudentSequence = 501542
         → Increments to 501543
         → Creates student with AdmissionNo = "501543"
         → Saves Tenant.LastStudentSequence = 501543
Frontend ← Returns success with new student
```

### 3. Next Student
```
Backend  → Tenant.LastStudentSequence = 501543 (already initialized)
         → Increments to 501544
         → Creates student with AdmissionNo = "501544"
         → Saves Tenant.LastStudentSequence = 501544
```

---

## 👨‍👩‍👧 Parent Linking Logic

### Validation Priority

1. **CNIC Match** (highest priority)
   ```
   If FatherCNIC matches existing parent → Link to existing
   ```

2. **Phone + Name Match**
   ```
   If FatherPhone + FatherName match → Link to existing
   ```

3. **Create New**
   ```
   If no match found → Create new parent account
   ```

### Guardian Type Auto-Sync

```typescript
// When GuardianType = "Father"
GuardianName = FatherName
GuardianPhone = FatherPhone
GuardianRelation = "Father"

// When GuardianType = "Mother"
GuardianName = MotherName
GuardianPhone = MotherPhone
GuardianRelation = "Mother"

// Changes in Father/Mother fields auto-update Guardian fields
```

---

## 🎯 Code Examples

### 1. Generate Next Admission Number

```csharp
// Backend - PeekNextAdmissionNumberAsync (for preview)
public async Task<string> PeekNextAdmissionNumberAsync(int tenantId)
{
    var tenant = await DbContext.Tenants.AsNoTracking()
        .FirstOrDefaultAsync(t => t.TenantId == tenantId);
    
    int currentSequence = tenant.LastStudentSequence;
    if (currentSequence == 0)
    {
        currentSequence = await GetMaxAdmissionNumberSequenceAsync(tenantId);
    }
    
    var nextSequence = currentSequence + 1;
    return GenerateFromTemplate(tenant.AdmissionNoFormat, nextSequence);
}
```

### 2. Phone Number Validation

```typescript
// Frontend - phoneNumberValidator
private phoneNumberValidator(control: AbstractControl): ValidationErrors | null {
  const value = control.value?.trim();
  if (!value) return null;
  
  if (/^\d{11}$/.test(value)) return null;           // 03018210558
  if (/^\d{4}-\d{7}$/.test(value)) return null;      // 0301-8210558
  if (/^\d{3}-\d{8}$/.test(value)) return null;      // 021-12345678
  
  return { invalidPhone: true };
}
```

### 3. CNIC/ParentId Validation

```csharp
// Backend - ValidateCnicOrParentId
private (bool isValid, string errorMessage) ValidateCnicOrParentId(
    string value, bool allowParentId, string fieldLabel)
{
    // Check CNIC format (13 digits with dashes)
    bool isCnicFormat = Regex.IsMatch(value, @"^\d{5}-\d{7}-\d{1}$");
    if (isCnicFormat) return (true, string.Empty);
    
    // Check numeric ParentId if tenant allows
    bool isNumericOnly = Regex.IsMatch(value, @"^\d+$");
    if (isNumericOnly && allowParentId) return (true, string.Empty);
    
    return (false, $"{fieldLabel} CNIC/Parent ID invalid format");
}
```

---

## 🐛 Common Issues & Solutions

### Issue 1: Admission Number Starts from 1
**Problem:** After bulk import, new admission numbers start from 1 instead of continuing sequence.

**Solution:** ✅ Auto-initialization implemented
```csharp
// Repository automatically detects and initializes from existing data
if (tenant.LastStudentSequence == 0)
{
    tenant.LastStudentSequence = await GetMaxAdmissionNumberSequenceAsync(tenantId);
}
```

### Issue 2: Phone Number with Dash Rejected
**Problem:** Bulk imported data has format `0301-8210558` but system only accepts `03018210558`.

**Solution:** ✅ Both formats now accepted
- Frontend: `phoneNumberValidator()` accepts both
- Backend: `ValidatePhoneNumber()` extracts digits and validates

### Issue 3: Menu Not Loading After Login
**Problem:** User must refresh page after login to see menu.

**Solution:** ✅ Menu re-initialized after login
```typescript
await this.menuService.initializeMenu(); // Added in sign-in.component.ts
```

### Issue 4: Cascade Delete Conflicts
**Problem:** Migration fails with "multiple cascade paths" error.

**Solution:** ✅ Global tenant FK restriction
```csharp
// All tenant foreign keys set to DeleteBehavior.Restrict
```

---

## 📈 Performance Tips

1. **Use Pagination:** Always paginate student lists
2. **Eager Load Wisely:** Only include needed navigation properties
3. **Cache Tenant Config:** Cache `Tenant` entity to avoid repeated DB hits
4. **Async Operations:** Use async/await for all DB operations
5. **Index Fields:** Ensure indexes on `AdmissionNo`, `RollNo`, `TenantId`

---

## 🔐 Security Checklist

- ✅ Multi-tenancy enforced (automatic `TenantId` filtering)
- ✅ Authorization required on all endpoints (`[Authorize]`)
- ✅ Input validation (frontend + backend)
- ✅ SQL injection protection (EF Core parameterized queries)
- ✅ Soft delete (no hard deletes, data recoverable)
- ⚠️ TODO: Encrypt sensitive fields (CNIC, phone)
- ⚠️ TODO: Field-level permissions (RBAC)

---

## 📚 Related Documentation

- **Full Documentation:** `STUDENT_MODULE_DOCUMENTATION.md`
- **Refactoring Plan:** See comments in `StudentParent.cs` (CNIC/ParentId field restructuring)
- **API Docs:** Swagger UI at `/swagger`
- **Entity Diagrams:** Database documentation folder

---

## 🎓 Key Takeaways

1. **Auto-Initialize Sequence:** System automatically detects highest admission number and continues from there
2. **Flexible Validation:** Phone numbers and CNIC/ParentId validation adapts to real-world data
3. **Tenant-Configurable:** Each school can customize admission number format and validation rules
4. **Transaction Safety:** All student operations wrapped in retry-able transactions
5. **Service-Repository Pattern:** Clear separation of concerns for maintainability

---

**Quick Reference Version:** 1.0  
**Last Updated:** October 24, 2025

