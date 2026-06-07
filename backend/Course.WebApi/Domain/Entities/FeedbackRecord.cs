using System;
using Course.WebApi.Domain.Common;

namespace Course.WebApi.Domain.Entities;

public class FeedbackRecord : AuditableEntity<Guid>
{
    public string ReceiptNumber { get; private set; } = string.Empty;
    public FeedbackType Type { get; private set; }
    public string Content { get; private set; } = string.Empty;
    
    public string? ManagerResponse { get; private set; }
    public Guid? RespondedByManagerId { get; private set; }
    public DateTime? ResponseDate { get; private set; }

    public FeedbackRecord() { }

    public FeedbackRecord(string receiptNumber, FeedbackType type, string content, Receipt receipt)
    {
        if (string.IsNullOrWhiteSpace(receiptNumber))
            throw new ArgumentException("Receipt number cannot be empty.");

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Feedback content cannot be empty.");

        if (receipt == null)
            throw new ArgumentNullException(nameof(receipt), "Receipt was not found in the system.");

        if (receipt.ReceiptNumber != receiptNumber)
            throw new ArgumentException("Provided receipt does not match the specified receipt number.");

        if (DateTime.UtcNow > receipt.SaleDate.AddHours(24))
            throw new InvalidOperationException("The 24-hour time limit for submitting feedback for this receipt has expired.");

        Id = Guid.NewGuid();
        ReceiptNumber = receiptNumber.Trim().ToUpper();
        Type = type;
        Content = content.Trim();
        
        SetCreated(null);
    }

    public void AddManagerResponse(string response, Guid managerId)
    {
        if (string.IsNullOrWhiteSpace(response))
            throw new ArgumentException("Manager response cannot be empty.");

        if (managerId == Guid.Empty)
            throw new ArgumentException("Invalid manager identifier.");

        ManagerResponse = response.Trim();
        RespondedByManagerId = managerId;
        ResponseDate = DateTime.UtcNow;

        SetUpdated(managerId);
    }
}