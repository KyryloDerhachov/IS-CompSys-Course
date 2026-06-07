using MediatR;
using Course.WebApi.Application.Interfaces;
using Course.WebApi.Features.Feedback.DTOs;

namespace Course.WebApi.Application.Feedback.Queries.GetAllFeedbacks;

public class GetAllFeedbacksQueryHandler : IRequestHandler<GetAllFeedbacksQuery, IEnumerable<FeedbackDto>>
{
    private readonly IFeedbackRepository _feedbackRepository;

    public GetAllFeedbacksQueryHandler(IFeedbackRepository feedbackRepository)
    {
        _feedbackRepository = feedbackRepository;
    }

    public async Task<IEnumerable<FeedbackDto>> Handle(GetAllFeedbacksQuery request, CancellationToken cancellationToken)
    {
        var records = await _feedbackRepository.GetAllAsync(cancellationToken);

        return records.Select(f => new FeedbackDto
        {
            Id = f.Id,
            ReceiptNumber = f.ReceiptNumber,
            Type = f.Type,
            Content = f.Content,
            ManagerResponse = f.ManagerResponse,
            RespondedByManagerId = f.RespondedByManagerId,
            ResponseDate = f.ResponseDate,
            CreatedAt = f.CreatedAt
        }).OrderByDescending(f => f.CreatedAt);
    }
}