using System;
using Course.WebApi.Domain.Entities;

namespace Course.WebApi.Features.Feedback.DTOs;

public class FeedbackDto
{
    public Guid Id { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public FeedbackType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ManagerResponse { get; set; }
    public Guid? RespondedByManagerId { get; set; }
    public DateTime? ResponseDate { get; set; }
    public DateTime CreatedAt { get; set; }
}