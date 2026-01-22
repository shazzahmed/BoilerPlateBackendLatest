-- Verify Notification Templates were added successfully

USE [SMSANGULAR]
GO

-- Check NotificationTypes
SELECT * FROM NotificationTypes
GO

-- Check NotificationTemplates (should have 12 new records with IDs 14-25)
SELECT 
    Id,
    NotificationTypeId,
    Subject,
    Description,
    CASE 
        WHEN NotificationTypeId = 1 THEN 'Email'
        WHEN NotificationTypeId = 2 THEN 'SMS'
        ELSE 'Other'
    END AS Type
FROM NotificationTemplates
WHERE Id BETWEEN 14 AND 25
ORDER BY Id
GO

-- Count by type
SELECT 
    CASE 
        WHEN NotificationTypeId = 1 THEN 'Email'
        WHEN NotificationTypeId = 2 THEN 'SMS'
    END AS Type,
    COUNT(*) AS Count
FROM NotificationTemplates
WHERE Id BETWEEN 14 AND 25
GROUP BY NotificationTypeId
GO

PRINT '✅ If you see 12 templates (6 Email + 6 SMS), setup is complete!'

