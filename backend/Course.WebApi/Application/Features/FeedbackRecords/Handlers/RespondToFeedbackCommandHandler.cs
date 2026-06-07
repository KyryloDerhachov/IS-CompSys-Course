using MediatR;
using Course.WebApi.Application.Interfaces;

namespace Course.WebApi.Application.Feedback.Commands.RespondToFeedback;

public class RespondToFeedbackCommandHandler : IRequestHandler<RespondToFeedbackCommand>
{
    private readonly IFeedbackRepository _feedbackRepository;

    public RespondToFeedbackCommandHandler(IFeedbackRepository feedbackRepository)
    {
        _feedbackRepository = feedbackRepository;
    }

    public async Task Handle(RespondToFeedbackCommand request, CancellationToken cancellationToken)
    {
        var feedback = await _feedbackRepository.GetByIdAsync(request.FeedbackId, cancellationToken);
        if (feedback == null)
        {
            throw new KeyNotFoundException($"Feedback record with ID '{request.FeedbackId}' was not found.");
        }

        feedback.AddManagerResponse(request.Response, request.ManagerId);

        await _feedbackRepository.UpdateAsync(feedback);
    }
}