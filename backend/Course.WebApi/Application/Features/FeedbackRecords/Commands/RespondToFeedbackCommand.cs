using System;
using MediatR;

namespace Course.WebApi.Application.Feedback.Commands.RespondToFeedback;

public record RespondToFeedbackCommand(
    Guid FeedbackId,
    string Response,
    Guid ManagerId) : IRequest;