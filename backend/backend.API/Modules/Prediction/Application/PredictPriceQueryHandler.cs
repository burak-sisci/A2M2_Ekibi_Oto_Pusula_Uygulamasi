using backend.API.Modules.Cars.Domain;
using backend.API.Modules.Prediction.Domain;
using MediatR;

namespace backend.API.Modules.Prediction.Application;

public class PredictPriceQueryHandler : IRequestHandler<PredictPriceQuery, PricePredictionResponse>
{
    private readonly IPredictionService _predictionService;

    public PredictPriceQueryHandler(IPredictionService predictionService)
    {
        _predictionService = predictionService;
    }

    public async Task<PricePredictionResponse> Handle(
        PredictPriceQuery request,
        CancellationToken cancellationToken)
    {
        return await _predictionService.PredictAsync(request.Car);
    }
}