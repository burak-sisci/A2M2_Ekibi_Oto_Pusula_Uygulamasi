using backend.API.Modules.Cars.Domain;
using backend.API.Modules.Prediction.Domain;

namespace backend.API.Modules.Prediction.Application;

public interface IPredictionService
{
    Task<PricePredictionResponse> PredictAsync(Car car);
}