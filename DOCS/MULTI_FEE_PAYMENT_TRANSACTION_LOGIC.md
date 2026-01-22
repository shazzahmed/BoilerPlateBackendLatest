# Multi-Fee Payment - Transaction Logic Alignment

## 📋 **Requirement**

Align `ProcessMultiFeePayment` logic with `SaveFeeTransaction` to ensure consistent transaction processing, validation, and fee assignment updates across both endpoints.

---

## 🔄 **Changes Implemented**

### **1. Duplicate Transaction Prevention** ✅

**Added 10-second duplicate detection window:**

```csharp
// ✅ Check for duplicate transaction (within last 10 seconds with same amount)
var recentDuplicateWindow = DateTime.UtcNow.AddSeconds(-10);
var duplicateTransaction = await _context.Set<Domain.Entities.FeeTransaction>()
    .Where(t => t.FeeAssignmentId == feePayment.FeeAssignmentId &&
               t.AmountPaid == feePayment.AmountPaying &&
               t.CreatedAtUtc >= recentDuplicateWindow)
    .OrderByDescending(t => t.CreatedAtUtc)
    .FirstOrDefaultAsync();

if (duplicateTransaction != null)
{
    errors.Add($"Duplicate transaction detected for fee {feePayment.FeeAssignmentId}");
    continue;
}
```

**Purpose:** Prevents accidental double-click submissions or duplicate API calls.

---

### **2. Overpayment Validation** ✅

**Calculate and validate against remaining balance:**

```csharp
// ✅ Calculate final due amount
var finalDueAmount = DateTime.Now > assignment.DueDate 
    ? assignment.Amount + assignment.AmountFine - assignment.PaidAmount 
    : assignment.Amount - assignment.PaidAmount;

// ✅ Validate overpayment
if (feePayment.AmountPaying > finalDueAmount)
{
    errors.Add($"Paid amount (₨{feePayment.AmountPaying}) exceeds remaining balance (₨{finalDueAmount})");
    continue;
}
```

**Purpose:** Prevents collecting more than the outstanding balance.

---

### **3. Partial Payment Validation** ✅

**Respect fee-level partial payment rules:**

```csharp
// ✅ Check if partial payments are disallowed
if (!assignment.IsPartialPaymentAllowed && feePayment.AmountPaying < finalDueAmount)
{
    errors.Add($"Partial payments are not allowed for fee {feePayment.FeeAssignmentId}");
    continue;
}
```

**Purpose:** Enforces fee-specific payment policies.

---

### **4. Auto-Generated Reference Numbers** ✅

**Generate structured reference numbers:**

```csharp
// ✅ Generate reference number (FeeAssignmentId/PaymentCount)
var paymentCount = await _context.Set<Domain.Entities.FeeTransaction>()
    .CountAsync(t => t.FeeAssignmentId == feePayment.FeeAssignmentId);
var generatedReferenceNo = $"{feePayment.FeeAssignmentId}/{paymentCount + 1}";

// Append bank name if provided
var finalReferenceNo = string.IsNullOrEmpty(feePayment.BankName) 
    ? (string.IsNullOrEmpty(feePayment.ReferenceNumber) ? generatedReferenceNo : $"{generatedReferenceNo} - {feePayment.ReferenceNumber}`)
    : $"{generatedReferenceNo} - {feePayment.ReferenceNumber} - {feePayment.BankName}";
```

**Format:** `{FeeAssignmentId}/{PaymentNumber}` (e.g., `7/1`, `7/2`)
**With Bank:** `7/1 - CHQ123 - UBL Bank`

**Purpose:** Provides traceable, sequential transaction references.

---

### **5. FeeAssignment Updates** ✅

**Update `PaidAmount`, `AmountDiscount`, and `Status`:**

```csharp
// ✅ Update FeeAssignment's PaidAmount
assignment.PaidAmount += feePayment.AmountPaying;

// ✅ Apply additional discount to assignment
if (feePayment.DiscountApplied > 0)
{
    assignment.AmountDiscount += feePayment.DiscountApplied;
}

// ✅ Update FeeAssignment status
assignment.Status = assignment.IsPaid ? FeeStatus.Paid :
                   assignment.PaidAmount > 0 ? FeeStatus.Partial :
                   FeeStatus.Pending;
```

**Purpose:** Keeps fee assignment status synchronized with payments.

---

### **6. Transaction Fields** ✅

**Include `Month` and `Year` from assignment:**

```csharp
var feeTransaction = new Domain.Entities.FeeTransaction
{
    FeeAssignmentId = feePayment.FeeAssignmentId,
    FeeGroupFeeTypeId = feePayment.FeeGroupFeeTypeId,
    StudentId = request.StudentId,
    Month = assignment.Month,                          // ✅ From assignment
    Year = assignment.Year,                            // ✅ From assignment
    AmountPaid = feePayment.AmountPaying,
    DiscountApplied = assignment.AmountDiscount + feePayment.DiscountApplied,  // ✅ Existing + Additional
    FineApplied = feePayment.FineApplied,              // ✅ User can waive
    PaymentDate = request.PaymentDate,
    PaymentMethod = feePayment.PaymentMethod,
    ReferenceNo = finalReferenceNo,                    // ✅ Auto-generated
    Note = feePayment.Note ?? string.Empty,
    Status = TransactionStatus.Completed,              // ✅ Set status
    CreatedAt = DateTime.Now,
    CreatedBy = request.CollectedBy,
    IsDeleted = false
};
```

**Purpose:** Ensures complete and accurate transaction records.

---

## 📊 **Comparison: Before vs After**

| Feature | SaveFeeTransaction (Old) | ProcessMultiFeePayment (Before) | ProcessMultiFeePayment (After) |
|---------|-------------------------|--------------------------------|-------------------------------|
| Duplicate Detection | ✅ Yes (10 sec window) | ❌ No | ✅ Yes (10 sec window) |
| Overpayment Check | ✅ Yes | ❌ No | ✅ Yes |
| Partial Payment Validation | ✅ Yes | ❌ No | ✅ Yes |
| Auto Reference Number | ✅ Yes (`7/1`) | ❌ Manual | ✅ Yes (`7/1`) |
| Update PaidAmount | ✅ Yes | ❌ No | ✅ Yes |
| Update AmountDiscount | ✅ Yes | ❌ No | ✅ Yes |
| Update Status | ✅ Yes | ❌ No | ✅ Yes |
| Month/Year Fields | ✅ Yes | ❌ No | ✅ Yes |
| Transaction Status | ✅ Completed | ❌ Not set | ✅ Completed |

---

## 🎯 **Transaction Flow**

### **Multi-Fee Payment Process:**

```mermaid
1. API Request
   ↓
2. Begin Transaction (with ExecutionStrategy)
   ↓
3. For Each Fee Payment:
   ├─ Load FeeAssignment (with FeeTransactions)
   ├─ Check Duplicate (last 10 seconds)
   ├─ Calculate Final Due Amount
   ├─ Validate Overpayment
   ├─ Validate Partial Payment Rules
   ├─ Count Existing Payments
   ├─ Generate Reference Number (FeeAssignmentId/Count)
   ├─ Create FeeTransaction
   │  ├─ Month/Year from Assignment
   │  ├─ DiscountApplied = Existing + Additional
   │  ├─ FineApplied = User Choice (can waive)
   │  ├─ ReferenceNo = Auto-Generated
   │  └─ Status = Completed
   ├─ Update FeeAssignment.PaidAmount
   ├─ Update FeeAssignment.AmountDiscount (if additional)
   ├─ Update FeeAssignment.Status
   └─ SaveChanges
   ↓
4. Handle Advance Payment (if requested)
   ↓
5. Commit Transaction
   ↓
6. Send Notifications (SMS/Email)
   ↓
7. Return Result
```

---

## 🧪 **Testing Scenarios**

### **Test Case 1: Duplicate Prevention**

```
1. Pay ₨500 for Fee Assignment 7
2. Immediately click "Pay" again (within 10 seconds)

Expected Result: ❌ Second payment rejected
Error: "Duplicate transaction detected for fee 7"
```

### **Test Case 2: Overpayment Prevention**

```
Fee: ₨16,207
Already Paid: ₨10,000
Balance Due: ₨6,207

User tries to pay: ₨7,000

Expected Result: ❌ Payment rejected
Error: "Paid amount (₨7,000) exceeds remaining balance (₨6,207)"
```

### **Test Case 3: Partial Payment Validation**

```
Fee: ₨16,207
Balance Due: ₨16,207
IsPartialPaymentAllowed: false

User tries to pay: ₨5,000

Expected Result: ❌ Payment rejected
Error: "Partial payments are not allowed for fee 7"
```

### **Test Case 4: Reference Number Generation**

```
Fee Assignment: 7
Existing Payments: 2 (7/1, 7/2)
New Payment: ₨1,000

Expected Result: ✅ Transaction created
ReferenceNo: "7/3"
```

### **Test Case 5: Reference Number with Bank**

```
Fee Assignment: 7
Payment Method: Cheque
Bank: UBL
Cheque Number: CHQ12345

Expected Result: ✅ Transaction created
ReferenceNo: "7/1 - CHQ12345 - UBL"
```

### **Test Case 6: Fee Assignment Status Update**

```
Fee: ₨16,207
Payment: ₨16,207 (full)

Expected Result: ✅ Transaction created
FeeAssignment.PaidAmount: 16207
FeeAssignment.Status: Paid
```

### **Test Case 7: Partial Payment Status**

```
Fee: ₨16,207
Payment: ₨10,000 (partial)

Expected Result: ✅ Transaction created
FeeAssignment.PaidAmount: 10000
FeeAssignment.Status: Partial
```

### **Test Case 8: Additional Discount**

```
Fee: ₨16,207
Existing Discount: ₨500
Additional Discount: ₨100
Payment: ₨10,000

Expected Result: ✅ Transaction created
Transaction.DiscountApplied: 600
FeeAssignment.AmountDiscount: 600
```

---

## 🔍 **Validation Rules**

### **1. Amount Validation**

```csharp
AmountPaying > 0                    // Must be positive
AmountPaying <= FinalDueAmount      // Cannot exceed balance
```

### **2. Partial Payment Rule**

```csharp
if (!IsPartialPaymentAllowed && AmountPaying < FinalDueAmount)
    → REJECT
```

### **3. Duplicate Check**

```csharp
if (Same FeeAssignmentId AND Same Amount AND Within 10 seconds)
    → REJECT
```

### **4. Status Calculation**

```csharp
Status = IsPaid ? Paid              // Balance = 0
       : PaidAmount > 0 ? Partial   // Balance > 0, but some paid
       : Pending                     // No payment yet
```

---

## 📁 **Files Changed**

### **Backend:**
- **File:** `SMSBACKEND/SMSBACKEND.Infrastructure/Services/Services/FeePaymentService.cs`
- **Method:** `PayMultipleFeesAsync()`
- **Lines:** 342-453

**Key Changes:**
1. Added duplicate transaction check
2. Added overpayment validation
3. Added partial payment validation
4. Auto-generate reference numbers
5. Update FeeAssignment PaidAmount
6. Update FeeAssignment AmountDiscount
7. Update FeeAssignment Status
8. Include Month/Year in transaction
9. Set transaction Status to Completed

---

## ✅ **Benefits**

### **1. Data Integrity**
- No duplicate transactions
- No overpayments
- Consistent fee assignment status

### **2. Business Rules**
- Partial payment rules enforced
- Proper reference numbering
- Accurate discount tracking

### **3. Audit Trail**
- Sequential reference numbers
- Complete transaction history
- Status changes tracked

### **4. User Experience**
- Clear error messages
- Prevents common mistakes
- Consistent behavior across endpoints

---

## 🚀 **Deployment Notes**

### **Database Requirements:**
- No schema changes needed
- All fields already exist

### **Testing Required:**
1. Duplicate transaction prevention
2. Overpayment validation
3. Partial payment rules
4. Reference number generation
5. Fee assignment updates
6. Status changes

### **Backward Compatibility:**
✅ **Fully compatible** - No breaking changes to API contract

---

## 📝 **API Contract (Unchanged)**

### **Request:**
```typescript
{
  StudentId: number;
  FeePayments: [
    {
      FeeAssignmentId: number;
      FeeGroupFeeTypeId: number;
      AmountPaying: number;
      DiscountApplied: number;
      FineApplied: number;
      PaymentMethod: string;
      ReferenceNumber: string;
      BankName: string;
      Note: string;
    }
  ];
  PaymentDate: Date;
  CollectedBy: string;
  SendSMS: boolean;
  SendEmail: boolean;
  IncludeAdvancePayment: boolean;
  AdvanceAmount: number;
}
```

### **Response:**
```typescript
{
  Success: boolean;
  Data: number[];  // Transaction IDs
  Message: string;
  Total: number;
  LastId: number;
  CorrelationId: string;
}
```

---

## 🎓 **Key Takeaways**

1. **Consistency:** Both `SaveFeeTransaction` and `ProcessMultiFeePayment` now follow the same logic
2. **Validation:** Multiple layers of validation prevent data integrity issues
3. **Traceability:** Auto-generated reference numbers provide clear audit trail
4. **Status Management:** Fee assignment status automatically reflects payment state
5. **Error Handling:** Graceful error handling with specific error messages

---

**Status:** ✅ **COMPLETE**  
**Last Updated:** 2025-10-31  
**Updated By:** AI Assistant  
**Approved By:** User (Requested alignment with SaveFeeTransaction)

