using MediatR;
using Course.WebApi.Features.Feedback.DTOs;

namespace Course.WebApi.Application.Feedback.Queries.GetAllFeedbacks;

public record GetAllFeedbacksQuery : IRequest<IEnumerable<FeedbackDto>>;