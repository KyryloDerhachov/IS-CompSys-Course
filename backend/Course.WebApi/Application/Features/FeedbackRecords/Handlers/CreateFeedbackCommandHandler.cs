using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Course.WebApi.Application.Interfaces;
using Course.WebApi.Domain.Entities;


namespace Course.WebApi.Application.Feedback.Commands.CreateFeedback;

public class CreateFeedbackCommandHandler : IRequestHandler<CreateFeedbackCommand, Guid>
{
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly IReceiptRepository _receiptRepository;

    public CreateFeedbackCommandHandler(
        IFeedbackRepository feedbackRepository,
        IReceiptRepository receiptRepository)
    {
        _feedbackRepository = feedbackRepository;
        _receiptRepository = receiptRepository;
    }

    public async Task<Guid> Handle(CreateFeedbackCommand request, CancellationToken cancellationToken)
    {
        var existingFeedback = await _feedbackRepository.GetByReceiptNumberAsync(request.ReceiptNumber, cancellationToken);
        if (existingFeedback != null)
        {
            throw new InvalidOperationException($"Feedback for receipt '{request.ReceiptNumber}' has already been submitted.");
        }

        var receipt = await _receiptRepository.GetByNumberAsync(request.ReceiptNumber);
        if (receipt == null)
        {
            throw new KeyNotFoundException($"Receipt with number '{request.ReceiptNumber}' was not found.");
        }
        var feedbackRecord = new FeedbackRecord(
            request.ReceiptNumber,
            request.Type,
            request.Content,
            receipt
        );

        await _feedbackRepository.AddAsync(feedbackRecord, cancellationToken);

        return feedbackRecord.Id;
    }
}