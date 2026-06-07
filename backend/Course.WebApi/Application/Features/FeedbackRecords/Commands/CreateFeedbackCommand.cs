using MediatR;
using Course.WebApi.Domain.Entities;

namespace Course.WebApi.Application.Feedback.Commands.CreateFeedback;

public record CreateFeedbackCommand(
    string ReceiptNumber,
    FeedbackType Type,
    string Content) : IRequest<Guid>;