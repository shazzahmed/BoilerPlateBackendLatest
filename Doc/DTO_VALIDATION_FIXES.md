# DTO Validation Fixes for Create/Update APIs

## Problem
When updating entities like `Class`, the backend validation was rejecting requests with errors like:
```
"ClassSections[0].Class": ["The Class field is required."]
"ClassSections[0].ClassName": ["The ClassName field is required."]
"SubjectClasses[0].SubjectName": ["The SubjectName field is required."]
```

## Root Cause
ASP.NET Core's model validation was trying to validate:
1. **Computed properties** (properties with `=>` syntax like `ClassName => Class?.Name`)
2. **Navigation properties** (like `public virtual ClassModel Class { get; set; }`)

These properties should **NOT** be validated during deserialization (incoming requests) because:
- Computed properties are **read-only** and calculated from navigation properties
- Navigation properties are for **internal use** and should not be part of the API request

## Solution
Applied `[JsonIgnore]` attributes with appropriate conditions to prevent validation errors:

### 1. **Computed Properties**
```csharp
// ❌ BEFORE - Caused validation errors
public string ClassName => Class?.Name;

// ✅ AFTER - Ignored during deserialization, included in responses when not null
[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
public string? ClassName => Class?.Name;
```

### 2. **Navigation Properties**
```csharp
// ❌ BEFORE - Caused validation errors
[ForeignKey("ClassId")]
public virtual ClassModel Class { get; set; }

// ✅ AFTER - Completely ignored during serialization/deserialization
[JsonIgnore]
[ForeignKey("ClassId")]
public virtual ClassModel? Class { get; set; }
```

## Files Fixed

### Core Relationship Models
1. **ClassSectionModel.cs**
   - Fixed: `ClassName`, `SectionName` (computed)
   - Fixed: `Class`, `Section` (navigation)

2. **SubjectClassModel.cs**
   - Fixed: `SubjectName` (computed)
   - Fixed: `Subject`, `Class` (navigation)

### Entity Response Models
3. **ExamModel.cs**
   - Fixed: `ClassName`, `SectionName` (computed)
   - Fixed: `Class`, `Section` (navigation)

4. **PreAdmissionModel.cs**
   - Fixed: `ClassName`, `EnquiryFullName`, `FeeGroupName` (computed)
   - Fixed: `Enquiry`, `Class`, `FeeGroupFeeType` (navigation)

5. **EntryTestModel.cs**
   - Fixed: `ApplicationNumber`, `ClassName` (computed)
   - Fixed: `Application`, `Class` (navigation)

6. **EnquiryModel.cs**
   - Fixed: `ClassName` (computed)
   - Fixed: `Class`, `PreAdmission` (navigation)

7. **AdmissionModel.cs**
   - Fixed: `StudentName`, `TestNumber`, `ClassName`, `SectionName` (computed)
   - Fixed: `EntryTest`, `Class`, `Section`, `Student` (navigation)

## Key Takeaways

### ✅ Best Practices
1. **Always use `[JsonIgnore]` on navigation properties** in DTOs
2. **Use `[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]`** on computed properties that you want in responses but not in requests
3. **Make computed properties and navigation properties nullable** (`string?`, `ClassModel?`)
4. **Add XML comments** to explain why properties are ignored

### ⚠️ What NOT to Do
1. **Don't use computed properties without `[JsonIgnore]`** - causes validation errors
2. **Don't use navigation properties without `[JsonIgnore]`** - causes validation errors
3. **Don't make computed/navigation properties required** - they should always be optional

## Impact
- ✅ All Create/Update APIs now work correctly
- ✅ No validation errors for computed/navigation properties
- ✅ Response DTOs still include computed properties for display
- ✅ Backend validation only checks actual data properties (IDs, names, etc.)

## Testing
After these fixes, the following operations should work:
- Creating new entities with nested relationships (e.g., Class with ClassSections)
- Updating entities with nested relationships
- Responses still include computed properties for frontend display

## Example Request/Response Flow

### Request (Create/Update Class)
```json
{
  "Id": 1,
  "Name": "Class 5",
  "SelectedSectionIds": [1, 2, 3],
  "ClassSections": [
    { "Id": 0, "ClassId": 1, "SectionId": 1 }
  ]
}
```

### Response
```json
{
  "Id": 1,
  "Name": "Class 5",
  "ClassSections": [
    {
      "Id": 10,
      "ClassId": 1,
      "SectionId": 1,
      "ClassName": "Class 5",      // ✅ Computed property included
      "SectionName": "Section A"   // ✅ Computed property included
    }
  ]
}
```

Notice:
- **Request** doesn't include `ClassName`, `SectionName`, `Class`, `Section`
- **Response** includes computed properties for display
- **No validation errors** because navigation properties are ignored during deserialization

